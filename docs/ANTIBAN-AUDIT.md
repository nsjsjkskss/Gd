# Anti-ban / anti-blacklist audit — what is real, what is marketing

Date: 2026-11-08
Rule for this document: every claim is tied to a source. Anything not verified
is marked **UNVERIFIED** and is not used as a basis for code.

---

## 0. Verdict first

1. There is **no credible open-source anti-ban** for Free Fire. GitHub search
   returns 14 repos for `free fire antiban`; every one checked is an empty
   shell, a renamed panel, or an APK with no source. `free fire device ban
   bypass` returns **0** repos, `free fire hwid` returns 2 unrelated repos.
2. Garena's own FAQ confirms **account + device bans**, says **IP bans are not
   used**, and says guest accounts are **not** a loophole. A patch cannot
   unban anything server-side.
3. The only things a client stack can really do:
   - not get flagged in the first place (behaviour),
   - never block/mock anti-cheat telemetry,
   - not leave local cheat artifacts,
   - not corrupt the device identity the client reports.
4. "Hiding" flags client-side (XenProxy zeroes `is_hacker`, special-pool and
   emulator fields in login responses) changes **what the client displays**,
   not what Garena stores. It is not an unban.

---

## 1. Primary sources

### 1.1 Garena official

Anti-Hack FAQ (`ffsupport.garena.com/hc/en-us/articles/4412913831194`):

> Q4: ... we will permanently ban their accounts used for cheating. **Devices
> used for cheating will also be banned** from playing Free Fire again using
> any other accounts.
>
> Q5: **IP ban is not an effective measure** because cheaters may use public
> WiFi ... We are aware of the **Guest account exploit** and are working
> continuously to counter this loophole. Currently, we are banning accounts
> immediately after misconduct.

Account Ban page (`.../4412913789978`):

> 7. **Bypassing Free Fire anti-hack system via illicit local data transfer.**

That last line is the reason the proxy must never block or alter anti-cheat
upload: blocking is itself a listed ban reason.

### 1.2 Protocol dump — `dzinn88x/ff-server-protocols`

`01_login.proto`, `LoginReq` (client → server) carries the device fingerprint:

| Tag | Field | Tag | Field |
|---:|---|---:|---|
| 19 | `device_id` | 43 | `imei_a` |
| 24 | `device_type` | 44 | `meid_a` |
| 25 | `device_model` | 45 | `subscriber_a` |
| 35 | `serial_a` | 47 | `device_a` |
| 36 | `board_a` | 48 | `product_a` |
| 37 | `brand_a` | 49 | `model_a` |
| 38 | `fingerprint_a` | 50 | `bootloader_a` |
| 39 | `hardware_a` | 54 | `is_emulator` |
| 40 | `manufacturer_a` | 56 | `is_root` |
| 41 | `network_operator_a` | 57 | `signature_md5` |
| 42 | `network_type_a` | 58 | `sensor_info` |
| | | 59 | `emulator_score` |

Same fields appear again in `COW.EventLogger.EventLoginInfo` (login analytics).

`proto.BlacklistInfoRes`:

```proto
message BlacklistInfoRes {
    proto_EAccount_BanReason ban_reason = 1;
    System_UInt32 expire_duration = 2;
    System_UInt32 ban_time = 3;
}
```

`40_hacker.proto`:

```proto
message HackerTagInfo { uint32 hacker_tag = 1; int64 hackerpool_time = 2; }
message HackerInfo    { uint32 hacker_cdt_id = 1; uint32 hacker_tag = 2; int64 hackerpool_time = 3; }
```

Consequences:

- device bans are technically possible because the client reports stable
  device identifiers at login;
- the ban/hacker-pool state is **server data returned to the client**
  (`BlacklistInfoRes`, `HackerTagInfo`) — the client cannot clear it;
- `41_mem_check` and `43_emulator` exist as message families; `33_client_info`
  has `Architecture` ARM/ARM64/X86 and channel enums, so a real iOS identity
  must not claim Android/x86.

