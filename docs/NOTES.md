# R8 core + AIM/Silent + advanced ESP

## Build
```text
R8-AIM-ESP-v1
payload sha256  97eec718116ce199d938105b527b48faf233f46d1ee64625ac9be770043b4922
bundle sha256   1e3c75e4ea3a0e4f1a4c188513663b5db9692ed1401b77da6dfd9c3f209b811a
bundle bytes    271429
fixes           82
storeys         10
```

The old crash-prone `Okmod.DrawEsp` body is still present in the patch table
but is **not reachable** from the new HUD. The HUD calls
`SampledEsp.DrawEsp`, which uses the R8 scheduled sampler, cache, rotating scan,
mesh batch and conditional GC.

## Features
| Feature | Path |
|---|---|
| Box / name / HP / distance | R8 cache + mesh batch |
| 3D box | cached record offset, no extra player reads |
| Corner/fill | mesh fill + 8 corner quads |
| Head marker | mesh V marker |
| Target line | mesh line from top center |
| Link / blue / grenade lines | screen-space mesh lines |
| Team ESP | sampler includes teammates, cyan records |
| Target mark | aim target record drawn gold |
| Silent Aim | Player getter hook -> `SilentAimingInfo` |
| Aimbot | HUD `UpdateAimbot`, mutually exclusive with Silent |
| Aim point | head / neck / chest (spine) |

## Static checks
- 138 q3 bodies and 79 q3 fixes unchanged.
- 10 official storeys preserved.
- 82 fixes: menuonly base + Silent getter hook.
- `OnGUI` redirected to the new HUD shim.
- Cache renderer (`Render` and `DrawBatch`) has no `COW.*` calls.
- Old `DrawEsp` body 319 is not reachable from `Hud`.
- All added game externs matched metadata; all mesh API signatures checked.

## Build command
```bash
cd /opt/okmod-sophia/cs/r8-aim
PYTHONPATH=/opt/ilvenv/lib/python3.12/site-packages python3 build.py
```
Requires the recovered kit at `/opt/okmod-sophia/cs/r8/buildkit/` to have
restored `/tmp/opencode`.

## Live safety
Only `/opt/okmod-sophia/var/aimtest/` may be updated for device testing.
The live `okmod` payload must remain:

```text
a0958ad93ce28329f08ba7cb6bc0727f2de2bc506392ad84a4118caf425cd570
```

## Published test channel
`/opt/okmod-sophia/var/aimtest/assembly-csharp-patch.bin` now contains this
candidate. Public manifest row:

```text
codepatch/assembly-csharp-patch,rjL9gP0WVxPGYOCO+UFQTVjM/70=,673899,0,07wDK7lL4gNNpILKbAaCDid1SCc=,271429,True,0
```

Backup made before publishing:

```text
assembly-csharp-patch.pre-r8aim-20261021T024643Z.bin
fileinfo.pre-r8aim-20261021T024643Z.txt
```

Public served hash matched the candidate; live `okmod` still served
`a0958ad93ce28329f08ba7cb6bc0727f2de2bc506392ad84a4118caf425cd570`.

## Core guard (mandatory)
Every future candidate must pass:

```bash
/opt/okmod-sophia/tools/core_guard.sh \
  --candidate <candidate.bin> \
  --report <build-report.json> \
  --mode r8-aim
```

Rules and release checklist: `/opt/okmod-sophia/CORE-RULES.md`.

## Visual/ESP batch (2026-10-21)
Added on top of the R8 cache renderer:

- ESP vehicle: enumerates `JMAGGLCNGIG.LGNHODMMCMJ`, casts `LevelVehicle`,
  projects cached screen points; no vehicle reads in `Render`.
- ESP alert: cached enemy count banner. This is **not** a raycast/wall-check.
- Boxfield: tinted cached-record fill.
- Enemy/team/vehicle color presets; target stays gold.
- Menu rows: Khung đặc, ESP xe, Cảnh báo địch, Màu địch, Màu team, Màu xe.

Candidate:
```text
sha256  0281b56d9fc892585120b035dcb7c59e0712e28c39c497cee5ccb071484563ff
bytes   272915
bodies  379
fixes   82
```

Guard: PASS 27/27. Live unchanged.


## Combat batch (R8.3)
- Added Aimbot lock (always active, bypasses scope/fire gating).
- Added aim point mode 3 = body/hip.
- Aim menu rows: Aimbot lock; point cycle Head/Neck/Chest/Body.
- Candidate 273000 bytes, sha256 706371cc7342bbf5dff0fc52e4262df1755140fa1ce0dea242e8ee43fcf26c91.


## Combat batch 2 (R8.4)
- Added No reload via `Player.get_Attributes().ShootNoReload`, with old value restored when disabled.
- Added Fast swap via `Player.ResetSwapWeaponTime()` while enabled.
- Aim page now scrolls to 360 for the new rows.
- Candidate 273285 bytes, sha256 2ab03c954618b34793a4047b1b224eb355da05f8085854ac6abbd371c2143ec3.


## Scroll fix (R8.5)
- AIM page swipe clamp was still 144; raised to 360 so No reload / Fast swap are reachable.
- ESP page clamp remains 864.
- Menu title R8.5; candidate sha256 fc5c7edc6734cf14495607141dbce1e9625a4ca757b0312e74b78d87a3feac26.


## Misc batch (R8.6)
- Camera 90: writes Camera.set_fieldOfView(90), restores old FOV when off.
- Speed run 1.3x: writes PlayerAttributes.RunSpeedUpScale, restores old value when off.
- AIM page scroll max 504.
- Candidate 273559 bytes, sha256 811444a470db81305b512d8636f4074bfa90857906f076fd9cd55ba748dd5b7a.


## Combat batch 3 (R8.7)
- No recoil: writes PlayerAttributes.SkillScatterRate and SkillScatterRateSighting to 0.1,
  restores both old values when off.
- AIM page scroll max 648.
- Candidate 273750 bytes, sha256 2d819dcf6d3e2524d59c53f42e7937b5995374b60e0791517d93383690bfc856.

## R8.8 — No recoil removed
Field writes `SkillScatterRate` / `SkillScatterRateSighting` did not affect
`PlayerAttributes.GetScatterRate()` on device. BS4484 hooks that method; a
plain field write is not equivalent. Removed from the menu to avoid a fake
toggle. Real no-recoil needs a method hook/trampoline (deferred).

## R8.9 — No recoil via v31 dump
Fresh IPA `/root/migul/ff-ipa` is 1.132.1 with metadata v31. Dumped with
Il2CppDumper-GUI-Fixed core; `GetScatterRate` reads:
- SkillScatterRate at +0x19c
- SkillScatterRateSighting at +0x1a0
- returns max(1.0 + sighting + skillRate, 0.1)

So writing 0.1 gives ~1.2 and does nothing. Setting both fields to -1.0
cancels the base 1.0; GetScatterRate clamps to 0.1. Old values are restored
when the toggle is off.
Candidate 273752 bytes, sha256 0f6bb76b47742cef2072436d1740b0f2be3535b202b373ff04780d775c4dbca6.

## R9.0 — real no-recoil hook (IFix)
Replaced the field-only attempt with an actual method fix on
`COW.GamePlay.PlayerAttributes.GetScatterRate`.
- On: local player returns 0.1.
- Off: reproduces native formula `max(1.0 + sighting + skill, 0.1)`.
- Other players always keep their normal value.
Fixes: 83 (menuonly 81 + Silent + Scatter).
Candidate 273940 bytes, sha256 925afdabb6f37e13b5d9816adbd8aefcdedb62738361d7207cb161bb6b08b70b.

## R9.2 — visible scatter diagnostic
Menu now shows, under the title:
`SC hook: <count> | NR: ON/OFF`
This counts every GetScatterRate hook call. Candidate 274063 bytes,
sha256 c8c4c25e833a5a70eed83a0ddb52e3955fac7a45ea52967072003409959d4f4d.

## R9.3 — camera recoil target
SC count proved GetScatterRate hook runs, but that is spread, not camera kick.
Added weapon gun-feel flag writes:
- `HBIBDMMOOOK.ILGIAJHNNDP()` -> `COW.WeaponGunFeelProfileData`
- set `enableRecoil=false`, `enableVisualRecoil=false` while No recoil is ON
- restore both old values when OFF
Candidate 274289 bytes, sha256 e675a1b22d199f4351e013b7da735bb78f7efc6912ef879b6d87ae91c6f00ee8.

## Device test — R9.3
User confirmed No recoil works on device (SC hook was being called; the fix
was to disable WeaponGunFeelProfileData.enableRecoil / enableVisualRecoil,
not only GetScatterRate). Keep this path for future combat features.

## R9.4 — Rapid fire
Writes PlayerAttributes.FireIntervalScale to 0.5 while enabled, restores the
old value when disabled. Uses the existing PlayerAttributes object from
UpdateCombat. Candidate 274418 bytes,
sha256 d2789d2093cc043b6d120cb65cd6589d55bbff71464ce668c52b3e49b914ef8f.

## R9.5 — High FPS toggle
Sets GameSettingData.m_IsHighFramerate and m_SmoothHighFrame while enabled,
restores old values when disabled. Candidate 274550 bytes,
sha256 86efd8659003cc8144e8172580798382ff70bb89559bb702f0bd8b49008a0922.

## R9.6 — Hide damage
Added an IFix method fix on Player.get_ShowDamageNum:
- returns false when Hide damage is ON
- returns true when OFF
Fixes: 84 (menuonly 81 + Silent + Scatter + ShowDamageNum).
Candidate 274603 bytes, sha256 d9fac98aaa466d33c97a5bb75a440e9eb9dddd16ca24ddbca509ba53ed74d4cc.

## R9.7 — Infinite ammo
Sets PlayerAttributes.IsAmmoFree while enabled, restores old value when
disabled. Candidate 274751 bytes,
sha256 fb3704141de69afac0fc8e9d7d2b5916061b54cfd84aae7c582e11a1134517ac.

## R9.8 — Vehicle ESP HP
Vehicle cache now stores HP ratio; Render draws a 40px HP bar under the
vehicle label. Candidate 274840 bytes,
sha256 1f054541c5f201d85ce597b9a1c59811c99a3c5865b4a624ab40a102ff9594ad.

## R9.9 — Fast reload
Sets PlayerAttributes.BuffWeaponReloadScale to 0.5 while enabled, restores the
old value when disabled. Candidate 274960 bytes,
sha256 742c283028f9fba5d31f25621eb3e8c77c7421141ad04ced94e6395920c75211.

## R10.0 — No fall damage
IFix fix on Player.IsIgnoreHighFalling:
- true while No fall damage is ON
- false while OFF
Fixes: 85.
Candidate 275016 bytes, sha256 8d3016f1a5c791530829d373f6f49e75c0992ee68d732bfe4f3d9451278ba43b.

## R10.1 — No fall damage override hooks
Added IsIgnoreHighFalling fixes for Player, PlayerReaper, PlayerRushingPets.
Menu shows `FC` = fall-hook call count next to SC/NR.
Candidate 275096 bytes, sha256 95c238eecfddfba1d98f6627d5916dd58b89aabdd7df0359be90557d29ecf2c9.

## R10.2 — remove no fall, add Auto swap
Removed the ineffective No fall damage hook/row. Added Auto swap via
GameSettingData.GetAutoSwitchWeapon / SetAutoSwitchWeapon, restoring the old
setting when disabled. Fixes: 84. Candidate 275067 bytes,
sha256 906a6565a87ce6cf7137f88495aaa6f1275935128e8faba0e8b6a13e172c5e9f.

## R10.3 — Auto swap calls Player.AutoSwitchWeapon
Added `Player.AutoSwitchWeapon(false,false)` every 0.25s while Auto swap is on,
in addition to `GameSettingData.SetAutoSwitchWeapon(true)`. Candidate 275118
bytes, sha256 54ce8f21ae1568ed00ee976cd32241e396b2067707ccb1fd18d5bb2e79a68b8f.

## R10.4 — Auto swap only while firing
Removed the 0.25s periodic AutoSwitchWeapon call (it reverted manual swaps).
Now calls Player.AutoSwitchWeapon only when IsFiring() and at most every 0.4s.
Candidate 275123 bytes, sha256 a3575883ad92fb5dbd6985f25ba52b64d60b615b4151dece47a3ab8d915ad668.

## R10.5 — BS4484 V2.4 movement/misc batch
BS4484 V2.4 source recovered at `cs/bs4484-v24/` (Android PUBG native:
libpubgm + Zygisk + Dobby + ImGui). It is a different game/platform, so only
feature concepts were ported, not code.

Ported into the R8 IFix core:
- Speed run slider 1.0–2.0 via `PlayerAttributes.RunSpeedUpScale`.
- Camera FOV slider 60–120 via `Camera.set_fieldOfView`; the Camera 90 toggle
  now uses the slider value.
- Fast fall via `PlayerAttributes.FallingSpeedUpScale = 2.0`, old value
  restored when off.

Not portable in the C# IFix patch:
- Chams (needs GL/shader hook).
- Ground hack / no gravity / super jump / fly / teleport (need to call the
  original method or manipulate native state; IFix patch bodies have no
  trampoline here).
Those belong to the native dylib path.

Candidate:
```text
bytes   275349
sha256  c1ca1972ee4b3bd6a52570cd183efa4ba2ddb8946a84a5d433586290c0f2a5c3
raw     690219
raw sha1    iYJezy8m1cYAKgPYsZ2ORFH0qig=
packed sha1 DaEqlKOa3nOrcRFStdp72zJgJ3Q=
fixes   84
guard   PASS 27/27
```

Published to `var/aimtest/`; live `var/okmod/` unchanged
(`a0958ad93ce28329f08ba7cb6bc0727f2de2bc506392ad84a4118caf425cd570`).
The gzip was recompressed with mtime=3 so the packed base64 hash contains no
`/` or `+` (safe for URL path and local filename).

## R10.6 — Super jump / Ground hack (native hooks)

Removed Fast fall. Added the two BS4484 movement features that need the original
method value:

- Super jump: `COW.GamePlay.Player.get_MaxJumpHeight` -> 9.0 when ON,
  original pointer when OFF.
- Ground hack: `UnityEngine.CharacterController.get_isGrounded` -> true when ON,
  original pointer when OFF.

C# IFix cannot call an unpatched original for these (no base proxy in the game),
so the hooks live in the native dylib. The C# menu rows only set PlayerPrefs and
call `PlayerPrefs.Save()`:

```text
OKMOD.Move.SuperJump
OKMOD.Move.GroundHack
```

The native M2 dylib reads the same keys from `NSUserDefaults` and also exposes
`J` / `G` fallback buttons in its overlay.

C# candidate:
```text
bytes      275294
sha256     79299692616044843e2a21d8e686460a9227769c6b4e51fdfb65022930e061c5
raw        689963
raw sha1   pzsjjb44x4QnrcXlG83lIUrcQ8M=
packed sha1 oQIy5LqxKArKfO6yDKH886nqq8c=
fixes      84
guard      PASS 27/27
```

Published to `var/aimtest/`; proxy 1297 updated. Live unchanged
(`a0958ad93ce28329f08ba7cb6bc0727f2de2bc506392ad84a4118caf425cd570`).

Native:
```text
dylib  ios/okmodnative/okmodloader-m2.dylib
sha256 0cffb466717acfe820e80e441d356b44f5db8206908869a5815818c1805a7fb1

IPA    ios/FreeFire-unpacked-okmod.ipa
sha256 399b5b86bd5d8111151ced8f6843d1779bdaac7a54bb66c7c3bd669e0a23386f
```

## R10.7 — SuperJump full BS4484 hook set + menu visibility
M2 only hooked `get_MaxJumpHeight`, which was not enough (BS4484 also alters
gravity and movement state). M3 adds:

```text
Player.get_MaxJumpHeight   -> 5.0 (TarGetJump)
Player.get_CustomGravity   -> 0.0
Player.get_RisingGravity   -> -0.9
Player.GetPhysXState       -> 13
CharacterController.get_isGrounded -> true (Ground hack)
```

Each hook logs a call counter at first call and every 600 calls so we can tell
whether IL2CPP actually invokes the swapped method or inlined it.

AIM page scroll max raised to 1224 so both movement rows are visible at the
bottom (row 22 was clipped at the old 1152 max).

C# R10.7:
```text
bytes      275296
sha256     473029d8b47ebc9dee9dd51765d8d2f9164dd91dcf6eefa51d06ddaab5d8ae6e
raw        689963
raw sha1   TeJB9iPAj2Kl616DUorS52N0SCU=
packed sha1 x5YqbiSBywDdOZ1LLP3AIzoxOrU=
fixes 84, guard PASS 27/27
```

## R11.14 — Top-line, FOV ring, name color and verified ESP additions

- Restores the classic top-center → enemy-box-top line in mesh and GUI paths.
- Replaces the FOV polyline with a cached 96-section annulus, 4 physical
  pixels thick. Rebuilds only on radius/viewport changes; thick GUI fallback.
- HUD no longer paints ESP over an open menu. A separate `Màu tên` row controls
  name text; enabling names no longer changes enemy/bot box colors.
- Scroll gesture and buttons both use 1224px, exposing all 24 ESP rows.
- Adds `Vòng đầu` anchored to projected head position (radius is estimated
  from the box, not a collider-bounds measurement).
- Adds Skeleton: 16 bone getters / 15 edges, max 8 sampled players within
  120m, optional 2ms per-skeleton budget. Bones are sampled only in Sample,
  stored box-relative and follow live rectangles; pose is sample-rate, not
  a full per-frame bone read. Missing/budget-skipped joints are omitted.
- Adds optional state flags FIRE / RELOAD / CROUCH / KNOCK and armor-presence
  flags VEST / HELMET. No invented armor level, durability or weapon name.
- Bone and state getters validated against supplied metadata. Disassembly
  evidence is saved in /tmp/opencode/esp-v31-getter-audit.txt.
- Vertex/color uploads stay aligned when live topology changes; mesh capacity
  is bounded at 1024 quads. No COW calls from Render/DrawBatch.

Validation:
- Build success; CORE GUARD 27/27.
- tests/test_visuals.py passes FOV radius/thickness/closure/cache/resize,
  top-line origin, 15 skeleton edges, head-circle geometry, combined mesh
  capacity, cache clearing, independent name color and menu render gate.
- These are instrumented-stub tests, not a Unity/device performance result.
- Backed up both aimtest payloads/manifests and proxy payload before publish.
- Download through proxy verified against candidate; live R8 hash unchanged.
- Device test remains pending. Native source and IPA not changed.

```text
title       OKMOD R11.14
bytes       282032
sha256      b1cdc5bdf0e55c92bb7081c8821cb8af0ca30565553978c14fdc6779e9cd6da2
raw         724915
raw sha1    zcemxHLphh7wFbX1ltjSlwxBIF4=
packed sha1 5RHEyD2VXkpIGKyEnceyhf5n9tU=
backup tag  pre-r1114-20261022T054849Z
```

Release metadata: release-R11.14.json.

Native M3:
```text
dylib ios/okmodnative/okmodloader-m3.dylib
sha256 d8137952aa91527d2cc3a40c3f7dad3c08e959bca6b51e9f5bfa16e9e82b5f63
IPA   ios/FreeFire-unpacked-okmod.ipa
sha256 8afb20992a09cfc825e61e4cda1a42271db9e9513bc2a983c2848fcc06a044fd
```

Published to aimtest/proxy. Live unchanged.

## R10.8 — title fix (the real "still R10.4" cause)
Source review found the visible menu title was hardcoded in `okmod.cs`:

```csharp
GUI.Label(..., "OKMOD R10.4", bs);
GUI.Label(..., "OKMOD R10.4", big);
```

`build.py` only replaced the base intern `"OKMOD R6-Lite"` with the new
version, so the patch could be R10.7 while the menu still displayed R10.4.
This was a source bug, not an IFix load failure.

