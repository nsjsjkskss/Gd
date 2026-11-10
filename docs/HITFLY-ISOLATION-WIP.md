# HF / HF MAP isolation — unfinished, not deployed

> A subsequent staged feedback-controller experiment is documented in
> `HITFLY-GUIDED-TRIAL.md`. It is compiled only with `--guided-map-trial`; no
> device/server validation or production deployment has occurred.

## User requirement

Two independent features. MAP must travel upward on a lofted path, reach the
selected destination, stop horizontal travel and descend/land there. An impulse
in marker direction is not sufficient. Increasing vertical impulse alone is
not an implementation of that requirement.

## Current work

Source now separates pending flags, deadlines, status, attempts, sent counts and
errors for manual HITFLY vs MAP. MAP does not read manual time/XZ/Y preferences.
Tests cover both queue orders, independent cooldowns, error isolation, recovery
cancellation, speed UI and retained original HITFLY values. All four recording
suites pass; core guard 27/27 for the saved candidate.

The earlier snapshot is saved under `build/candidates/R12.19-isolation-only/`.
The current staged candidate is `build/candidates/hf-review/` with **R12.19 DEV**
identification. No new release was published. The service was not restarted.
`build/okmod-R8-AIM-ESP.bin` was restored from the verified
pre-work backup to R12.18 SHA256:

`e8e7882c4b4dc0f3acb8aa03114de7a496d93c48b99e85e844b3b164378ada7d`

Other generated build reports/patches refer to the isolation candidate, not the
restored production artifact. Rebuild/reverify before any future release.

Use `python3 build.py --output-dir candidates/hf-review` from `build/` to keep
all new generated files out of the live artifact path. The new option still
reads source from `build/`. Tests for HF can use
`OKMOD_TEST_BUILD_DIR=/root/okmod-1297-clean/build/candidates/hf-review` to select
the staged generated stubs.

## Outstanding motion problem

Isolation of configuration and queues does not isolate simultaneous physical
impulses applied to the same player. The test that both queued requests reach
Send is only queue behavior, not a concurrency solution for motion ownership.
Do not claim physical independence or exact landing based on these tests.
The current source rejects same-update simultaneous requests for both modes
(status14, explicit conflict) instead of dispatching two impulses. Sequential
requests can still overlap actual motion; a full motion-owner handoff remains
unimplemented pending a verified stop. No hidden claim of complete movement
independence is made.

Read-only native inspection:

- Metadata resolves `Player.SetForceMoveVelocity(Vector3, uint)` and
  `GetForceMoveVelocity()` correctly, but this is not a verified HitFly brake.
- At native `0x5702f18..0x5702fe8`, SetForceMoveVelocity updates a keyed
  dictionary at Player+0x10a0, clears/recomputes a sum at +0x10a8/+0x10ac/+0x10b0.
  The getter at `0x5696644..0x569664c` returns that sum. Setting a single keyed
  force to zero does not demonstrate clearing native HitFly momentum.
- The dump's `StopHitfly()` at `0x6406328` belongs to NetworkAIPawn, not the
  local human Player; its body invokes an animation-component virtual call and
  records time. Other StopHitfly(bool) entries are animation components.
- Player exposes state-switch methods, including server-authorized handling.
  Neither exact transition semantics nor an authoritative endpoint-stop command
  is validated. No speculative state-switch/velocity/teleport call was added.

R12.18 live only sends one opcode657 impulse. It still has no waypoint flight
controller or end-of-travel brake. Exact upward-then-destination descent remains
unfinished. The previous R12.16 ballistic model assumed packet time was airtime
without native proof and must not be reintroduced as a proven solution.

## Deeper native review

Reproduce the static checks with:

```sh
python3 buildkit/audit/hitfly_motion_audit.py --output docs/hitfly-motion-native-audit.json
python3 build/tests/test_hitfly_native_audit.py
```

The script verifies the exact native image SHA256 and selected disassembly
anchors; mismatches fail rather than silently reusing fixed addresses.

- `GetVelocity` reads Player+0x1d48 -> +0x48/+0x50. The human state controller
  method at 0x5830d00 returns HitFly state5 on its native fallback.
- `PlayerNetwork.SyncStateWithServer` at 0x5a53958 decodes packet velocity and
  writes that same vector; state5 additionally updates interpolation state.
  This proves a native incoming update can replace the vector; it does not
  establish all movement is server-only or that no other control exists.
- Base UpdateKinematics' state-dispatch table sends state5 to 0x568a96c,
  skipping the per-state local calculation in that switch. Other update paths
  exist, so do not generalize this to every physics mode.
- Packet657's metadata contains no separate destination-position field. The
  current sender never transmits the map marker as a destination. A scalar
  timer/direction impulse alone does not implement destination feedback.
- The short name GFLBFKKDAMK is duplicated in metadata. The nested Player
  motion object is TypeDefIndex31659 (velocity at0x48), not the unrelated global
  type chosen by a lossy name-only dictionary. The audit selects by type index.

The report records exact contracts, selected raw instruction ranges/hashes and
unproven assumptions in `docs/hitfly-motion-native-audit.json`.

## Remaining blocker / acceptance criteria

No authoritative waypoint-steering or endpoint brake is established. Do not add
speculative state switches, zero-force calls, transform snapping or replayed
server sync packets as a claimed exact-landing fix. A viable next implementation
needs a verified movement actuator and stop/release path first, then a bounded
feedback controller (ascent, transit, brake, descent, landed/cancelled), explicit
motion ownership and native/device evidence. Those are not completed here.

Required device checks once such a controller exists: starts from captured
position/marker, gains height before nearby obstruction, approaches target with
decreasing XZ error, arrests lateral velocity without resetting position,
descends to a loaded surface, releases control after grounded confirmation,
and survives cancellation, death/match changes and manual-HF handoff. Stub
packets, Send=true and a mathematical arc cannot substitute for these checks.
