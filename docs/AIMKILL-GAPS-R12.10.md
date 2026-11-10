# R12.09/12.10 vs BS4484 and PC: remaining gaps

## Current device evidence

R12.09 was reported working. An initial report showed starts=stops=21,
fault=0, twelve HP-decrease windows and zero unchanged windows. Later online
reports show starts=stops=103, fault=0, HP-decrease windows=18 and unchanged/
increased windows=32. These are cumulative, not an accuracy percentage. They
mix weapons, targets, manual firing and HP recovery; they do not identify a
specific failed RPC or a server rejection reason.

## Reference differences

| Topic | BS4484 | PC | OKMOD R12.10 |
|---|---|---|---|
| Entry | original LHNJLFOINDO hook | LHNJLFOINDO alias in send path; V2 also explicit TakeDamage | one LHNJLFOINDO engine call |
| Fire lifetime | aim/start/hit/stop | V2 start+whole-body/start-damage/explicit damage/stop; send starts without same-function stop | scoped aim/start/hit/stop/finally |
| Inventory sync | sends message 108 after hit | V2 syncs inventory, send variant differs | no added inventory-sync RPC |
| Hit storage | allocates isolated object | reuses local hit object | isolated managed object |
| Point/normal | bone position; normal assigned position | bone position; normal assigned direction | collider center; normal assigned direction |
| Damage input | hardcoded 200 | V2 weapon base damage; send path hardcoded 999 | hardcoded 200, separately logs weapon base |
| Aim after hit | does not explicitly restore in helper | variant-dependent | conditionally restores original aim |
| Timing | invoked via hooked LateUpdate, multi-enemy loop | chrono/time and variant-dependent loops | HUD repaint, once/frame, bounded scan/cadence |
| Cover | optional visibility | optional pull/restore for hidden targets | mandatory LOS; no pull/teleport |
| Binding | native hook original/trampoline | native method pointers | IFix extern resolution |

None of these sources proves which difference is responsible for an individual
ineffective hit. Copying them all at once would hide causality and risk repeating
the prior trigger-lock/disconnect symptoms.

## Verified weaknesses fixed in R12.10

1. `ObserveManualHit` formerly ran during mod-owned StartFiring too. That could
   incorrectly label a synthetic hit-info request as manual input. It now skips
   while R7AimKillFiring is owned.
2. HP windows crossing an observed manual shot were counted with clean windows.
   A per-slot manual-sample baseline now sends those observations to probeMixed.
   This is conservative, partial detection: firing outside the sampled getter,
   other players and healing remain unobservable causes of HP changes.
3. After native StartFiring, code formerly submitted even if native callbacks
   swapped the weapon or invalidated the target. Revalidation now aborts and
   finally stops the captured original weapon. It also rechecks LOS.
4. Added mode 4 = 80ms without remapping old modes 0..3. Effective interval still
   equals max(user,native); 80ms is not a promise of 80ms on a 130ms weapon.

## Important unresolved questions

- Is same-frame start/hit/stop correctly synchronized for every weapon type?
  Balanced method-call counters establish caller cleanup, not complete native
  firing-state completion or packet delivery.
- Native PlayerNetwork.StartFiring at 0x5A5B868 builds/sends message 0x68; StopFire
  at 0x5A5C464 builds its own state message. It remains necessary to verify actual
  runtime bridge dispatch and native state/shot sequence for ineffective hits.
- Reference inventory message 108 may matter to weapon state, but no trace proves
  it is missing on a failed device shot. It is not added blindly as a per-hit RPC.
- The hardcoded 200 affects local hit processing/popups. It is not authoritative
  damage. A getter sample with damage=0/layer=8/group=0 is pre-processing data,
  not sufficient evidence to overwrite final hit group or damage input.
- There is no ammo-before/after/shot-sequence/server-ACK telemetry. Empty clip,
  warmup/charge, armor/temporary HP, mode-specific behavior and authoritative
  rejection are not distinguishable from current HP-only observations.
- Collider-center geometry vs bone position, original-angle restoration and
  OnGUI vs LateUpdate need isolated tests, not a simultaneous rewrite.
- Native method GetId/MJCENIGKEPH is not the instance unique ID: reference sync
  message uses IEFJGCGOMID.ODFDGMOCJLL. Never substitute data ID for instance ID.

## Recommended next validation

Retain R12.09 backup. Test R12.10 at 200ms with one weapon, RapidFire/NoReload/
speed/jump off; then 80ms with native floor enabled. Compare observed HP,
probeMixed, aborted shots, cleanup, actual cadence and exact disconnect message.
No claim of guaranteed server acceptance, improved damage or no timeout is made
from recording-stub/guard tests alone.
