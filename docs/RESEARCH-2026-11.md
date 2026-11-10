# Research log — Free Fire ecosystem, anti-cheat, projects, forums

Date: 2026-11-08
Method: browse MCP (real Chrome) + GitHub MCP (search/code/files) + PyPI.
Sites reached: iOSGods, Platinmods, DuckDuckGo Lite, GitHub, PyPI.
Cloudflare blocked UnknownCheats and MPGH during this pass; no attempt was made
to bypass the challenge.

Rule: only findings with a source are listed. Unknowns are marked **UNVERIFIED**.

---

## 1. Ecosystem map (what actually exists)

| Layer | Project / source | What it really is | Verdict |
|---|---|---|---|
| Android MITM | `xenproxy` (PyPI 1.0.12) | mitmproxy fork + `firebypass`: login AES mutation, response field zeroing, device_gen, UID block | PC/emulator pipeline |
| Android MITM (fork) | `shadowfire` (PyPI 1.0.0) | rebrand of XenProxy: same `firebypass`, raw TCP :9999, JWT mutation service :1080, UID sync | same code, different name |
| Android anti-cheat | `libanogs.so` (ACE/AnoSDK) | 21 exports, JNI table `com/gamesafe/ano/AnoSdk`, report upload via `senddatatosvr` | blocking it deadlocks the game (ANR) |
| Android emulator bypass | `ScxttVX/Bypass-Check-Emulador-FreeFire` | patches `AnoSDK__Ioctl` query 10 (`IsEmulator`) | Android only |
| Android packet patch | `sahilneverdies/winsock-redirect` (`BypassDll.cpp`) | zeroes `is_in_special_pool` (14), `is_in_emulator_pool` (22), `is_hacker` (6) by byte pattern on SEND path | client display only; can corrupt unrelated fields |
| Android script cheats | GameGuardian Lua menus | hardcoded `libil2cpp.so` offsets, "DesbaneaR Imei/Serial/Model" | version-specific, Android only |
| Android APK patchers | Platinmods thread 88999 | external menu patcher writes into `contentcache/.../splitedresconfs` | claims "ANTI BANNED", no evidence |
| iOS anti-cheat | **FairGuard** | score flags, `h=SHA256(__TEXT)`, Team ID, GOT checks, App Attest, native probes | the real iOS surface |
| iOS cheats (public) | iOSGods, `34306/ESP_FreeFire`, `zheedevVn/Tweak-FreeFire-ModZhee`, `ngt1338-ship-it/ImguiIos` | jailbreak tweaks or sideloaded IPA mods | trip resign/repack/signature checks |
| iOS "antiban panels" | TikTok / Instagram / APK sites | marketing pages, no source, no mechanism | scam / ban bait |
| Protocol dumps | `dzinn88x/ff-server-protocols`, `soneca7/FreeFireServer`, `nkzshawty/FreeFire2022` | protobuf schemas, UDP TEA+CRC7 match server, auth/login | reference only |
| Ours | official App Store FF + CDN codepatch + IFix | no JB, no sideload, signature intact | structurally different |

---

## 2. FairGuard — iOS anti-cheat (primary source)

iOSGods `topic/221481-fairguard-reversal-free-fire` (sasd1g, 2026-10-16),
reverse of FF iOS 1.132.x.

### 2.1 Score report

Score object getters `0x9711d80–0x97121bc`. JSON keys:

```text
nRoot, nSimu, nAccel, nDebug, nModify, nRepack, nResign, thSigMatch, defense
```

POST `https://gin.freefiremobile.com/ios/.../create` with:

```text
jb, fg_ver, ig1, ig2, pid, h, path, s, bd, cpu, sv, gv, dm, gm, cd, n*
```

- `h` = **SHA256 of `__TEXT`**; sending the hash of a patched TEXT is a ban.
- `cd` = CodeDirectory; official magic `fade0c02`.
- Team ID official: `98573T3W6L`; bundle `com.dts.freefireth`.
- CDHash SHA256-20 `1208f25f6e25499a3a03ccaec74967f97cb89149`,
  SHA1 `d9cf9ae60f2433bf47fe33a29d7122bb1bcc5965`.
- UnityFramework CDHash `6eccef487246d3f8e55d5abb0b34a6d47c3c02e7`.
- `CC_MD5("com.dts.freefireth_<TEAM>")` at `0x9381f90`.
- Provision checks: TeamName / Entitlements / application-identifier via
  CFDictionary (`0xa19ce40`, `0xa19ce10`), on-disk
  `Frameworks/UnityFramework.framework/UnityFramework`.

### 2.2 Hook / debug detection

