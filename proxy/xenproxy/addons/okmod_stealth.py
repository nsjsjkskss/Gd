"""
okmod_stealth.py — iOS-safe request hygiene for the 1106 codepatch proxy.

XenProxy's built-in firebypass is Android/emulator oriented: it injects an
Android device identity, rewrites login bodies and opens a TCP control port.
None of that belongs on a real iPhone. This addon neutralises those mutations
and keeps only passive header hygiene.

Rules:
  * no Android identity injection, no login-body mutation
  * no TCP control listener (disabled by unit flags)
  * strip proxy / CDN fingerprint headers from every request
  * never touch anti-cheat telemetry (ANO/TSS/SLS/SGSSR/CheckHackBehavior)
  * only the CDN hosts are MITM'd (okmod_patch.py tls_clienthello)
"""

from __future__ import annotations

import logging

from xenproxy import http
from xenproxy.addons import firebypass as fb

logger = logging.getLogger("okmod.stealth")

IOS_UA = (
    "Mozilla/5.0 (iPhone; CPU iPhone OS 26_1 like Mac OS X) "
    "AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148"
)

TOOL_UA = (
    "mitmproxy", "xenproxy", "python-requests", "python/", "curl/",
    "wget/", "httpx/", "aiohttp/", "go-http-client", "libwww-perl",
)

PC_ONLY_HEADERS = {
    "x-device-info", "x-platform-info", "x-client-type", "x-emulator",
    "x-pc", "x-garena-platform",
}


# ---------------------------------------------------------------------------
# Neutralise the Android pipeline (same idea as the old ios_antiband addon).
# ---------------------------------------------------------------------------

def _noop_mutate_login_body(self, flow, path):
    return None


def _noop_inject_mobile_identity(self, flow):
    return None


fb.FreeFireBypass._mutate_login_body = _noop_mutate_login_body
fb.FreeFireBypass._inject_mobile_identity = _noop_inject_mobile_identity

# Same-length scrub for tool names that could leak into telemetry bodies.
_EXTRA_SCRUBS = (
    (b"mitmproxy", b"iPhoneOS "),
    (b"xenproxy", b"iPhoneOS"),
    (b"okmod", b"apple"),
)
for _find, _replace in _EXTRA_SCRUBS:
    if len(_find) == len(_replace) and (_find, _replace) not in fb._STR_PATCHES:
        fb._STR_PATCHES.append((_find, _replace))

_PROXY_REQUEST_HEADERS = {h.lower() for h in fb._PROXY_HEADERS_STRIP}
_PROXY_RESPONSE_HEADERS = {h.lower() for h in fb._PROXY_RESPONSE_HEADERS_STRIP}


CDN_HOSTS = {
    "core-gmc.freefiremobile.com",
    "core-aw.freefiremobile.com",
    "core-bs.freefiremobile.com",
    "core-th.freefiremobile.com",
    "core-vn.freefiremobile.com",
    "core-sg.freefiremobile.com",
    "core-id.freefiremobile.com",
    "dl-core.cdn.freefiremobile.com",
}


def _strip_request(flow: http.HTTPFlow) -> None:
    host = (flow.request.pretty_host or "").lower().split(":")[0]
    for name in list(flow.request.headers.keys()):
        low = name.lower()
        # Proxy fingerprints are ours: strip them on every intercepted flow.
        if low in _PROXY_REQUEST_HEADERS:
            del flow.request.headers[name]
        # Identity headers are only normalised on the CDN. Login and anti-cheat
        # traffic must keep whatever the real iPhone sent.
        elif low in PC_ONLY_HEADERS and host in CDN_HOSTS:
            del flow.request.headers[name]

    if host not in CDN_HOSTS:
        return
    ua = flow.request.headers.get("User-Agent", "")
    if not ua or any(sig in ua.lower() for sig in TOOL_UA):
        flow.request.headers["User-Agent"] = IOS_UA


def _strip_response(flow: http.HTTPFlow) -> None:
    for name in list(flow.response.headers.keys()):
        if name.lower() in _PROXY_RESPONSE_HEADERS:
            del flow.response.headers[name]


def request(flow: http.HTTPFlow) -> None:
    _strip_request(flow)


def response(flow: http.HTTPFlow) -> None:
    if flow.response is not None:
        _strip_response(flow)


def load(loader) -> None:
    logger.warning("[okmod-stealth] loaded: iOS identity kept, proxy headers stripped")
