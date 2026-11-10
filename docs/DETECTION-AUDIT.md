# Free Fire detection-surface audit — OKMOD 1106 / IFix patch

Date: 2026-11-08 (second pass)
Scope: patch payload, hooks, fields, strings, PlayerPrefs, on-device files,
XenProxy 1106, PAC scope, protocol/telemetry, behaviour.

References:

- XenProxy 1.0.12 — `addons/firebypass.py`, `addons/device_gen.py`.
- `dzinn88x/ff-server-protocols` — 47 proto families incl. `40_hacker`,
  `41_mem_check`, `43_emulator`, `33_client_info`, `46_crypto`.
- `nkzshawty/FreeFire2022` — client `Local Config/localConfig.json`
  (`{"verAddr":"http://…/live/"}`), auth/login split.
- `soneca7/FreeFireServer` — Go match server: TEA + CRC7, cmd 106
  `RUDP_TAKE_DAMAGE` carries **client-reported** damage, per-source UDP guard.
- Current payload `build/okmod-R8-AIM-ESP.bin` and XenProxy unit.

---

## 1. What changed in this pass

| # | Finding | Fix |
|---|---|---|
| 1 | PlayerPrefs keys were plaintext cheat names (`u3d.q.Aim.AimKill`, `u3d.q.Esp.Skeleton`, `u3d.q.Combat.NoRecoil`, plus legacy unprefixed `ESP`, `Máu`, `Lam`, `EnableTeamESP`, `EspFire`, `EspFireBlue`). Anti-cheat reading `com.dts.freefireth.plist` sees the full feature list. | All pref keys renamed at build to `u3d.q.<8 hex>` (112 keys, zero descriptive). Legacy unprefixed keys migrated to `OKMOD.*` first, then hashed. |
| 2 | Old `OKMOD.*` keys from previous builds stay in the plist forever. | One-shot `CleanupLegacyResidue()` on first HUD frame deletes all 111 legacy keys (names stored XOR-obfuscated, never plaintext in payload). |
| 3 | `Documents/localConfig.json` (ARIFI residue, `verAddr` redirect vector). | Same cleanup deletes `Application.persistentDataPath + "/localConfig.json"` once. |
| 4 | Cheat strings still interned: `AimKill`, `Silent Aim`, `SILENT: …`, `aimbot`, `No recoil`, `EspFire`, `EnableTeamESP`, plus old strings baked into `menuonly.patch` (`EspFire / tia native…`). | Build-time intern scrub (exact + token fallback), applied to the merged patch including base strings; build assert now fails on any of these. |
| 5 | Port 1106 was a fully open proxy. Journal showed a third party (`204.76.203.177`) using it for `icanhazip.com` while auditing. | New `okmod_guard.py`: CONNECT + request allowlist. Non-CDN hosts get 403. Verified: `curl -x 127.0.0.1:1106 https://icanhazip.com` → `CONNECT tunnel failed, 403`. |
| 6 | Status page advertised "XenProxy · codepatch swap · không yêu cầu xác thực IP". | Neutralised to a plain `OK` page. |
| 7 | `deploy.sh` would have dropped the new addon on redeploy. | `proxy/okmod-xenproxy.service` source updated too. |

Current payload:

```text
raw  : 801,915 bytes
gz   : 298,189 bytes
sha  : 6368c24f482f15e80e8da764af3943b64b8a4ba716ffd1227d364c64dd14a22b
row  : codepatch/assembly-csharp-patch,OUHRGXx9G8JXwIf0Xu4Of4spAso=,801915,0,ACjI6b7iNQk0x7nFg1b6dwHQOH4=,298189,True,0
guard: PASS 27/27
proxy: verify PASS
```