- FairGuard loads `GOT._syscall` / `GOT._dlsym` and checks function bytes:
  `+4 == 0x31`, `+7 == 0x91`, plus a CFF checker at `0x9457f6c` (34 callers).
- Replacing GOT entries or using software breakpoints → modifier ban.
- `P_TRACED` debugger can also count as a modifier.

### 2.3 Attestation / probes

- App Attest cmds: `0xf2 0xf3 0xf4 0xf6 0xf9 0xfa`.
- Adjacent: `0xed 0xee 0x79 0x7b 0x7c`.
- Native probes: `0x65–0x70`; skipping `0x65`/`0x70` leaves the lobby stuck.

### 2.4 CheckHackBehavior (iOS)

`POST https://clientbp.ppmainecoonghj.com/CheckHackBehavior`, built in
`COW.UIModelSkinModCheck::RequestHackerWarningInfo 0x48e08cc`.

| Field | Meaning |
|---|---|
| 1 | `tpsdk_str` = iOS uid |
| 2 | `start_time` |
| 3 | `file_exception`, XOR with decimal uid |
| 4 | `lib_result` (13 bytes), same XOR |
| 6 | `scan_count` |
| 7 | `native_result` |
| 8 | `client_version` |
| 9 | `channel_type` = 3 |
| 10 | `architectural_type` = 1 |

XOR routine `0x6ce9e24` is plain `out[i] = data[i] ^ key[i % keylen]`, key =
decimal uid. No GOP on this packet; server replies empty.

### 2.5 What a sideloaded IPA has to spoof (thread section 8)

`identifierForVendor`, `advertisingIdentifier`, NSBundle path, environment,
hide the injected dylib, spoof CoreSimulator paths, NSURLSession
dataTask/upload/config, `Log`, `clean_report`, `deliveryFlagIos`,
`deliveryFeatureIos`, `setHTTPBody`, NSJSONSerialization write/parse,
`sanitize_obj`, `h` param, official `cd`, `thSigMatch = 1`, store path,
`getFFSecurityInfo` (spoof JSON + team), plus CC_MD5/CC_SHA1/CC_SHA256 at
`0xa19cc78` / `0xa19cca8` / `0xa19ccc0`.

→ This is the sideload arms race. Our codepatch path avoids all of it: the
App Store app, `__TEXT`, Team ID, CDHash and CodeDirectory stay official.

---

## 3. FairGuard official capabilities (vendor blog)

Sources: `fairguard/fairguard.github.io`, posts
`2023-06-15`, `2023-08-27`, `2026-04-02`, `2026-04-23`, `2026-06-04`,
`2026-08-27`.

| Capability | Detail |
|---|---|
| Anti-debug | ptrace, syscall, sysctl, exception checks; protection code encrypted |
| Anti-jailbreak | installed apps, files, directory permissions |
| Anti-resign | compares embedded signature with runtime signature |
| Anti-modifier | local detection code, protected for integrity |
| Runtime detection | signature, Team ID, Bundle ID, file integrity, rootless, debugger, abnormal dylib, Frida, **key-function hook** |
| Resource integrity | AssetBundle loaded with integrity check; hot-update resources covered |
| Hot-update DLL | HybridCLR whole-DLL and function-level encryption; functions decrypted on demand |
| Metadata | `global-metadata.dat` encryption, obfuscation, structure virtualization |
| AI aim detection | view-rotation trajectory, shot-decision latency, micro-jitter spectrum; time-space dual-modal model; vendor claims 96.5% detection, <0.3% false positives |

The AI-aim paragraph matters more than any file check: FairGuard sells
**behavioural** aim detection. No string rename or proxy trick changes it.

---

## 4. What this means for OKMOD 1106

### Clean by construction

| Check | Why clean |
|---|---|
| `nRoot`, jailbreak | device not jailbroken |
| `nSimu`, `nAccel` | real iPhone |
| `nDebug` | no debugger |
| `nRepack`, `nResign` | official App Store install, never re-signed |
| `thSigMatch` | Team ID `98573T3W6L`, official CDHash |
| `h = SHA256(__TEXT)` | we never modify the Mach-O; codepatch is downloaded content |
| GOT `_syscall`/`_dlsym` | IFix patch does not hook GOT |
| App Attest / probes | gin and probe traffic are not MITM'd; PAC routes only CDN |
| `CheckHackBehavior` | forwarded untouched (PAC direct, guard fail-open) |

### Open unknowns

1. **`nModify` vs IFix codepatch** — FairGuard advertises key-function hook
   detection. Does it inspect IL2CPP method bodies replaced by the hotfix
   system? **UNVERIFIED** — no public evidence either way.