Fixed both labels to `OKMOD R10.8`; the new patch contains only R10.8
(no R10.4/R10.7 strings).

Candidate:
```text
bytes      275295
sha256     5a6659b2c5589751a5d1ce60fe9ed9e2e74b653704efdad1a54585be3cd1fb78
raw        689963
raw sha1   mmeDiVESPkS1T2M6XJ9K3ufspbo=
packed sha1 MFKSdwhnNqLEQR2fKoGdE9dBEgs=
fixes 84, guard PASS 27/27
```

Published to aimtest and proxy 1297. Live unchanged.

## R11.0 — Hayrick jump (C#) + Hitfly investigation
Hitfly/SetSpeed via PlayHitfly/StopHitfly/SetSpeed was rejected by the build:

```text
extern absent from supplied game: COW.GamePlay.Player PlayHitfly 0
```

Those methods live on NetworkAIPawnAnimBaseComponent / NetworkAIPawnAnimComponent /
UGCDummyAIPawnAnim, not on Player. RequestHayrickJump is on Player (virtual) and
PlayerNetwork (override).

Implemented Hayrick jump:
```csharp
if (wantHayrick && now >= R7HayrickNext) {
    local.RequestHayrickJump();
    R7HayrickNext = now + 1.0f;
}
```
Extern verified against metadata: owner COW.GamePlay.Player, arity 0, method 210757.

Candidate R11.0:
```text
bytes      275342
sha256     0b12ab2a67b1b1baf255a6bf481039207cce4ae324e7837703e36e25adbc4695
raw        690139
raw sha1   1Tw5dslX00kHXD12JZe9gXLOKcQ=
packed sha1 DlYVl7A9m6DhLw2LwisCIzLxIog=
fixes 84, guard PASS 27/27
```

Raw + packed hashes are slash/plus-free so the local codepatch filename is safe.
Published to aimtest/proxy. Live unchanged.

## R11.1 — Silent Aim hit-point accuracy
Root cause of inconsistent silent hits: the write point used the bone
transform, which can sit outside the actual hit capsule. The collider/group
could be correct while the point was not inside the hitbox.

Changes:
- `SilentHitPoint()` now uses `collider.get_bounds().get_center()` for the
  written hit point, falling back to the bone only if bounds are invalid.
- Head mode: if `get_HeadCollider()` is null, search the nearest non-trigger
  fire collider within 0.75 m instead of rejecting the target.
- Body modes skip trigger colliders before the nearest-center search.
- Title: OKMOD R11.1.

Candidate:
```text
bytes      275801
sha256     21cf90cea21c3dc6dba11f6135512ff6025241e0b939fc134fb34b5c03689b98
raw        690587
raw sha1   8aCZtQ4q3nLxxDfV6NWmpmO2G78=
packed sha1 OIUTdxTeTF20YH08gHEBb4xdcaA=
fixes 84, guard PASS 27/27
```

Published to aimtest/proxy. Live unchanged.

## R11.2 — Silent Aim from behind/side
Front-only silent behavior pointed to the native hit info still carrying the
original ray's collider/distance. From the front those values nearly matched the
silent point, so the game accepted; from behind/side they did not.

Changes:
- Write `CFIPDNFBKHK` (hit distance) to the new origin->point distance.
- Write `DIDOKHNBBAD` (second collider field) to the selected collider.
- Save/restore both, together with the existing fields.
- Keep collider-bounds-center hit point, head collider fallback and trigger
  collider filtering from R11.1.

Candidate:
```text
bytes      275929
sha256     3a64197b7f32411701af62bbab88ac5bbab92eb825b631d3de51b53495078a07
raw        691003
raw sha1   sSkSdiFPmp8LiXE3XoLtyk3+t3s=
packed sha1 z9XEfCLe7ab0pbxjmh8FBzkLeow=
fixes 84, guard PASS 27/27
```

Published to aimtest + running proxy. Live unchanged.

## R11.3 A/B Silent field isolation
- A (R11.3D): only `CFIPDNFBKHK = distance`. Gun fires, but Silent still
  front-only. Distance is not the cause.
- B (R11.3C): only `DIDOKHNBBAD = collider`. Silent breaks firing entirely.
  That field is not a hit collider to override; it corrupts the fire state.
- Both variants reverted. R11.1 restored byte-for-byte:
  sha256 21cf90cea21c3dc6dba11f6135512ff6025241e0b939fc134fb34b5c03689b98.
- Front-only Silent is therefore not explained by these two fields. Remaining
  suspects: native ray/hit validation and server-side direction checks.
  Do not override CFIPDNFBKHK/DIDOKHNBBAD without disassembling the fire path.

## R11.4 — Camera aimbot any-angle
Server fire sync only sends a fire byte; the silent hit object is local/skill
data. Any-angle therefore rotates the actual aim/camera before the shot instead
of editing hit info.

- New toggle `OKMOD.Aim.AnyAngle` ("Aimbot any-angle") at AIM row 23.
- `AimScoreAny`: no screen bounds/FOV; selects nearest valid enemy by world
  distance within Range, with the existing team/HP/knocked/visibility filters.
- Activation: Aimbot ON, Silent OFF. Any-angle aims while IsFiring() (or always
  when Aimbot lock is ON); scope/fire gating is bypassed.
- Uses the existing `SetAimRotation` path and Smooth slider. Smooth 0 is needed
  for fast 180-degree turns.

Candidate:
```text
bytes      275927
sha256     95d6d1d4fab387c2764f97f056b3581db94551be3b4b74b1dff24ecaa6d013e1
raw        691715
raw sha1   rk+YUtlmH3dgjGBxutRV4wZKjcY=
packed sha1 W2Fjf3vQVloxGBDEDhF0cEwlx2g=
fixes 84, guard PASS 27/27
```

Published to aimtest/proxy. Live unchanged.

## R11.6 — Menu repair
- Removed Hayrick Jump (row/logic/field/stub).
- Removed Fast reload (row/logic/fields).
- Restored Aimbot any-angle as AIM row 1, directly under Aimbot.
- Renumbered AIM rows 0..22 (no gaps).
- R11.5 had Hayrick at row 1 and Fast reload at row 19 because the reorder
  replace missed; that is why the user saw them and not Any-angle.

Candidate:
```text
bytes      275739
sha256     5a1cee12cad107eecf9b125cd6c64423aac6a9492c48a8064c9b22065db2a183
raw        690891
raw sha1   DcO0po2k1Ut4gEfvWsGAYCL2jD0=
packed sha1 GIsTZYiO9j0Ug2T7meWz22WODV4=
fixes 84, guard PASS 27/27
```

Published to aimtest/proxy. Live unchanged.

## R11.7 — Fire dash
New `OKMOD.Move.FireDash` row at AIM 2 ("Lướt khi bắn") and
`OKMOD.Move.FireDashSpeed` slider ("Lướt bắn", 5–40).

Writes `PlayerAttributes.ForceSetRunAndDashSpeed` while `local.IsFiring()`,
restores the old value when not firing/off/owner changes. This is the field
that forces run + dash speed; no native hook.

Candidate:
```text
bytes      275920
sha256     9481398277f3c36c7892d886444fc611fe96c1bf3fd38449bca4a19c8a5a9d4b
raw        691723
raw sha1   CpaaqqxMn66pm2zMtuFvKUw6vWY=
packed sha1 lkvtdhxhTfofpuOUK87MZQjfjK4=
fixes 84, guard PASS 27/27
```

Published to aimtest/proxy. Live unchanged.

## R11.8 — Fire dash pulse
Continuous `ForceSetRunAndDashSpeed` write fought server speed reconciliation:
works on bot/offline maps, rubber-bands/jitters on human online maps.

Changed to pulses:
- 0.35 s dash ON, 0.35 s restore (0.70 s cycle).
- Default value 12 (slider 5–25).
- Still writes each HUD update during the ON window to counter resets.

Candidate:
```text
bytes      275979
sha256     b8111eaab77358964b7f7cc0bf94347fadc30aa128b711802d3909d56d5a72b5
raw        691987
raw sha1   bEUih4LYmhaZut59rzpd9CKYRY4=
packed sha1 vNa5y3MBj7fFscEDZBDkOVi3OJc=
fixes 84, guard PASS 27/27
```

Published to aimtest/proxy. Live unchanged.

## R11.9 — ESP smoothness
- Sample cadence: removed the `drawn > 6 -> 0.06s` penalty and softened the
  heavy-sample backoff (0.10s -> 0.08s). Normal path is now 0.04s (25 Hz).
- Small matches (<=16 list entries) scan from index 0 so record order is
  stable; large matches keep the rotating cursor.
- Render keeps the previous sample (`R7Prev`) and extrapolates box/name/HP
  coordinates by `age / interval`, clamped. Pure cache math, no COW calls in
  Render/DrawBatch.
- Mesh is rebuilt during the interpolation window so boxes move every Repaint.
- For >16-player lists the rotating window disables extrapolation to avoid
  teleport artifacts; those still use the faster 0.04–0.08s sampler.

Candidate:
```text
bytes      276523
sha256     2232d4d20812776eac5e1cc2ca9e7f751ffe3a4546b9b3cc3fa50490515b0f1c
raw        694939
raw sha1   X8SQtynYEuuoEPE8G297Ml1mMbM=
packed sha1 lJIxCcSM8zcB2Vs7uLmjIdDtceU=
fixes 84, guard PASS 27/27
```

## R11.10 — Live projection (camera-smooth ESP)
Research: public Free Fire IFix ESP (ipameo/MeoMeo `ESPLogic.template.cs`)
projects every Repaint with `camera.WorldToScreenPoint`; it does not cache
screen rectangles. Its iOS branch culls distance and avoids allocations.

R11.10 keeps the scheduled R8 sampler for expensive player/bone reads, but:
- Sample caches world feet/head (`R7World`) instead of only screen rects.
- `ProjectCached` runs every Repaint after sampling and projects those cached
  world points with the live camera. No player list, bone, name or HP reads
  happen in this pass (2 projections per cached player).
- World positions are interpolated between samples by player instance ID
  (`R7Ids`), so reordering/rotating slices do not teleport boxes.
- Offscreen players are no longer discarded during Sample; they enter view
  immediately instead of waiting for the next sample.
- Sample cadence 0.04–0.08 s; colors upload is split from vertex upload;
  mesh batch flag is cached instead of read from PlayerPrefs every frame.
- Render/DrawBatch stay COW-free; guard PASS 27/27.

Candidate:
```text
bytes      277634
sha256     0aef71a98c2c78d42ce0423c079d89f3601c7f29b9d5c2fc4328f9bf8a170376
raw        700579
raw sha1   pLJVoMUVhXWpfbAvH73c2iqQl4w=
packed sha1 QFdmBDqlJpl1S6ZDoTEgghuhgZA=
fixes 84
```

## R11.11 — Per-frame transform projection (manguon reference)
Reference `/root/manguon.zip`:
- `jni/src/Android_draw/ч╗ШхЫ╛/TouchTooT.h` `CalMatrixMem` projects cached
  world coordinates with a matrix read once per frame.
- `DrawGame.h` reads the 4x4 matrix every frame, then projects all entities.
- `smoothPosition` is a 0.14 low-pass on world coords (adds latency, so not
  copied as-is).

R11.11 changes:
- `Sample` caches `Transform` root/head per record (`R7RootTf`, `R7HeadTf`).
- `ProjectCached` reads those transform positions every Repaint and projects
  with the live camera; no player list, bone search, name or HP read in this
  pass. If a transform is invalid, fall back to `R7World` + per-ID interp.
- `UpdateAimbot`/`UpdateCombat` run once per Repaint, not on Layout.
- Render/DrawBatch remain COW-free; guard PASS 27/27.

Candidate:
```text
bytes      277859
sha256     98255335a028d86bfe8b325c87d6c0d23e6f8beeab7b8e632a8fab82654311ae
raw        701595
raw sha1   xsdV3BThsZAlotiCDTGDYMPJOUs=
packed sha1 NZXLOVSat7Oq8sy29RxwYiDRbdY=
fixes 84
```

## R11.12 — ESP style A (Clean Tactical)
Visual-only redesign on the R8 cache/mesh renderer:
- 4-corner brackets (2px) with 1px black shadow instead of a full box.
  `Khung đầy` is now the optional full-box toggle and overrides corners.
- HP bar moved to the right side, 4px, dark backing, red→yellow→green
  gradient (bottom-up). HP number only for target or <=50%.
- Name/distance drawn in translucent pills with 1px black text shadow;
  names are cached uppercase. Target gets a gold pulse and top chevron.
- Head marker is a 5px dot + cross instead of the old V.
- Target-only ray from bottom center; enemy coral, team cyan, knocked gray,
  bot purple, target gold; distance alpha fade.
- Off-screen enemies get an edge chevron.
- ESP page button `Màu A` resets enemy/team/target to the new palette.
- All drawing still comes from cache/mesh; Render/DrawBatch stay COW-free.

Candidate:
```text
bytes      279372
sha256     367956a56b4a71a817ef580f182dd2d62d6ad83a2b5c933bc4f0671363b43fb8
raw        709587
raw sha1   9g8PDvoVfNUDZhJ6gPxDBd84ay4=
packed sha1 rM9MBnaNIgWHeyYZXyd71YytQGI=
fixes 84, guard PASS 27/27
```

## R11.13 — Readability pass
Fixes after Style A feedback:
- `Đường ngắm mục tiêu` now draws for every enemy again; target line is
  thicker/brighter.
- Full box no longer suppresses corner brackets; each box toggle is visible.
- HP bar moved inside the right edge (5px) so it is not clipped off-screen.
- Name/distance text is white (target gold) with black shadow and a dark
  pill plus colored 1px border; knocked gray and bot purple brightened.
- Default ESP font 24 (slider 16–32); name/distance/HP/vehicle/alert scale
  from `OKMOD.Esp.Font.Float`.
- Distance fade reduced to max 18%; target pulse kept bright.
- HP number uses the larger font and is white/gold.

Candidate:
```text
bytes      279689
sha256     c6838033cd0e03492f27b77c2fa923b3e7fdea18c685dcd0893bf77849e2924a
raw        711443
raw sha1   GaAiCrklN5wMlG7OQUXYTmrdgkM=
packed sha1 yrCNPZ0TwKHdG6cYJUb4kDAqsO8=
fixes 84, guard PASS 27/27
```

## R11.15 — Verified sampled ESP extensions

- Added camera-to-head ray visibility colors and direct-only filtering.
  State is clear / blocked / unknown; budget-skipped rays remain unknown.
- Added enemy aim-direction line from GetAimRotation quaternion; cached
  box-relative endpoints. Looking-at-you warning uses dot >=0.985 (~10deg),
  range <=100m, not a confirmed lock-on/visible-threat event.
- Added 100m aim-up mini radar from sampled relative XZ positions. Bots are
  purple, local is white. Limited to current 16-record R8 window.
- Added explicit offscreen-arrow toggle, behind-camera direction correction,
  distance label and pulse; HP 7 segments; outlined Dot/Cross/T crosshair;
  combat-info mode hiding labels/status/numeric HP below 35m.
- Extended ESP list to 36 rows; gestures/buttons use 2088px limit. AIM remains
  1224px. All new toggles default off.
- No new hooks, base-proxy remapping, native changes or IPA build; 84 fixes.
- IsVisible/base-proxy audit showed model-visibility semantics, not line-of-
  sight. No misuse of that getter for occlusion. Weapon-name binding deferred.
- Complete requested-feature feasibility map: ESP-FEASIBILITY.md.
- Reproducible metadata/disassembly: tools/audit_esp_bindings.py writes
  audit-esp-R11.15.json (11 methods). Sources are read-only.
- tests/test_visuals.py passes geometry, aim quaternion math, radar/clamping,
  arrow anchors, ray clear/blocked/target/unknown, direct-only filters,
  HP separators, view-line cache, capacities and cleanup. Device test pending.
- CORE GUARD PASS 27/27. Both aimtest channels and proxy updated after backups.
  Download through proxy verified. Live R8 hash remains unchanged.

```text
title       OKMOD R11.15
bytes       285391
sha256      1226d57c97659bad6a2c6d08a16527fc2dd2af970b098d941c9b98711c01a0d3
raw         745323
raw sha1    UCdlY+iaTtnIngxTQniCGIAMz1c=
packed sha1 1RBbyhMUYR7AX7I8ESvbcSah8w8=
backup tag  pre-r1115-20261022T061150Z
```

Release record: release-R11.15.json (device_test: pending).

## R11.16 — BS4484 Hitfly one-shot port

- Actual BS4484 `ExecuteHitFly` path: build `message.NNDOIBJBAJI`, pack camera
  forward with `message.LMCBHLCPAJD.MGOGJCMCIPG`, Send(657,msg,2,false).
- Uses actual metadata types `GCommon.UDPClientMessageBase`,
  `message.BJFGLKKKOAN`, `COW.GamePlay.BEADLMGGGGL`. Player ID is fetched via
  get_PlayerID + matching DEIPJDIGKIP overload, not a uint raw offset read.
- AIM rows 24..29: floating-button toggle, one-shot action, time/XZ/Y +/-
  controls and send status/counters. AIM swipe/button scroll limit 1656.
- Floating button touch region is included in the existing input router.
- Defaults time=1, XZ=25, Y=20; configurable 0.2..3s and 0..100 speeds.
- Queue is consumed once on Repaint before any game calls; cooldown
  max(1s,time+0.25s). Invalid/dead/observer inputs and exceptions do not retry.
- Client Send true/false is shown without claiming server approval.
- Read-only metadata/native audit: tools/audit_hitfly_bindings.py ->
  audit-hitfly-R11.16.json. Native trigger/serializer corroborate the route.
- tests/test_hitfly.py passes defaults/custom packet fields, ID/direction,
  one-shot/cooldown, invalid inputs, false Send and exception paths. No test
  network traffic. tests/test_visuals.py also passes; core guard PASS 27/27.
- 84 fixes, 10 storeys unchanged. No PlayHitfly call, native/IPA changes.
- Published only to aimtest/proxy after backups. Proxy download verified;
  live R8 hash unchanged. Device/server-acceptance tests remain pending.

```text
title       OKMOD R11.16
bytes       286789
sha256      9d2369de192a7d0ba82ed5f21cf89f3bba04fd1df7ad361faa0c4822c2ac1c47
raw         752939
raw sha1    OMbHSknqqdvs+B+Ck0qffR1L1w4=
packed sha1 y949ZfZV92b9GdLe9ZRQXylJKeg=
backup tag  pre-r1116-20261022T064443Z
```

Details: HITFLY.md. Release record: release-R11.16.json.

## R11.17 — Native map marker -> Hitfly bearing

- Added floating CHON MAP / HF MAP buttons and AIM rows 30/31. AIM gesture
  and button max scroll 1800px; ESP remains 2088px.
- CHON MAP queues native UIInGameScene.SetBigMapActive(true) on Repaint.
- HF MAP reads active UIMapContentController real-position/valid fields.
  Uses local-root -> marker horizontal XZ bearing; retains configured
  duration and XZ/Y launch speeds. No endpoint/landing guarantee.
- Missing/invalid/near markers do not send, and do not silently fall back to
  camera-forward. Camera Hitfly remains separate; one-shot/cooldown retained.
- NO FALL DAMAGE was investigated but is NOT in the release. Damage cause
  Falling=-4 exists, but PlayerNetwork base proxy calls parent TakeDamage,
  not the full original network body. Trial hook removed before publication
  to avoid changing damage processing when OFF. Fixes remain 84.
- Audit: tools/audit_fall_map_bindings.py -> audit-fall-map-R11.17.json.
  Map valid flag set/clear and coordinates verified from native code.
- tests/test_hitfly.py passes marker/map-open/horizontal direction and
  missing/near/NaN cases; visual regressions pass; guard PASS 27/27.
- Backed up and published aimtest/proxy only; downloaded artifact verified;
  live R8/native/IPA untouched. Device/server tests pending.

