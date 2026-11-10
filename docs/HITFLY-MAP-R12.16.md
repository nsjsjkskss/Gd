# R12.16 — HF MAP predicted lofted trajectory

> Historical experiment, superseded by R12.18. The active HF MAP no longer uses
> this solver, surface lookup, observation lock or movement-mode preflight.
> See `release-R12.18.json` for the simplified marker-bearing/upward-impulse path.

## Behavior

- Place a native map marker, then press **HF MAP**. Map opening/closing remains native.
- New independent preferences: `OKMOD.Move.HFMapSpeed.Float` (5–100 m/s, default 25,
  step 5) and `OKMOD.Move.HFMapHeight.Float` (10–200 m, default 30, step 10).
  They are exposed at AIM rows 35–36. Existing camera-HITFLY time/XZ/Y settings
  retain their original behavior. AimKill retains its R12.15 single-toggle preset.
- MAP speed is a horizontal **ceiling**, not a guarantee of that exact speed.
  A short trip is slowed to make room for the requested loft. A long/slow trip
  may have a higher apex than the minimum loft setting.
- Resolve the first loaded non-trigger collider at marker X/Z with a downward
  ray from `origin.y + 1024`, length 2048, all layers, triggers ignored.
  Marker Y is deliberately not used as terrain height. The target uses surface Y
  plus 0.2 m. This picks a roof/upper surface if it is the first solid hit; it
  does not select the interior/bottom floor of a building or prove walkability.
- Missing/unloaded surface, invalid endpoint, or an out-of-bounds solution means
  **no gameplay packet**, rather than a fallback to camera direction.
- MAP requires a grounded local player, recovery OFF, high-jump/hover OFF,
  no snow-slide dashing and no pending modified-global-gravity restoration.
  Existing settings are never silently changed to make these conditions true.

## Explicit model and limits

Let `D` be horizontal distance, `dy` target height minus launch height,
`g = -VanillaGravity()`, `s` the selected speed ceiling and `h` the minimum loft:

```text
A = max(0, dy) + h
T_loft = sqrt(2*A/g) + sqrt(2*(A-dy)/g)
T = max(D/s, T_loft)
VX_Z = D/T
VY = dy/T + g*T/2
```

The formula produces a descending endpoint at the desired X/Z/Y under a
constant-gravity, constant-horizontal-velocity point model. It rejects invalid
numbers, D outside (1, 2000] m, g outside [1, 30], T outside [0.2, 20] s,
or velocities outside (0, 100] m/s. These are **application limits**, not proven
server limits. Out-of-range outputs are not clamped into a different endpoint.

One existing `Send(657, NNDOIBJBAJI, 2, false)` request carries T, VX_Z and VY.
The existing direction packer and local player ID route are retained. Packet
flag `IMAOCDIIIHI=0` is preserved; no new message type, per-frame impulse,
transform write, teleport, forced landing or gravity hook is introduced.

**The client's actual HitFly receiver has not been verified to implement this
model. In particular the time field may be a state lifetime rather than total
airtime.** Native integration, network correction, packet quantization, initial
velocity, collisions, walkability and server restrictions can all cause a miss.
The model does not inspect the whole arc for obstacles. This release is a
device-test candidate, not a proven exact-landing implementation.

## Observation, lifecycle and reporting

- After Send=true, observe the captured local player/match at most 10 Hz. Do not
  re-read a moved marker as the destination during flight.
- State 1: sent/awaiting takeoff. State 2: an airborne sample was observed.
  State 3: two consecutive grounded samples after an airborne sample; report
  horizontal miss and last position. `landed-near` additionally requires miss
  <=3 m and vertical error <=2 m, and is labeled a **client observation**.
- State 4: Send=false or no takeoff observed after 2 s. State 5: no landing by
  predicted time + 5 s. State 6: local/match/death/recovery/position cancellation.
  State 7: exception. Observing an unrelated fall can contaminate attribution;
  these states do not prove the HitFly request caused movement.
- Reject both HITFLY and MAP requeue while a flight is being observed. Existing
  T+0.25 s cooldown remains. Recovery consumes pending requests so they cannot
  unexpectedly launch after recovery is turned off.
- Logs include `hfMap status/state/reason`, predicted T/XZ/VY/gravity, original
  target, origin, last sampled position, sampled apex and `missXZ`. They sit near
  the front of LOG/COPY/GỬI and the bounded online payload, before truncation.
  Start/end snapshots use the existing rate-limited Nginx telemetry path,
  including when AimKill is OFF. Sending telemetry is separate from game ACK.
- Build exports `hitfly-map-observer-vm.json` and `hitfly-map-solver-vm.json`,
  rejecting movement setters, gameplay Send and damage/fire calls in these
  graphs. The observation path reads movement only.

## Verification

Recording tests exercise the actual C# helpers against stubs:

- 234 reachable ballistic-model combinations: endpoint equality, descent,
  minimum loft, speed ceiling and bounded values. Unreachable and NaN/invalid
  cases are rejected.
- Packet fields, native marker requirement, surface lookup failures, triggers,
  custom speed, camera-independent MAP and unchanged camera-HITFLY settings.
- Queue/cooldown/recovery, false Send, exceptions, context replacement/death,
  takeoff timeout, airborne timeout and stable-grounded landing observation.
- UI plus controls change only the corresponding preferences and clamp bounds.
- Existing AimKill, HUD lifecycle and visuals suites; core guard 27 checks.

These checks do not validate real IFix dispatch, loaded terrain or native/server
movement. Reopen the game for R12.16, disable recovery/high-jump/hover, place a
nearby outdoor marker and test once. Check `hfMap` logs before treating a Send
success as movement success. No device result was available at release time.
