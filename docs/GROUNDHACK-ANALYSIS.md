# GroundHack — deep dissection (dump / IFix / UnityFramework)

Version under review: R11.65 (`build/candidate.patch`, 85 fixes).
Binary: `/tmp/opencode/ff-unity/UnityFramework` (sha256 `2de005ae6c9d263d…`),
metadata dump `/tmp/opencode/ff-dump-v31d/dump.cs`,
reporting dump `/root/ffdump/com.dts.freefiremax-dump.cs`.

Everything below is from disassembly + metadata, not from guesswork.

---

## 1. What the engine actually uses as "grounded"

`UnityEngine.CharacterController.get_isGrounded` — RVA `0x8DF4F10`,
`UnityEngine.PhysicsModule.dll`.

Callers found by scanning all `BL` targets (31 sites):

| Owner | Sites |
|---|---|
| `Player` | 24 (`JLPJMAFLADI`, `HADKJKALGOL`, `FGKGMCGKJDL`, `NMIALGDPJGN`, `FANJOEGAAFC`, `ResetHighFrameFlags`, `SwitchPhysXState`, …) |
| `ECDBGGPCDIA` | 5 |
| `LevelScout`, `KMIDHMEAIDH` | 1 each |

`Player.IsGrounded()` — RVA `0x56870BC` — has **4 callers only**:

```
0x51E3848  SceneGraphics.UpdateDesertEffect
0x51E70F4  SceneGraphics.UpdateTerrainLod
0x51E7844  SceneGraphics.UpdateSnowMode
0x5209FC4  ParticleEffectInRoomController.Update
```

`Player.IsGroundedByRayCastHit()` — RVA `0x5687124` — has **0 callers**.

### Consequence
Movement never calls `Player.IsGrounded()`. Therefore the R11.45 hook
(`Player.IsGrounded → GroundHackIsGrounded`) can never affect movement.
It only changed desert/snow/LOD/particle visuals.

---

## 2. The movement path that matters

Entry chain for the local player:

```
PlayerNetwork.UpdateKinematics (0x5A56694)
  └─ Player.UpdateKinematics (0x568A2F8)      // virtual slot 359 table @ 0x568A6B4
       ├─ PhysXState == 0  → JLPJMAFLADI (0x5784154)   (grounded / standing logic)
       ├─ PhysXState == 1  → HADKJKALGOL (0x568BBAC)   (falling / airborne)
       └─ PhysXState == 13 → falls through, no movement
```

`JLPJMAFLADI` reads grounded state three times:

```
0x5784458  bl get_isGrounded   → w20/w23, decides horizontal control
0x5785E5C  bl get_isGrounded   → step-down after CC.Move
0x5785E94  bl get_isGrounded   → extra downward probe
0x578612C  bl get_isGrounded   → decides SwitchPhysXState(1) i.e. enter falling
```

At `0x578612C` the fall decision is:

```c
if (!stepDownHandled
 && !Player.InSnowSlideWayDashing      // Player+0x2080
 && !Player.CNOPBPNFJFN                // Player+0x1648 (climb state)
 && !Player.IsDieing())
     this.SwitchPhysXState(1, 0);      // vtable +0x19E8
```

`Player.SwitchPhysXState` entry (`0x57D11AC`) has **no `WrappersManagerImpl.IsPatched`
call**, so it cannot be hooked by this IFix build either.

---

## 3. Why each previous attempt failed

### R11.45 — hook `Player.IsGrounded`
Dead on arrival: zero movement callers (see §1). Only 4 rendering callers.

### R11.64 — write `Player.m_GroundHitResult = true` every frame
`m_GroundHitResult` is `Player+0x1DC4`. Inside `JLPJMAFLADI` it is addressed as
`x22 = this + 0x1249`, field `[x22 + 0xB7B]`.

It is written by the downhill raycast and read **8 instructions later**, in the
same frame:

```
0x57856C4  bl  Player.FOLJNNHADNN(...)     ; raycast, out hit
0x57856C8  strb w0, [x22, #0xB7B]          ; m_GroundHitResult = hit
0x5785700  ldrb w8, [x22, #0xB7B]          ; read back
0x5785708  b.ne #0x57857C0                 ; only used for slope-angle branch
```

It is **not** the gameplay ground state. Forcing it from `OnGUI` is overwritten
by the next raycast and never influences `isGrounded`. Pure no-op.

### R11.65 — replace `Player.OnFixedUpdate`
Two defects in 24 lines of `build/build.py`:

**Bug A — wrong `Call` argument count.**
`build.py:544`:

```python
[p.op_code('Call'), ids['GroundHackAfterFixedUpdate']]
```

The `Call` operand is `(nargs << 16) | body_id`
(see `buildkit/tools/cs2patch.py:400-401` and `:476-477`).
`GroundHackAfterFixedUpdate(Player self)` needs `nargs = 1`, so the operand must be
`(1 << 16) | id`. Emitted with nargs `0`, the callee's `Ldarg 0` reads the caller
frame's uninitialised slot — `self` is garbage/null and the arg stack is off by one.

**Bug B — the base proxy does not call the override.**
`Player.<>iFixBaseProxy_OnFixedUpdate @ 0x57F7438` is:

