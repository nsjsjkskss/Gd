# Sophia-derived port1300 HitFly map reference — read-only review

## Artifact identity

User provided `http://proxy.okmod.sbs:1300`. Direct `/` is a proxy status page,
not the game patch. The running process uses `/opt/okmod-1300/okmod_addon.py`.
Its two-stage delivery serves a CDN bootstrap, then a separate runtime via the
existing API. Do not confuse the8757-byte bootstrap with feature code.

- Actual CDN bootstrap downloaded through port1300: SHA256
  `b4a90537c95c18e6f97b9ccb9fd857861ae363d09441bbe3ffe8219246673f55`,
  matches `/opt/okmod-1300/var/patch.bin`.
- Runtime inspected: `/opt/okmod-1300/var/runtime.bundle`,174803bytes, SHA256
  `39ce90c5293a45a4529580da26dd98e816f01efc80f9ba6d59cd37c186826ff4`.
  Default backend artifact path points here; a fresh authenticated runtime
  retrieval was not performed for this read-only review.
- Runtime source provenance/hashes are pinned by `/opt/okmod-1300/build_patch.py`.
  The runtime is Sophia-derived with domain/watermark substitutions.
- Features are in IFix OnGUI body20,16982 instructions across21 bodies. The map
  HitFly block is body20 instructions6593..7142, not C# source in source/.

Reproduce the pinned instruction comparison:

```sh
python3 buildkit/audit/sophia_hitfly_reference.py --output docs/sophia-1300-hitfly-reference.json
```

The output includes instruction annotations and anchors, not only string hits.

## Recovered behavior

1. A HUD release sets `ffxc_hitFlyArmed=false`, `ffxc_hitFlyTriggered=true`.
   The next feature execution consumes trigger and arms it, avoiding per-frame
   sends. No persistent destination-following state was found in this block.
2. Find `UIMapContentController` including inactive objects. Prefer reflected
   `m_HasLocalMapMarkRealPos` and `m_LocalMapMarkRealPos`; fallback to
   `m_LocalMapMarkController.m_pos` when distance from player>=0.75.
3. With a valid non-near marker, compute `delta=marker-player.transform.position`,
   horizontal distanceD and vertical differencedy. No terrain-height raycast.
4. Set upward parameter **150**, default gravityg=**50**. Try reflected
   `Player.get_FallingGravity`; accept abs(result) in[5,160], else retain50.
5. Compute:

```text
T = clamp((150 + sqrt(max(0, 150*150 - 2*g*dy))) / g, 2, 15)
K = clamp(2.1 + D/1000, 2.15, 2.55)
speedXZ = D/T * K
packetTime = clamp(T, 1.8, 6)
direction = normalize(delta.x/D, 0, delta.z/D)
```

   When horizontalD<=0.001, direction uses player forward. Non-near validation
   before this branch is full3D magnitude>=0.75.
6. Reflection creates `message.NNDOIBJBAJI`, unwraps local PlayerID with the
   matching `BEADLMGGGGL.DEIPJDIGKIP` overload and packs direction using
   `message.LMCBHLCPAJD.MGOGJCMCIPG`.
7. Assign ID, time, XZ, Y and packed direction fields, then
   **`Send(657,msg,2,false)`**, discard the returned bool. The inspected branch
   does not explicitly set `JBEHMJFBEMG` or `IMAOCDIIIHI`.
8. Without a usable marker, fallback is normal forward HITFLY: time1/XZ25/Y20,
   preferring runtime camera transform, then Camera.main, then local forward.

## Differences from OKMOD attempts

| Part | Sophia reference | OKMOD R12.18 | Failed R12.19 DEV |
|---|---|---|---|
| Movement route | one-shot657 | one-shot657 | per-frame keyed force |
| Upward scalar |150 |50 |feedback desired velocity |
| Horizontal scalar |distance/time * empirical2.15..2.55 factor |selected5..100 |feedback correction |
| Packet time |estimatedT clamped1.8..6 |fixed1 |no657 |
| Destination height |markerY |ignored |raycast surface |
| Explicit arrival brake |not in inspected block |none |ideal-model brake, device ascent failed |

The empirical multiplier and distinct packetTime clamp mean this is **not**
the simple ideal-ballistic equation used in R12.16. For exampleD300/dy0/g50 gives
T6, K2.4, XZ120 and packetTime6. Withg9.8, T clamps15, givingXZ48 but packetTime6.
Do not cap its XZ at100 or replaceg with default9.8 and call that identical.

## What this establishes / does not establish

The reference is useful and materially different from the failed force-key
controller. It explains a much larger upward launch and distance-adjusted XZ.
It does **not** show an explicit stop when actual position reaches the marker.
There is one657 Send, no post-arrival transform write, no velocity setter or
StopHitfly call within this block. Other unrelated features write transforms
elsewhere in body20; they must not be attributed to map landing without evidence.

Whether the reference lands correctly depends on actual native/server handling
of657, selected map position and runtime gravity. The multiplier looks like
empirical compensation, but its rationale and device effectiveness are not
proved by this inspection. Do not promote a port as exact landing before a
controlled device run. User requested reference inspection only: no changes to
port1300 or1297 live service/payload were made.

## R12.20 staged port

The candidate `build/candidates/hf-sophia-trial/` ports the formula and packet
shape only, with independent scale and read-only observations. It is not the
force-key controller. It was promoted to live by explicit user request at
2026-10-26T09:50:35Z after staged tests; live proxy SHA256 matched. Metadata:
`docs/release-R12.20-sophia-trial.json`. Device movement/server acceptance
remains pending.
