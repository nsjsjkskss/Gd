"""
okmod_patch.py — XenProxy addon for the OKMOD CDN codepatch swap.

Ported 1:1 from proxy/okmod_addon.py (mitmproxy) to XenProxy so the 1106
listener runs on the same engine as the rest of the stack.

What it does
  - only intercepts the FreeFire core ABHotUpdates CDN hosts (PAC routes them)
  - rewrites the single codepatch line in the main /ABHotUpdates/.../fileinfo
  - serves the built gzip codepatch from OKMOD_PATCH (default build/okmod-R8-AIM-ESP.bin)
  - honours Range requests
  - everything else is tunnelled untouched (tls_clienthello ignore_connection)
  - keeps the old auxiliary endpoints: /, /healthz, /localconfig.json, /okmod/aimlog

Payload reload
  The file is re-read when its mtime/size changes, so a new build is served
  without restarting the unit (deploy.sh still restarts on purpose).

Env
  OKMOD_PATCH       gzip-packed codepatch bundle        (required)
  OKMOD_ONLINE_LOG  aimlog sink path                    (optional)
"""

from __future__ import annotations

import base64
import gzip
import hashlib
import json
import logging
import os
import re
import time

from xenproxy import ctx, http

logger = logging.getLogger("okmod.proxy")

PATCH_PATH = os.environ.get(
    "OKMOD_PATCH", "/root/okmod-1297-clean/build/okmod-R8-AIM-ESP.bin"
)
ONLINE_LOG = os.environ.get(
    "OKMOD_ONLINE_LOG", "/root/okmod-1297-clean/logs/aimkill-online.log"
)

# Hosts that actually serve the ABHotUpdates manifest/codepatch.
# Extend if Garena moves the region endpoint.
CORE_HOSTS = {
    "core-gmc.freefiremobile.com",
    "core-aw.freefiremobile.com",
    "core-bs.freefiremobile.com",
    "core-th.freefiremobile.com",
    "core-vn.freefiremobile.com",
    "core-sg.freefiremobile.com",
    "core-id.freefiremobile.com",
    "dl-core.cdn.freefiremobile.com",
}

DIRECT_HOSTS = {
    "okmod.sbs",
    "www.okmod.sbs",
    "proxy.okmod.sbs",
    "origin.okmod.sbs",
    "217.217.255.156",
    "127.0.0.1",
    "localhost",
}

STATUS_HTML = (
    b"<!doctype html><meta charset='utf-8'><title>OK</title>"
    b"<meta name='viewport' content='width=device-width,initial-scale=1'>"
    b"<body style='font:18px system-ui;padding:24px'><p>OK</p></body>"
)

_cache: dict = {"key": None, "packed": None, "raw": None, "row": None}


def _load_patch() -> tuple[bytes, bytes, str] | None:
    """Return (packed, raw, manifest_row), reloading when the file changes."""
    try:
        stat = os.stat(PATCH_PATH)
    except OSError as exc:
        logger.warning("OKMOD_PATCH missing %s: %s", PATCH_PATH, exc)
        return None

    key = (stat.st_mtime_ns, stat.st_size)
    if _cache["key"] == key and _cache["packed"] is not None:
        return _cache["packed"], _cache["raw"], _cache["row"]

    try:
        with open(PATCH_PATH, "rb") as fh:
            packed = fh.read()
        raw = gzip.decompress(packed)
    except Exception as exc:
        logger.warning("OKMOD_PATCH unreadable %s: %s", PATCH_PATH, exc)
        return None

    raw_hash = base64.b64encode(hashlib.sha1(raw).digest()).decode()
    pack_hash = base64.b64encode(hashlib.sha1(packed).digest()).decode()
    row = (
        "codepatch/assembly-csharp-patch,"
        f"{raw_hash},{len(raw)},0,{pack_hash},{len(packed)},True,0"
    )
    _cache.update(key=key, packed=packed, raw=raw, row=row)
    logger.warning(
        "OKMOD_PATCH loaded %s raw=%d packed=%d row=%s",
        PATCH_PATH, len(raw), len(packed), row,
    )
    return packed, raw, row


def _decode_body(resp: http.Response) -> bytes:
    import zlib
    data = resp.content or b""
    enc = (resp.headers.get("Content-Encoding") or "").lower()
    try:
        if "gzip" in enc:
            data = gzip.decompress(data)
        elif "br" in enc:
            import brotli
            data = brotli.decompress(data)
        elif "deflate" in enc:
            data = zlib.decompress(data)
    except Exception:
        pass
    return data


def _set_body(resp: http.Response, body: bytes) -> None:
    for header in ("Content-Encoding", "Transfer-Encoding"):
        try:
            del resp.headers[header]
        except KeyError:
            pass
    resp.content = body
    resp.headers["Content-Length"] = str(len(body))


# ---------------------------------------------------------------------------
# local helper endpoints
# ---------------------------------------------------------------------------