### 1.3 Local HAR + endpoint schema (this project)

`docs/HAR-20261026-181109-FREEFIRE-URLS.md`:

- main game API: `clientbp.ppmainecoonghj.com` (102 requests, all HTTP 200);
- `CheckHackBehavior`, `GetReportAndPenalty` are ordinary observed calls on
  that host.

`docs/HTTP-ENDPOINT-SCHEMAS.md` (`CheckHackBehavior`):

- request `CSCheckHackBehaviorReq{ MatchClientInfo client_info }` with
  `tpsdk_str`, `file_exception`, `lib_result`, `native_result`, `scan_count`,
  `exception_count`, `client_version`, `channel_type`, `architecture_type`;
- response `CSCheckHackBehaviorRes{ repeated HackWarningInfoDesc hack_warning }`;
- handler behaviour: **empty warning list clears local `isHacker`**, any
  warning sets it;
- the same document explicitly warns: do not treat this as the only
  anti-cheat check, a server ban decision, or a way to clear server state.

So the client has a visible "hacker warning + repair" flow. Blocking the call
prevents the repair path and looks like anti-hack tampering; spoofing the
request is worse.

### 1.4 XenProxy `firebypass` (the module people call "antiban")

Verified in `xenproxy/addons/firebypass.py` (v1.0.12):

| Function | What it actually does |
|---|---|
| `_mutate_login_body` | AES-CBC decrypts the login body, forces protobuf fields 23/99/100 to a random mobile platform, sets client variant 93 = `android`, form factor `Handheld`, re-encrypts |
| `_patch_binary_response` | Zeroes claimed `is_hacker`, `emulator_score`, `is_in_special_pool`, `is_in_emulator_pool`, `platform_type`, `is_emulator`, `is_cs_special_pool`, device-type and match-stats emulator flags |
| `_scrub_telemetry_strings` | Same-length replacement of `is_emulator=1` → `0`, etc. |
| `device_gen.py` | Generates a per-IP Android fingerprint (brand/model/build/UA/locale) from 12 brands; ~10^14 combinations |
| `BLOCKED_PATH_KEYWORDS` | Blocks emulator-detection paths (`GetEmulator`, `DeviceBan`, `PatchCheck`, ...) — but the file warns never to block ANO/TSS/SLS/SGSSR/`security.ff` |

This is a **PC/emulator Android** pipeline: it makes a PC client look like a
phone. It is not usable on a real iPhone — injecting a Dalvik UA and Android
platform fields into iOS login traffic is itself a mismatch, and it requires
MITM of the login host.

### 1.5 GitHub reality check

| Query | Result |
|---|---|
| `free fire antiban` | 14 repos; checked `defta56/FreeFire-Antiban-Injector` = 7.1 MB APK + 73-byte README; `WessznSX/...` = 113-byte README; `bi-love-si/BISI-Free-Fire-Hack` = tiny WinForms demo |
| `free fire device ban bypass` | 0 repos |
| `free fire hwid` | 2 repos, unrelated (a Lua script and a C# panel skeleton) |

No repository exposes a working device-ban or blacklist bypass. Claims like
"100% ANTIBAN" are marketing text, not code.

### 1.6 GitHub MCP code search — the actual "antiban" code

Searches used: `free fire antiban`, `free fire device ban`, `free fire bypass`,
`free fire unban`, `free fire hwid`, `free fire emulator bypass`,
`free fire guest account`, plus code search for `AnoSDK Ioctl`,
`hackerpool_time`, `is_in_special_pool`, `MatchmakingSussNtf`, `libanogs`.

**a) libanogs / ACE (Android)**

`Lixense/ff-ace-anticheat-analysis`, `analysis/libanogs/notes/01_exports.md`:

- `libanogs.so` exports 21 symbols; JNI table on `com/gamesafe/ano/AnoSdk`.
- `AnoSDKGetReportData` / `AnoSDKGetReportData2..4` pull detection report
  buffers; JNI `senddatatosvr` uploads them to the game server.
- Documented runtime result: neutering `AnoSDKInit` with `return 0`
  **deadlocks the Free Fire boot sequence at resource chunk 1 (ANR)** because
  the client waits for the `tss_sdk_send_data_to_svr` callback.

→ Blocking or breaking the anti-cheat SDK does not "disable detection"; it
breaks the game and is trivially visible.

`ScxttVX/Bypass-Check-Emulador-FreeFire` (C, 41★) reverses the emulator query:

- `GCloud_AnoSDK_AnoSDK__Ioctl(10, "files_dir=/data/data/com.dts.freefireth/files|wait=1")`
- query `10` = `IsEmulator`; the bypass returns an empty string so
  `IsNullOrEmpty` stays true.

UnknownCheats threads (`523608`, `593761`, `673866`) show the same case-10
patch and a "fix ban" offset, and the 2024 thread states plainly: *"Almost
everything on UC is detected and they will patch anything new asap"*.

**b) Special pool / "blacklist" hiding**

`soneca7/FreeFireServer`, `protocol/protos/tcp.proto`:

```proto
message MatchmakingSussNtf {
  ...
  bool is_in_special_pool = 14;
  ...
  bool is_in_emulator_pool = 22;
}
```

`sahilneverdies/winsock-redirect`, `BypassDll.cpp` (the Xenitronix/WinSock
lineage that XenProxy's `firebypass.py` mirrors) patches exactly these:

```cpp
case 14: // is_in_special_pool
case 22: // is_in_emulator_pool
case 58: // is_cs_special_pool  → data[i] = 0;
```

It also zeroes `is_hacker` (field 6 of `MatchmakingPlayer`). Two facts matter:

1. the field numbers are real (validated against `tcp.proto`);
2. `PatchBuffer` is the **SEND** path, so these server→client flags are patched
   in outbound bytes by pattern matching. That is not a protocol-aware edit and
   can hit unrelated fields with the same byte pair.

Even when applied correctly, zeroing `is_in_special_pool` changes only what the
client displays; the server already placed the account in that pool.

**c) "Unban IMEI / serial / model" scripts**

A public GameGuardian Lua menu (`unknowncheats` Lua source thread) exposes
`DesbaneaR ImeI`, `DesbaneaR SeriaL`, `DesbaneaR Model`, implemented as hardcoded
`libil2cpp.so` byte patches at fixed offsets. That is the real Android method:
patch the getters so the client reports different identifiers. It is
version-specific, and iOS has no IMEI/serial API to patch.

**d) Account status checkers**

`ISMAILdz13/FreeFireGuestChecker` does guest OAuth → `MajorLogin` →
`AccountPersonalShowInfo` and reads `credit_score_info`. Its classification is
heuristic, not documented:

- `credit.status >= 100` → `BLACKLISTED`
- HTTP 400/401/403 from `MajorLogin` → `BANNED`

Those thresholds are **UNVERIFIED** (403 can be region/token, and no enum dump
backs `>= 100`). Do not treat it as an authoritative ban check.

`kaifcodec/freefire-jwt-generator-api` reproduces the guest OAuth + protobuf +
AES-128-CBC flow. It proves guest accounts are scriptable; it does not make them
ban-resistant — Garena's FAQ says the guest loophole is handled by immediate
bans.

**e) iOS forensic checklist**

`lovelyoyk/a4ther` is an anti-cheat **scanner**, not a bypass. Its iOS section
checks: jailbreak paths, Configuration Profiles, Substrate/tweaks, Frida,
sideloaded apps (TrollStore/AltStore/iGameGod), Free Fire bundle id + App Store
signature, suspicious processes, and HWID via `ioreg`.

