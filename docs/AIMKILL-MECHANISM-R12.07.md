# AimKill: mechanism, latency and limitations (R12.07)

## Evidence and confidence

- Current code: `build/okmod.cs`, `build/build.py`, `build/aimkill-runtime-contract.json`.
- Native binary SHA256: `2de005ae6c9d263ddd705aedda833c29a5b27044d2f56655ecc3cf98dc7be54a`.
- Metadata SHA256: `597e77fc1dd15bac9ff75215bd328d60c6d205f5fc4a7e32cec1b37d3cc2ea5d`.
- References: audit bundle `bs4484-reference/{aimbot,updates}.h` and
  `pc-source/{backend.cpp,class.h}`; hook binding confirmed in extracted BS4484
  `/tmp/opencode/bs4484/src/main/jni/ZygModule/cheat.cpp:130`.
- Native evidence: `aimkill-cadence-R12.07-disassembly.txt` and
  `AIMKILL-REFERENCE-R12.06.md`. Legacy dump offsets/types are not blindly used.
- User reports R12.06 works. Device logs include result=200, immediate/delayed
  HP 200->0. This supports effectiveness in that test but is not a server ACK,
  proof of attribution or a universal result across modes/weapons.

## Why R12.06 was slow

1. Explicit UI intervals were 500/1000/1500/2000ms. Default was 1000ms.
2. `HasAimKillProbe()` stopped new submissions while any 700ms observation was
   pending. The observation itself therefore became a fire throttle.
3. `R7AimKillNext` was advanced before checking that observation. At a selected
   500ms, t+500ms found the probe pending and advanced next to t+1000ms even if
   the probe completed at t+700ms. Frame timing could add further quantization.
4. No-target acquisition also waited the entire selected fire delay. Twelve
   rotating candidates meant a large list could take many seconds to traverse.

These are proven client scheduler limits, not measured server limits.

## Native weapon cadence

Registration confirms `HBIBDMMOOOK.LDKCJEMDAMC()->Single`,
`BDHMJJIADFG()->Single`, `KENANOGLPJO()->Boolean`.

- `LDKCJEMDAMC` at 0x5BF3B0C reads FireIntervalScale and other weapon/skill scales,
  then multiplies them by the virtual interval getter (slot 77).
- `BDHMJJIADFG` at 0x5BF3E58 similarly uses the slot 78 repeat interval getter.
- In the native firing condition 0x5C22650-0x5C226B8, the engine calls
  LDKCJEMDAMC, selects BDHMJJIADFG when KENANOGLPJO is true, and compares the
  interval to elapsed simulation time since the weapon timestamp.
- We use these as a weapon-aware lower bound. This does not export every native
  firing precondition or establish a server-side permitted rate.

## R12.07 scheduling

- User intervals: 120/200/300/500ms, default 200ms. No zero-delay mode.
- Effective interval = max(user interval, native selected interval).
- Invalid/non-positive/non-finite or implausible >=60s native reading falls back
  to 500ms. No weapon stat or native cooldown timestamp is overwritten.
- Candidate polling is independently limited to once per 50ms. When cooling
  down, no target-list raycast scan is run. At most one hit attempt per poll.
- Twelve rotating candidates and a cooperative 2.5ms scan budget are retained.
  A native call itself cannot be interrupted to enforce that budget.
- Next-hit time is reserved before native work; elapsed stalls do not create a
  catch-up burst. Engine failures back off instead of retrying each HUD pass.
- HP observations remain 700ms but no longer block fire. One pending window per
  target preserves its first baseline/deadline; a full 8-slot observation table
  skips telemetry, never overwrites a pending window and never blocks fire.
- Repeated hits within one observation window make probes *window observations*,
  not per-shot successes/failures. Never use probeOk/dispatched as accuracy or
  a server acceptance percentage.

## Client damage, server damage and timeouts

- A popup or positive LHNJLFOINDO return is a local engine result, not receipt of
  server confirmation. Immediate HP can be predicted locally. Delayed HP can
  include authoritative updates, other players, healing or target respawns.
- R12.03-05 explicit TakeDamage returned zero and mostly unchanged HP. It is not
  sound to infer all zero returns are server rejection or identify a cause from
  return value alone. R12.06 changed to the reference engine-hit entry.
- Engine LHNJLFOINDO builds dynamic data/check parameters and has mode-dependent
  hit handling. R12.07 never also submits explicit TakeDamage for the same hit.
- PC V2 also calls SetStartDamage then explicit TakeDamage; BS4484 uses its hook
  original plus start/stop/aim/swap. Those native modifications are not a contract
  that an iOS IFix port can safely copy byte-for-byte or at unlimited frequency.
- Faster calls can increase duplicate work, allocations, native state conflicts
  and network traffic without increasing accepted hits. FPS hitches, packet loss,
  disconnect policy and server rejection need separate measurements. There is no
  proven server packet/rate threshold here and no claim this prevents timeouts.

## Cover, manual input and target lifetime

- Every selected target is rechecked for alive/team/knock/range/FOV/LOS. Cover
  invalidates the lock. Only confirmed local-hierarchy ray hits may be stepped
  past; unknown close cover is not ignored. Pass exhaustion fails closed.
- Final LOS/collider validation immediately precedes constructing the isolated
  hit. No enemy teleport, wall bypass or position manipulation is added.
- Manual fire and reload preempt synthetic hits. No StartFiring/StopFire,
  StartWholeBodyFiring, forced refill, auto swap or shared-hit writes are added by
  this path. Separately enabled combat/movement options can still affect play.
- Recovery takes precedence; aim toggle and recovery remain separate.

## Diagnostics and validation

`cadenceMs user/native/effective/lastGap` distinguishes UI limits, weapon limits
and actual wall-clock spacing. `workMs call/scan` exposes native call versus scan
cost. `probeDropped` identifies telemetry capacity, not missed damage. Existing
blocked/manualSkips/localReason/lastDispatchAt fields remain.

Tests cover native slow/repeat/invalid intervals, pending probe without fire
blocking, deduped probe deadlines, no catch-up, manual input, cover, recovery,
isolated hit data, no duplicate explicit damage and HTTPS telemetry. Stub tests
do not execute the real IL2CPP reflection bridge, physics world or game server.

Recommended device test: one visible target and one covered target, one weapon,
RapidFire/speed/jump off to isolate cadence, first 200ms then 120ms. Compare
effective and lastGap, immediate and delayed HP, call/scan cost and the game's
exact disconnect message. Retain the slowest setting that behaves consistently;
do not treat a higher call count or damage popup as a successful network hit.
