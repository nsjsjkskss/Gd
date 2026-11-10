# AimKill type-registration resolution

Generated offline with the temporary instrumented Il2CppDumper using:

```text
UnityFramework:       /tmp/opencode/ff-ipa-fresh/UnityFramework
global-metadata.dat: /tmp/opencode/ff-ipa-fresh/global-metadata.dat
Il2Cpp version:       31
runtime:              .NET 7.0.20
```

The instrumented dumper output is a registration-level result, not a guessed
name copied from a legacy dump.

## Resolved current registration

### `HBIBDMMOOOK.CIHHEOKFADJ`

```text
return: int
p0: SingleTeleportDoorAttributeConfig   type index 25633  IL2CPP_TYPE_CLASS
p1: UINB1RESYSGrandPrizeRewardBoxView   type index 44163  IL2CPP_TYPE_CLASS
p2: float                                type index 50346  IL2CPP_TYPE_R4
```

### `HBIBDMMOOOK.LHNJLFOINDO`

```text
return: int
p0: SingleTeleportDoorAttributeConfig   type index 25633  IL2CPP_TYPE_CLASS
```

### `HBIBDMMOOOK.NPLMKLBKJPE`

```text
return: BornPosSetting.DeviceQuailty<float>
p0: UIHudCurvePointItem                 type index 45598  IL2CPP_TYPE_CLASS
p1: UIGachaFeatureAnimSoundChanger       type index 29286  IL2CPP_TYPE_CLASS
p2: bool                                  type index 25157  IL2CPP_TYPE_BOOLEAN
p3: MoveObjectEditAttributeConfig        type index 37478  IL2CPP_TYPE_CLASS
```

### `Player.JGIEOGCGFEP`

```text
return: UIHudCurvePointItem
p0: SingleTeleportDoorAttributeConfig   type index 25638  IL2CPP_TYPE_CLASS
```

### `PlayerNetwork.TakeDamage`

```text
return: int
p0: int                                   type index 39587  IL2CPP_TYPE_I4
p1: UVmove.RenderInfo                     type index 23963  IL2CPP_TYPE_VALUETYPE
p2: MoveObjectEditAttributeConfig         type index 37478  IL2CPP_TYPE_CLASS
p3: int                                   type index 39611  IL2CPP_TYPE_I4
p4: UINB1RESYSCollectionProgressNodeView  type index 71887  IL2CPP_TYPE_VALUETYPE
p5: UINB1RESYSCollectionProgressNodeView  type index 71887  IL2CPP_TYPE_VALUETYPE
p6: BornPosSetting.DeviceQuailty<float>   type index 13962  IL2CPP_TYPE_GENERICINST
p7: GoalAttributeConfig                   type index 46548  IL2CPP_TYPE_CLASS
p8: uint                                  type index 71290  IL2CPP_TYPE_U4
```

## What this proves

- The current registration has an exact 9-parameter `PlayerNetwork.TakeDamage`.
- The current registration has exact type indices for every parameter.
- `CIHHEOKFADJ`'s second parameter is registered as
  `UINB1RESYSGrandPrizeRewardBoxView`, not `MonoBehaviour` or `GameObject`.
- `NPLMKLBKJPE` is fully resolved at registration level.
- The fresh dump's aliases are internally consistent with the current
  registration, even though the legacy dump prints friendlier names.

## Remaining interpretation issue

The current registration prints the two position parameters of
`PlayerNetwork.TakeDamage` as `UINB1RESYSCollectionProgressNodeView` value
types, while the legacy dump prints them as `Vector3`. This is a metadata/type
alias collision in the obfuscated build. The raw registration kind and index
are known; the semantic alias is not safely interchangeable with a C# type
name.

The registration audit therefore resolves the exact current names and indices,
but does not create an IFix caller or payload. Live source and payload remain
unchanged.