2. **AssetBundle integrity for the codepatch** — official docs say AB resources
   are integrity-checked at load, including hot-update. Our plain bundle works,
   so either this path is not checked or the client-side manifest satisfies it.
   **UNVERIFIED** whether the server compares the manifest/hash.
3. **Garena server-side manifest check** — the fileinfo we rewrite is what the
   client trusts; a server-side official-hash comparison would still catch it.
   **UNVERIFIED**.

### Residual forensic exposure (a4ther)

- Wi-Fi proxy enabled (`HTTPProxy`/`HTTPSProxy` in sysdiagnose).
- Custom root CA profile (currently named `mitmproxy`; rotate script exists).
- Both are inherent to MITM and not part of FairGuard's score keys.

---

## 5. Projects / forums catalog

| Source | Finding |
|---|---|
| `xenproxy` PyPI | 1.0.12; `firebypass` Android pipeline; warns never to block ANO/TSS/SLS/SGSSR |
| `shadowfire` PyPI | 1.0.0; same code rebranded; UID blocking + JWT mutation service |
| `Lixense/ff-ace-anticheat-analysis` | `libanogs` exports; neutering `AnoSDKInit` → boot ANR |
| `ScxttVX/Bypass-Check-Emulador-FreeFire` | Ioctl query 10 = IsEmulator |
| `sahilneverdies/winsock-redirect` | `BypassDll.cpp`: special-pool/hacker field patches, SEND-path byte patterns |
| `ISMAILdz13/FreeFireGuestChecker` | guest OAuth → credit score; `status >= 100` = blacklist is heuristic (**UNVERIFIED**) |
| `kaifcodec/freefire-jwt-generator-api` | guest token generation; does not make guest ban-resistant |
| `34306/ESP_FreeFire` | iOS JB ESP source for 1.93.1; requires jailbreak |
| iOSGods 221481 | FairGuard reversal (section 2) |
| iOSGods 163775 / 204110 | iOS ESP sources (JB) |
| Platinmods 88999 | Android patcher writing `splitedresconfs`; "ANTI BANNED" claim unproven |
| TikTok/Instagram "antiban panel iOS" | marketing only |
| UnknownCheats / MPGH | Cloudflare-blocked this pass; earlier snippets say no safe public FF cheat |
| Garena FAQ | account + device bans; no IP bans; guest loophole handled; blocking anti-hack is a ban reason |

---

## 6. Action list from this research

1. Keep the official app untouched; never sideload/re-sign. The codepatch path is
   the main structural advantage over every public iOS cheat.
2. Keep hooks minimal. `assert_hooks_clean` already blocks identity/anti-cheat
   hooks; next step is to review whether the 6 existing hooks are worth the
   `nModify` surface.
3. Do not touch FairGuard cmds/probes/App Attest or `CheckHackBehavior`. PAC
   direct + guard fail-open already enforce this.
4. Treat AimKill as the top risk: FairGuard explicitly markets behavioural aim
   detection. File/string hygiene does not help against it.
5. Forensic posture: proxy + CA are visible; clean-state procedure only.
6. Watch three unknowns: `nModify`, codepatch AB integrity, server-side manifest
   hash. Any of them can only be resolved by observation on-device or a newer
   leak, not by guessing.


---

## 7. Final model (after the second pass)

### 7.1 FairGuard capability taxonomy (official product + blog)

| Feature (vendor name) | What it detects | Evidence |
|---|---|---|
| 反修改器 (anti-modifier) | GG, 八门神器, iMemEditor/iGameGuardian and variants; behavioural check of value-change frequency, movement distance, attack interval, resource growth | product/42, news/206, news/319 |
| 反加速器 (anti-speed) | gettimeofday/clock_gettime/timeScale tampering; compares multiple time sources | news/319 |
| 反注入器 (anti-inject) | Xposed, Frida, dylib injection; loaded modules, abnormal executable memory, key functions | product/42, news/319 |
| 反调试 | ptrace, syscall, sysctl, exception checks | 2023-06-15 post |
| 反越狱 / ROOT | files, installed apps, directory permissions, runtime signals | news/261 |
| 反重签 / 反重打包 | embedded vs runtime signature, Team ID, Bundle ID, file integrity | 2023-06-15, 2026-08-27 |
| 反模拟器/虚拟机/云手机 | environment fingerprints | product/42, news/261 |
| 反抓帧 | screen-capture / frame-grab tools | product/42, help changelog V2.0.5 |
| AI 行为检测 | aim trajectory, shot latency, micro-jitter; vendor claims 99.99% | product/42, 2026-04-23 |
| DMA 硬件检测 | external DMA hardware | product/42 |
| AB/hot-update protection | AssetBundle encryption incl. hot-update, integrity check at load | 2026-06-04, 2026-08-27 |
| HybridCLR DLL protection | whole-DLL + function-level encryption, on-demand decrypt | 2026-04-02 |