Payload scans (intern table): `okmod` 0, `aimlog` 0, `aimkill` 0, `aimbot` 0,
`silent` 0, `hitfly` 0, `espfire` 0, `enableteamesp` 0, `no recoil` 0.
Fields: `R7*`/`Ok*`/`Qx*` 0, `Zx*` 333. Pref keys: 112 × `u3d.q.<hex>`,
0 descriptive (build assert checks the `u3d.q.<8hex>` shape).

---

## 2. Detection-surface matrix

### A. Patch / runtime

| Item | Detection idea | Risk | Status |
|---|---|---|---|
| IFix codepatch row (hash/size) | server compares against official bytes | high | manifest matches our file; only a server-side official-hash check would catch it — not fixable without official bytes |
| Bundle header flags `0x40` vs official `0x243` | loader/AC checks encryption flag | **high** | ours is uncompressed + unencrypted; official is LZ4HC + UnityCN-encrypted. Re-encrypt needs the 1.132.18 key; every local `global-metadata.dat` is 1.132.1 and the brute force (4,430 hex + 27,793 tokens) failed |
| 333 injected fields on `SceneEditBoxSelectTool` | field-table count/pattern anomaly | med | names obfuscated (`Zx…`); count still visible. P2: pack state into arrays |
| 6 added hooks | method-pointer integrity, behaviour | med | `get_LastAimingInfoFromWeapon`, `GetScatterRate`, `get_ShowDamageNum`, `get_CustomGravity`, `get_RisingGravity`, `get_FallingGravity` |
| `OnGUI` redirect | UI hook | low | official UI class |
| String literals | memory/string-table scan | high → **fixed** | 0 cheat tokens after scrub |
| PlayerPrefs keys | plist scan | high → **fixed** | 112 opaque keys |
| Legacy prefs / `localConfig.json` | file scan | high → **fixed** | deleted on first HUD frame |
| Added method names | metadata scan | low | IFix q3 stores them hashed/encoded; none appear as ASCII in the payload |

### B. Device files

| Item | Status |
|---|---|
| `Library/Preferences/com.dts.freefireth.plist` | only `u3d.q.<hex>` keys remain; legacy keys deleted by patch |
| `Documents/localConfig.json` | deleted by patch; do not recreate |
| `Documents/contentcache/.../codepatch/assembly-csharp-patch.*` | expected file; manifest hashes match |
| Proxy CA profile | system trust, app can see the issuer on MITM'd CDN traffic. Remove when not testing |
| Other ARIFI files | not created by this build; remove manually if present |

### C. Proxy / network

| Item | Detection idea | Risk | Status |
|---|---|---|---|
| Open proxy abuse | third-party traffic, IP reputation | med → **fixed** | host allowlist; telemetry hosts fail-open, other hosts 403 |
| Proxy headers / UA | CDN fingerprint | low | stripped, iPhone UA kept |
| Android identity injection | iOS traffic with Android fields | was critical | disabled (`ff_bypass=false`, `ff_mask_fingerprint=false`) |
| TCP 19112 control port | open port scan | — | closed |
| PAC scope | login/AC must stay direct | good | only `core-*` / `dl-core` |
| System proxy visibility | iOS app can call `CFNetworkCopySystemProxySettings` and see a proxy is configured | **cannot hide** | any MITM (proxy or VPN) is visible at the network-config level; only "not checked" assumption |
| TLS issuer | app sees `mitmproxy`/XenProxy CA instead of the real CDN CA | **cannot hide** | game does not pin, but it can log the issuer |
| TLS ClientHello | OpenSSL fingerprint vs iOS | low | CDN-only |
| Exit IP | datacenter reputation | med | CDN only; login uses the phone IP |

### D. Protocol / telemetry

