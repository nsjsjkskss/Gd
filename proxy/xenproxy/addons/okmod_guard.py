"""okmod_guard.py — CDN-only scope for the 1106 listener.

The PAC routes only the FreeFire hotupdate hosts here, but a proxy port is
still reachable by anything that knows the address. This addon enforces the
scope at the proxy itself.

Two different allow sets, on purpose:

  INTERCEPT  CORE_HOSTS + STATUS_HOSTS
             TLS is terminated and okmod_patch may rewrite the codepatch
             manifest. Only CDN hosts belong here.

  TUNNEL     TELEMETRY_HOSTS + TELEMETRY_SUFFIXES
             Login, anti-cheat and analytics hosts. They are allowed through
             untouched and are never intercepted. This is fail-open on
             purpose: Garena treats blocked anti-hack upload as "bypassing
             the anti-hack system", and XenProxy's own firebypass notes call
             blocking ANO/TSS/SLS/SGSSR an instant-ban path. If a stale PAC
             or a manual proxy setting sends them here, they must pass.

Everything else is refused, so 1106 cannot be used as a general open proxy.
"""

from __future__ import annotations

import logging

from xenproxy import http

logger = logging.getLogger("okmod.guard")

# Hotupdate / codepatch CDN hosts: TLS is intercepted here.
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

# Health/status host used by verify.py.
STATUS_HOSTS = {
    "okmod.sbs",
    "www.okmod.sbs",
    "proxy.okmod.sbs",
    "origin.okmod.sbs",
    "217.217.255.156",
    "127.0.0.1",
    "localhost",
}

# Login / anti-cheat / analytics endpoints observed in the project HAR and in
# the public protocol dumps. Never intercept, never block.
TELEMETRY_HOSTS = {
    "loginbp.ggblueshark.com",
    "clientbp.ppmainecoonghj.com",
    "prod-api.reward.ff.garena.com",
    "100067.connect.garena.com",
    "100067.msdk.garena.com",
    "networkselftest.ff.garena.com",
    "gin.freefiremobile.com",
    "dl.gmc.freefiremobile.com",
    "vnevent.ggblueshark.com",
    "vnnetwork.ggblueshark.com",
}

# Suffix fallback for regional/login hosts we have not seen yet.
TELEMETRY_SUFFIXES = (
    ".garena.com",
    ".ggblueshark.com",
    ".garenanow.com",
    ".freefiremobile.com",
    ".ppmainecoonghj.com",
)

INTERCEPT_HOSTS = CORE_HOSTS | STATUS_HOSTS


def _host(flow: http.HTTPFlow) -> str:
    try:
        return (flow.request.pretty_host or "").lower().split(":")[0]
    except Exception:
        return ""


def is_telemetry(host: str) -> bool:
    if host in TELEMETRY_HOSTS:
        return True
    return any(host.endswith(suffix) for suffix in TELEMETRY_SUFFIXES)


def is_intercepted(host: str) -> bool:
    return host in INTERCEPT_HOSTS


def is_allowed(host: str) -> bool:
    """Hosts that may pass through 1106 at all (intercepted or tunnelled)."""
    if not host:
        return False
    return is_intercepted(host) or is_telemetry(host)


def _peer(flow: http.HTTPFlow) -> str:
    try:
        return flow.client_conn.peername[0] if flow.client_conn.peername else "?"
    except Exception:
        return "?"


def _deny(flow: http.HTTPFlow) -> None:
    logger.warning("[guard] denied host=%s client=%s", _host(flow), _peer(flow))
    flow.response = http.Response.make(
        403,
        b"blocked",
        {"Content-Type": "text/plain", "Cache-Control": "no-store",
         "Connection": "close"},
    )


def http_connect(flow: http.HTTPFlow) -> None:
    host = _host(flow)
    if is_telemetry(host):
        # This must never happen with the PAC in place: anti-cheat upload has
        # to leave from the phone IP, not the VPS. Log loudly for diagnosis.
        logger.warning("[guard] TELEMETRY VIA PROXY host=%s client=%s",
                       host, _peer(flow))
    if not is_allowed(host):
        _deny(flow)


def request(flow: http.HTTPFlow) -> None:
    host = _host(flow)
    if not is_allowed(host):
        _deny(flow)
