#!/usr/bin/env python3
"""Verify that the OKMOD XenProxy serves the built codepatch.

Checks, through the live proxy:
  1. /healthz answers
  2. the main ABHotUpdates fileinfo row matches the local payload
  3. the codepatch URL it advertises returns exactly the local gzip file
  4. the decompressed bundle matches the advertised sha1/size

Usage:
  ./verify.py --proxy http://127.0.0.1:1106 --payload build/okmod-R8-AIM-ESP.bin
"""
from __future__ import annotations

import argparse
import base64
import gzip
import hashlib
import subprocess
import sys
import urllib.parse
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DEFAULT_CA = ROOT / "proxy" / "conf" / "mitmproxy-ca-cert.pem"
DEFAULT_PAYLOAD = ROOT / "build" / "okmod-R8-AIM-ESP.bin"
CDN = "https://dl-core.cdn.freefiremobile.com/live/ABHotUpdates/ios/{version}"


def fetch(url: str, proxy: str, ca: Path) -> bytes:
    cmd = ["curl", "-sS", "--fail", "-m", "60", "-x", proxy]
    if ca.exists():
        cmd += ["--cacert", str(ca)]
    cmd.append(url)
    return subprocess.run(cmd, capture_output=True, check=True).stdout


def sha1_b64(data: bytes) -> str:
    return base64.b64encode(hashlib.sha1(data).digest()).decode()


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--proxy", default="http://127.0.0.1:1106")
    ap.add_argument("--payload", type=Path, default=DEFAULT_PAYLOAD)
    ap.add_argument("--cacert", type=Path, default=DEFAULT_CA)
    ap.add_argument("--version", default="1.132.18")
    args = ap.parse_args()

    ok = True

    health = fetch("http://proxy.okmod.sbs/healthz", args.proxy, args.cacert)
    print(f"healthz: {health.decode(errors='replace').strip()}")
    ok &= health.strip() == b"ok"

    packed = args.payload.read_bytes()
    raw = gzip.decompress(packed)
    print(f"payload: {args.payload} raw={len(raw)} packed={len(packed)}")

    base = CDN.format(version=args.version)
    manifest = fetch(base + "/fileinfo", args.proxy, args.cacert).decode("utf-8", "replace")
    rows = [l for l in manifest.splitlines() if l.startswith("codepatch/assembly-csharp-patch,")]
    if not rows:
        print("FAIL: codepatch row missing from fileinfo")
        return 1
    row = rows[0]
    name, raw_sha, raw_size, _zero, gz_sha, gz_size, flag, _zero2 = row.split(",")
    print("row:", row)
    ok &= raw_sha == sha1_b64(raw)
    ok &= int(raw_size) == len(raw)
    ok &= gz_sha == sha1_b64(packed)
    ok &= int(gz_size) == len(packed)
    if not ok:
        print("FAIL: manifest row does not match the local payload")
        return 1

    url = base + "/gameassetbundles/codepatch/assembly-csharp-patch." + urllib.parse.quote(gz_sha, safe="")
    served = fetch(url, args.proxy, args.cacert)
    print(f"served: {len(served)} bytes")
    ok &= served == packed
    ok &= gzip.decompress(served) == raw

    print("PASS" if ok else "FAIL")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