```text
title       OKMOD R11.17
bytes       287579
sha256      6c8a40c8099dbde3181d3b19e66dcf4b92b2886ad0f46bb9274b39c99903a249
raw         758139
raw sha1    vLJyBjDrDP32LYKUvI9jSIQVwvw=
packed sha1 pduf7xHLyewtAcBDwL2Ddv40jac=
backup tag  pre-r1117-20261022T071904Z
```

Details: FALL-MAP.md. Release record: release-R11.17.json.

## R11.19 — Remove mod-controlled map opening

- User reported native close still unusable after CHON MAP. Removed CHON MAP
  button, action handler, pending field and UIInGameScene.SetBigMapActive stub.
- Removed its touch regions. Floating control is now HF MAP only; the AIM
  action uses the full 390px row, matching its visible label/hitbox.
- Label: `Hiện nút HF MAP`. User opens/closes the native map normally, places
  a local marker, then presses HF MAP. The mod only reads existing marker data.
- Retains read-only GetBigMapActive yield from the previous close workaround;
  no overlay routing/drawing while native big map reports active.
- Candidate extern table verified free of SetBigMapActive/OpenBigMap/
  TryCloseBigMap/CloseBigMap references. No new hooks; fixes remain 84.
- Hitfly marker/queue tests and visual tests pass; guard PASS 27/27.
- Backed up and published aimtest/proxy; downloaded candidate verified.
  Live R8/native/IPA untouched; device test pending.

```text
title       OKMOD R11.19
bytes       287438
sha256      3e8840daaac6ea85786a9afd69caeb652bae4134e7423084b12e56ee30409dd5
raw         757163
raw sha1    EPDwtQeAUDXuSdwwjbFQ8R3Y7So=
packed sha1 BZNtu9X8SimwLKhLqBDzSk6KfDM=
backup tag  pre-r1119-20261022T074802Z
```

Release metadata: release-R11.19.json.

## R11.20 — Separate gameplay updates, native map and mod-menu layers

- Replaced controller-only map detection with scene get_MapState (BigMap=1,
  SideMap=2) OR controller GetBigMapActive. Each read has independent fallback.
- Gameplay update pass runs once per Repaint before map/UI gating, so opening
  native map or mod menu no longer stops aim/combat/pending Hitfly updates.
- Native map open: no mod menu, floating buttons, ESP, input router or event
  consumption; releases active touch, slider and drag capture. Does not mutate
  game map state or feature preferences.
- Mod menu open: ESP continues sampling/rendering, drawn behind menu rather
  than disabled. Menu drawn last, so labels do not cover controls.
- Core sampled ESP and mesh graph retained; fixes 84; no native/IPA changes.
- tests/test_hud_lifecycle.py tests state/controller disagreement, exception
  fallbacks, map input/draw suppression, capture reset, feature frame guard,
  open-menu ESP and closed-menu recovery. Hitfly and visual regressions pass.
- Core guard PASS 27/27, backed up aimtest/proxy before publishing. Proxy
  download verified and live R8 hash unchanged. Device verification pending.

```text
title       OKMOD R11.20
bytes       287235
sha256      6aa5b57a8717d77810b901c1e7ce817a0b0807265204cd7b80ed9875ea78c54d
raw         757547
raw sha1    Z4LsZP59GnaP9XA3nEXmVdyeyDU=
packed sha1 hELx5CBeCcTnNDvsZ3t6BSHPWPo=
backup tag  pre-r1120-20261022T075847Z
```

Release metadata: release-R11.20.json.

## R11.21 — User correction: keep OKMOD / HF MAP visible on native map

- User clarified the desired behavior is visible overlay while native map
  is open. Removed NativeMapOpen gating and map-driven pointer reset.
- Mod menu/icon and enabled HF MAP remain available over the native map;
  feature updates continue once per Repaint and ESP remains behind mod menu.
- Native map opening/closing is still entirely owned by game UI; no map
  state mutator added. HF MAP reads the user's existing marker only.
- HUD lifecycle tests now assert overlay/HF MAP visible with BigMap active,
  menu/map-independent feature/ESP ticks and per-frame guard. Hitfly and
  visual regression tests pass; core guard 27/27.
- Published aimtest/proxy after backups, download verified; live unchanged.
- Device verification pending.

```text
title       OKMOD R11.21
bytes       287099
sha256      352ac7458c799007f65dac68e712fe2995a35f7ff2d8b13f85b2b3faa4dfcdd9
raw         756731
raw sha1    PqNmcj4yaWWCpXLy4NCx1uwAs5Y=
packed sha1 6OHcXqhmFRCpjfJv5zlJKe6V6ck=
backup tag  pre-r1121-20261022T080811Z
```

## R11.22 — Experimental BS4484 AimKill client hit path

- Added default-off `AimKill (BS4484 test)` at AIM row 32 and client status
  at 33; swipe/button scroll limit 1944px. ESP menu unaffected.
- Mutually exclusive with Aimbot/Silent, including saved combinations.
- One nearest valid world-distance enemy per 0.8s, 6ms/128-entry selection
  budget; uses actual weapon range clamped by configured range and selected
  head/neck/chest/body collider, optional visibility. Manual firing/reload
  skipped, team/knocked/dead/inactive/local targets excluded.
- Sequence: aim, Player.StartFiring(weapon), HBIBDMMOOOK.LHNJLFOINDO(hit),
  Player.StopFire(weapon) in finally. Fresh managed hit info; no raw pointers,
  memset, new native hooks or base-proxy remaps. No inventory packet spam.
- Source constant hit integer 200 retained; not claimed as guaranteed damage.
- `đã gọi hit path` and native integer result do not confirm a kill.
- tools/audit_aimkill_bindings.py -> audit-aimkill-R11.22.json validates exact
  owners/parameter types and native hit-path evidence.
- tests/test_aimkill.py passes target/mode/range/sequence/cadence/manual-fire/
  exception-cleanup cases. Existing HUD/Hitfly/visual tests pass. Guard 27/27.
- 84 fixes, native/IPA/live R8 unchanged. Aimtest/proxy backed up and updated;
  proxy download verified. Device/IFix/server acceptance remains pending.

```text
title       OKMOD R11.22
bytes       288327
sha256      a7680a63edc8f3e2f8ef23f37ddff9addad8f06ac5416ccfb1ad3da288a0c820
raw         762523
raw sha1    SEQKDvEGSNJNlVrWyQ1V8GSdvDI=
packed sha1 SN3KVw8ROQqBQt5VS3KFWqXHlMQ=
backup tag  pre-r1122-20261022T082621Z
```

Details: AIMKILL.md. Release metadata: release-R11.22.json.

## R11.23 — AimKill menu, delay and prediction refresh

- Bumped the visible marker to `OKMOD R11.23` in the new HUD/menu.
- Added the AimKill delay row (`20 / 40 / 80 ms`), optional prediction toggle,
  and the expanded AIM scroll limit of 2160px.
- Prediction uses per-list-slot delta-time samples with a capped lead heuristic;
  the submitted hit point remains the validated collider centre.
- Rebuilt from the IFix menu-only/R8 graph with 84 fixes; core guard PASS 27/27.
- AimKill, HitFly, HUD lifecycle and visual regression tests pass. Binding audit
  refreshed as `audit-aimkill-R11.23.json`.
- Published to `http://217.217.255.156:1297/` after a rollback backup;
  `okmod-1297.service` restarted and health-checked. Aimtest was restored to
  its previous R11.22 artifact. Live hash remains unchanged. Device/server
  confirmation pending.

```text
title       OKMOD R11.23
bytes       288845
sha256      461d14c89610588ef4144a42621e0a80c426b65a60c0f7eb6a5660a4ab640d89
raw         762557
raw sha1    0YwHe5Cq8poF5gsQ48OtGgos0vo=
packed sha1 7+KwJ60nD4fyas/rcrWcyJXBMUw=
backup tag  pre-r1123-20261022T101109Z
```

Release metadata: release-R11.23.json.

## R11.25 — AimKill BS4484 parity test (object wipe + inventory packet)

- Two source behaviours added on top of the R11.23 hit path, nothing else
  changed:
  1. After filling the hit object, clear the two collections the constructor
     creates — `MGBCGEBLKJB` (0x88) and `ANJGNHKHOKO` (0x90) — to match the
     source `memset(hitData + 0x10, 0, ...)` before its field writes.
  2. After `StopFire`, send the source's `Syns_SwapWeapon_Impl` packet:
     `message.EOBFCLNNIJH` with player id + on-hand weapon unique id via
     `GameFacade.Send(108, msg, 2, false)`. The weapon unique id is the
     inherited `IEFJGCGOMID.ODFDGMOCJLL` field at +0x10 (same field the source
     reads with its `GetUniqueID` macro; present and public in v31 metadata).
     The packet is skipped when either id is zero and never sent on the
     exception path.
- `tests/test_aimkill.py` asserts no packet while the feature is off, and
  exactly one opcode-108 packet carrying the expected player/weapon ids after a
  successful hit. Core guard PASS 27/27; HUD/visual/Hitfly tests pass.
- Published to the 1297 proxy after a rollback backup, service restarted, and an
  end-to-end proxy fetch returned the identical sha256. Live `okmod` unchanged.
  Device/server acceptance pending.

```text
title       OKMOD R11.25
bytes       289054
sha256      78922dea6b789050df2939d2406daf14db06c54be20d58482820df084cf1b711
raw         766315
raw sha1    +PZoqy8qInb4wUVQ3MmWW41WtIE=
packed sha1 9Fx2UMuegZiOevcli6p42O3f/zE=
backup tag  pre-r1125-20261022T142555Z
```

Release metadata: release-R11.25.json.

## R11.26 — PullEnemy port

- New default-off `Kéo địch (PullEnemy)` toggle on the AIM page (row 36) plus a
  status row (37); AIM scroll limit raised to 2232px.
- Port of BS4484 V2.4 `PullEnemyV1` (ESP.h closestEnemy block): when the nearest
  valid enemy is within 8 m, lerp their root transform toward `local + forward*1`
  with step `8 * dt`, using `Transform.set_position`.
- Core-rule safe: the target is re-picked on a 0.25 s cadence by scanning at most
  128 list entries; each frame only reads/writes the two cached transforms. All
  writes are behind a feature gate and try/catch, with a diagnostic string.
- `tests/test_aimkill.py` now asserts pull off-by-default, target selection and
  distance report, the lerp moving the enemy toward the front, and no target
  beyond 8 m. Core guard PASS 27/27; HUD/visual/Hitfly tests pass.
- Published to the 1297 proxy after a rollback backup, service restarted, and an
  end-to-end proxy fetch returned the identical sha256. Live `okmod` unchanged.
  Device/server acceptance pending.

```text
title       OKMOD R11.26
bytes       289936
sha256      f246591939800b972948c79e03fbc7e70d32448c212cfd6a1a02d9b4ff55a8c6
raw         771331
raw sha1    DsUWrZI2ppHDU2BxEph2dRQb5EA=
packed sha1 nVp4O+JT8EE4Xpe+LuXXU7MC0kI=
backup tag  pre-r1126-20261022T144947Z
```

Release metadata: release-R11.26.json.

## R11.27 — PullEnemy menu reachability fix

- The AIM drag-scroll clamp was still `1944f`, so the last AIM rows (including
  the new PullEnemy row 36 and its status row 37) could never be scrolled into
  view even though the arrow buttons allowed 2232. Raised the AIM clamp to
  `2232f` to match `ScrollButtons`.
- No gameplay change; PullEnemy/AimKill code is identical to R11.26.
- Core guard PASS 27/27; AimKill/HUD/visual/Hitfly tests pass. Published to the
  1297 proxy after a rollback backup, service restarted, end-to-end fetch
  matched. Live `okmod` unchanged.

```text
title       OKMOD R11.27
bytes       289933
sha256      bdc71addd84421c07da13d8e90075134606bfcaeb39c92adc5ace8d9836f5bcf
raw         771331
raw sha1    CT8zlKmN/ZDenAixBDzOJ0CNC/s=
packed sha1 +krctyWvQdzTz0B2DX6RJ4kZ/mI=
backup tag  pre-r1127-20261022T145647Z
```

Release metadata: release-R11.27.json.

## R11.28 — PullEnemy rework: firing-triggered vertical gather

- Behaviour changed per request: no more horizontal pull of the nearest enemy.
  While the local player holds fire, every valid enemy is gathered directly
  above the player (same X/Z as local) at 8 m and lowered down onto the player
  at 12 m/s. Releasing fire stops all writes; the height resets to 8 m so the
  next burst drops from above again.
- Target list is refreshed on a 0.25 s cadence (max 128 entries, same
  self/team/dying/HP/inactive filters) and cached in `R7PullTargets`; each frame
  only iterates the cached players and writes their root transform.
- Status row now shows `Cao <height> m | <n> địch` and a state
  (chờ bắn / không có địch / đang kéo / lỗi).
- `tests/test_aimkill.py` asserts off-by-default, wait-for-fire with no writes,
  gather over local X/Z, start at ~8 m, lower on the next frame, and stop on
  release. Core guard PASS 27/27; HUD/visual/Hitfly tests pass.
- Published to the 1297 proxy after a rollback backup, service restarted, fetch
  matched. Live `okmod` unchanged.

```text
title       OKMOD R11.28
bytes       289925
sha256      02a89afa9952470c983569d782eea64196138daf50e41c9fce1cf611953f28d2
raw         770675
raw sha1    0C2osZ/m55wGiha/baBLVs9Fjfc=
packed sha1 PEbN5ASjFRV8OpLcrUZJnCSgEs4=
backup tag  pre-r1128-20261022T151646Z
```

Release metadata: release-R11.28.json.

## R11.29 — PullEnemy: pull to the front of the face

- The 8 m overhead drop was too high. New target while firing is the front of
  the face: local head position + forward * 1.2 m, held there for as long as the
  trigger is down. No height animation; no horizontal side pull.
- Still firing-gated: releasing fire performs no transform writes, and the
  cached target list (0.25 s refresh, max 128, same filters) is cleared.
- Status row second line now reads `Trước mặt 1.2 m | <n> địch`.
- Tests updated for front-of-face placement and the hold; guard PASS 27/27;
  HUD/visual/Hitfly pass. Published to the 1297 proxy after a rollback backup,
  service restarted, fetch matched. Live `okmod` unchanged.

```text
title       OKMOD R11.29
bytes       289963
sha256      0b47417a23a2e34bee6876e7d5c7e99a0342f0268f5cd5bb10dc97f817b9a412
raw         771123
raw sha1    f4SIBeIbPzmpFnY/Cg3X8OxoRTg=
packed sha1 iJ5h4scYUul9T/UK3odxx8ATAsQ=
backup tag  pre-r1129-20261022T153654Z
```

Release metadata: release-R11.29.json.

## R11.30 — PullEnemy: one target, head on the crosshair

- Now pulls a single enemy at a time (nearest valid target, held until it dies
  or becomes invalid, re-picked on a 0.25 s cadence).
- Placement is calculated, not a fixed offset: take the camera aim ray
  (camera position + forward), pick the point 8 m ahead, read the target's head
  offset from its root (`headWorld - rootWorld`), then write
  `root = aimPoint - headOffset`. The enemy's head therefore lands on the
  crosshair, so the shot is a headshot instead of the body sitting at the
  player's own head.
- Still firing-gated: no writes unless `local.IsFiring()`; releasing stops and
  clears the target.
- Status row: `Trước mặt 8 m | 1 địch` + state.
- Tests assert exactly one target, root compensation `(0,0,8)` for a head
  offset `(0,1,0)`, hold, and stop-on-release. Guard PASS 27/27;
  HUD/visual/Hitfly pass. Published to the 1297 proxy after a rollback backup,
  service restarted, fetch matched. Live `okmod` unchanged.

```text
title       OKMOD R11.30
bytes       290093
sha256      9cc088d960f94415cd892240a2484622c254be131779c84499e79c2fbd2f6734
raw         771891
raw sha1    PbBObLKvdSpFptNGV2gXtk92lYM=
packed sha1 JYK/+Dl5CQxPH1i0shbpZsMSYjE=
backup tag  pre-r1130-20261022T154701Z
```

Release metadata: release-R11.30.json.

## R11.31 — PullEnemy: verify the pull, skip ghosts

- Report: sometimes a pulled target is a "ghost"/shadow that cannot be killed.
  Added write verification: before writing again, read the target's root and
  head. If the root is >2 m from the last written root, or the head is >2 m from
  the stored aim point, the write did not stick (server snap-back / duplicate
  entity) — blacklist that PlayerID for 8 s and immediately select the next
  target instead of rendering a phantom.
- Target filters tightened: `get_MaxHP() > 0`, `PlayerID != 0`,
  `CurHP > 0`, not dying/team/local/inactive, plus the 8-slot skip list.
- Status row now shows `Trước mặt 8 m | bỏ qua <n>` so failed pulls are visible.
- Added `R7PullExpected`, `R7PullAimPoint`, `R7PullSkipIds[8]`,
  `R7PullSkipUntil[8]`, `R7PullSkipCursor`, `R7PullSkipCount`.
- Tests cover the ghost path (root snapped back -> skip count 1, no target).
  Guard PASS 27/27; HUD/visual/Hitfly pass. Published to the 1297 proxy after a
  rollback backup, service restarted, fetch matched. Live `okmod` unchanged.

```text
title       OKMOD R11.31
bytes       290505
sha256      6a5416e68a36d6902af805bf0a398c7a86f66e697a9ecd62eb937b1abe973312
raw         773907
raw sha1    EKzFps0Xw7NQX5pxsCfz9NTMGYo=
packed sha1 D56qH9VHjL0NwX/N1r5grIYE200=
backup tag  pre-r1131-20261022T160343Z
```

Release metadata: release-R11.31.json.

## R11.32 — PullEnemy: server-side hit-fly impulse attempt

- Client-side transform pull abandoned (it only moved the local model and the
  server rejected the hit). PullEnemy now uses the game's own hit-fly movement
  message, which the server already accepts for self movement:
  `GameFacade.Send(657, message.NNDOIBJBAJI, 2, false)`.
- Packet contents: `BEADLMGGGGL` = the *target's* player id,
  `DCEMJOGHMEG` = 0.5 s, `KJGOMDJCKEB` = distance/0.5 clamped 8..45,
  `DPJAIECLHGL` = 0, `MGJNBIJNLIJ` = packed horizontal direction
  enemy -> local player.
- Firing-gated, one impulse per second, single held target (nearest valid,
  MaxHP>0, player id != 0, not team/dying/inactive).
- Status row: `Đã gửi <n> | cách <m>` with states (chờ bắn / không có địch /
  đã gửi 657 / Send false / lỗi).
- This is the experiment that determines whether the server applies opcode 657
  to the id inside the packet or only to the sending session. If it applies to
  the id, the enemy is moved by the server. If it validates the sender, nothing
  happens (and the local player must NOT be launched).
- Tests assert off/wait-for-fire, one 657 with the target id/time/direction,
  rate limiting, and stop on release. Guard PASS 27/27; HUD/visual/Hitfly pass.
  Published to the 1297 proxy after a rollback backup, service restarted, fetch
  matched. Live `okmod` unchanged.

```text
title       OKMOD R11.32
bytes       290098
sha256      7a95cadb0fd4e3f9b9f0fa1390668511c989008dcebe3e48c2c259291bcc6a1b
raw         771827
raw sha1    8RgEUCBNK6dM8ureUgMsVmB7xu8=
packed sha1 nnwisLtoo7x1ERpps8M5VmbDXZY=
backup tag  pre-r1132-20261022T161957Z
```

Release metadata: release-R11.32.json.

## R11.33 — PullEnemy: diagnostic 657 aligned with the game's own sender

