#!/usr/bin/env python3
"""Assert the 1106 scope rules.

Rules under test:
  1. PAC only routes CDN hosts to the proxy.
  2. Login / anti-cheat hosts are never intercepted (fail-open tunnel).
  3. No CDN host is also classified as telemetry.
  4. Everything else is refused (not an open proxy).

Run with the XenProxy venv (the guard imports `xenproxy`):

    /opt/xenproxy/venv/bin/python proxy/xenproxy/test_guard.py
"""

from __future__ import annotations

import importlib.util
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
GUARD = ROOT / "proxy" / "xenproxy" / "addons" / "okmod_guard.py"
PAC = ROOT / "proxy" / "pac-1297.js"

spec = importlib.util.spec_from_file_location("okmod_guard_test", GUARD)
guard = importlib.util.module_from_spec(spec)
spec.loader.exec_module(guard)

failures: list[str] = []


def check(name: str, condition: bool) -> None:
    print(("PASS " if condition else "FAIL ") + name)
    if not condition:
        failures.append(name)


pac_hosts = set(re.findall(r'host === "([^"]+)"', PAC.read_text()))
check("PAC has codepatch hosts", bool(pac_hosts))
check("PAC hosts are all CDN hosts", pac_hosts <= guard.CORE_HOSTS)
check("PAC never routes a telemetry host",
      not (pac_hosts & guard.TELEMETRY_HOSTS))
check("CDN and telemetry sets are disjoint",
      not (guard.CORE_HOSTS & guard.TELEMETRY_HOSTS))
check("telemetry hosts are never intercepted",
      not any(guard.is_intercepted(h) for h in guard.TELEMETRY_HOSTS))
check("every CDN host is intercepted",
      all(guard.is_intercepted(h) for h in guard.CORE_HOSTS))
check("telemetry hosts are allowed through",
      all(guard.is_allowed(h) for h in guard.TELEMETRY_HOSTS))
check("unknown hosts are refused", not guard.is_allowed("example.com"))
check("open-proxy test host is refused", not guard.is_allowed("icanhazip.com"))
check("empty host is refused", not guard.is_allowed(""))

# Suffix coverage for regional endpoints.
for host in ("security.ff.garena.com", "loginbp.ggblueshark.com",
             "whatever.garenanow.com", "dl.gmc.freefiremobile.com"):
    check(f"suffix covers {host}", guard.is_allowed(host) and
          not guard.is_intercepted(host))

if failures:
    print(f"GUARD TEST: {len(failures)} failure(s)")
    sys.exit(1)
print("GUARD TEST: PASS")