### 7.2 How the score flags map

| Flag | Most likely meaning | Basis |
|---|---|---|
| `nRoot` | jailbreak | name + news/261 |
| `nSimu` | simulator | name |
| `nAccel` | accelerometer/emulator signal | name |
| `nDebug` | debugger (`P_TRACED`) | name + thread note |
| `nModify` | **modifier behaviour** (memory/value/rate tampering), not a code patch | vendor anti-modifier description + behavioural metrics |
| `nRepack` | repacked bundle | name + FairGuard resign docs |
| `nResign` | re-signed app | name + FairGuard resign docs |
| `thSigMatch` | Team ID / signature match | thread section 7 |
| `defense` | internal protection state (hook/integrity) | name only — **UNVERIFIED** |

Important correction from this pass: `nModify` should not be read as
"IFix patch detected". FairGuard's own anti-modifier material describes
behavioural memory/value detection. Code/hook detection is described
separately (loaded modules, abnormal executable memory, key functions) and may
feed `defense` or a separate check. Which one catches an IFix body replacement
is **UNVERIFIED**.

### 7.3 Local evidence from this machine

- `version.ggwhitehawk.com` response (captured 2026-11):
  `abhotupdate_check = "cache_res;assetindexer;SH-Gpp;assembly-cssharp-patch"`,
  `should_check_ab_load = false`, `ggp_url = gin.freefiremobile.com`,
  `hotfile_force_update`, `vercfg_sign`.
  → the client knows the `assembly-cssharp-patch` hotupdate type; whether it
  validates it against official bytes or only against the supplied fileinfo is
  **UNVERIFIED**.
- `DumpSDK/Global/HttpVerInfo.h` (`LegiteriumZ/FreeFire-SDK`) confirms the
  fields `should_check_ab_size`, `ggp_url`, `abhotupdate_check`,
  `hotfile_force_update`, `vercfg_sign`.
- GIN capture `hideReportIos` (2026-11): body is encrypted; the `tk` JWT carries
  `key` (16 bytes hex) and `nonce` (32 bytes). AES-128-CBC/CTR/GCM attempts with
  those values failed, so the scheme is **not** the known CGP AES key/IV.
  Without the client routine the `n*` values cannot be read.
- `FreeFire_1.132.1.ipa` → `UnityFramework` is a Mach-O with `cryptid 0`, but
  `gin.freefiremobile`, `hideReport`, `nModify`, `thSigMatch`, `FairGuard`,
  `CheckHackBehavior` are **absent as plaintext** → FairGuard string encryption
  is active. Static resolution of the score setters is blocked without an
  unpacked dump.

### 7.4 Competing iOS approach found

`bts2019amo-ctrl/assembly-proxy-ios-remote-menu` (FF iOS 1.132.10):

- mitmproxy on :7777 serving a custom `assembly-csharp-patch` (74 IFix),
- rewrites the same `codepatch/assembly-csharp-patch` fileinfo line,
- remote menu on :8080, state written to **`localConfig.json`**
  (`testCodePatch`, `esp`, `aim`, `team_esp`, ...).

Same architectural family as ours, but with the artifact we removed
(`localConfig.json`) and no string/field/pref hygiene. This confirms both the
viability and the usual detection residue of the codepatch approach.

### 7.5 Risk ranking for our features

| Feature | Mechanism | Detection path | Risk |
|---|---|---|---|
| AimKill | `TakeDamage(9999)` / kill rate | server-side damage validation + behavioural | **critical** |
| Silent aim | getter hook + perfect aim | key-function hook / aim trajectory model | **high** |
| Speed / jump / gravity | movement values | movement-distance and time-source checks | high |
| No recoil | scatter getter | spray-pattern statistics | medium |
| ESP | read-only draw | none server-side; only memory/hook integrity | low |

### 7.6 What is still unknowable from public sources

1. Which FairGuard check (if any) observes IFix method-body replacement.
2. Whether the `assembly-cssharp-patch` hotupdate is integrity-checked against
   official bytes, or only against the fileinfo we serve.
3. The actual `n*` values Garena receives for our device.

Resolving any of them needs one of: a FairGuard-unpacked binary, Frida on a
test device to instrument the score getters, or a server-side leak. Public
browsing has reached its limit; further "research" would be guessing.
