# AimKill — review against PC~AIMKILL~SRC

Reference: `/root/PC~AIMKILL~SRC.7z` (Android, `backend.cpp` / `Hack/class.h`).
Target: 1297 iOS IFix patch (`build/okmod.cs`).

## Correction (important)

An earlier note claimed our patch was "missing the damage step". That was wrong
for this build. Verified against the 1297 metadata:

- `HNLIMBLIANL` (33908) is **not** a DamageInfo struct passed to TakeDamage.
  It is a static damage-calculation helper
  (`BBBH LHICGDE(float, MoveObjectEditAttributeConfig, I_BE_BlockButton, ...)`,
  `CDODNBMICDE(...)`, `FIMHMJFHNCJ(...)`).
- There is **no** `PlayerNetwork.TakeDamage(int, PlayerID, DamageInfo2, int,
  Vector3, Vector3, List, object, uint)` overload anywhere in 1297.
- 1297's damage entry point is
  `AttackableEntity.TakeDamage(CGKJLKPMGDJ hit, HNLIMBLIANL info, float damage, uint)`
  and `HBIBDMMOOOK.LHNJLFOINDO(CGKJLKPMGDJ)` returns the computed damage int.
  The engine's own fire flow consumes both.

So the reference's explicit `Player_TakeDamage(...)` chain belongs to a different
game build and cannot be ported 1:1. Our `StartFiring → LHNJLFOINDO → StopFire`
is the closest mapping for 1297, and the HP probe (RecordAimKillProbe /
UpdateAimKillProbes) already measures whether damage lands.

## What was actually wrong (fixed in R11.86)

`backend.cpp:3097` reuses the player's own hit object:
`hitInfo = *(void**)(local + Player.FDMIEDDNCEC)`.
Our patch allocated `new CGKJLKPMGDJ()` per shot, so the engine could keep reading
its own instance. Now uses `local.FDMIEDDNCEC` with a `new` fallback.
`FDMIEDDNCEC` = Player+0xE50.

## Remaining differences (real, by design)

1. Silent pull: the reference teleports the enemy in small steps until the ray is
   clear (`SilentGhostPullV2` ±4.5 m / `SafeSilentPull` ±1.2 m), then restores.
   Ours needs a clear line of sight and skips hidden targets.
2. FOV / target lock / 360: the reference has `aimbotFOV`, `targetLock`,
   `Aimkill360`. Ours is always nearest-in-any-direction.
3. `SetStartDamage` cadence: reference calls it once per burst (static flag) and
   resets after damage; ours calls it every shot.
4. Ammo: ours refills with `FullWeaponAmmo()` every 0.5 s; the reference does not.
5. Hit part: reference always head (1); ours matches the chosen bone.

## Not ported on purpose

- `Player_TakeDamage` chain (does not exist in this build).
- Enemy teleport pull (high detection risk; the reference itself limits offsets to
  avoid "game back/kick").

## Safe next candidates

- FOV limit + target-lock toggle (client-side only, no teleport).
- Once-per-burst `SetStartDamage`.