- Static RE of the client's 657 sender (`RUDP_C2S_ClientHitFlyPlayer`):
  - The game's function takes a *player argument*, reads that player's
    `PlayerID` (BEADLMGGGGL.DEIPJDIGKIP) and writes it to msg+0x14.
  - It writes msg+0x18 = time, +0x1c = speedXZ, +0x20 = speedY, +0x28 =
    packed direction, +0x30 = string, and sets the byte at +0x38
    (`IMAOCDIIIHI`) to **1**.
  - It also applies a per-player cooldown (`now - player[0x3c8] <
    0.5 * trigger[0xb6c]`) before sending.
  - `RUDP_C2S_ActiveHitFlyPunch = 462` also exists in v31; no direct send site
    was found for it (may be legacy or sent through a wrapper).
- Port updated to match the real sender: `IMAOCDIIIHI = 1` (was 0), time 1.0 s,
  speedY 20, speedXZ = distance clamped 10..45, direction enemy -> local.
- Status row now exposes the target player id: `ID <id> | gửi <n> | <m> m`,
  so a zero/invalid id is visible on device.
- Tests updated for the new fields. Guard PASS 27/27; HUD/visual/Hitfly pass.
  Published to the 1297 proxy after a rollback backup, service restarted, fetch
  matched. Live `okmod` unchanged.

```text
title       OKMOD R11.33
bytes       290148
sha256      5e812a95b2ca85be27766562a47c8b7df8e1cb2504c11e1c7705e4ca9eff0ee4
raw         771947
raw sha1    Y88s4ha2aeq6rE0jl38KLXv38xg=
packed sha1 guWmHOQB3XPMPNOYme+k+GZ3ppc=
backup tag  pre-r1133-20261022T163421Z
```

Release metadata: release-R11.33.json.

## R11.34 — PullEnemy removed, AimKill retained

- PullEnemy fully removed after the server-side experiments proved it cannot
  move server-authoritative players:
  - client transform writes only produced ghosts,
  - opcode 657 (`RUDP_C2S_ClientHitFlyPlayer`) was sent with a valid target id
    but the server ignored it (needs a trigger/punch context),
  - 462 `ActiveHitFlyPunch` has no client send site in v31,
  - 543 `PUSH_AUTOFLIGHT_ENEMRY` belongs to a server-created
    `LevelAutoFlightBomb` (owner/target ids), which the client cannot spawn.
- Removed: `UpdatePullEnemy`, `ResetPullState`, `FindPullTarget`,
  `PullEnemyStatusRow`, the row-36/37 menu entries, all `R7Pull*` fields and
  the `Transform.set_position` stub. AIM scroll clamp restored to 2160.
- AimKill/hit path kept at the R11.25 parity state (no pooled hit object;
  ctor collections 0x88/0x90 cleared; inventory opcode 108 after StopFire).
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly tests pass. Published to the 1297
  proxy after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.34
bytes       289050
sha256      0dde486069a9b50a91509cd4a15a4dd288fde9b4b8bfd9212b5a18d95b3a553a
raw         766315
raw sha1    DNTNEgB6YU8PUcUXM6pMwtV+0AA=
packed sha1 vSUqGYEmbBnZSmQ2bMRLvfPb/LQ=
backup tag  pre-r1134-20261022T170618Z
```

Release metadata: release-R11.34.json.

## R11.35 — AimKill: center pick, 108 toggle, group mapping, lead/delay

1. Target selection: default now picks the enemy closest to the camera aim ray
   (dot product > 0.2, distance as tie-break) instead of the nearest in space.
   New toggle `AimKill chọn theo tâm` (default ON); turning it off restores the
   nearest-distance behaviour.
2. Packet 108: now behind `Gửi packet 108` (default OFF). It is sent only when
   the toggle is on and `hitSent` is true, so stability can be compared with and
   without the inventory sync.
3. Hit group/collider: group is derived from the collider actually used —
   `Head` only when the chosen collider is the head collider, otherwise `Body`.
   The group is exposed in the status row (`đầu`/`thân`) and stored in
   `R7AimKillGroup`. Neck/chest/body colliders map to Body because the game enum
   only has Default/Head/Body.
4. Cadence/lead: delay cycle is now 0 / 20 / 40 / 80 ms (0 = every frame,
   default 40); new `AimKill lead` row cycles 0..150% in 25% steps and scales the
   predicted displacement. Velocity cap raised to 16 m/s, lead cap 0.12 s,
   displacement cap 3.0 m.
- Menu rows: 35 center pick, 36 send 108, 37 lead, 38 status; AIM scroll clamp
  raised to 2376 so the new rows are reachable.
- Tests cover center-vs-nearest selection, group recording, 20/80 ms cadence,
  100% and 0% lead, and the 108 toggle. Guard PASS 27/27; HUD/visual/Hitfly
  pass. Published to the 1297 proxy after a rollback backup, service restarted,
  fetch matched. Live `okmod` unchanged.

```text
title       OKMOD R11.35
bytes       289700
sha256      f7af5c6038c83428135db57d37427dc5b1c540ff9d207b6e884ff85b8c0c54d3
raw         769539
raw sha1    YRb6k7fLmPOQASxV+nm5zSpwGxs=
packed sha1 9UZEtSheuMkq4uiYGpl3+6meXCs=
backup tag  pre-r1135-20261022T171539Z
```

Release metadata: release-R11.35.json.

## R11.36 — Packet 108 removed (device: OFF is more stable)

- Device feedback: with `Gửi packet 108` OFF, AimKill damage errors were fewer.
  The inventory-on-hand packet was destabilising the hit path, so it is removed
  entirely rather than left as a default-off toggle.
- Removed: the `hitSent` flag, the finally-block send, `SendInventoryOnHand`,
  the `EOBFCLNNIJH` stub and the row-36 toggle. AimKill now only does:
  build hit object (collections cleared) → SetAimRotation → StartFiring →
  LHNJLFOINDO → StopFire.
- Menu rows after 35: `AimKill lead` (36), status (37); AIM scroll 2304.
- Tests no longer assert packet 108; center/lead/group/cadence tests unchanged.
  Guard PASS 27/27; HUD/visual/Hitfly pass. Published to the 1297 proxy after a
  rollback backup, service restarted, fetch matched. Live `okmod` unchanged.

```text
title       OKMOD R11.36
bytes       289486
sha256      c267dcc1addc4b20a89578ba9107283493fe07604694556bb1aac0ca9f7e5f5f
raw         768739
raw sha1    XPmjqJ9eNiiNpBAJK6VXaEWNJ3Y=
packed sha1 27ZJt+xyOxxG8OGswHOsJDZjSh4=
backup tag  pre-r1136-20261022T174359Z
```

Release metadata: release-R11.36.json.

## R11.37 — AimKill delay floor (disconnect fix)

- Device report: very low AimKill delay caused a connection timeout / kick to
  lobby. Each dispatch calls StartFiring + LHNJLFOINDO + StopFire, which are
  network operations; at 0 ms that is 60+ fire-state/hit sequences per second,
  far above any real weapon cadence, so the server's rate/anti-cheat path drops
  the connection.
- Removed the 0 ms option. Cycle is now 20 / 40 / 80 / 120 ms (default 40) with
  the row warning that below 80 ms is kick-prone; 80/120 recommended for stable
  testing.
- AimKill lead and "chọn theo tâm" are local-only (no packets) and cannot cause
  a disconnect.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.37
bytes       289486
sha256      d8d4188a8056da947b94237980f740c381d4aad27575d53741eae1b09be266f8
raw         768731
raw sha1    WsqxRH8bvDaA77IGWed2nRUQo0s=
packed sha1 RgUzV/I/0ReLUKk4wDdqzVX3hkk=
backup tag  pre-r1137-20261022T175653Z
```

Release metadata: release-R11.37.json.

## R11.38 — AimKill option A: single StartFiring

- Per-tick StartFiring/StopFire pairs replaced by a held fire state:
  - On the first successful dispatch, if the player is not already firing,
    `StartFiring` is called once and `R7AimKillFiring` is set.
  - Every tick only `LHNJLFOINDO(hit)` is sent.
  - `StopFire` is called only when: feature off, local invalid/observer,
    reloading, dispatch exception, or 1 s passes without a valid target.
  - A fire the player already owns is never touched (we only start/stop ours).
- `R7AimKillFiring` and `R7AimKillLastTarget` added; status row shows `giữ cò`
  while the held fire is active.
- Packet count at 80 ms drops from ~3 per tick to 1 hit per tick plus two state
  changes per burst.
- Tests updated for the held fire, reload release and the 1 s timeout.
  Guard PASS 27/27; HUD/visual/Hitfly pass. Published to the 1297 proxy after a
  rollback backup, service restarted, fetch matched. Live `okmod` unchanged.

```text
title       OKMOD R11.38
bytes       289580
sha256      001b5edce483103d0c785d95aecd8f488d8f91566ad14056b3232f1cdb26a76f
raw         769163
raw sha1    dGTMKnfFwELJhQ5h6WZyy0fc9FU=
packed sha1 sB7pG44FzAP+VpzmbTooq/yKGI0=
backup tag  pre-r1138-20261022T180715Z
```

Release metadata: release-R11.38.json.

## R11.39 — AimKill all-direction pick, lead removed, reload status

- Removed the `AimKill lead` row and the lead multiplier; prediction now always
  uses the fixed 850 m/s lead with its caps.
- Removed the crosshair-only target pick. AimKill always picks the nearest valid
  enemy in any direction (360°), and SetAimRotation turns the local player
  toward it.
- Out of ammo: the game starts reloading, `IsReloading()` releases the held
  fire (StopFire) and status shows `AK: đang nạp / hết đạn`. Hits stop during
  the reload and resume after it. A dry weapon with no reserve ammo still has no
  reliable ammo counter in v31; use Infinite Ammo or pick up ammo for long tests.
- Menu rows now: 32 AimKill, 33 delay, 34 prediction, 35 status; AIM scroll back
  to 2160.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.39
bytes       289149
sha256      da688de45fa9794a41d1426e070c0303c2391fb6ad6e16fbc9e1d7d83edcbe88
raw         766731
raw sha1    JmnjMh9chLilTsUK5Xf5jonl5HE=
packed sha1 wGX0+ueO30HGMu/Xz+sA/nr6qGg=
backup tag  pre-r1139-20261022T183609Z
```

Release metadata: release-R11.39.json.

## R11.40 — AimKill: SetAimRotation removed

- AimKill no longer calls `local.SetAimRotation(...)`, so the character/camera
  does not turn toward the target. Hits are still dispatched to the nearest
  valid enemy in any direction.
- Removed the prediction path with it: `AimKillPredict`, its toggle row and the
  `R7AimKillPrevPos` / `R7AimKillPrevTime` fields. Prediction only ever changed
  the rotation, so it became dead code.
- Menu rows now: 32 AimKill, 33 delay, 34 status; AIM scroll 2088.
- Trade-off: without rotation the submitted hit direction may not match the
  weapon/camera facing, so server-side direction/LOS validation may reject more
  hits. If damage gets worse, restore rotation or keep only a small yaw assist.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.40
bytes       288767
sha256      8fef53e70c9f05ec3037ddb83e298f01b18655e41bc19cf7ba419e7b7367bb28
raw         765115
raw sha1    3zgxlx8bnS/kz47zLLlhVKGLsM4=
packed sha1 gm0dIM8cTHlXrmglextTVwyr19A=
backup tag  pre-r1140-20261022T184031Z
```

Release metadata: release-R11.40.json.

## R11.41 — AimKill always requires line of sight

- AimKill no longer follows the aimbot's "Kiểm tra vật cản" toggle. It always
  runs `ClearSight` on every candidate during the scan and again inside
  `StartAimKillXNEW` before dispatch. A target behind a wall/obstacle is skipped
  even if it is the nearest.
- The check still originates at the local head and tests the selected aim point
  (head/neck/chest/body collider centre), advancing only past confirmed local
  player hits.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.41
bytes       288756
sha256      aab1080b3d484346fa4644fb2f954c20a654681636fcba9bb1d9495df57c9793
raw         765067
raw sha1    iHUmuRM4fOLh5gGcN9oyDgqSqJ4=
packed sha1 uC3cprwoMiTVKwRXpTEDeYDvzmk=
backup tag  pre-r1141-20261022T185047Z
```

Release metadata: release-R11.41.json.

## R11.42 — AimKill keeps the clip free

- Device symptom: after a while AimKill damage fails until the player presses
  fire or reloads. Diagnosis: the held fire state (option A) is applied through
  the API, not a real trigger press, so the game's trigger-driven auto-reload
  does not run. `IsReloading()` stays false with an empty clip, and the mod keeps
  submitting hits that the server rejects. A real press/reload resets it.
- Fix: while `OKMOD.Aim.AimKill` is on, `PlayerAttributes.IsAmmoFree` is applied
  exactly like the Infinite Ammo toggle (old value saved/restored, released when
  AimKill and Infinite Ammo are both off). The clip no longer depletes during a
  held AimKill burst.
- Status 6 still reports a game-driven reload; the 1 s no-target timeout still
  releases the fire.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.42
bytes       288759
sha256      0f90b59531068fc3d75ea8e5fa346829eac38f7a9943396dbbf7936c8a6a52dd
raw         765131
raw sha1    1pmUtjsph1QfEwCZZlgo5c6lEQs=
packed sha1 IT1XhFwLjpeckvTUnxQlK+UB13E=
backup tag  pre-r1142-20261022T185905Z
```

Release metadata: release-R11.42.json.

## R11.43 — AimKill uses ShootNoReload instead of IsAmmoFree

- Removed the automatic `IsAmmoFree` application for AimKill (Reverted R11.42's
  wantAmmo condition).
- AimKill now rides the existing No-reload path: while `OKMOD.Aim.AimKill` is
  on, `PlayerAttributes.ShootNoReload` is applied (old value saved/restored);
  it is released when AimKill and the No reload toggle are both off.
- This is a different mechanism from IsAmmoFree: it targets the reload/ammo
  consumption path of the weapon rather than the ammo-free flag.
- If ShootNoReload does not actually keep the clip usable, the next fallbacks
  are a periodic `Player.ReloadWeapon()` or `Player.FullWeaponAmmo()` call.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.43
bytes       288760
sha256      62a27dfc01e3301aa46e5d45614dab384a8c6461191a003789cb13a3c392b2d5
raw         765131
raw sha1    W85TMbkKwYX4PXyUTXq4v97v42A=
packed sha1 kI4+UrFc2UfNfHeMWrxDpqI0MNU=
backup tag  pre-r1143-20261022T190819Z
```

Release metadata: release-R11.43.json.

## R11.44 — AimKill uses Player.FullWeaponAmmo()

- Removed both automatic ammo mechanisms: `IsAmmoFree` (R11.42) and
  `ShootNoReload` (R11.43) are back to their own toggles only.
- While AimKill is active, the mod calls `Player.FullWeaponAmmo()` at most once
  per second (`R7AimKillAmmoNext` gate) to refill the current weapon's clip.
  This is the game's own method, not a raw attribute write.
- The call is wrapped in try/catch and only runs when a local weapon exists and
  AimKill is on; reload/status handling is unchanged.
- Test asserts `FullWeaponAmmo` is invoked on the first AimKill dispatch.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Published to the 1297 proxy
  after a rollback backup, service restarted, fetch matched. Live `okmod`
  unchanged.

```text
title       OKMOD R11.44
bytes       288814
sha256      c098592ccc3caa4b833d31b71ffd30fd7b90c9f2e29f85357ed3241efb0a01b6
raw         765275
raw sha1    FvnnaOkEdjA7NKJsMb8DLXPkrbQ=
packed sha1 rEMbCxnlX+0hapxIL2BEKjXAqpI=
backup tag  pre-r1144-20261022T191704Z
```

Release metadata: release-R11.44.json.

## R11.45 — Ground hack: hook Player.IsGrounded (game-side, approach 1)

- Reverse: Player has `IsGrounded()` (0x56870BC) and
  `IsGroundedByRayCastHit()` (0x5687124); the latter is the raycast-based ground
  query. The Unity `CharacterController.get_isGrounded` hook used by BS4484 is
  in UnityEngine.PhysicsModule and cannot be redirected safely from the q3
  patch without a base proxy.
- Added a game-side fix: `Player.IsGrounded` -> `GroundHackIsGrounded`.
  - Toggle `Ground hack (native)` ON: returns true (always grounded).
  - OFF: returns `self.IsGroundedByRayCastHit()` as the closest original
    behaviour, so normal movement/jump keeps a real ground query.
- Fix count 85 (base 81 + Silent + ScatterRate + ShowDamageNum + ground); guard
  PASS 27/27 (allowed base+7). AimKill/HUD/visual/Hitfly pass.
- Risk: OFF path now always goes through the raycast query; if the original
  IsGrounded used a cached value, movement may differ slightly when the toggle
  is off. Test normal jumping with the toggle OFF.
- Published to the 1297 proxy after a rollback backup, service restarted, fetch
  matched. Live `okmod` unchanged.

```text
title       OKMOD R11.45
bytes       288865
sha256      01b67c511b669f32de897c8ca313e81ae844791e13e915f2fb9403a2f022e371
raw         765483
raw sha1    P8+etXNn2b7RD0jXPZiNVE8BWYw=
packed sha1 oy8HMjvk081QrRuO2vHPwjkgFFU=
backup tag  pre-r1145-20261022T194837Z
```

Release metadata: release-R11.45.json.

## R11.46 — AimKill: line of sight at the collider centre

- Symptom: status showed "chưa có mục tiêu" with an enemy right in front, and
  hits only worked from certain angles.
- Cause: the scan tested line of sight toward the bone point, while the hit
  itself is submitted at the collider centre. The bone can sit behind an edge,
  and the local weapon/arms could block the near ray, so candidates were
  rejected before StartAimKill.
- Fix:
  - Resolve the target collider and its centre during the scan, and run
    `ClearSight` against that same centre used for the hit.
  - Treat hits within 0.9 m of the ray origin as own weapon/body/camera and
    advance past them; real cover beyond that still blocks.
- Direction selection remains nearest-in-any-direction. Walls/cover are still
  respected by design.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed to 1297 via
  deploy.sh after backup; proxy fetch matched.

```text
title       OKMOD R11.46
bytes       288919
sha256      7f88a65c03f5b7ef0e36b6b0c708fc96370c6d344b6bdee0c490b0c7364fd5ec
raw         765707
raw sha1    lGpGcz5NOkv23YDjHkeHvdRd2z0=
packed sha1 KV4JqwRsuJMZL1Ifwo6Wb4qqKxk=
backup tag  pre-clean-20261023T073314Z
```

## R11.47 — AimKill diagnostics in the menu

- Added a debug row under the AimKill status row (AIM row 35, scroll 2232):
  - `T <id> | <dist>m | 0:<n> | stop <code>`
  - `nạp <n> | ammo <fills>/<fails> | <last error>`
- Stop codes: 0 running, 1 feature off, 2 local/weapon invalid, 3 reload
  release, 4 one second without a target, 5 exception, 6 weapon changed.
- Ammo refill moved **before** the `IsReloading` early-return (the old order
  never refilled while reloading, which could leave the held fire stuck) and
  cadence reduced to 0.5 s. Fill/failure counters added.
- Target id, distance and consecutive zero-result count recorded on every
  dispatch. Zero result is a client return value, not server damage proof.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; proxy fetch matched.

```text
title       OKMOD R11.47
bytes       289292
sha256      bcbb323191db26f1184b652e8abe9c858a4aa60df69404eb4b79dd49f75a0e66
raw         767571
raw sha1    BPbbHrLJ6LH4R6VHThQtF8FlG3Y=
packed sha1 LI1sF7E4Gh8TuaOpIdRhXhBP2R0=
backup tag  pre-clean-20261023T075254Z
```

## R11.48 — AimKill copy-log button

- New AIM row 36: `Sao chép log`. On tap it builds a diagnostic snapshot and
  writes it to the iOS clipboard with `GUIUtility.systemCopyBuffer`
  (metadata-checked: UnityEngine.GUIUtility.set_systemCopyBuffer, static,
  arity 1). Shows `Đã copy log` for 2 seconds.
- Snapshot fields: stop, status, target id, distance, consecutive zeros, reload
  count, ammo fill/fail, calls, last result, hit group, fire held/released,
  weapon instance, delay mode, range, point mode, last error, timestamp.
- Scroll extended to 2304 so row 36 is reachable.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.48
bytes       290003
sha256      f86b21518277b98f3da993e86d81c85fe59456587c61eb8c6882d24d62faa3c0
raw         770603
raw sha1    fItK7cwWl7IwoMDI0HG0v0D76ak=
packed sha1 HJsQtDep1kEBgTrqgirVKIsajnQ=
backup tag  pre-clean-20261023T075918Z
```

