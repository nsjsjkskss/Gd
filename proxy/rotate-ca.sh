#!/usr/bin/env bash
# Rotate the 1106 MITM CA to a neutral name.
#
# Why: the current CA is the stock mitmproxy root, so iOS shows a profile /
# trusted root literally named "mitmproxy". Forensic checklists (including the
# public a4ther scanner) match "mitmproxy" as a cheat/injector keyword.
#
# Usage:
#   ./rotate-ca.sh              # generate proxy/conf-next, do not switch
#   ./rotate-ca.sh --apply      # backup conf, install, restart, print steps
#
# After --apply every device must install and trust the new certificate once,
# then remove the old "mitmproxy" profile. Until it does, CDN TLS fails and the
# game falls back to the official codepatch (no ban risk, cheat just off).
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
CONF="$ROOT/conf"
OUT="$ROOT/conf-next"
APPLY=0
[ "${1:-}" = "--apply" ] && APPLY=1

CN="Local Device Trust CA"
ORG="Local Device Trust"

rm -rf "$OUT"
mkdir -p "$OUT"

openssl genrsa -out "$OUT/xenproxy-ca.key" 2048 >/dev/null 2>&1
openssl req -x509 -new -key "$OUT/xenproxy-ca.key" -sha256 -days 3650 \
  -subj "/O=$ORG/CN=$CN" \
  -addext "basicConstraints=critical,CA:TRUE,pathlen:0" \
  -addext "keyUsage=critical,keyCertSign,cRLSign" \
  -addext "subjectKeyIdentifier=hash" \
  -out "$OUT/xenproxy-ca-cert.pem" >/dev/null 2>&1

# XenProxy expects key first, then certificate (same layout as mitmproxy-ca.pem).
cat "$OUT/xenproxy-ca.key" "$OUT/xenproxy-ca-cert.pem" > "$OUT/xenproxy-ca.pem"
chmod 600 "$OUT/xenproxy-ca.pem" "$OUT/xenproxy-ca.key"
chmod 644 "$OUT/xenproxy-ca-cert.pem"

if [ -e "$CONF/mitmproxy-dhparam.pem" ]; then
  cp "$CONF/mitmproxy-dhparam.pem" "$OUT/xenproxy-dhparam.pem"
fi

echo "generated in $OUT"
openssl x509 -in "$OUT/xenproxy-ca-cert.pem" -noout -subject -issuer -dates
echo "sha256: $(openssl x509 -in "$OUT/xenproxy-ca-cert.pem" -noout -fingerprint -sha256 | cut -d= -f2)"

if [ "$APPLY" -ne 1 ]; then
  echo
  echo "not applied. review, then run: $0 --apply"
  exit 0
fi

TS="$(date -u +%Y%m%dT%H%M%SZ)"
cp -a "$CONF" "$ROOT/conf.bak-$TS"
for f in xenproxy-ca.pem xenproxy-ca-cert.pem xenproxy-dhparam.pem; do
  rm -f "$CONF/$f"
done
cp "$OUT/xenproxy-ca.pem" "$CONF/xenproxy-ca.pem"
cp "$OUT/xenproxy-ca-cert.pem" "$CONF/xenproxy-ca-cert.pem"
[ -e "$OUT/xenproxy-dhparam.pem" ] && cp "$OUT/xenproxy-dhparam.pem" "$CONF/xenproxy-dhparam.pem"
systemctl restart okmod-xenproxy
sleep 2
systemctl is-active okmod-xenproxy || true

cat <<EOF

applied. backup: $ROOT/conf.bak-$TS

On each iPhone:
  1. AirDrop / open: $CONF/xenproxy-ca-cert.pem
  2. Settings -> Profile Downloaded -> Install
  3. Settings -> General -> About -> Certificate Trust Settings
     -> enable full trust for "$CN"
  4. Settings -> General -> VPN & Device Management
     -> remove the old "mitmproxy" profile
  5. Re-open Free Fire once so the codepatch is fetched again

Verify: python3 proxy/xenproxy/verify.py --proxy http://127.0.0.1:1106
EOF