| Item | Evidence | Rule |
|---|---|---|
| `40_hacker.proto` | `HackerTagInfo{hacker_tag, hackerpool_time}`, `HackerInfo{hacker_cdt_id, hacker_tag, hackerpool_time}` — client reports its own tag/pool time | never spoof or block |
| `41_mem_check.proto` | `EMemCheck` family exists (fields not in dump) | never block |
| `43_emulator.proto` | `EEmulator` family exists | real iPhone identity is the only answer |
| `33_client_info.proto` | `ChannelType` GP/SAMSUNG/HUAWEI/THIRDPARTY/DEV/TRAIL, `Architecture` ARM/ARM64/X86 | send real iOS values; never Android |
| `46_crypto.proto` | TEA temp buffers (`dec_k`, `enc_k`, IVs) | match crypto is TEA + CRC7, UDP |
| Match traffic | `FreeFireServer` cmd 106 `RUDP_TAKE_DAMAGE` carries victim, damage, attacker, hit context from the **client** | damage validation is server-side; cheats are caught by behaviour, not by HTTP filtering |
| ANO / TSS / SLS / SGSSR / `CheckHackBehavior` / `security.ff` | live telemetry upload | **never block/MITM** — XenProxy notes call blocking an instant-ban path |

### E. Behaviour (server-side, cannot be patched away)

| Feature | Signal | Risk | Mitigation |
|---|---|---|---|
| AimKill `TakeDamage(9999)` | damage/hit/kill-rate validation; cmd 106 is client-authoritative in the emulator, so retail almost certainly validates | **critical** | disable in release, or scale to weapon damage + delay + miss chance; never in ranked |
| Silent aim getter hook | perfect aim, no misses, headshot rate | high | FOV limit, smoothing, miss chance, fire-only |
| No recoil (`GetScatterRate`) | impossible spray patterns | high | partial reduction only |
| Gravity / high jump / hover / speed | movement validation | high | small values, avoid ranked |
| Infinite ammo / rapid fire | ammo/fire-rate validation | high | off by default |
| ESP only | none server-side | low | — |

---

## 3. What can and cannot be blocked

**Blocked now**

- every plaintext cheat signature in the payload (brand, URLs, feature names);
- every descriptive PlayerPrefs key, including legacy ones on disk;
- `localConfig.json` residue;
- open-proxy abuse of 1106;
- Android identity injection, proxy headers, TCP control port.

**Cannot be blocked from the client**

1. **Bundle encryption flag** — needs the 1.132.18 UnityCN key. Without it the
   payload stays plain `0x40` while official is `0x243`. A loader/AC that
   compares flags sees the difference.
2. **Codepatch hash/size vs official** — only a server-side official-hash check
   would catch it; the client-side manifest is ours and consistent.
3. **System proxy / VPN visibility** — iOS apps can read proxy settings; MITM
   necessarily changes network config and the CDN certificate issuer.
4. **Behavioural detection** — damage, aim, movement and fire-rate statistics are
   computed server-side from match traffic (UDP, TEA/CRC7). No proxy or string
   obfuscation changes them.
5. **Field-count anomaly** — 333 injected fields are structurally visible even
   with obfuscated names.

**Assumption to keep in mind:** the current build is only "quiet" against
signature/string/file scans. It is not invisible to a determined behavioural
or integrity check.

---

## 4. Priority roadmap

### P0 — done in this pass

1. Hash all PlayerPrefs keys (112).
2. Delete 111 legacy keys + `localConfig.json` once.
3. Scrub all cheat strings, including base-patch leftovers; extend the build assert.
4. Host allowlist on 1106 + neutral status page.

### P1 — next

5. AimKill release policy: disable by default, or weapon-scaled damage with
   delay/miss (needs a decision; it is the single highest-risk feature).
6. Re-encrypt the bundle when a 1.132.18 IPA/metadata is available.
7. Reduce injected fields by packing state into a few arrays.

### P2

8. TLS fingerprint / residential upstream for CDN.
9. Re-audit after each game update (field offsets, hook bodies, AC endpoints).

---

## 5. Verify current state