## R11.49 — AimKill log overlay + copy verification

- The copy row now reads `GUIUtility.systemCopyBuffer` back after writing: only
  shows `Đã copy log` when the readback matches; otherwise shows `Copy lỗi`
  and stores the reason in the diagnostic error field.
- New AIM row 37 `Xem log`: opens a full-screen overlay with the complete
  diagnostic snapshot, plus a `ĐÓNG` button (touch handled with
  TouchPhase.Began and a mouse fallback). This works even if iOS blocks the
  clipboard.
- Added `GUIStyle.set_wordWrap` to the stub (metadata-checked) for the overlay.
- Scroll extended to 2376 so row 37 is reachable.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.49
bytes       290538
sha256      5e6f702e19105c6f136236dba0210d227a96a842db7437a02ac803c6a7233da6
raw         773275
raw sha1    X3lG1EV3PYC3YioOSbPBJFWWFsg=
packed sha1 huUA4zkbjf9Rh3JXTS+i+ON06Is=
backup tag  pre-clean-20261023T081135Z
```

## R11.50 — Log panel inside the menu

- The R11.49 full-screen overlay was drawn in screen space before the scaled
  menu and did not render on device. Replaced it with a panel inside the 800x680
  scaled menu: `Xem log` (row 37) replaces the menu content with the diagnostic
  text and a `ĐÓNG` button.
- `BuildAimKillLog` is covered by a unit check (non-null, contains stop/ammo).
- Copy still does a clipboard readback; if iOS refuses it the row shows
  `Copy lỗi`, and the log panel remains readable/screenshotable.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.50
bytes       290381
sha256      7777a428d05126645e91c091d61fd8ae2d1db83bb0f4215bc2e23a969c0ea14b
raw         772827
raw sha1    LGE6KqsD1GdeBP9/51QGgh5FMP0=
packed sha1 mCV+AeySEVo3ba088q8wWp9w2EU=
backup tag  pre-clean-20261023T082409Z
```

## R11.51 — On-demand online log

- New AIM row 38 `Gửi log online`: builds a numeric diagnostic snapshot and
  sends it with `UnityEngine.Networking.UnityWebRequest` over HTTPS to
  `https://okmod.sbs/aimlog?...` (manual tap only, not automatic).
- The request is polled in UpdateFeaturesOnce (isDone -> Dispose); the row shows
  `Đang gửi log...` / `Đã gửi log`; errors go to the diagnostic error field.
- Server: nginx `location = /aimlog` (204) with a dedicated access log at
  `/var/log/nginx/aimlog.log`; verified with curl before shipping.
- Snapshot fields: stop/status/target/dist/zeros/reloads/ammo fills/fails/calls/
  result/group/fire/weapon/delay/range/point.
- Trade-off: this is an outbound request from the game; it adds a detection
  surface. Keep it manual and only while debugging.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.51
bytes       290837
sha256      9e7fed0e025a21f860b2908b45c74dfa368c0423b06776a6acc36226b6dfc1d9
raw         775995
raw sha1    nh0uY0asvLYTnLJ625wF6QtRSMU=
packed sha1 OubyNGKgmKWOLC7x5+Wrgzq5lfI=
backup tag  pre-clean-20261023T083027Z
```

## R11.52 — Online log send fix

- Device error on send: `Object of type System.Object[] cannot be converted to
  type System.String`. The long URL `+` chain was lowered to
  `String.Concat(object[])`, which the IL2CPP patch runtime could not convert.
  The URL is now assembled with short two-operand concatenations.
- Send completion now reads `UnityWebRequest.get_error()`: success shows
  `Đã gửi log`, failure shows `Gửi lỗi` and stores `send: <error>` in the log
  error field (visible in the Xem log panel).
- Log panel background made fully opaque so menu rows cannot show through.
- New `R7LogSendOk` field.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.52
bytes       290981
sha256      572990479cbb0e3f5924899519806aa3467e525c6247e9dc8496f880b95e629b
raw         776083
raw sha1    x6al96Jo+qCuSmSKwiwXND/CAR0=
packed sha1 TUJ84dXvfkoCRltHeVuQfEB8tf4=
backup tag  pre-clean-20261023T094743Z
```

## R11.53 — String concat fixes for log/copy/send

- Two device errors traced to the patch runtime's string concat lowering:
  - `copy: Object of type System.Object[] cannot be converted to System.String`
    (long BuildAimKillLog chain -> String.Concat(object[])),
  - `send: Object of type System.Int32 cannot be converted to System.String`
    (int/float operands concatenated directly).
- Both the log and the URL are now built in short chunks, and every numeric
  field is `.ToString()` before concatenation. Booleans use string literals.
- Log panel draws a full-screen opaque backdrop before the panel.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.53
bytes       290915
sha256      46f3d6e18be50d8abcd51e3583105894511bfb2961296e98906c9bc027fe66a2
raw         775883
raw sha1    0nMJMNceDm9e7ZMj1QKvleThayY=
packed sha1 LzRlp2vnWIq8+EXZZ8QZo5F+Otk=
backup tag  pre-clean-20261023T095642Z
```

## R11.54 — AimKill fire-rate controls from the first server log

- First online log (v=11.52, Free Fire UA) showed a healthy client:
  status=4, result=200, zeros=0, reloads=0, ammo 113/0, fire=held,
  head group, 24 m, delay=120 ms. So the remaining damage failures are
  server-side validation, not ammo/reload/exceptions.
- Delay cycle changed to 80 / 120 / 200 / 300 ms (default 120) so the dispatch
  rate can be matched to slower weapons; 120 ms is 8.3 hits/s and can exceed the
  real fire interval.
- New toggle `Chỉ bắn khi giữ cò` (row 39): requires `local.IsFiring()`, does not
  call StartFiring synthetically, and only dispatches while the player holds the
  trigger. This aligns the hit stream with real firing.
- Weapon change detection now compares the weapon object reference (GetInstanceID
  logged 0 on device); log adds `weaponRange` (actual `OEOBPHDFJMO`).
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.54
bytes       291004
sha256      978ca6453f37bf417a749039bf205e731332b2c0facd64c1eb9eb0f5d35cc552
raw         776267
raw sha1    8Ix3rJYwKD7N9jRpcUmKsr6iZMM=
packed sha1 ZTOBCVDUI9w9Y0o32rDGsbnj9u4=
backup tag  pre-clean-20261023T100541Z
```

## R11.55 — One-tap AimKill auto test with HP probe

- Damage probe: every hit stores the target and its HP; 0.7 s later the mod
  reads HP again and increments `R7ProbeOk` (HP dropped) or `R7ProbeFail`
  (unchanged). Ring buffer of 8 pending samples.
- New row 40 `Test tự động (120/200/300)`: on tap it runs three 4-second phases
  with delay mode 1/2/3 (120/200/300 ms), collects per-phase probe ok/fail and
  call counts, restores the saved delay, builds a summary and uploads it
  automatically to `/aimlog?v=11.55&test=1&ok0=...`.
- Normal `Gửi log online` now also includes `probeOk/probeFail` and
  `weaponRange`.
- The auto test requires AimKill ON and a visible target; it runs ~12 s.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.55
bytes       292265
sha256      c1fe4bcb842420ee6405f1819b04df63c002d72df3d9655000b3083ae9eaf1ad
raw         783339
raw sha1    gGBT0AWCKnbNZriaQBPly2bjZmA=
packed sha1 JT7n1PEU2DFE8Pwfku7pxSz8QKg=
backup tag  pre-clean-20261023T101350Z
```

## R11.56 — Cleaner auto-test phases

- The first auto-test samples were contaminated: a hit at the end of one phase
  was read 0.7 s later in the next phase. The probe ring is now cleared at every
  phase boundary (and at test start).
- Phase length increased from 4 s to 6 s (18 s total) to get more samples.
- Log adds `probeLast=<before>-><after>`, so a single HP read can be inspected:
  if before == after for every sample, the client HP is not reflecting damage.
- Version/URL corrected to 11.56 (R11.55 still sent v=11.54).
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.56
bytes       292412
sha256      c337a8c153e824ba161f541b7fb69905489f5abcb66e7b9b9c3378dd37a3a94d
raw         783987
raw sha1    XSrgh4tvPUUtr9uhlciRsDPiCDU=
packed sha1 nALZgE8ZGYRPy1WlCBTiF8dfaoI=
backup tag  pre-clean-20261023T102259Z
```

## R11.57 — A/B rotation test + weapon range in URL

- Clean R11.56 auto-test aggregate (5 runs): 120 ms 18 ok/13 fail (58%),
  200 ms 29/16 (64%), 300 ms 9/26 (26%); overall 56 ok/55 fail (~50%).
  120 ms had one 0/13 outlier; 300 ms was consistently poor.
- New opt-in `Xoay nhân vật khi bắn` (row 41, default off): calls
  `SetAimRotation` toward the collider centre before firing, to test whether the
  server rejects hits whose claimed direction does not match the character.
- Normal log/URL add `weaponRange` (`wr`) and `rotate` (`rot`); log adds
  `rotate` and keeps `probeLast`.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.57
bytes       292470
sha256      b95830c37dd92edafece81c3024699409ebe663b0edc5cedb823e41369327cbc
raw         784395
raw sha1    pm1CCoForn0xx2JLljJWel2NJgM=
packed sha1 RHAghpBf5jeAMs4KI4MRPjm5uEc=
backup tag  pre-clean-20261023T103043Z
```

## R11.58 — Comparable auto-test phases

- R11.57 logs showed the 200 ms phase often had only 3 calls because the target
  died in phase 0, so its 100% was a tiny-sample artifact. 120/300 phases showed
  heavy rejection in that session (aggregate 24 ok / 109 fail).
- Each phase now arms on the first hit and then runs 5 s; if no hit arrives for
  10 s the phase is skipped with zeros. Phases are independent of target death
  in the previous phase.
- Summary records `rot` (rotation toggle at test start) and `wr` (weapon range)
  so the A/B rotation test is interpretable.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.58
bytes       292642
sha256      685ecef32b052112f6688991c69c8e837656318029b72ba21a40af892f730dbc
raw         785379
raw sha1    EZ5ixvMQESDpvnRCuz/5GGJbDVE=
packed sha1 k+VnUumzwBTe2vWfOIOPp56LU2k=
backup tag  pre-clean-20261023T111054Z
```

## R11.59 — Camera lock for the rotate toggle

- `SetAimRotation` rotates the character, not the camera, and only ran on each
  AimKill dispatch (120/300 ms), so the view never hard-locked.
- The dispatch now stores `R7AimKillTargetPlayer`. When `Xoay nhân vật` is ON,
  `UpdateAimKillCameraLock` runs every frame and sets:
  - local character rotation toward the target head,
  - main camera `Transform.set_rotation` toward the target head.
- Wrapped in try/catch; skipped if the target is dead/null or AimKill is off.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.59
bytes       292804
sha256      d826c5d478a754ec9b3ff2b40745c79b4efce239a448a96b007575677edbcc81
raw         786403
raw sha1    KhU8wvEoiZfGzQhQ79k6jnEBIKc=
packed sha1 5wJzVCFR79EvMCFsnImEoQfEvDo=
backup tag  pre-clean-20261023T112755Z
```

## R11.60 — Source-parity shot mode

- Rotation ON did not improve damage (R11.58 rot=1 run: 120 ms 5/28,
  200 ms 0/19, 300 ms 0/12; wr=227). The remaining differences from BS4484:
  - held fire vs one StartFiring/hit/StopFire per shot,
  - hit normal `(0,1,0)` vs the source's `hitPoint`.
- New toggle `Kiểu BS4484 (từng phát)` (row 42):
  - each dispatch does StartFiring -> LHNJLFOINDO -> StopFire in finally,
  - sets `hit.DOJDNHCGFGK = hitPoint` like the source.
- Auto-test summary records `src=0/1` so both modes can be compared.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.60
bytes       292946
sha256      0f3bab7d146f3faa1c753776e2d8651c6e0d85f8899320c8b6f4b9217a0f920f
raw         787195
raw sha1    g0eSqisSpPqVy3NfiOLp/LZSVXE=
packed sha1 sGfWViEnomMhExuDV86INtBln3k=
backup tag  pre-clean-20261023T114653Z
```

## R11.61 — Source-parity mode is the damage fix (default ON)

- R11.60 A/B logs:
  - `src=0` (held fire, normal 0,1,0): 120 ms 5 ok / 17 fail (23%).
  - `src=1` (one StartFiring -> hit -> StopFire, normal=hitPoint):
    120 ms 15 ok / 2 fail (88%), 300 ms 9 ok / 1 fail (90%);
    several 200 ms phases had no calls because the target died in phase 0.
  - `rot=1 src=1` also worked (120 ms 9/0, 3/2; 300 ms 3/1, 4/0), so rotation is
    not required.
- Conclusion: the missing combination was per-shot firing + source normal,
  not delay/rotation. Held fire was causing most server rejections.
- `Kiểu BS4484 (từng phát)` is now default ON (toggle remains to compare).
  Rotation stays default OFF. Recommend 120 or 300 ms; 200 ms phases often had
  no target left.
- Test suite sets held-fire mode explicitly for its old assertions; all tests
  pass. Guard PASS 27/27; deployed via deploy.sh after backups; fetch matched.

```text
title       OKMOD R11.61
bytes       292953
sha256      c4a8742011d88da11f2d003c36da442cc39fb6e75fd86c6ce4647554d33bfdf8
raw         787195
raw sha1    9yITyL6CpO0+zA2hra/crhd6dfQ=
packed sha1 iVUg129+kC1R1c5MlI5AyMcgjVw=
backup tag  pre-clean-20261023T134435Z
```

## R11.62 — AIM menu cleanup

- Removed the test/debug rows from the AIM page: detailed debug row, copy log,
  view log, send log online, and auto test.
- Remaining AIM rows: AimKill, delay, status, require-fire, rotate, source-parity
  (defaults: source ON, rotate OFF, delay 120 ms). Scroll 2304.
- The diagnostic code/fields remain in the patch but are unreachable from the
  menu; re-add a row if logs are needed again.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.62
bytes       291687
sha256      e7b085225ac7db2c49be996551461622840a418e5a6939ececf809f65e2b3154
raw         778899
raw sha1    TVsSjiGg212a1xyntKxiVZXp+Ac=
packed sha1 Wsmnro1sXbBCthJDf/ETxpZJryA=
backup tag  pre-clean-20261023T135234Z
```

## R11.63 — Source parity integrated into AimKill

- Removed the separate Rotate, RequireFire and Source toggles. AimKill now always
  uses the source-parity path:
  - `StartFiring` -> `LHNJLFOINDO` -> `StopFire` per dispatch,
  - `hit.DOJDNHCGFGK = hitPoint`.
- Removed the camera-lock code and the rotate/require-fire branches. No held
  fire state remains.
- AIM menu is now only: AimKill, delay, status (scroll 2088).
- Tests updated for one shot per tick; Guard PASS 27/27; HUD/visual/Hitfly pass.
  Deployed via deploy.sh after backups; fetch matched.

```text
title       OKMOD R11.63
bytes       291382
sha256      306d3567dde4f3db1bdb3dd8963fafb65e091c6ba71174bc906d6a375a29de69
raw         777251
raw sha1    iW1M/8bzEverxu5kT4R/2Q+KDnw=
packed sha1 AII72nC6M3eM6ijqVAwZ2QL1jQM=
backup tag  pre-clean-20261023T135901Z
```

## R11.64 — Ground hack forces m_GroundHitResult

- The IsGrounded hook alone did not change movement on device. Player also has
  `public bool m_GroundHitResult` (0x1DC4), updated by the game's ground
  detection; that is the state gameplay code reads.
- While `Ground hack (native)` is ON, UpdateFeaturesOnce now writes
  `local.m_GroundHitResult = true` every frame (OnGUI runs after Update), in
  addition to the existing Player.IsGrounded hook.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.64
bytes       291421
sha256      bdf8b61a5f62a6a477ae2013583d167a0f99bd48bbf1274acd2971442e66ef0e
raw         777483
raw sha1    vAMDeZccd5auFnR/Y3qijgCwMRY=
packed sha1 sjaizlfwEczdwlWjS5Y4HUOptqw=
backup tag  pre-clean-20261023T140543Z
```

## R11.65 — Ground hack post-OnFixedUpdate hook

- The previous Player.IsGrounded redirect did not affect native direct calls.
  Ground hack now hooks the earlier `Player.OnFixedUpdate(float,float)` path.
- The generated body calls `<>iFixBaseProxy_OnFixedUpdate` first (preserving the
  original fixed-update logic), then calls `GroundHackAfterFixedUpdate(Player)`
  to force `m_GroundHitResult = true` when the toggle is on.
- This runs after the game's physics/ground update instead of from OnGUI.
- Guard PASS 27/27; AimKill/HUD/visual/Hitfly pass. Deployed via deploy.sh
  after backups; fetch matched.

```text
title       OKMOD R11.65
bytes       291449
sha256      35ea36c75e17a004bbe692588736cacc9ec3ee48cf6585227d7ed1786c5aec8e
raw         777555
raw sha1    d4vn3CzIIk5pFp3pDfgkWxsRbcc=
packed sha1 g5ihPTkftjG+wsLlKgzQdfxjZyE=
backup tag  pre-clean-20261023T152301Z
```

## R11.65 marker correction

- The first R11.65 deploy contained the R11.64 visible marker because the clean
  source labels were not bumped; the hook/build itself was R11.65.
- Corrected all visible/log markers to `OKMOD R11.65`, rebuilt and redeployed.
  Live bundle now contains two `OKMOD R11.65` strings and zero `OKMOD R11.64`.

## R11.66 — GroundHack: root cause, regression removed, real gravity hooks

Full analysis in `docs/GROUNDHACK-ANALYSIS.md`. Summary:

- Movement never uses `Player.IsGrounded()`; that method has only 4 callers, all
  rendering (`SceneGraphics` desert/LOD/snow, `ParticleEffectInRoomController`).
  `Player.IsGroundedByRayCastHit()` has zero callers. The R11.45 hook was inert.
- `Player.m_GroundHitResult` (0x1DC4) is written by the downhill raycast and read
  8 instructions later in the same frame (`0x57856C8` / `0x5785700`). The R11.64
  per-frame write was overwritten and never influenced ground state.
- R11.65 replaced `Player.OnFixedUpdate` with a body that (a) emitted `Call` with
  `nargs = 0` for `GroundHackAfterFixedUpdate(Player)` and (b) called
  `<>iFixBaseProxy_OnFixedUpdate`, which resolves to the empty
  `Entity.OnFixedUpdate` (`0x6B4B274: ret`) instead of the override. That erased
  `m_IsFixedUpdated` (0xAA5), which `Player.PCDNBOBFJBG` reads to discard pitch
  writes — the aim/pitch freeze.
- Removed both broken hooks; removed the dead R11.45 hook.
- Added leaf hooks `Player.get_RisingGravity` / `Player.get_FallingGravity`
  (consumed by `HADKJKALGOL` at `0x568D02C`).
  - ON  -> `-0.25f` (glide / bay)
  - OFF -> `InSnowSlideWayDashing ? SnowSlideWay*Gravity : self.get_CustomGravity()`
  - `get_CustomGravity` is left unpatched so UGC gravity is untouched.
