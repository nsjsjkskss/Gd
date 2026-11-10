# Guided HF MAP trial — staged only

> **Device result: FAILED ascent; rolled back.** At 2026-10-26T10:03:26..28+01:00,
> the deployed candidate stayed in phase1, applied force0,16.29,0, rose only
> about0.35m from originY11.44, then cancelled with `guide-no-progress`.
> Telemetry reported force0/owned0/fault0 after cleanup. User observed a small
> hop, jitter and no travel. This falsifies the useful-ascent assumption of the
> ideal additive model on this device. The cause is not isolated between native
> physics integration, mode handling, input timing and server correction.
> R12.18 was restored and its served payload hash verified; this trial is no
> longer live. Do not promote it unchanged or tune force upward as a proven fix.
> R12.18 itself still does not implement exact destination landing.

> Deployment update, 2026-10-26T07:20:51Z: the user explicitly requested live
> deployment. The exact candidate below was promoted, the service restarted and
> the live-proxy download hash verified. See `release-R12.19-guided-live.json`.
> Device flight/braking/server validation is still pending. References below to
> "not deployed" describe the original build/test stage, not current live status.

## Outcome and status

A compiled, isolated experimental controller now exists. It is **not deployed**
and has no device/server validation. It replaces the one-shot MAP packet only
when built with `--guided-map-trial`. Normal builds exclude the controller via
`HF_GUIDED_TRIAL`. Manual HITFLY keeps its opcode657 route and own preferences.

The user-visible behavior requested is implemented as a candidate feedback loop,
not established as working game movement: ascend vertically, transit above the
destination, reduce horizontal speed, descend and release the owned force after
grounded confirmation.

## Why this experiment differs from previous attempts

The native audit now verifies an additive force consumer in Player.JLPJMAFLADI:
`0x5785068..0x578508c` reads the keyed-force sum and adds scaled force to scaled
baseline displacement. This provides static evidence for trying a keyed force
adapter instead of guessing packet657 airtime. It does **not** establish that
all live movement modes use this path or that the server accepts it.

The candidate uses exact metadata contracts:

- `Player.GetVelocity() -> Vector3`
- `Player.GetForceMoveVelocity() -> Vector3`
- `Player.SetForceMoveVelocity(Vector3, uint) -> void`

Key `0x4f4b4846` is reserved by convention for this trial. There is no guarantee
that another unknown mod never uses it; this is not a native allocated handle.
Only this key is written/cleared. No transform setter, state switch, replayed
server sync, native patch or gameplay RPC is used by the guided controller.

## Controller

- Freeze launch point, destination surface and selected speed on start. Marker Y
  is ignored; surface comes from a loaded non-trigger downward ray at marker XZ.
  Selects the first solid hit from above (possibly a roof), not a walkable/navmesh
  guarantee. Range is limited to 2..1000m for this trial.
- Require a live grounded walking local player, recovery/high-jump/hover OFF and
  restored gravity. Do not silently change those settings. Avoid native HitFly,
  vehicle and other special physics states.
- **Ascent:** hold launch XZ, rise toward 35m above the higher endpoint.
- **Transit/brake:** hold cruise height, move toward target with selected speed
  ceiling and `min(sqrt(2*12*distance), 2*distance)` approach limits. Enter brake
  inside12m; enter descent only inside0.75m with observed lateral speed<0.75m/s.
- **Descent:** maintain XZ with small corrections, descend at up to6m/s, refresh
  loaded landing surface. Grounded near the target must persist0.2s to finish.
- Desired velocity minus reported baseline minus other keyed forces yields the
  trial's force command, capped at magnitude100. This assumes the audited native
  path acts approximately as an additive velocity; the assumption is explicit.
- Position deltas, not GetVelocity alone, determine braking observations and
  detect corrections. Maximum flight90s, no progress3s, per-tick gap>0.25s,
  clock reversal, invalid values, obstacles, missing surface and context changes
  cancel and clear only the owned key.
- Partial apply errors run cleanup. Cleanup failure latches a fault: no more
  force application or automatic cleanup retries until restart. A zero owned
  key removes the trial contribution; it is **not** claimed to cancel native
  momentum or act as a verified emergency brake.
- Second MAP press cancels. Manual HF is blocked while guided MAP owns motion;
  MAP start is blocked during the manual packet's bounded cooldown interval.
  That interval is not proof that native manual motion has finished; grounded
  walking start validation is also required. Simultaneous queued requests are
  rejected rather than stacked. Preferences/deadlines/counters remain separate.

## Known limitations

- No real IL2CPP/IFix setter or server movement response has been tested.
- A center-line short ray is not capsule clearance or complete obstacle routing.
  The candidate cancels on a detected obstacle, it does not navigate around it.
- Root-vs-feet offsets, roofs, moving surfaces, slope/water and changing terrain
  are not validated. Position corrections can cause cancellation.
- Removing an additive force may leave native momentum. Do not claim verified
  arrival braking, emergency stopping or safe release based on model tests.
- User input, other forces and sync packets may defeat the modeled response.

## Build and test

```sh
cd /root/okmod-1297-clean/build
python3 build.py --guided-map-trial --output-dir candidates/hf-guided-trial
OKMOD_TEST_BUILD_DIR=/root/okmod-1297-clean/build/candidates/hf-guided-trial python3 tests/test_hitfly_guided.py
```

Trial builds require a separate output directory and cannot overwrite the live
artifact path through the normal CLI. Build validates exact force contracts and
exports `hitfly-guided-vm.json`, rejecting RPC/teleport/state-switch/fire/damage
dependencies in the guided graph.

Test results:

- 18 ideal additive-velocity model flights (3 speeds x3 distances x2 frame rates)
  complete ascent/transit/brake/descent with endpoint error<1m.
- Tests include unrelated force preservation, no opcode657 for MAP, frozen
  marker, cancel/death/match/local replacement, time gaps/reversal, no-progress,
  correction/NaN, lost terrain, obstruction, physics exceptions, partial setter
  failure, cleanup-fault latch and manual/MAP ownership.
- Ordinary HF regression plus AimKill/HUD/visual suites PASS.
- Native static audit regression PASS; trial core guard27/27.

These are source/recording/ideal-model tests, not actual flight. Candidate:

`build/candidates/hf-guided-trial/okmod-R8-AIM-ESP.bin`

SHA256 `be444a7b38f1f0584f3addfc6e117686c95631a17642bd8410c256e2d8f6193f`,
301451 bytes. **No service restart/deployment performed.** Live R12.18 artifact
remains SHA256 `e8e7882c4b4dc0f3acb8aa03114de7a496d93c48b99e85e844b3b164378ada7d`.

The first live device run failed before transit/braking/descent. Further work
requires identifying why the additive-force assumption fails in the actual
movement path, using correlated native velocity/force/physics-mode/timing and
position observations. An ideal model pass cannot be used as evidence that the
actuator works on the device. Do not repeat the failed trial as a completed fix.