def request(flow: http.HTTPFlow) -> None:
    host = (flow.request.pretty_host or "").lower().split(":")[0]
    path = (flow.request.path or "/").split("?", 1)[0]

    # Online AimKill diagnostics sink (payload GETs this through the PAC host).
    if path == "/okmod/aimlog":
        query = flow.request.query
        entry = {
            "at": time.strftime("%Y-%m-%dT%H:%M:%S%z"),
            "client": flow.client_conn.peername[0] if flow.client_conn.peername else "?",
            "scheme": flow.request.scheme,
            "host": host,
            "tag": str(query.get("t", "")),
            "v": str(query.get("v", "")),
            "log": str(query.get("d", "")),
        }
        try:
            os.makedirs(os.path.dirname(ONLINE_LOG), exist_ok=True)
            with open(ONLINE_LOG, "a", encoding="utf-8") as fh:
                fh.write(json.dumps(entry, ensure_ascii=False) + "\n")
        except Exception as exc:
            logger.warning("aimlog write failed: %s", exc)
        logger.warning("AIMLOG %s %s %s", entry["client"], entry["tag"], entry["scheme"])
        flow.response = http.Response.make(
            200, b"ok", {"Content-Type": "text/plain", "Cache-Control": "no-store"}
        )
        return

    if host not in DIRECT_HOSTS:
        return

    if path in ("", "/"):
        flow.response = http.Response.make(
            200, STATUS_HTML,
            {"Content-Type": "text/html; charset=utf-8", "Cache-Control": "no-store"},
        )
        return

    if path == "/healthz":
        flow.response = http.Response.make(
            200, b"ok", {"Content-Type": "text/plain", "Cache-Control": "no-store"}
        )
        return

    if path == "/localconfig.json":
        flow.response = http.Response.make(
            200, b'{"testCodePatch":true}',
            {"Content-Type": "application/json; charset=utf-8", "Cache-Control": "no-store"},
        )
        return


# ---------------------------------------------------------------------------
# CDN patch swap
# ---------------------------------------------------------------------------

def response(flow: http.HTTPFlow) -> None:
    host = (flow.request.pretty_host or "").lower()
    path = (flow.request.path or "").split("?", 1)[0]

    if host not in CORE_HOSTS or flow.response is None:
        return

    main_fileinfo = (
        path.endswith("/fileinfo")
        and "/ABHotUpdates/" in path
        and "optional" not in path
    )
    is_codepatch = (
        "/gameassetbundles/codepatch/" in path
        and "assembly-csharp-patch" in path
    )
    if not (main_fileinfo or is_codepatch):
        return

    loaded = _load_patch()
    if loaded is None:
        logger.warning("codepatch request but no payload available")
        return
    packed, _raw, row = loaded

    client = flow.client_conn.peername[0] if flow.client_conn.peername else "?"

    if main_fileinfo:
        data = _decode_body(flow.response)
        text = data.decode("utf-8", "replace")
        lines = text.splitlines()
        found = False
        for i, line in enumerate(lines):
            if line.startswith("codepatch/assembly-csharp-patch,"):
                lines[i] = row
                found = True
        if not found:
            logger.warning("codepatch row not found in %s", path)
            return
        body = ("\n".join(lines) + "\n").encode()
        _set_body(flow.response, body)
        flow.response.headers["Content-Type"] = "binary/octet-stream"
        flow.response.headers["Cache-Control"] = "no-store"
        logger.warning("fileinfo patched for %s -> %s", client, row)
        return

    # codepatch bundle
    rng = flow.request.headers.get("Range")
    if rng:
        m = re.match(r"bytes=(\d+)-(\d*)", rng)
        if m:
            start = int(m.group(1))
            end = int(m.group(2)) if m.group(2) else len(packed) - 1
            end = min(end, len(packed) - 1)
            if start <= end:
                chunk = packed[start:end + 1]
                flow.response = http.Response.make(
                    206, chunk,
                    {
                        "Content-Type": "binary/octet-stream",
                        "Content-Range": f"bytes {start}-{end}/{len(packed)}",
                        "Content-Length": str(len(chunk)),
                        "Accept-Ranges": "bytes",
                        "Cache-Control": "no-store",
                    },
                )
                logger.warning("codepatch range %d-%d served to %s", start, end, client)
                return

    flow.response = http.Response.make(
        200, packed,
        {
            "Content-Type": "binary/octet-stream",
            "Content-Length": str(len(packed)),
            "Accept-Ranges": "bytes",
            "Cache-Control": "no-store",
        },
    )
    logger.warning("codepatch served %d bytes to %s", len(packed), client)


# ---------------------------------------------------------------------------
# TLS: only MITM the CDN, tunnel everything else untouched
# ---------------------------------------------------------------------------

def tls_clienthello(data) -> None:
    try:
        sni = (data.client_hello.sni or "").lower()
    except Exception:
        sni = ""
    if sni not in CORE_HOSTS:
        data.ignore_connection = True


def load(loader) -> None:
    loaded = _load_patch()
    if loaded is None:
        logger.warning("[okmod-xenproxy] loaded WITHOUT payload (%s)", PATCH_PATH)
    else:
        logger.warning("[okmod-xenproxy] loaded, payload %s", PATCH_PATH)