```bash
ss -ltnp | grep -E ':1106|:19112'        # 19112 must be absent
python3 proxy/xenproxy/verify.py          # manifest + payload PASS
./buildkit/tools/core_guard.sh --candidate build/okmod-R8-AIM-ESP.bin \
  --report build/build-aim-report.json --mode r8-aim
curl -x http://127.0.0.1:1106 https://icanhazip.com -I   # must be 403
```

Last run: guard PASS 27/27, proxy verify PASS, open-proxy test 403.


---

## 6. Build profiles (surface reduction, 2026-11-08)

`build.py` now takes `OKMOD_PROFILE`:

| Profile | Added hooks | Bodies | Zx fields | gz size |
|---|---:|---:|---:|---:|
| `full` (default) | 6 | 460 | 333 | 298,189 |
| `nohook` | 0 | 449 | 330 | 295,764 |

`nohook` skips every hook this project adds (silent getter, scatter, damage
number, three gravity getters) and compiles only the `Hud` graph. Guard runs
with `--profile esp` and asserts no risky hook is present (26 checks PASS).

Build:

```bash
python3 build/build.py                      # full -> build/okmod-R8-AIM-ESP.bin
OKMOD_PROFILE=nohook python3 build/build.py # nohook -> build/okmod-nohook.bin
```

Deploy `nohook` live (keeps a backup of the full payload):

```bash
cp -a build/okmod-R8-AIM-ESP.bin backup/patches/patch.full-$(date -u +%Y%m%dT%H%M%SZ).bin
install -m 0644 build/okmod-nohook.bin build/okmod-R8-AIM-ESP.bin
systemctl restart okmod-xenproxy
python3 proxy/xenproxy/verify.py --proxy http://127.0.0.1:1106
```

Note: in `nohook` the menu rows for Silent / no-recoil / jump still exist but the
corresponding hooks are absent, so those toggles are inert. AimKill/Aimbot code
is still compiled (it does not need the removed hooks); use the menu or remove
the feature if you want it gone as well.

Dead-code cleanup in the same pass removed 15 unreachable diagnostic functions
(`Dump*`, `CaptureNetPackets`, `SendJumpLog`, `SendAimKillOnline`,
`UpdateHitFly`, `OnlinePost`, `OnlineEncode`) and 49 field declarations that
were no longer referenced. The serialized payload was unchanged because the
converter already pruned unreachable bodies; the source is now smaller and the
field table carries only fields the live code uses.


---

## 7. Patch size anomaly (2026-11-08)

Official `fileinfo` for `1.132.18` (fetched directly from
`dl-core.cdn.freefiremobile.com`):

```text
codepatch/assembly-csharp-patch,...,197056,0,...,41498,True,0
```

| Build | raw | gz | vs official |
|---|---:|---:|---:|
| official | 197,056 | 41,498 | 1.0x |
| ours before | 801,915 | 298,189 | 4.1x / 7.2x |
| ours after icon strip | 552,363 | 112,227 | 2.8x / 2.7x |

Cause: the base patch string table carried **18 base64 PNG icons
(250,068 chars, ~94% of all intern characters)** from old HUD builds. The
current source (`okmod.cs`, `esp_sampled.cs`) contains no `iVBOR` /
`FromBase64String`, so nothing decodes them.

`build.py::scrub_interns` now blanks any intern string longer than 1000 chars
that starts with `iVBOR`. Result: gz 298,189 -> 112,227 (-62%). Guard PASS,
proxy verify PASS.

Why it matters: `HttpVerInfo` exposes `should_check_ab_size`, and FairGuard
sells AssetBundle/hot-update integrity checks. A patch 4-7x larger than the
official delta is a structural anomaly independent of which cheat features are
enabled, and is a plausible ban signal even when the menu was never used.

Remaining gap (2.7x) is the custom base patch (81 fixes / 331 bodies) versus
the official delta (197 KB). Building on the official patch as base would close
it; the official file is 403 without client auth, so it must come from a
pre-cheat device backup or an authenticated capture.