- Menu row renamed `Ground hack (native)` -> `Bay / lơ lửng (IFix)`; the
  PlayerPrefs key `OKMOD.Move.GroundHack` is unchanged so saved settings survive.
- True walk-on-air needs a native hook on
  `UnityEngine.CharacterController::get_isGrounded` (PhysicsModule). IFix only
  rewrites Assembly-CSharp, and patched non-virtual methods have no trampoline.

```text
title       OKMOD R11.66
bytes       291469
sha256      27682dcacc40c1e55b786c8131383430d1bfbba51fb10f6c5f513616611f83c1
fixes       86 (81 base + Silent + ScatterRate + ShowDamageNum + 2 gravity)
guard       PASS 27/27
tests       aimkill / hud_lifecycle / visuals / hitfly PASS
published   no
```

## R11.67 — Bay: sửa jump bị teo (gravity dùng cho cả v0)

- R11.66 hạ cả `get_RisingGravity` xuống `-0.25f`. Sai: `Player.ELGCCFOEIID`
  (`0x57746BC`) dùng chính nó để tính vận tốc nhảy:

  ```
  0x5774800  bl  Player.get_RisingGravity     ; g
  0x5774804  s1 = -2.0f
  0x577480C  s0 = (h * -2.0f) * g
  0x577481C  s0 = sqrt(s0)                    ; v0
  ```

  `v0 = sqrt(2·h·|g|)` → `|g|` 9.8 → 0.25 làm v0 còn ~16%, jump thành cái nhảy tí.
- R11.67: ON chỉ hạ `get_FallingGravity` (`-0.25f`); `get_RisingGravity`
  giữ nguyên bản gốc. Kết quả: jump bình thường, rơi chậm/lơ lửng.

```text
title       OKMOD R11.67
bytes       291469
sha256      dd6b8d40fb409436847c01cad724cf8e06eb813d7f43dc266da0fd4bf98f109a
fixes       86
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261023T175632Z.bin
```

## R11.68 — Bay: đúng nhánh gravity (CustomGravity)

- R11.67 hạ `get_FallingGravity` nhưng không thấy gì. Nguyên nhân: trong
  `Player.HADKJKALGOL`, Rising/Falling chỉ được dùng khi
  `InSnowSlideWayDashing` (`0x568CE60 cbz [Player+0x2080], 0x568CFA4`).
  Nhánh rơi bình thường đi tới `0x568CFFC bl get_CustomGravity`.
- R11.68 patch cả ba getter:
  - `get_CustomGravity`: ON → `-0.25f`, OFF → `IPBGFJNGLOH.AMMOCNCMBBG`.
  - `get_RisingGravity`: luôn trả hằng gốc (hoặc field snow-slide) để
    `ELGCCFOEIID` giữ nguyên v0 nhảy.
  - `get_FallingGravity`: ON → `-0.25f`, OFF → snow field / hằng gốc.
- `AMMOCNCMBBG` = `-9.8f` (ghi trong `IPBGFJNGLOH..cctor` tại `0x63A6684`).

```text
title       OKMOD R11.68
bytes       291535
sha256      af590371ccd592b13130af7467be674d51fa44fd260f61205c908b9ad02f5bb7
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261023T180428Z.bin
```

## R11.69 — Bay thành slider gravity

- Row 22 đổi từ toggle sang slider `Bay gravity (9.8=tắt)`, 0.00–9.80, step 0.25.
  `get_CustomGravity` / `get_FallingGravity` trả `-value`; `9.8` = vanilla.
- `get_RisingGravity` vẫn trả hằng gốc để `ELGCCFOEIID` giữ v0 nhảy.
- Key cũ `OKMOD.Move.GroundHack`="X2" được migrate một lần sang 0.25.

### Chưa làm được: nhảy cao hơn

`Player.get_MaxJumpHeight` khi có match trả về config của match:
`CurrentMatch()` → `[+0x5B8]` → `[+0x14]` (float). Không dùng
`GameVarDef.MaxJumpHeight`, nên ghi static đó vô tác dụng trong trận.
Muốn slider nhảy thì phải xác định class/field của object `[+0x5B8]`
(dump metadata bị thiếu method đó) rồi write như `MaxJumpHeight`.

```text
title       OKMOD R11.69
bytes       291609
sha256      9e11d897b20ef6112b949eab8f1a6c2dd45fa11ced70d462752e8499b85e594d
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261023T181147Z.bin
```

## R11.70 — Slider nhảy + tốc chạy

- **Lực nhảy x** (row 21, 1.0–3.0 bước 0.2) → ghi `COW.GameVarDef.JSDeadGhostJumpScale`
  (0x4380), có lưu/khôi phục giá trị gốc.
  `Player.ELGCCFOEIID` (`0x57746BC`):
  `h = MaxJumpHeight * JSDeadGhostJumpScale * scaleMap`,
  `v0 = sqrt(h * -2.0f * RisingGravity)`, gravity không đổi ⇒ độ cao đỉnh tỉ lệ
  tuyến tính với slider (x2 = cao gấp đôi).
- **Tốc chạy x** (row 14, 1.0–2.0 bước 0.1) → `PlayerAttributes.RunSpeedUpScale`.
  `GetSpeedByType` (`0x6482A18`) = `GetBaseSpeedBySpeedType × GetSpeedScaleBySpeedType`,
  nên tốc độ bay ngang trong không trung cũng tăng.
- Giữ **Bay gravity** (row 22) như R11.69.
- Migrate key cũ: `OKMOD.Move.Speed=X2` → 1.3x.

```text
title       OKMOD R11.70
bytes       291736
sha256      a11fb27df27ac5127e7101e6e27bee9aeb0d55c548ea3ea0251a606ada6bf317
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261023T181945Z.bin
```

## R11.71 — Lực nhảy: đúng biến (RisingGravity), không phải JSDeadGhostJumpScale

- R11.70 ghi `GameVarDef.JSDeadGhostJumpScale` (0x4380) → không tác dụng, vì
  `ELGCCFOEIID` (0x57746BC) chỉ được gọi từ `Player.ICBEPIHDJIA`, mà hàm này
  chỉ được gọi từ `DDHJPJFOJHJ.onExecute(PraiseKeyData)` — nhánh ghost/replay,
  không phải jump thật.
- Bằng chứng jump thật dùng `get_RisingGravity`: R11.66 hạ `RisingGravity` thì
  jump teo; R11.67 trả lại thì bình thường. Và `HADKJKALGOL` chỉ dùng
  Rising/Falling khi `InSnowSlideWayDashing` (`0x568CE60`), còn ascent bình
  thường dùng `get_CustomGravity` (`0x568CFFC`).
- Công thức: `v0 = sqrt(2·H·|g_rise|)`, `apex = v0²/(2·|g_custom|)`
  ⇒ `apex = H·|g_rise|/|g_custom|` → **tuyến tính với slider**.
- R11.71: `RisingGravity` trả `VanillaGravity() * scale` (scale 1.0–3.0),
  nhánh snow-slide nhân tương tự. Bỏ write `JSDeadGhostJumpScale`.

```text
title       OKMOD R11.71
bytes       291766
sha256      a6aac5b051be79e8651a8589296cac78b4829915a336e71933ab42db6f87ddfe
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261023T184407Z.bin
```

## R11.72 — Lực nhảy: chỉ nhân khi còn grounded (hết giật)

- R11.71 nhân `get_RisingGravity` suốt cả ascent → từ ~1.3x trở lên physics bị
  giật (tích phân vận tốc/độ cao lệch giữa các frame).
- R11.72: chỉ nhân khi `Player.IsGrounded()` còn true. Cú nhảy được tính lúc
  nhân vật còn đứng đất (`I_BE_BlockButton.KPEHJKOBALP.EJODNAAHGMH`), nên xung
  lực vẫn được scale; khi đã bay thì trả về vanilla ⇒ ascent mượt.
- Không dùng được cửa sổ thời gian: `set_LastStartJumpTime` nhận
  `GameFacade.CurrentGameTimer()[0x10]` (đồng hồ trận), không phải
  `Time.realtimeSinceStartup`.

```text
title       OKMOD R11.72
addon       okmod_addon.py (unchanged)
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261023T185658Z.bin
guard       PASS 27/27
tests       4/4 PASS
```

## R11.73 — Giới hạn Lực nhảy ở 1.2

- Hiện tượng: >1.2 thì nhảy **thấp hơn** bình thường.
- Nguyên nhân: frame bật nhảy vẫn trả `IsGrounded()==true` (controller chưa rời đất),
  nên gravity đã nhân bị tích phân vào đầu ascent và triệt tiêu xung lực bật.
  Trên ~1.2 phần triệt tiêu lớn hơn phần cộng ⇒ thấp hơn vanilla.
  ⇒ Không thể dùng "nhân gravity" để có jump cao sạch.
- R11.73: slider `Lực nhảy x (max 1.2)`, step 0.05.
- Muốn cao hơn: hạ **Bay gravity** (đã có). `apex = H * |g_rise| / |g_custom|`
  ⇒ 4.9 ≈ x2, 3.27 ≈ x3, 2.45 ≈ x4, mượt vì gravity là hằng số.

```text
title       OKMOD R11.73
sha256      b101d95c7c8563b561fe4f230341640aa4860e7f03758c4e73096d9c0f3b3a57
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T022937Z.bin
```

## R11.74 — Nhảy cao: cửa sổ giảm gravity, không đụng RisingGravity