The repo ships encrypted one-liner scripts (`a4ther.sh.enc`,
`a4ther-ios.js`); **do not run it**. The value is the checklist: it is what a
human/tournament check looks for. Our stack adds a trusted CA certificate and a
Wi-Fi proxy setting; it does not add jailbreak, Substrate, Frida or a sideloaded
client.

### 1.7 Forums

- UnknownCheats: emulator bypass = patch `libanogs` case 10; "fix ban" offset
  posts exist but are version-specific and publicly patched.
- UnknownCheats 2024: spoof every device parameter rather than patch memory;
  "almost everything on UC is detected".
- MPGH Free Fire threads: *"there is no public working script/hack for Free Fire
  that's actually safe"* — downloads are fake, malware, or fast-ban.
- Unban repos on GitHub (8 hits) are landing pages/READMEs, no working tool.

---

## 2. Claim vs reality

| Claim seen in the scene | Reality | Basis |
|---|---|---|
| "Antiban injector APK" | APK with no source, cannot change server state; "unban tools" are flagged as scams by FF community guides | repo inspection, ban guides |
| "Device ban bypass" | device identifiers are reported at login (`device_id`, `serial_a`, `fingerprint_a`, `imei_a`); changing them requires modifying login traffic or the device | `01_login.proto` |
| "Guest account rotation" | Garena says the guest exploit is known and accounts are banned immediately after misconduct | official FAQ Q5 |
| "IP ban bypass" | Garena says IP bans are not used; VPN/datacenter IPs can still look suspicious | official FAQ Q5 |
| "Remove hacker pool / blacklist" | pool/ban state is server data (`HackerTagInfo`, `BlacklistInfoRes`); client patches can only hide the UI | `40_hacker.proto`, `01_login.proto` |
| "Hide emulator/PC flags" | XenProxy does hide claimed response fields, but only for a login it MITMs; server state is unchanged | `firebypass.py` |
| "Anti-cheat bypass" | blocking ANO/TSS/SLS/SGSSR/`CheckHackBehavior` is a listed ban reason ("bypassing anti-hack via illicit local data transfer") | official Account Ban page |

---

## 3. What was upgraded in this pass

### 3.1 Telemetry fail-open in the proxy guard

`proxy/xenproxy/addons/okmod_guard.py` now separates two sets:

- **INTERCEPT**: CDN hosts + status hosts (TLS terminated, codepatch rewrite);
- **TUNNEL**: login / anti-cheat / analytics hosts, passed through untouched,
  never intercepted.

Telemetry hosts are taken from the project HAR and the protocol dump:
`clientbp.ppmainecoonghj.com`, `loginbp.ggblueshark.com`,
`100067.connect.garena.com`, `100067.msdk.garena.com`,
`prod-api.reward.ff.garena.com`, `networkselftest.ff.garena.com`,
`gin.freefiremobile.com`, `dl.gmc.freefiremobile.com`,
`vnevent.ggblueshark.com`, `vnnetwork.ggblueshark.com`, plus suffix fallback
`.garena.com`, `.ggblueshark.com`, `.garenanow.com`, `.freefiremobile.com`,
`.ppmainecoonghj.com`.

Why: a stale PAC or manual proxy setting must never turn anti-cheat upload into
a 403. Blocking it is a ban reason; tunnelling it is harmless. Everything
outside both sets is still refused, so 1106 is not an open proxy.

Tested live:

```text
icanhazip.com            -> CONNECT 403 (blocked)
clientbp.ppmainecoonghj  -> tunnel (upstream 503, not blocked)
loginbp.ggblueshark      -> tunnel (upstream 503, not blocked)
www.garena.com           -> 302 (tunnel)
core CDN verify          -> PASS
```

### 3.2 Identity scoping in the stealth addon

`okmod_stealth.py` now only rewrites User-Agent and removes PC-only identity
headers on CDN hosts. Login / anti-cheat flows keep exactly what the iPhone
sent; only our own proxy fingerprints are stripped. No body scrubbing is
active (`ff_bypass=false`).