```
0x57F7438  mov x1, #0
0x57F743C  b   #0x6B4B274      ; Entity::OnFixedUpdate
```

and `Entity.OnFixedUpdate @ 0x6B4B274` is literally `ret`.

So the hook body
(`call proxy → call GroundHackAfterFixedUpdate → ret`) erased
`Player.OnFixedUpdate @ 0x5686DE4` completely, including:

- `m_IsFixedUpdated` (`Player+0xAA5`) is never set to `true`
  (only writers: `0x5686EB8`, `0x5686EF8` — both inside the erased body).
- The dead-body physics branch (`DEHIMGOCHDO` / `ECPHNMGNOBD`) never runs.

`m_IsFixedUpdated` is read by:

```
0x5638650  Player.get_IsFixedUpdated
0x569B524  Player.PCDNBOBFJBG   → if false, pitch write is DISCARDED
0x569B530/534                    (aim/pitch freeze)
0x57437E4  Player.PMCABFAKJHI
```

→ **this is the "aim đơ / pitch đứng yên" regression**, not a GroundHack win.

Note: the general rule this exposes — for an IFix *virtual override*, the
`<>iFixBaseProxy_X` stub calls the **base class** body, not the override.
Hooking any `Player` override (`OnFixedUpdate`, `OnUpdate`, `UpdateKinematics`,
`SwitchPhysXState`, …) destroys the override's logic. Only leaf methods are safe.

---

## 4. What is actually reachable from this IFix layer

`get_CustomGravity` (`0x5633CA4`), `get_FallingGravity` (`0x570ACCC`),
`get_RisingGravity` (`0x570AB30`), `get_MaxJumpHeight` (`0x570AE68`) all start with
`WrappersManagerImpl.IsPatched(...)` → **patchable**.

None of them has a `<>iFixBaseProxy_*` (they are non-virtual), so the patch body must
reproduce the original when the toggle is OFF.

Disassembly of the gravity getters:

```
get_CustomGravity:
    if (GameFacade.IsUGCCommonMode() && PlayerCmpt != null)
        return PlayerCmpt[0x218];
    return COW.GamePlay.IPBGFJNGLOH.AMMOCNCMBBG;   // -9.8f (written in .cctor 0x63A6684)

get_RisingGravity:
    if (Player.InSnowSlideWayDashing) return Player.SnowSlideWayRisingGravity;  // +0x2088
    ... UGC match override (match[0x18], negated) ...
    return get_CustomGravity();

get_FallingGravity:
    if (Player.InSnowSlideWayDashing) return Player.SnowSlideWayFallingGravity; // +0x208C
    ... UGC match override (match[0x1C], negated) ...
    return get_CustomGravity();
```

`HADKJKALGOL` consumes them as:

```
gravity = (vy >= 0) ? get_RisingGravity() : get_FallingGravity();
vy     += gravity * dt;          // 0x568D02C
```

So in **normal** matches all three collapse to `-9.8f`. Hooking Rising/Falling and
delegating the OFF path to the *unpatched* `get_CustomGravity()` is therefore
bit-faithful for normal play, and leaves UGC's `CustomGravity` untouched.

### What cannot be done
`CharacterController.get_isGrounded` lives in `UnityEngine.PhysicsModule`.
IFix only rewrites `Assembly-CSharp` bodies. There is no trampoline to the original
of a patched non-virtual method. **True "walk on air" (BS4484 parity) is
native-dylib-only** — BS4484 does it with a Dobby hook on
`UnityEngine.CharacterController::get_isGrounded`.

---

## 5. Fix applied in this tree

1. **Removed** the R11.65 `Player.OnFixedUpdate` post-hook
   → restores `m_IsFixedUpdated`, un-freezes aim, restores dead-body movement.
2. **Removed** the R11.45 `Player.IsGrounded` hook (dead code, zero movement effect).
3. **Added** two leaf hooks: `Player.get_RisingGravity`, `Player.get_FallingGravity`.
   - `get_RisingGravity`: never overridden — it also feeds the jump launch speed
     (`Player.ELGCCFOEIID @ 0x57746BC`: `v0 = sqrt(max(h * -2.0f * g, 0))`), so
     lowering it turns the jump into a hop.
   - `get_FallingGravity`: toggle ON → `-0.25f` (slow float down), OFF → original.
   - OFF path: `InSnowSlideWayDashing ? player.SnowSlideWay*Gravity : self.get_CustomGravity()`
   - `get_CustomGravity` itself is **not** patched, so UGC mode is untouched.
4. Menu row renamed `Ground hack (native)` → `Bay / lơ lửng (IFix)`,
   same PlayerPrefs key `OKMOD.Move.GroundHack` so existing configs keep working.

Fix count: `81 base + 3 (`Silent`, `ScatterRate`, `ShowDamageNum`) + 2 gravity = 86`.
Guard limit is `base + 7 = 88`.

## 6. Residual risk (device test required)

- If `GameVarDef.EnableAccelerationOnFalling` (`0x2CDF`) is false in a mode,
  `HADKJKALGOL` skips the gravity block entirely and the toggle will appear inert —
  same failure mode as before, but now for a documented reason.
- UGC match-data gravity override is not reproduced (delegates to
  `get_CustomGravity`), so UGC rooms keep `CustomGravity`'s value instead of the
  match-tuned one.