- Bỏ hoàn toàn việc nhân `get_RisingGravity` (nguồn gốc của giật và "cao hơn 1.2
  lại thấp hơn").
- Cơ chế mới: mỗi frame đọc `Player.get_LastStartJumpTime()`; khi giá trị đổi
  (vừa nhảy), đặt `R7JumpBoostUntil = Time.frameCount + 18`.
  Trong cửa sổ đó `get_CustomGravity` trả `gravity / JumpScale`.
  - Tốc độ bật giữ nguyên vanilla ⇒ không triệt tiêu xung lực.
  - Gravity là hằng số trong suốt cửa sổ ⇒ không dao động/giật.
  - Tốc độ rơi không đổi (hết cửa sổ trả về như cũ).
- Slider row 21: `Nhảy cao x` 1.0–3.0, step 0.2.

```text
title       OKMOD R11.74
sha256      808ae9c4c7346d7f9b36803260d189bbd2e7a8e3d8179ca00bf74efefcb8e256
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T023358Z.bin
```

## R11.76 — Log chẩn đoán nhảy

- Row 21 chỉnh được như cũ, hiển thị thêm: `v<vy×100> a<frame còn lại> g<số lần CustomGravity gọi trong cửa sổ>`.
- Chạm **giữa** row 21 → copy snapshot chi tiết vào clipboard:
  `jumpScale, bay, speed, jumps, stamp, state, grounded, vy, peak, arm, hitsInWindow,
   customCalls/customLast, risingCalls/risingLast, fallingCalls/fallingLast`
  (mọi giá trị float in dạng ×100).
- Mục đích: xác định (a) detector jump có bắn không, (b) ascent dùng gravity nào,
  (c) có bị clamp tốc độ không — trước khi đổi thêm hành vi.
- Không đổi cơ chế so với R11.74/R11.75.

```text
title       OKMOD R11.76
sha256      4e0ad233a8f6326965b30fbb51f2c258530c0014c9143180fb1bf3b03ed60e99
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T025552Z.bin
```

## R11.77 — Auto log nhảy (tự phân biệt mức x)

- Mỗi lần **rời đất với `GetVelocity().y > 1.0`** (nhảy thật) → mở entry, gắn
  `R7JumpScaleAt` = slider hiện tại.
- Tiếp đất → chốt entry: `x<scale> peak<vy max> god<n> rise<n> fall<n> st<state>`.
- 5 entry gần nhất hiển thị ở row 35–39 (scroll max nâng 2088 → 2520).
- Row 21: `v`/`a` live; chạm giữa → copy snapshot + toàn bộ history.
- Không cần bấm gì để ghi log.

```text
title       OKMOD R11.77
sha256      ed17830d5dbdd3752a5f179e3f716247ffc8fbb9d1e23e519a3a19a8374f8e86
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T030840Z.bin
```

## R11.78 — Log nhảy tự bắn lên server

- Mỗi entry log (sau khi tiếp đất) được GET lên
  `https://okmod.sbs/aimlog?v=11.78&jump=1&x=..&pk=..&god=..&rs=..&fl=..&st=..&n=..&bay=..&spd=..`
  (giới hạn 1 req / 0.6 s, dùng `R7JumpReq` riêng để không đụng AimKill log).
- nginx: `location = /aimlog { access_log /var/log/nginx/aimlog.log; return 204; }`
  → đọc trực tiếp bằng `grep jump=1 /var/log/nginx/aimlog.log`.
- Đã test: curl trả 204 và dòng log xuất hiện.

```text
title       OKMOD R11.78
sha256      eca5f99cf40730f0b75d3be5d4c779c2c3bfe49177fb0fa6e8bc0c4575093cd4
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T031346Z.bin
```

## R11.84 — Gộp thành 2 chức năng

- Row 21 `Nhảy cao x3.0`  → `OKMOD.Move.HighJump`: jumpScale 3.0, bay 9.8.
- Row 22 `Bay lơ lửng x3.0 + 0.3` → `OKMOD.Move.Hover`: jumpScale 3.0, bay 0.3.
- Hai công tắc loại trừ nhau (bật cái này tự tắt cái kia).
- Bỏ slider và chẩn đoán trên row; giữ auto-upload + 5 dòng history row 35–39.
- Cơ sở: log R11.83 (bay tắt) cho x1.0 h=69.0, x3.0 h=194.5 = 2.82×, `pk` không đổi.

```text
title       OKMOD R11.84
sha256      c06f75343f0c2f2e76f5df3c23b1f284c2f9d471cdcef2fa12b2e9009ced4414
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T034647Z.bin
```

## R11.85 — Xoá "Lướt Khi Bắn"

- Bỏ row toggle `OKMOD.Move.FireDash` ("Lướt Khi Bắn").
- Bỏ slider `OKMOD.Move.FireDashSpeed` ("Lướt bắn") ở cột phải.
- Bỏ toàn bộ block runtime trong `UpdateCombat` (pulse `ForceSetRunAndDashSpeed`
  khi `IsFiring()`), cùng các field `R7DashOwner/Old/Until/Next/Applied`.
- Các row phía sau dời lên 1; log nhảy giờ ở row 34–38.
- Giữ `Nhảy cao x3.0` và `Bay lơ lửng x3.0 + 0.3` (loại trừ nhau).

```text
title       OKMOD R11.85
sha256      051a0e47f1f0f6401873d4ee4d301dcd0a4a53fd3ea02f5f2cbffc900a6c7e18
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T035236Z.bin
```

## R11.87 — AimKill: burst fire, 0.08s, no ammo refill, target lock, FOV option

- **Burst**: `StartFiring` một lần cho cả burst, `LHNJLFOINDO` mỗi phát,
  `StopFire` khi mất target (0.4 s), reload, exception hoặc tắt tính năng.
  Không giảm `LHNJLFOINDO` xuống 1 lần/burst vì ở build 1297 **nó chính là call
  gây damage**; cờ `hasStartedDamage` của bản Android thuộc chain
  `Player_TakeDamage` không tồn tại ở đây.
- **Cadence**: mặc định 0.08 s (trước 0.12); mode 0.05/0.08/0.15/0.25.
- **Bỏ refill đạn** `FullWeaponAmmo()` mỗi 0.5 s.
- **Target lock**: giữ 3 s, gia hạn mỗi phát; nhả khi chết/dieing/hp<=0/team/
  inactive/ngoài range/mất LOS/hết hạn.
- **FOV option**: row 34 `AimKill giới hạn FOV`, mặc định tắt, dùng bán kính
  `OKMOD.Aim.FOV`. Log rows dời 35–39, scroll 2520 → 2592.
- `test_aimkill.py` cập nhật theo contract burst; 4/4 suite PASS.

```text
title       OKMOD R11.87
sha256      56c1107f1a890179ef49034052e33fa72e3d022fe4e5678590381296ca7d618e
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T044354Z.bin
```

## R11.90 — AimKill: gọi đúng entry point của hit (CIHHEOKFADJ)

Truy vết call graph:

```
HBIBDMMOOOK.CIHHEOKFADJ  (slot 223, 0x5C22FA8)   <-- entry thật
   ├─ LHNJLFOINDO(hit)        (0x5C23B54)  chỉ là bước set/damage calc
   ├─ UGCHardCodeManager.HandleCanWeaponHitEntity
   ├─ LJNIOABIFCA / hit effect
   └─ GameFacade.Send(...)    gửi hit lên server
```

`LHNJLFOINDO` chỉ có **2 caller**, cả hai đều nằm trong `CIHHEOKFADJ`.
Vì vậy gọi `LHNJLFOINDO` trực tiếp = tính damage local (hiện số) nhưng **không
gửi hit** → server không trừ HP. Đúng triệu chứng "hiện damage mà không kill".

R11.90 đổi sang `weapon.CIHHEOKFADJ(hit, hitObject, 0f)`.

```text
title       OKMOD R11.90
sha256      402413bbd3e0408be4003e4a3674d55b02e6a6a3e3db2b415448b0d414eaf31f
fixes       87
guard       PASS 27/27
deployed    /opt/okmod-1297/patch.bin
backup      patch.pre-clean-20261024T051616Z.bin
```

## Hạ tầng — bỏ /opt, chạy trực tiếp từ /root (2026-10-24)

- Unit `okmod-1297.service` trỏ thẳng vào repo:
  `WorkingDirectory=/root/okmod-1297-clean`,
  `OKMOD_PATCH=/root/okmod-1297-clean/build/okmod-R8-AIM-ESP.bin`,
  `-s proxy/okmod_addon.py --set confdir=proxy/conf`.
- `deploy.sh` không còn copy sang /opt: backup payload cũ vào
  `backup/patches/`, build, guard, copy PAC sang `/var/www/okmod-1297/`
  (nginx không đọc được /root), restart, verify.
- nginx: `/pac-1297.js` và `/pac.resolved.js` chuyển sang
  `alias /var/www/okmod-1297/...`.
- Toàn bộ `/opt/okmod-1297` cũ (kèm ~40 bản patch `.pre-clean-*`) đã nén vào
  `attic/opt-okmod-1297-20261024T060530Z.tar.gz` rồi xoá; các bản patch được
  giữ riêng trong `backup/patches/` để rollback.
- Đã kiểm chứng sau chuyển: service `active`, `/healthz` 200, PAC 200/200,
  journal ghi `Loading script /root/okmod-1297-clean/proxy/okmod_addon.py`.

## R11.92 — Bỏ hẳn CIHHEOKFADJ (nguyên nhân menu biến mất)

- R11.90 thêm `weapon.CIHHEOKFADJ(hit, hitObject, 0f)`. Metadata build này bị tráo
  tên kiểu, không xác định được kiểu tham số thứ 2 → IFix không bind được extern
  → **reject toàn bộ patch** → mất menu.
- Rollback R11.89 chỉ đổi file trên server; **source vẫn còn call**, nên R11.91
  build lại và chết y như vậy. 1299 vẫn chạy vì payload của nó không có extern đó.
- R11.92: trả về `weapon.LHNJLFOINDO(hit)`, xoá stub khỏi `build.py` và test.
  Payload đã kiểm: **0 lần** chuỗi `CIHHEOKFADJ`.

```text
title       OKMOD R11.92
sha256      b385dd86ea8c6ab8f360e123858b06c41c07075be04692a27751994433a8d01f
packed sha1 6NrpReGj2+luaCvggJYnaLhkrTo=
size        293210
fixes       87
guard       PASS 27/27
served via  /root/okmod-1297-clean (no /opt)
```

## R11.93 — AimKill giết thật: dùng đúng entry CIHHEOKFADJ

- R11.92 chỉ gọi `LHNJLFOINDO` (hàm tính damage nội bộ, chỉ có CIHHEOKFADJ gọi
  tới ngoài thunk) → hiện số damage nhưng server không nhận → không chết.
- Đã giải mã đúng kiểu tham số thứ 2 của `CIHHEOKFADJ`: type index `44163` →
  class table `tix 40942` → **`UnityEngine.MonoBehaviour`**. Slot đầu đã chạy
  được là `CGKJLKPMGDJ`: type index `25633` → `tix 34207` → `COW.GamePlay.CGKJLKPMGDJ`,
  nên phép ánh xạ này đã được kiểm chứng chéo.
- R11.90 chết vì truyền `GameObject` vào slot đó → IFix không bind → reject patch.
  R11.93 khai báo `UnityEngine.MonoBehaviour`, thêm `Behaviour/MonoBehaviour` vào
  stub và cho `Player` kế thừa `UnityEngine.MonoBehaviour` (đúng chain thật
  `Player → AttackableEntity → COWReplicationEntity → ReplicationEntity → Entity → MonoBehaviour`)
  nên không sinh `castclass` lúc chạy.
- AimKill giờ gọi `weapon.CIHHEOKFADJ(hit, enemy, 0f)` (tương đương
  `original_WeaponFire` của bản PC); `LHNJLFOINDO` chỉ còn là fallback khi call
  engine ném exception. Log có thêm `pc=1/0` để biết nhánh nào chạy.

```text
title       OKMOD R11.93
sha256      2ea2e26dd1ca0da190c4f16658edbd052e58bf929c320d9a0a78d433ed1cac3f
packed sha1 ScO7Kc5wBuRi6QEy2EOJQWEaOUU=
size        293271
fixes       87
guard       PASS 27/27
rollback    cp backup/patches/patch.pre-clean-20261024T112041Z.bin build/okmod-R8-AIM-ESP.bin && systemctl restart okmod-1297
```

## R11.94 — Mở được log AimKill (pc=) + nút COPY

- R11.93 có ghi `pc=` trong log nhưng `AimKillLogRow`/`AimKillCopyRow` **không được
  gọi** trong `DrawAimMenu` → log không có đường mở, nên trong game không thấy.
- Thêm row 40 "Xem log" (scroll 2592 → 2664) mở overlay log toàn màn hình.
- Overlay có thêm nút **COPY** cạnh **ĐÓNG** (copy `BuildAimKillLog()` vào clipboard,
  báo `ĐÃ COPY` / `COPY LỖI`).
- Payload mới: xem `docs/release-R11.94.json`.

```text
title       OKMOD R11.94
sha256      b641cbcf6287ee2e1aac935ef08b96614499b81a1970670dc8dcf2243429a1c7
rollback    cp backup/patches/patch.pre-clean-20261024T113016Z.bin build/okmod-R8-AIM-ESP.bin && systemctl restart okmod-1297
```

## R11.95 — Log AimKill tự gửi lên proxy (không cần copy tay)

- Payload gửi log AimKill của chính nó tới `http(s)://core-gmc.freefiremobile.com/okmod/aimlog`
  — host đã có trong PAC 1297 nên request đi thẳng vào proxy.
- Addon trả `200 ok` và ghi mỗi báo cáo thành một dòng JSON vào
  `logs/aimkill-online.log` (thời gian, IP client, scheme, tag, toàn bộ log).
- Tự gửi khi AimKill ra kết quả, khi ngoại lệ, và từ nút **GỬI LOG** mới trong overlay
  log (cạnh COPY). Giới hạn 3 giây/lần, tag `shot` / `err` / `man`.
- Log có thêm `online=<state> sent=<count> tag=<last>` để biết transport có chạy.
- Vận chuyển: `UnityEngine.Networking.UnityWebRequest` thuộc
  `UnityEngine.UnityWebRequestModule` (image 24 của metadata hiện tại) — khai sai module
  là cả patch bị từ chối y như R11.90.

```text
title       OKMOD R11.95
size        294339
sha256      c17d7b59997b8bad14994d1237990c4bfce41d034b47d193f6636d87f359247b
fileinfo    codepatch/assembly-csharp-patch,NfmitIMiM+YUs2LbBY8ADM/AgRs=,790947,0,7a4VpaZy9eYz4N/B85/buzxbp+U=,294339,True,0
rollback    cp backup/patches/patch.pre-clean-20261024T114143Z.bin build/okmod-R8-AIM-ESP.bin && systemctl restart okmod-1297
```

## R11.96 — Log AimKill qua Nginx HTTPS

- Chuyển sang `https://okmod.sbs/aimlog?aimkill=1`, cùng đường gửi log nhảy
  đã thấy request thật từ game. Nginx trả `204` và ghi riêng
  `/var/log/nginx/okmod-aimkill.jsonl`; access log cũ vẫn được ghi.
- Đọc giải mã: `python3 proxy/read_aimkill_log.py -n 10`.
- Tự gửi mỗi 3 giây khi bật AimKill (kể cả không tìm được mục tiêu), sau shot/error,
  hoặc từ nút GỬI LOG. Request đang chạy được giữ tới khi hoàn tất/timeout 10 giây.
- `online=1`: đang gửi; `online=2`: nhận HTTP 2xx; `online=-2`: lỗi.
  `sent` chỉ đếm thành công đã xác nhận; `http`/`onlineError` tách khỏi lỗi AimKill.
- Mã hóa UTF-8 cho tiếng Việt và ký tự ngoài ASCII; có kiểm tra roundtrip.
- Nginx public selftest `204`, đọc lại JSONL đúng; chưa có xác nhận thiết bị R11.96.
- Core guard PASS 27/27; AimKill và transport recording suite PASS;
  nginx/proxy active, health 200. Chi tiết và rollback: `release-R11.96.json`.

## R11.97 — Hiện LOG / COPY / GỬI cố định trong trang AIM

- Lỗi thực tế: `ReadMenuPointer` vẫn chặn vuốt AIM ở 2088; row 40 Xem log
  cần cuộn ít nhất 2440 để lọt khung. `ScrollButtons` có max 2664 nhưng vuốt
  vẫn bị clamp lại. Dùng chung `MenuScrollMax()` cho vuốt và nút AIM (2664).
- LOG / COPY / GỬI luôn hiện ở cột phải, dưới thanh Mượt và trên hai nút
  Trực tiếp / Mượt 6; không phụ thuộc cuộn danh sách trái.
- COPY gọi chung helper với overlay log, xác minh clipboard và báo kết quả.
- HUD suite kiểm tra hiển thị khi chưa cuộn, click LOG ở cuối danh sách,
  clipboard chứa pc/online, max AIM/ESP; PASS. AimKill suite PASS, guard 27/27.
- Proxy active, health 200; chờ kiểm tra hiển thị trên thiết bị.
- Hash và rollback: `release-R11.97.json`.

## R11.98 — Nối đầy đủ PC damage chain

- Đã thay đường `CIHHEOKFADJ` bằng chuỗi đã kiểm tra exact theo registration:
  lấy `JGIEOGCGFEP`, gọi `LHNJLFOINDO`, lấy base damage/weapon id,
  tạo `HNLIMBLIANL` + `PIAEKELILCJ`, lấy `List<float>` từ
  `NPLMKLBKJPE`, rồi gọi `PlayerNetwork.TakeDamage` đủ 9 tham số.
- Không còn `CIHHEOKFADJ` trong source OKMOD.
- Contract runtime phân biệt alias dump với type thật và kiểm tra cả return type,
  parameter type, static/instance flag, field type/offset trước khi build.
- Log mới: `chainStage`, `dispatched`, `baseDamage`, `checks`, `startResult`,
  `chainError`; `pc=1` chỉ được đặt sau khi `TakeDamage` trả về.
- Toàn bộ test AimKill/HUD/visual/hitfly PASS; core guard 27/27; proxy health 200.
- Chưa coi là đã kill thật cho tới khi log thiết bị có `dispatch=1` và `probeOk` tăng.
  Chi tiết/hash/rollback: `release-R11.98.json`.

## R12.00 — Chặn xuyên vật cản, không can thiệp bắn tay, giảm giật/timeout

- Dùng `ClearAimKillSight` riêng: mọi collider đầu tiên không thuộc local hoặc
  target đều chặn dispatch. Không còn quy tắc bỏ qua collider lạ ở khoảng cách gần.
- Nếu người chơi đang bắn tay, AimKill bỏ qua tick đó (`manualSkips`) thay vì
  gọi `StartFiring`/`StopFire` cạnh input thật.
- Giảm scan mỗi tick tối đa 48 ứng viên và ngân sách 2.5 ms; delay mặc định từ
  120 ms lên 250 ms để giảm giật và áp lực timeout.
- Log thêm `scanned`, `blocked`, `manualSkips`, `effectiveRange`; dùng để phân biệt
  wall-block, xung đột nút bắn và sai range vũ khí.
- Toàn bộ test PASS, guard 27/27, proxy health 200. Chờ log thiết bị R12.00.

## R12.01 — Khôi phục bắn/chuyển động gốc để cô lập lỗi

- Báo cáo thiết bị R12.00 vẫn kẹt cò, không ra đạn, giật khi đi và timeout.
  Tăng delay/giảm scan chưa loại bỏ synthetic Start/Stop và shared-hit writes.
- Tạm ngắt damage chain tự gửi; menu ghi rõ **Khôi phục bắn/đi gốc**, trạng thái 8.
  Không còn đường từ HUD tới Start/StopFire, refill ammo hay damage RPC. Legacy
  implementation được giữ trong source nhưng không nằm trong graph payload mới.
- Khi bật chế độ khôi phục, suppress các tùy chọn đạn/reload/rapid/swap/speed/gravity
  xung đột và restore giá trị đã áp dụng; không xóa tùy chọn người dùng.
- Log `mode=native-recovery syntheticDamage=disabled`, `recovery=on/off`,
  `speedApplied`/`gravityApplied`. Tốc độ mặc định 1.0, tự gửi log tối đa mỗi 10 giây.
- Sửa tiếp encoder: bỏ concat Char cho dấu cách; percent-encode ASCII bằng string.
- Các suite PASS, guard 27/27; tải payload qua proxy live khớp SHA256.
  Chưa có chứng cứ thiết bị xác nhận hết giật/timeout. Xem `release-R12.01.json`.

## R12.02 — Tách recovery và khôi phục công tắc độc lập

- R12.01 gộp recovery vào key AimKill gây hiểu nhầm. Nay dùng key riêng
  `OKMOD.Diagnostic.NativeRecovery` (mặc định tắt), row 41, max scroll AIM 2736.
- Row 31 ghi AimKill tạm ngừng, chạm mở log; không giả lập công tắc bật không chạy.
- Recovery giữ nguyên lựa chọn Aimbot/Silent để khi tắt có thể hoạt động lại.
- Sửa lệch đơn vị speed giữa ESP (10-20) và AIM (1-2): cả hai dùng 1-2,
  dữ liệu cũ 10-20 chuẩn hóa một lần. Tắt jump/hover trả gravity ngay.
- Log thêm speedSetting, rapid/ammo/reloadApplied, combatError.
- Test bật/tắt/bật lại No reload/Infinite ammo/Rapid/speed PASS; cả 4 suite và
  guard 27/27 PASS; payload tải qua proxy live khớp hash. AimKill tự gửi damage
  vẫn tạm ngừng; chưa có xác nhận trên thiết bị cho từng chức năng khác.

## R12.03 — Trả lại công tắc AimKill và đường damage độc lập cò

- Thanh tiêu đề có AK ON/OFF cố định; row 31 AIM là công tắc AimKill thật.
  Cả hai dùng key cũ, recovery vẫn là key riêng và có quyền tạm dừng.
- Đường damage được đưa lại vào graph payload: JGIEOGCGFEP, KBIFKMJODNA,
  NPLMKLBKJPE, HNLIMBLIANL/PIAEKELILCJ, TakeDamage 9 tham số.
- Không gọi StartFiring/StartWholeBodyFiring/StopFire/FullWeaponAmmo/LHNJLFOINDO.
  Hit-info riêng, không sửa FDMIEDDNCEC; tránh chồng hit-effect và explicit damage.
- Đợi probe trước lần gửi tiếp, ưu tiên bắn tay/reload; scan xoay 12 mục/tick,
  delay hiển thị khớp 500/1000/1500/2000ms. Probe chỉ là quan sát HP, không phải
  bằng chứng server chấp nhận hay hạ mục tiêu do riêng mod.
- Test nút header, bật/tắt/bật, damage dispatch, shared-hit isolation, cover,
  manual fire, probe backpressure PASS. Các suite khác PASS, guard 27/27.
- Tải payload qua proxy live khớp hash, health 200; chờ thử thiết bị R12.03.

## R12.04 — Sửa dữ liệu check params theo luồng native

- Log thiết bị R12.03: chuỗi trả về nhưng HP phần lớn không giảm. Không coi
  stage 9/pc=1 là bằng chứng server nhận damage.
- Native tại 0x5C25194-0x5C251A8 truyền damage-info thực vào NPLMKLBKJPE;
  R12.03 truyền null. Nay truyền info đã điền trước TakeDamage.
- Native đi weapon.PIILKNEHNLB.MHMGCLILKGJ.GetId() để điền damage-info definition
  ID (0x5C24B0C-0x5C24B24); thay cách đọc JAOECECKADN không đúng field đó.
- Log mới localReason/lastDispatchAt/dataId; khi không có trận/local hợp lệ,
  xóa pc flag, target lock và probe đang chờ, giữ counters lịch sử.
- Test dùng hai ID khác nhau và kiểm tra damage-info không null; các suite PASS,
  guard 27/27. Payload tải qua proxy khớp hash; chờ xác nhận thiết bị R12.04.

## R12.05 — Chẩn đoán không giảm HP, bỏ lỗi recovery lưu cũ

- R12.04 chưa có tác dụng theo báo cáo thiết bị. Log trực tiếp còn thấy HP tăng
  89->140, nhiều 200->200; không dùng probeOk để kết luận kill/server accepted.
- Thêm IsInvincible check (không sửa trạng thái bất tử) và xác nhận attackable
  trả về đúng gameObject mục tiêu. Ghi HP ngay trước/sau lời gọi, đếm probeHealed.
- Resume recovery xóa chainError/chainStage lưu cũ, tránh recovery=off vẫn báo paused.
- Regression cases mới PASS; cả 4 suite PASS, guard 27/27; payload tải proxy khớp.
- Đây là bản chẩn đoán, chưa khẳng định sửa được auto damage; cần log và chế độ trận.

## R12.06 — Đối chiếu BS4484/PC, chuyển về engine hit entry

- BS4484 `old_BLAGCMCGEJG1` hook trỏ LHNJLFOINDO (cheat.cpp:130), PC
  `offset_WeaponFire` cũng trỏ method đó (backend.cpp:643). Kết luận trước rằng
  method chỉ tính số local là sai; native có dựng damage info, dynamic info,
  check params và xử lý hit. Chi tiết: `AIMKILL-REFERENCE-R12.06.md`.
- Thay caller TakeDamage tự dựng bằng `weapon.LHNJLFOINDO(hit)` duy nhất.
  Không nhân đôi explicit damage và engine damage; không thêm lại trigger/refill.
- Log `mode=engine-hit`, path=LHNJLFOINDO, engineInternals=unobserved.
  `checks=0`/`dataId=0` ở mode này nghĩa không quan sát intermediates engine.
- Test entry/không double-submit/manual fire/cover/isolated-hit PASS; mọi suite
  và guard 27/27 PASS; payload tải qua proxy live khớp hash. Chờ thiết bị xác nhận.

## R12.07 — Tách probe và nhịp hit, dùng nhịp vũ khí native

- R12.06 dùng 500/1000/1500/2000ms và probe 700ms chặn hit; ở mode 500ms
  kiểm tra probe còn pending rồi dời timer làm nhịp có thể thành gần 1 giây.
- Nay UI 120/200/300/500ms; nhịp thực max(user,native), chọn LDKCJEMDAMC hoặc
  BDHMJJIADFG theo KENANOGLPJO như native tại 0x5C22650-0x5C226B8.
- Poll 50ms tách deadline hit; không catch-up burst. Probe 700ms chỉ quan sát,
  không chặn hit, không overwrite/postpone pending window cùng mục tiêu.
- Log cadenceMs/workMs/probeDropped. Giữ LOS trước hit, 12 ứng viên/poll,
  không thêm giữ cò/refill/duplicate damage. Các suite và guard PASS; live hash khớp.
- Tổng hợp client/server/IFix/reference/latency: `AIMKILL-MECHANISM-R12.07.md`.
  Chưa có server ACK/rate threshold; không coi probe ratio là tỷ lệ hit chính xác.

## R12.08 — Mẫu bắn tay thật để đối chiếu ngữ cảnh

- Người dùng xác nhận trận thường: cùng mục tiêu nhận damage bắn tay, không nhận
  synthetic damage. Không coi mode/sân tập là lời giải cho log đó.
- Thêm observer chỉ đọc trong getter hit-info hiện có, khi AK bật và local bắn thật;
  tối đa mỗi 250ms, chỉ giữ text fields damage/distance/layer/group/flags/positions.
- Không sửa shared hit, không gọi thêm fire/hit/RPC. Bản này không sửa cadence,
  không tuyên bố khắc phục damage; cần manualHit mẫu từ thiết bị.
- Observer test, các suite và guard PASS; hash payload tải live khớp, health 200.

## R12.09 — Chu kỳ phát bắn có ownership và cleanup đồng bộ

- Thêm lại hướng ngắm/start/hit/stop theo nhánh BS4484, nhưng giữ quyền sở hữu
  bằng local/weapon capture và `finally`, không hẹn StopFire frame sau.
- Bắn tay/reload/vật cản bỏ qua trước khi mở cò; lỗi Start một phần vẫn cleanup.
  Đổi súng trong native call vẫn Stop đúng khẩu đã Start; chỉ restore aim nếu
  game chưa đặt rotation mới. Cleanup lỗi khóa retry tới khi khởi động game lại.
- Giữ hit riêng, nhịp weapon-aware; không swap/refill/teleport/explicit TakeDamage.
- Test thứ tự và các nhánh lỗi PASS; cả 4 suite và guard 27/27 PASS; live hash khớp.
  Log shotStarts/shotStops/shotFault/cleanupError. Chờ thiết bị, chưa khẳng định
  damage server hoặc hết kẹt cò từ recording tests.

## R12.10 — Audit reference còn thiếu, mức 80ms và bỏ telemetry lẫn bắn tay

- Thêm mode 4=80ms, vẫn giữ nhịp native làm sàn; không đổi nghĩa mode cũ.
- Observer không ghi native firing do mod tự mở là bắn tay. Probe qua mẫu bắn tay
  được tách sang probeMixed; chưa thể loại trừ người khác/heal/server correction.
- Sau StartFiring recheck local/vũ khí/HP/LOS, hủy hit cũ và cleanup đúng khẩu.
- Tổng hợp các khác biệt swap-sync/native pointers/LateUpdate/hit info/restore aim
  và điểm chưa chứng minh: `AIMKILL-GAPS-R12.10.md`.
- Các regression mới PASS; toàn bộ 4 suite và guard 27/27 PASS; live hash khớp.
  Không khẳng định hết damage ảo từ test stub; chờ log R12.10.

## R12.11 — Tùy chọn thử inventory sync message 108

- Key `OKMOD.Aim.Sync108`, default OFF; row 42 Thử đồng bộ súng 108, max scroll 2808.
- Chỉ sau engine hit trả dương và cleanup xong; player id + instance unique id
  (ODFDGMOCJLL), Send(108, EOBFCLNNIJH, 2, false), không dùng dataId.
- Recheck match/local/on-hand/UID/reload/manual firing/recovery/fault. Max một
  attempt/250ms, không queue, lỗi sync không biến thành retry hit hay giữ cò.
- Log sync108 counters/state/uid/error. Tests default-off/order/IDs/false/throw/
  stale uid/cleanup-failure PASS, cả 4 suite và guard 27/27 PASS; live hash khớp.
- Ghi chú lịch sử R11.36: OFF từng ổn hơn. Đây là thử A/B, chưa chứng minh hết
  damage ảo; Send true không phải server ACK. Xem release-R12.11.json.

## R12.12 — Bỏ chờ dư của scheduler, hiển thị native cadence thật

- Log R12.11: user80/native1820/effective1820/lastGap1848ms; 108 false 4/4.
  Đây là nhịp vũ khí đang làm chậm chủ yếu, không phải call native ~0.37ms.
- Cooldown hẹn poll tại deadline nếu sớm hơn chu kỳ poll 50ms; đổi khẩu không
  kế thừa deadline từ khẩu cũ. LastDispatchAt vẫn là lịch sử không bị giả reset.
- Menu chọn/thực, status10 và cooldownRemainingMs minh bạch điểm chờ; giữ native
  lower bound/no catch-up. Không tuyên bố 80ms vượt giới hạn mọi súng.
- Regression và 4 suites PASS, guard27/27; live hash khớp; chờ thử thiết bị.

## R12.13 — Bỏ sàn native do OKMOD tự thêm

- Đính chính: getter native được dùng ở nhánh firing không chứng minh là giới hạn
  bắt buộc của mọi direct engine-hit. max(user,native) trước đây là policy của mod.
- Nay lịch hit bằng delay người dùng, native chỉ quan sát. Invalid/exception
  getter ghi -1 và không ép fallback 500ms. UI nói rõ lịch thay vì hứa hit hiệu lực.
- Regression user80/native1820 vẫn hẹn80, repeat/invalid/throw không kéo dài;
  giữ kiểm tra vật cản/manual/reload/cleanup/no catch-up. 4 suites và guard PASS.
- Proxy live tải khớp hash, health200; chờ thiết bị. Không hứa server nhận mọi hit
  ở nhịp chọn hay hết damage ảo/timeout. Xem release-R12.13.json.

## R12.14 — Cấu hình AK riêng và tách quyền bắn tự động/cò tay

- Key OKMOD.AK.Range/FOV/Point/ManualSmooth tách khỏi aimbot, không copy ngầm
  giá trị cũ. Defaults150m/220px/đầu/0; chỉnh riêng trong AIM rows43-48.
- Auto/manual toggles riêng. Giữ cò tay chỉ ngắm cho native shot, không bắn thêm,
  không Start/StopFire/engine-hit/108; thả cò có grace150ms trước auto tiếp tục.
- Owner/match/weapon/target cache có expiry200ms, recheck cover/range/FOV/reload/
  dead/inactive mỗi update manual. Không sửa shared-hit hay xóa lựa chọn aimbot.
- Tests config/smoothing isolation và transition/cover/reload/recovery/switch/dead/
  stale/match cases PASS. Build gate cấm fire/RPC calls trong graph manual.
- 4 suite và guard27/27 PASS; tải proxy khớp hash, health200. Chờ thiết bị;
  manual là aim assistance cho đạn gốc, không phải thêm synthetic damage khi giữ cò.

## R12.15 — AimKill một công tắc, preset cố định

- Gộp preset đầu/tầm500m theo range súng/không FOV/direct manual/80ms; auto và
  manual tự chuyển quyền. Bỏ hàng thiết lập AK và jump-history khỏi menu AIM.
- Không xóa setting cũ nhưng không còn dùng để tác động preset. Các tính năng
  khác giữ nguyên; recovery/log/copy/send vẫn còn. Sync108 không còn caller.
- Test preset với settings cũ mâu thuẫn, transition/cover/native-hit/error-cleanup
  PASS; HUD/visual/HitFly PASS, guard27/27. Payload tải proxy khớp, health200.
- Đây là preset rộng/nhanh nhất hiện có, không phải chứng minh damage server
  mạnh nhất. Chờ thiết bị R12.15; rollback trong release-R12.15.json.

## R12.16 — HF MAP: mô hình bay vồng + tốc độ riêng

- Thêm MAP tốc tối đa 5–100m/s (mặc định25), độ vồng10–200m (mặc định30),
  AIM rows35–36. Tính thời gian/vận tốc để đạt đích ở nhánh đi xuống trong
  mô hình trọng lực hằng; điểm đáp lấy raycast mặt rắn tại XZ dấu map, không
  dùng Y của layer map. Không có mặt đã tải/không có nghiệm thì không gửi.
- Quan sát cất cánh/đỉnh/đáp/độ lệch10Hz, log hfMap đặt đầu report và gửi qua
  route Nginx sẵn có cả khi AK OFF. Không teleport, không gửi bù mỗi frame.
- Giữ camera HITFLY cũ và mã bắn/ngắm AimKill. Hủy queue khi recovery; chặn
  phóng MAP lúc đang bay, hover/highjump/snow-slide hoặc chưa phục hồi gravity.
- Test234 nghiệm mô hình cùng packet/UI/lifecycle/error paths PASS; bốn suites
  PASS, guard27/27. Live proxy tải khớp SHA256, service active, health200.
- Chưa có thiết bị xác nhận đáp đúng: time field/native trajectory/server limits
  còn chưa được chứng minh. Xem HITFLY-MAP-R12.16.md và release-R12.16.json.

## R12.17 — Báo rõ HF MAP bị chặn, không còn im lặng cạnh nút nổi

- Log thiết bị R12.16 có status12/state0/t0/xz0/vy0 cùng gravityApplied1:
  lệnh đã vào preflight nhưng chưa gửi vì điều kiện movement/gravity.
- Tách reason high-jump/hover/not-grounded/snow-slide/gravity-restoring. Hiện
  thông báo cụ thể8s cạnh nút HF MAP khi đóng menu; queue/rejection cũng đẩy
  dirty snapshot vào telemetry cũ ngay cả AK OFF. Không tự tắt setting khác.
- Bốn suite PASS, thêm test lý do/thông báo/bảo toàn setting/launch lại sau khi
  hết xung đột; guard27/27. Proxy hash khớp, service active health200.
- Đây là sửa phản hồi lúc chặn, không phải xác nhận đã sửa native flight hay
  đáp đúng. Chờ thử lại sau khi tắt Nhảy cao/Bay và đứng đất.

## R12.18 — HF MAP đơn giản: hướng dấu + lực lên cao

- Theo yêu cầu bỏ mô hình điểm rơi R12.16/17: không solver, raycast mặt đáp,
  khóa theo state bay hay preflight highjump/hover/ground/snow/gravity. Giữ
  recovery và kiểm tra local/ID/marker/cooldown. Không tự đổi setting khác.
- MAP dùng hướng ngang XZ dấu, thời gian1, lực dọc50, tốc ngang chỉnh5..100
  mặc định25. Menu chỉ thêm hàng tốc MAP, bỏ độ vồng. Camera HITFLY cũ giữ nguyên.
- Bốn suite PASS; guard27/27; build loại4 helper cũ khỏi compiled graph. Service
  active health200, payload tải proxy hash khớp. Release-R12.18.json có rollback.
- Không cam kết tránh mọi vật cản/đáp đúng: đây chỉ là impulse lên cao, chưa có
  thiết bị xác minh lực50. Hồ sơ mô hình HITFLY-MAP-R12.16.md chỉ là lịch sử.

## HF / HF MAP isolation WIP — chưa deploy

- Tách queue/cooldown/status/counter/error trong source; bốn suite PASS và
  guard27/27. Candidate lưu build/candidates/R12.19-isolation-only, chưa phát hành.
- Đường bay vồng tới đích rồi dừng/rơi còn thiếu bộ điều khiển/phanh đã xác minh.
  Native StopHitfly tìm thấy là NPC/animation; SetForceMoveVelocity là cộng force
  theo key, chưa chứng minh dừng HitFly. Không thêm call phỏng đoán.
- Khôi phục production artifact R12.18 từ backup do build ghi trực tiếp file đó;
  không restart service. Generated report/patch hiện thuộc candidate, không phải
  artifact production đã khôi phục. Xem HITFLY-ISOLATION-WIP.md trước khi build.

## HF review — native evidence + staged build, chưa phát hành

- Thêm audit read-only `buildkit/audit/hitfly_motion_audit.py`, xác minh hash native,
  instruction anchors, shape packet657, vector GetVelocity/sync và type-index của
  motion object lồng (tên GFLBFKKDAMK bị trùng với class khác). Report tại
  docs/hitfly-motion-native-audit.json; regression native-audit PASS.
- Xác nhận SyncStateWithServer có thể ghi đè vector vận tốc; packet657 không có
  tọa độ đích riêng. Chưa xác minh actuator/phanh đích; không thêm setter giả định.
- Candidate không gửi chồng HF/MAP khi cùng lượt xử lý: hủy cả hai với status14,
  giữ deadline riêng. Bổ sung exception-isolation hai chiều, conflict cả2 thứ tự.
- Build có `--output-dir`: source vẫn build/, toàn bộ generated files vào thư mục
  riêng. Đã build candidates/hf-review nhãn R12.19 DEV, 4 suites + native audit
  PASS, guard27/27. SHA256 candidate
  100c0c86c74cbe297b1f87963d13dce0fd6e0046c5b5429dbdd9248466dea502.
- Không deploy/restart. Live R12.18 giữ nguyên hash. Yêu cầu bay vồng/đến đích/hãm/
  hạ cánh còn chưa hoàn tất; ghi rõ blocker/acceptance tại HITFLY-ISOLATION-WIP.md.

## Guided HF MAP trial — staged, chưa thiết bị/deploy

- Native audit thêm consumer tại0x5785068: force theo key được cộng vào baseline
  displacement. Dựa vào đó thêm controller thử nghiệm ascent/transit/brake/descent,
  không RPC657/teleport/state-switch. Chỉ compile với --guided-map-trial và bắt buộc
  --output-dir riêng. Bản thường không compile controller.
- Trial sở hữu force key0x4f4b4846, đóng băng đích/tốc, bấm MAP lần2 hủy. Có nhả key
  khi hủy/lỗi/context transition, fault latch khi cleanup lỗi; ownership HF/MAP.
- 18 chuyến mô hình additive lý tưởng PASS cùng fault/cleanup/collision/time/NaN/
  ownership tests; HF cũ + AimKill/HUD/visual PASS, native audit PASS, guard27/27.
- Candidate301451 bytes SHA256 be444a7b38f1f0584f3addfc6e117686c95631a17642bd8410c256e2d8f6193f.
  Chưa thử native/thiết bị/server. Không xác nhận phanh/đáp đúng. Live vẫn R12.18,
  không restart. Chi tiết/giới hạn tại docs/HITFLY-GUIDED-TRIAL.md.

## R12.19 DEV guided trial — deploy live theo yêu cầu

- User yêu cầu deploy: cài đúng candidate be444a7b38f1f0584f3addfc6e117686c95631a17642bd8410c256e2d8f6193f,
  301451bytes, giữ nhãn DEV. Trước deploy chạy lại18 chuyến mô hình/lifecycle PASS,
  guard27/27 PASS. Backup pre-R12.19-guided-live-20261026T072051Z.
- Service restart active/health200. Payload thực tải qua proxy live khớp cả file
  đã cài và candidate. Rollback và hashes trong release-R12.19-guided-live.json.
- Chưa có thiết bị/native/server flight validation. Deployment thành công không
  chứng minh lên cao/phanh/nhả lực/đáp chính xác hoạt động trên game.

## R12.19 DEV failed device ascent — rollback R12.18

- User báo chỉ nhảy nhẹ/giật/đứng im. Telemetry10:03:26..28+01:00: phase1,
  force0,16.29,0; originY11.44 ->11.76..11.79, sau đó phase6/no-progress,
  owned0/force0/fault0. Không tới transit/brake/descent. Chưa đủ bằng chứng để
  kết luận riêng server kéo lại; mô hình additive không khớp hành vi thiết bị.
- Đã khôi phục backup pre-R12.19-guided-live-20261026T072051Z và restart service.
  Probe đầu sau restart connection-refused; probe lại health200/service active.
  Payload tải proxy khớp R12.18 SHA256
  e8e7882c4b4dc0f3acb8aa03114de7a496d93c48b99e85e844b3b164378ada7d.
- Candidate guided giữ riêng, đánh dấu FAILED, không sửa force/deploy tiếp.
  R12.18 không phải bản đáp đúng điểm; rollback chỉ loại guided gây giật khỏi live.

## Sophia port1300 HitFly map — tham khảo read-only

- Theo URL user, xác minh bootstrap8757bytes tải qua port1300 khớp deployed file.
  Tách đúng runtime API174803bytes (hash39ce90c5...), không nhầm trang status hay
  bootstrap với code feature. Phân tích IFix body20 instructions6593..7142.
- Sophia map dùng657 một lần, Y150; g fallback50 hoặc abs(FallingGravity)5..160;
  T clamp((150+sqrt(max(0,22500-2*g*dy)))/g,2,15), XZ=D/T*clamp(2.1+D/1000,2.15,2.55),
  packet time clampT1.8..6, hướng ngang dấu; marker fallback native controller.m_pos.
- Không thấy lệnh dừng/feedback đích trong block; không force-key. Đây là reference
  khác đáng kể nhưng chưa chứng minh đáp chính xác trên thiết bị.
- Audit script sophia_hitfly_reference.py PASS, report JSON +
  SOPHIA-1300-HITFLY-REFERENCE.md. Không sửa/deploy/restart1300 hoặc1297.

## R12.20 Sophia-derived MAP trial — staged, chưa deploy

- Port công thức Sophia body20 sang candidate riêng: gói657 một lần, Y150,
  gravity50/FallingGravity hợp lệ, T1.8..6, XZ distance/time*hệ số2.15..2.55,
  scale MAP riêng0.5..1.5. Marker real-position + controller fallback.
- Không force-key, không teleport, không transform correction, không giả nhận
  stop tại đích. Chỉ observation độ cao/nearest/airborne/landing để log.
- Candidate300891bytes SHA2562ece48b64ef814db6383da4e4a11d457296ea32a5791165cc1e8bcee0d2c2b78.
  Test C#240 cases + HF packet/observation PASS, AimKill/HUD/visual PASS,
  guard27/27, reference audit PASS. Chưa test máy/server; live vẫn R12.18.

## R12.20 Sophia-derived MAP — deploy theo yêu cầu

- User yêu cầu deploy candidate Sophia-derived. Backup
  pre-R12.20-sophia-live-20261026T095035Z; candidate300891bytes SHA256
  2ece48b64ef814db6383da4e4a11d457296ea32a5791165cc1e8bcee0d2c2b78.
- Service restart active/health200; payload tải lại qua proxy khớp candidate.
- Bản live dùng gói657/formula Sophia, không force-key; observation read-only.
  Chưa có device/server acceptance, nên chưa gọi là đáp chính xác.

## R12.20 MAP x1.1 + ESP cache fix — deploy

- Cố định Sophia MAP scale x1.1, bỏ slider MAP tốc; Hover không còn lấy scale
  lưu cũ. ESP dọn stale cache khi sample hết hạn/slot trống, bỏ render
  interpolation theo slot xoay ID, LiveCount chỉ record project thành công.
- Backup pre-R12.20-esp-cache-fix-20261026T104313Z; candidate300829bytes SHA256
  16754c03de6e08f40c4d7c7b549815dd401c83ce9dc1e890d4d57ffc8736b910.
- Sophia240 cases, visual/AimKill/HUD PASS, guard27/27; service active health200,
  payload proxy khớp installed/candidate. Device vẫn cần kiểm tra.

## R12.33 — SkillDrive auto hit liên tục tới kill feed

- Bổ sung đúng vào SkillDrive: giữ 1 mục tiêu và tông lặp mỗi 80 ms (cùng nhịp
  AimKill) tới khi HP<=0/IsDieing mới ghi kill rồi chuyển địch gần nhất kế tiếp.
- Sửa lỗi bản trước: damage chỉ chạy khi teleport thành công, nên khi xe đã nằm
  trên mục tiêu thì hit chain dừng. Giờ teleport là best-effort, damage vẫn gửi
  mỗi nhịp; nếu from==to thì lùi from 1 m để có hướng hit hợp lệ.
- Thêm toggle "Teleport xe tới địch" (0.6 s/lần) và "SkillDrive: auto hit tới kill"
  (0.08 s/lần, HUD hiện tông/trúng/kill). Ba chế độ xe loại trừ nhau.
- Build 303281 bytes SHA256 08d5f85d5142f304ce15e1a7bd422b1a78a7cface3db2c256f2fad5f12a87941,
  guard 27/27, 4 suite PASS. Live /opt/okmod-1297 vẫn là bundle InTexX, chưa ghi đè.

## R12.34 — gộp 1146 AUTH DRIVEKILL + ram damage theo chuyển động

- Gộp toàn bộ vào một toggle "1146 AUTH DRIVEKILL (ALT)"; bỏ 2 hàng Teleport xe
  và SkillDrive riêng. Menu ESP trở lại max scroll 2232.
- Damage chuyển sang đường va chạm: mỗi 80 ms đưa xe ra sau mục tiêu 7 m rồi
  quét qua theo bước 3.5 m, gửi PushSyncState kèm velocity 60 m/s, gọi
  Vehicle.SetVelocity và Vehicle.CollisionEnter(collider, true, velocity).
  Không còn gọi TakeDamage trực tiếp (đã bị server trả r=0).
- Giữ mục tiêu tới HP<=0/IsDieing rồi mới chuyển địch; "trúng" chỉ tăng khi HP
  thực sự tụt, HUD hiện Change/Push/trúng/kill + HP before->after.
- Khi tắt toggle: ChangeState(false) + SetVisibility(false) để nhả ghế, người
  chơi ra khỏi xe bình thường.
- Build 301723 bytes SHA256 1d98755d674adecf922fb2f45dd64a93e966ff62a5b5d614c7becb022efdf53c,
  guard 27/27, 4 suite PASS. Đã cập nhật proxy 1146 (backup trước khi thay).

## R12.35 — 1146 teleport thẳng vào địch + sửa crash CollisionEnter

- Bỏ kiểu chạy đà 7 m / quét 3.5 m. Giờ mỗi 80 ms teleport xe thẳng vào vị trí
  địch, set velocity 80 m/s theo hướng xe→địch, gửi PushSyncState kèm velocity
  và SyncStateWithServer(pos, rot, 0) để server thấy xe đang chạy qua địch.
- Bỏ lời gọi Vehicle.CollisionEnter vì gây IndexOutOfRangeException; thay bằng
  lấy component VehicleCollision (GetComponentInChildren) và gọi chính
  OnTriggerEnter của engine với collider địch.
- Vẫn giữ mục tiêu tới HP<=0 rồi chuyển địch; "trúng" chỉ tăng khi HP tụt.
- OFF: ChangeState(false) + SetVisibility(false) để nhả ghế, ra khỏi xe được.
- Build 301720 bytes SHA256 1d6fc09655391b52cffa684aef9ea4f96f827a87bc3014543f1e110e5ac74e4e,
  guard 27/27, 4 suite PASS, đã cập nhật proxy 1146.

## R12.36 — xoá SkillDrive, trả 1146 về bản gốc

- Theo yêu cầu: xoá toàn bộ SkillDrive/auto-drive/log vhit. `UpdateVehicleAuth`
  trở lại đúng bản 1146 AUTH DRIVEKILL gốc (ChangeState + PushSyncState +
  DispatchVehicleHit 0.5s), bỏ Rigidbody/DriveVehiclePhysics/UpdateVehicleLog
  và các field R7VehHits/Kills/RamDir/VLog*.
- Build lại khớp byte-for-byte bản trước: 301814 bytes SHA256
  eb49b45798d36b5d106cd2a00ae50aa4d341393263e02f202ed6ec325e2ec415.
  Guard 27/27, 4 suite PASS. Đã cập nhật proxy 1146.
