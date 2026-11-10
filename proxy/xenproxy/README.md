# OKMOD XenProxy (port 1106)

The 1106 codepatch proxy now runs on **XenProxy** instead of mitmdump.
Everything lives in this project; nothing is copied to `/opt`.

```text
proxy/xenproxy/addons/okmod_patch.py   addon (manifest rewrite + patch serve)
proxy/okmod-xenproxy.service           systemd unit (installed by deploy.sh)
proxy/conf/                            CA + dhparam (mitmproxy names + xenproxy symlinks)
proxy/xenproxy/verify.py               end-to-end check of the live proxy
```

## How it works

```text
iPhone PAC  -> only core-*.freefiremobile.com / dl-core CDN goes through :1106
TLS         -> tls_clienthello: anything that is not a core CDN host is
               tunnelled untouched (login/Apple/optional CDN never MITM'd)
fileinfo    -> the single codepatch/assembly-csharp-patch line is replaced
               with sha1/size of build/okmod-R8-AIM-ESP.bin
codepatch   -> served straight from the build file, Range supported
```

Payload hot reload: the addon stats `OKMOD_PATCH` on every CDN request and
reloads when mtime/size changes, so a rebuilt payload is picked up even
without a restart.

## Unit

```text
/etc/systemd/system/okmod-xenproxy.service
/opt/xenproxy/venv/bin/xenproxy dump --listen-port 1106 ...
```

`okmod-keyfeed.service` (mitmdump) is **disabled**; it must not hold 1106.

## Environment

| Variable | Default |
|---|---|
| `OKMOD_PATCH` | `/root/okmod-1297-clean/build/okmod-R8-AIM-ESP.bin` |
| `OKMOD_ONLINE_LOG` | `/root/okmod-1297-clean/logs/aimkill-online.log` |

## Endpoints served by the addon

| URL | Purpose |
|---|---|
| `http://proxy.okmod.sbs:1106/` | status page |
| `http://proxy.okmod.sbs:1106/healthz` | `ok` |
| `http://proxy.okmod.sbs:1106/localconfig.json` | `{"testCodePatch":true}` |
| `http://proxy.okmod.sbs:1106/okmod/aimlog?...` | online aimkill log sink |

## Verify

```bash
python3 proxy/xenproxy/verify.py --proxy http://127.0.0.1:1106
```

Checks `/healthz`, the rewritten `fileinfo` row and that the advertised
codepatch URL returns exactly the local gzip payload (sha1 + bytes).

## Logs

```bash
journalctl -u okmod-xenproxy -f | grep -aE 'OKMOD_PATCH|fileinfo patched|codepatch served'
```

## Rollback engine

The old mitmproxy unit is kept as `proxy/okmod-1297.service` for reference.
To go back:

```bash
systemctl disable --now okmod-xenproxy
systemctl enable --now okmod-keyfeed     # the unit that ran mitmdump on 1106
```
