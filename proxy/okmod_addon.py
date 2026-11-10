# DEPRECATED: mitmproxy addon kept for rollback only.
# Live addon: proxy/xenproxy/addons/okmod_patch.py (XenProxy).
"""
okmod_addon.py — mitmproxy addon for FreeFire CDN codepatch swap.

What it does:
  - Only touches the FreeFire core hotupdate CDN hosts
  - Rewrites the single codepatch line in the main ABHotUpdates fileinfo
  - Serves our gzip codepatch from /opt/okmod/patch.bin
  - Everything else, including optional avatar fileinfo, passes through

Deploy:
  mitmdump --mode regular --listen-host 0.0.0.0 --listen-port 1106 \
    --set block_global=false --set connection_strategy=lazy \
    -s /opt/okmod/okmod_addon.py
"""

from mitmproxy import http, ctx
import base64
import gzip
import hashlib
import json
import os
import re
import time
import zlib

PATCH_PATH = os.environ.get("OKMOD_PATCH", "/opt/okmod/patch.bin")
ONLINE_LOG = os.environ.get(
    "OKMOD_ONLINE_LOG", "/root/okmod-1297-clean/logs/aimkill-online.log"
)
def _load_patch():
    with open(PATCH_PATH, "rb") as f:
        packed = f.read()
    raw = gzip.decompress(packed)
    raw_hash = base64.b64encode(hashlib.sha1(raw).digest()).decode()
    pack_hash = base64.b64encode(hashlib.sha1(packed).digest()).decode()
    row = (
        f"codepatch/assembly-csharp-patch,"
        f"{raw_hash},{len(raw)},0,"
        f"{pack_hash},{len(packed)},True,0"
    )
    return packed, row


PACKED, ROW = _load_patch()

# Only the host that actually serves ABHotUpdates fileinfo/codepatch.
# Add another core-* host here if the region changes.
CORE_HOSTS = {
    "core-gmc.freefiremobile.com",
    "core-aw.freefiremobile.com",
    "core-bs.freefiremobile.com",
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
    b"<!doctype html><meta charset='utf-8'><title>Trang thai proxy</title>"
    b"<meta name='viewport' content='width=device-width,initial-scale=1'>"
    b"<body style='font:18px system-ui;padding:24px'>"
    b"<h1>Proxy s\xe1\xba\xb5n s\xc3\xa0ng \xe2\x80\x94 kh\xc3\xb4ng c\xe1\xba\xa7n x\xc3\xa1c th\xe1\xbb\xb1c IP</h1>"
    b"<p>Proxy mo: khong yeu cau xac thuc IP hoac Firebase</p>"
    b"<p>Game tu hien thong bao loi cua game; trang nay hien ly do tu proxy.</p>"
    b"<p><b>proxy.okmod.sbs:1106</b></p>"
    b"</body>"
)


def request(flow: http.HTTPFlow) -> None:
    """Serve the proxy status page, localconfig and the OKMOD online log sink."""
    host = (flow.request.pretty_host or "").lower().split(":")[0]
    path = (flow.request.path or "/").split("?", 1)[0]
    # Online AimKill diagnostics: the payload GETs this path on a host the PAC
    # already routes here, so the log lands on disk without a copy/paste step.
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
            ctx.log.warn(f"aimlog write failed: {exc}")
        ctx.log.info(f"AIMLOG {entry['client']} {entry['tag']} {entry['scheme']}")
        flow.response = http.Response.make(
            200, b"ok", {"Content-Type": "text/plain", "Cache-Control": "no-store"}
        )
        return
    if host not in DIRECT_HOSTS:
        return
    if path in ("", "/"):
        flow.response = http.Response.make(
            200,
            STATUS_HTML,
            {"Content-Type": "text/html; charset=utf-8", "Cache-Control": "no-store"},
        )
        return
    if path == "/healthz":
        flow.response = http.Response.make(200, b"ok", {"Content-Type": "text/plain"})
        return
    if path == "/localconfig.json":
        flow.response = http.Response.make(
            200,
            b'{"testCodePatch":true}',
            {"Content-Type": "application/json; charset=utf-8", "Cache-Control": "no-store"},
        )
        return


def _decode_body(resp: http.Response) -> bytes:
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


def response(flow: http.HTTPFlow) -> None:
    host = (flow.request.pretty_host or "").lower()
    path = (flow.request.path or "").split("?", 1)[0]

    if host not in CORE_HOSTS:
        return
    if flow.response is None:
        return

    main_fileinfo = (path.endswith("/fileinfo") and "/ABHotUpdates/" in path
                     and "optional" not in path)
    is_codepatch = ("/gameassetbundles/codepatch/" in path
                    and "assembly-csharp-patch" in path)

    if main_fileinfo or is_codepatch:
        client = flow.client_conn.peername[0] if flow.client_conn.peername else "?"
        ctx.log.info(f"REQ {client} {host}{path}")

    # Main hotupdate manifest
    if main_fileinfo:
        data = _decode_body(flow.response)
        text = data.decode("utf-8", "replace")
        lines = text.splitlines()
        found = False
        for i, line in enumerate(lines):
            if line.startswith("codepatch/assembly-csharp-patch,"):
                lines[i] = ROW
                found = True
        if not found:
            ctx.log.warn("codepatch row not found, leaving fileinfo untouched")
            return
        body = ("\n".join(lines) + "\n").encode()
        flow.response.content = body
        flow.response.headers["Content-Length"] = str(len(body))
        flow.response.headers["Content-Type"] = "binary/octet-stream"
        flow.response.headers["Cache-Control"] = "no-store"
        flow.response.headers.pop("Content-Encoding", None)
        ctx.log.info(f"fileinfo patched: {ROW}")
        return

    # Codepatch bundle
    if is_codepatch:
        body = PACKED
        rng = flow.request.headers.get("Range")
        if rng:
            m = re.match(r"bytes=(\d+)-(\d*)", rng)
            if m:
                start = int(m.group(1))
                end = int(m.group(2)) if m.group(2) else len(body) - 1
                end = min(end, len(body) - 1)
                if start <= end:
                    chunk = body[start:end + 1]
                    flow.response = http.Response.make(
                        206,
                        chunk,
                        {
                            "Content-Type": "binary/octet-stream",
                            "Content-Range": f"bytes {start}-{end}/{len(body)}",
                            "Content-Length": str(len(chunk)),
                            "Accept-Ranges": "bytes",
                        },
                    )
                    ctx.log.info(f"codepatch range {start}-{end}")
                    return
        flow.response = http.Response.make(
            200,
            body,
            {
                "Content-Type": "binary/octet-stream",
                "Content-Length": str(len(body)),
                "Accept-Ranges": "bytes",
            },
        )
        ctx.log.info(f"codepatch served: {len(body)} bytes")


def tls_clienthello(data) -> None:
    """Only intercept TLS for the core hotupdate CDN.

    Everything else (login, Garena, Apple, Facebook, optional CDN) is tunneled
    untouched, so even a Manual/global Wi-Fi proxy cannot break login TLS.
    """
    try:
        sni = (data.client_hello.sni or "").lower()
    except Exception:
        sni = ""
    if sni not in CORE_HOSTS:
        data.ignore_connection = True
