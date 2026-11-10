# R12.06 reference reconciliation

This review uses the supplied BS4484 and PC files, not their method nicknames.
Archived bundle files are historical snapshots; several old reports contain
conclusions invalidated by later registration/native analysis.

## What the references actually call

* `bs4484-reference/aimbot.h:269-323`: `StartAimKillXNEW` allocates an isolated
  hit object and invokes `old_BLAGCMCGEJG1(ist, hitData)`.
* `/tmp/opencode/bs4484/src/main/jni/ZygModule/cheat.cpp:130` establishes that
  this original function hooks `HBIBDMMOOOK.LHNJLFOINDO`, arity 1.
* `pc-source/backend.cpp:641-643`: both `offset_SetStartDamage` and
  `offset_WeaponFire` resolve **LHNJLFOINDO**, arity 1.
* `pc-source/backend.cpp:3150-3228`: the PC V2 variant also starts firing,
  invokes LHNJLFOINDO before preparing explicit damage and finally stops fire.
* `pc-source/backend.cpp:3567-3573`: the send variant starts firing and calls
  `original_WeaponFire`; it is not a CIHHEOKFADJ alias.

The references include firing, aim rotation and/or weapon swapping, whereas
R12.06 intentionally keeps the native trigger untouched after prior device
reports of trigger lock. R12.06 is therefore a reference-informed engine-entry
variant, **not an assertion of exact behavioral parity or proven kills**.

## Current iOS native evidence

Native binary: `/tmp/opencode/ff-ipa-fresh/UnityFramework`. Reproducible disassembly
captured in `/tmp/opencode/weapon-hit-full-R12.05.txt` using Capstone ARM64.

* `LHNJLFOINDO` begins at `0x5C23B54`.
* `0x5C24AB0-0x5C24BB0` fills damage info and allocates dynamic weapon info.
* `0x5C24BB4-0x5C24CD0` fills dynamic data from the actual weapon state.
* `0x5C25194-0x5C251A8` invokes NPLMKLBKJPE with the engine's damage info.
* `0x5C25438-0x5C2545C` dispatches the engine hit processing interface.
* The method also has conditional early exits and mode-specific paths.

This disproves the earlier blanket claim that LHNJLFOINDO only computes a local
number. It does **not** prove which branch a particular device takes or that a
remote server accepts a resulting hit.

## R12.06 implementation

`StartAimKillXNEW -> DispatchEngineHit -> weapon.LHNJLFOINDO(hit)`.

No separate `TakeDamage`, `NPLMKLBKJPE` or JGIEOGCGFEP call is emitted from the HUD
graph. The legacy explicit implementation remains in source for reference but
is not reachable in the built payload. Engine invocation is logged separately
from immediate/delayed HP observations. `checks=0` and `dataId=0` in this mode
mean engine intermediates are unobserved by OKMOD, not that native code used
empty data. `startResult` records the actual engine return.

Native registration validates the exact signature and compile-time stubs. Tests
validate caller control flow, cover filtering, recovery, trigger preservation,
isolated hit data and absence of a duplicate explicit damage call. Device
effectiveness remains unconfirmed until an in-game test.
