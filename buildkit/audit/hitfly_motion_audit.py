#!/usr/bin/env python3
"""Read-only HitFly native evidence audit. Never executes or patches game code."""
import argparse
import hashlib
import json
import struct
from pathlib import Path

from capstone import Cs, CS_ARCH_ARM64, CS_MODE_ARM
from aimkill_runtime_contract import RuntimeContract

ROOT = Path(__file__).resolve().parents[2]
EXPECTED_NATIVE = '2de005ae6c9d263ddd705aedda833c29a5b27044d2f56655ecc3cf98dc7be54a'


def collect(binary):
    runtime = RuntimeContract(binary)
    digest = hashlib.sha256(runtime.native).hexdigest()
    if digest != EXPECTED_NATIVE:
        raise ValueError('Native binary differs from the audited v31 image; address evidence must be re-established')
    disassembler = Cs(CS_ARCH_ARM64, CS_MODE_ARM)

    def instructions(start, end):
        return list(disassembler.disasm_lite(runtime.native[start:end], start))

    def require(start, end, expected):
        code = instructions(start, end)
        actual = {address: (mnemonic, operands) for address, _, mnemonic, operands in code}
        for address, instruction in expected.items():
            if actual.get(address) != instruction:
                raise ValueError(f'Native anchor mismatch at {address:#x}: {actual.get(address)} != {instruction}')
        return {'start': hex(start), 'end': hex(end),
                'sha256': hashlib.sha256(runtime.native[start:end]).hexdigest(),
                'instructions': [f'{address:#x}: {mnemonic} {operands}'
                                 for address, _, mnemonic, operands in code]}

    # The convenience resolver's name->class dictionary loses duplicate nested
    # names. Use TypeDefIndex for nested evidence instead of silently choosing
    # the unrelated global class with the same short name.
    classes = json.loads((ROOT / 'docs/aimkill-audit-bundle/dump/ffmeta_classes.json').read_text())

    def exact_fields(type_index):
        cls = next(c for c in classes if c['tix'] == type_index)
        offsets, = struct.unpack_from('<Q', runtime.native, runtime.field_offsets + type_index * 8)
        fields = []
        for i in range(cls['fc']):
            ni, ti, _ = struct.unpack_from('<III', runtime.metadata, runtime.fields + (cls['fs'] + i) * 12)
            offset, = struct.unpack_from('<i', runtime.native, offsets + i * 4)
            fields.append({'name': runtime.string(ni), 'type': runtime.type_name(ti), 'offset': hex(offset)})
        return {'type_index': type_index, 'short_name': cls['name'], 'fields': fields,
                'same_name_type_indices': [c['tix'] for c in classes if c['name'] == cls['name']]}

    get_velocity = require(0x5659058, 0x5659074, {
        0x5659058: ('ldr', 'x8, [x19, #0x1d48]'),
        0x5659060: ('ldp', 's0, s1, [x8, #0x48]'),
        0x5659064: ('ldr', 's2, [x8, #0x50]'),
    })
    sync_velocity = require(0x5a53958, 0x5a53a30, {
        0x5a5395c: ('ldr', 'x0, [x20, #0x30]'),
        0x5a53964: ('bl', '#0x3715998'),
        0x5a5396c: ('stp', 's0, s1, [x23, #0x48]'),
        0x5a53970: ('str', 's2, [x23, #0x50]'),
        0x5a539a8: ('cmp', 'w22, #5'),
        0x5a539ac: ('b.ne', '#0x5a53ab4'),
    })
    hitfly_state = require(0x5830d4c, 0x5830d5c, {
        0x5830d4c: ('mov', 'w0, #5'),
        0x5830d58: ('ret', ''),
    })
    dispatch = require(0x568a69c, 0x568a6d4, {
        0x568a6a8: ('bl', '#0x5822080'),
        0x568a6c0: ('add', 'x9, x9, #2'),
        0x568a6c4: ('adr', 'x10, #0x568a6d4'),
        0x568a6c8: ('ldrb', 'w11, [x9, x8]'),
        0x568a6cc: ('add', 'x10, x10, x11, lsl #2'),
    })
    targets = [0x568a6d4 + 4 * runtime.native[0xa51e002 + i] for i in range(20)]
    assert targets[5] == 0x568a96c
    dispatch['jump_table'] = {str(i): hex(target) for i, target in enumerate(targets)}
    force = require(0x5702f18, 0x5703000, {
        0x5702f18: ('ldr', 'x0, [x19, #0x10a0]'),
        0x5702f3c: ('bl', '#0x82584e8'),
        0x5702f78: ('str', 'd1, [x19, #0x10a8]'),
        0x5702fd0: ('fadd', 's0, s3, s0'),
        0x5702fdc: ('str', 's0, [x19, #0x10a8]'),
    })
    reset = require(0x5636acc, 0x5636af0, {
        0x5636acc: ('ldr', 'x8, [x19, #0x7f8]'),
        0x5636adc: ('stur', 'd0, [x8, #0x14]'),
        0x5636ae0: ('strb', 'wzr, [x19, #0x801]'),
    })
    force_consumer = require(0x5785024, 0x57850d4, {
        0x5785068: ('ldr', 'd0, [x19, #0x10a8]'),
        0x578506c: ('ldr', 's2, [x19, #0x10b0]'),
        0x578507c: ('fmul', 'v1.2s, v8.2s, v4.s[0]'),
        0x5785084: ('fmul', 'v0.2s, v0.2s, v4.s[0]'),
        0x578508c: ('fadd', 'v0.2s, v1.2s, v0.2s'),
    })
    contracts = [runtime.method(owner, name, arity) for owner, name, arity in (
        ('COW.GamePlay.Player', 'GetVelocity', 0),
        ('COW.GamePlay.Player', 'SetForceMoveVelocity', 2),
        ('COW.GamePlay.Player', 'GetForceMoveVelocity', 0),
        ('COW.GamePlay.Player', 'ResetHitFlyInfo', 0),
        ('COW.GamePlay.Player', 'SwitchPhysXState', 2),
        ('COW.GamePlay.PlayerNetwork', 'SyncStateWithServer', 1),
        ('COW.GamePlay.PlayerNetwork', 'SyncServerAuthorizedSwitchPhysXState', 1),
        ('COW.GamePlay.NetworkAIPawn', 'StopHitfly', 0),
    )]
    packet_class = runtime.classes['message.NNDOIBJBAJI']
    packet_fields = []
    for i in range(packet_class['fc']):
        ni, _, _ = struct.unpack_from('<III', runtime.metadata,
                                      runtime.fields + (packet_class['fs'] + i) * 12)
        packet_fields.append(runtime.field('message.NNDOIBJBAJI', runtime.string(ni)))
    return {
        'native_sha256': digest,
        'metadata_sha256': hashlib.sha256(runtime.metadata).hexdigest(),
        'scope': 'Static read-only evidence; no device run and no server implementation available.',
        'contracts': contracts,
        'packet657_fields': packet_fields,
        'nested_motion_data': exact_fields(31659),
        'server_velocity_field': runtime.field('message.PBGBMJIAKCI', 'IEEIGMJLPEL'),
        'regions': {'get_velocity': get_velocity, 'sync_velocity': sync_velocity,
                    'hitfly_state5': hitfly_state, 'kinematics_dispatch': dispatch,
                    'keyed_force_accumulator': force, 'reset_hitfly_info': reset,
                    'movement_additive_force_consumer': force_consumer},
        'findings': [
            'GetVelocity reads the motion vector at Player+0x1d48 -> +0x48/+0x50.',
            'SyncStateWithServer decodes message.PBGBMJIAKCI.IEEIGMJLPEL and writes that same motion vector; its state5 branch also updates interpolation data.',
            'Base UpdateKinematics state5 skips the per-state movement calculation in this particular switch. Other update paths exist; this alone does not prove all HitFly motion is server-only.',
            'ResetHitFlyInfo changes recovery metadata/flags, not the GetVelocity vector. It is not established as a horizontal brake.',
            'SetForceMoveVelocity updates a keyed force accumulator distinct from the actual motion vector. A zero force is not a proven HitFly stop.',
            'JLPJMAFLADI at0x5785068 reads the keyed-force sum and the following instructions add scaled force to scaled baseline movement. This supports an additive-force trial, not a verified universal actuator/brake or server-approved movement route.',
            'NetworkAIPawn.StopHitfly is not a human Player method.',
            'Nested GFLBFKKDAMK and an unrelated same-short-name class collide in name-only resolver dictionaries. Do not use the latter field layout for Player motion.',
            'Packet657 contains player ID, time, two scalar speeds, packed direction, string and flag; no separate destination position. The current client sender never transmits the map marker as a destination.',
        ],
        'not_established': [
            'Packet657 time/velocity semantics and server validation rules.',
            'Authoritative destination-aware flight and stop/landing request.',
            'Native/IFix behavior of any proposed new movement setter.',
            'Collision-free path, safe landing surface and endpoint precision on device.',
        ],
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--binary', type=Path, default=Path('/tmp/opencode/ff-ipa-fresh/UnityFramework'))
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    report = collect(args.binary)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + '\n')
    print(f'PASS: static native anchors and exact contracts; report={args.output}')
    print('No exact landing or stop implementation validated.')


if __name__ == '__main__':
    main()