### 3.3 Invariants enforced before deploy

`proxy/xenproxy/test_guard.py` (called from `deploy.sh` step 5/7) asserts:

- PAC routes only CDN hosts;
- no telemetry host is in the PAC or in the intercept set;
- CDN and telemetry sets are disjoint;
- telemetry hosts are allowed to tunnel and never intercepted;
- unknown hosts (including `icanhazip.com`) are refused.

Current result: 14/14 PASS.

### 3.4 Hook guard: never touch identity or anti-cheat

`build.py` now runs `assert_hooks_clean(base, candidate)` before serialisation.
It compares the candidate fix table against the base patch and rejects any
**newly added** hook whose method name contains identity or anti-cheat tokens:

```text
device, unique, identifier, imei, serial, macaddress, androidid, idfv,
systeminfo, anogs, anosdk, tsssdk, security, hackbehavior, anticheat, anti_hack
```

Tested with synthetic hooks: `get_deviceUniqueIdentifier` → BLOCKED,
`AnoSDKGetReportData` → BLOCKED, `get_ShowDamageNum` → PASS.

The current build adds exactly six hooks and none match: silent getter,
`GetScatterRate`, `get_ShowDamageNum`, and the three gravity getters.

### 3.5 Unchanged, on purpose

- login and anti-cheat are never MITM'd (PAC direct);
- `ff_bypass`, `ff_mask_fingerprint`, `ff_tcp_enabled` stay false;
- no response field is zeroed, no login body is mutated;
- the patch never touches `device_id`, `serial_a`, `fingerprint_a`, `imei_a`,
  `is_root`, `signature_md5` or any `EventLoginInfo` field.

---

## 4. Explicit unknowns (do not build on these)

| Question | Status |
|---|---|
| Which exact field a device ban is keyed on (`device_id` vs `fingerprint_a` vs `serial_a`) | **UNVERIFIED** — the protocol exposes many identifiers; no source shows the server key |
| Whether iOS Free Fire uses IDFV, keychain, or a server-issued device token | **UNVERIFIED** — Apple documents IDFV reset on vendor-app uninstall, but the game's storage is unknown |
| Whether Garena correlates the CDN exit IP with the account | **UNVERIFIED** — login stays on the phone IP, CDN goes through the VPS |
| Contents of `CheckHackBehavior` `lib_result`/`native_result` blobs | **UNVERIFIED** — do not spoof |
| Any working device-ban bypass | **does not exist** in public sources |

Consequence: there is no honest "device unbrick" feature to ship. The only
real device-level lever is changing the identifiers the client reports, which
requires modifying login traffic or the device — outside a CDN-only patch, and
detectable.

---

## 5. Practical anti-ban rules (all evidence-backed)

1. Never block, mock, or rewrite anti-cheat telemetry — including
   `CheckHackBehavior`, ANO/TSS/SLS/SGSSR, `security.ff`.
2. Keep the real iOS identity; never inject Android/Dalvik fields.
3. Keep login direct; do not MITM it to hide flags — server state is unchanged
   and the tampering risk is real.
4. Behaviour is the actual detection surface: AimKill (`TakeDamage(9999)`),
   silent aim, no-recoil, speed/gravity are what gets accounts flagged.
5. Keep local artifacts clean (already implemented: hashed prefs, legacy key
   cleanup, `localConfig.json` removal, string scrub).
6. If an account is banned, no client tool can reverse it; Garena states bans
   are final and device bans are hardware-level.

---

## 6. Verify

```bash
/opt/xenproxy/venv/bin/python proxy/xenproxy/test_guard.py
python3 proxy/xenproxy/verify.py --proxy http://127.0.0.1:1106
curl -x http://127.0.0.1:1106 https://icanhazip.com -I   # 403
curl -x http://127.0.0.1:1106 https://www.garena.com -I  # tunnel, not 403
```
