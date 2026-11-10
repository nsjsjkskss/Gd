#!/usr/bin/env python3
"""Read-only audit of the Sophia-derived port1300 runtime's map HitFly block."""
import argparse
import hashlib
import json
import math
import struct
import sys
from pathlib import Path

sys.path.insert(0, '/root/okmod-1297-clean/buildkit/tools')
sys.path.insert(0, '/opt/okmod-1300')
from patchc import Patch
from build_patch import unpack_bundle

EXPECTED = '39ce90c5293a45a4529580da26dd98e816f01efc80f9ba6d59cd37c186826ff4'


def model(distance, dy, gravity):
    """Transcribed reference calculation, not a native/server physics model."""
    t = max(2.0, min(15.0, (150 + math.sqrt(max(0, 150 ** 2 - 2 * gravity * dy))) / gravity))
    factor = max(2.15, min(2.55, 2.1 + distance / 1000))
    return {'distance': distance, 'dy': dy, 'gravity_input': gravity,
            'estimated_time': t, 'horizontal_factor': factor,
            'packet_time': max(1.8, min(6.0, t)),
            'packet_xz': distance / t * factor, 'packet_y': 150}


def collect(runtime):
    data = runtime.read_bytes()
    digest = hashlib.sha256(data).hexdigest()
    if digest != EXPECTED:
        raise ValueError('Reference runtime changed; re-establish instruction indices before audit')
    _, _, payload, _ = unpack_bundle(data)
    p = Patch(payload)
    body = p.bodies[20]

    def instruction(i):
        code, value = body.instrs[i]
        op = p.op_name(code)
        detail = ''
        if op == 'Ldstr':
            detail = p.interns[value]
        elif op in ('CallExtern', 'Newobj'):
            m = p.extm[value & 65535]
            detail = p.types[m.tid].split(',')[0] + '.' + m.name
        elif op in ('Ldsfld', 'Stsfld', 'Ldfld', 'Stfld', 'Ldflda', 'Ldsflda'):
            f = p.fields[value]
            detail = p.types[f.tid].split(',')[0] + '.' + f.name
        elif op == 'Ldc_R4':
            detail = struct.unpack('<f', struct.pack('<i', value))[0]
        return {'index': i, 'op': op, 'operand': value, 'detail': detail}

    anchors = {i: instruction(i) for i in (6593, 6624, 6788, 6792, 6829, 6835,
                                            6897, 6899, 6903, 6958, 6959, 6962,
                                            6967, 6968, 6978, 6979, 7129, 7134)}
    assert anchors[6593]['detail'].endswith('.ffxc_hitFlyTriggered')
    assert anchors[6624]['detail'] == 'message.NNDOIBJBAJI'
    assert anchors[6788]['detail'] == 'm_HasLocalMapMarkRealPos'
    assert anchors[6792]['detail'] == 'm_LocalMapMarkRealPos'
    assert anchors[6897]['detail'] == 150.0
    assert anchors[6899]['detail'] == 50.0
    assert anchors[6903]['detail'] == 'get_FallingGravity'
    assert anchors[6958]['detail'] == 2.0 and anchors[6959]['detail'] == 15.0
    assert anchors[6978]['detail'] > 1.79 and anchors[6979]['detail'] == 6.0
    assert anchors[7129]['operand'] == 657
    assert anchors[7134]['detail'] == 'COW.GameFacade.Send'
    block = [instruction(i) for i in range(6593, 7143)]
    calls = [x for x in block if x['op'] in ('CallExtern', 'Newobj')]
    forbidden = ('SetForceMoveVelocity', 'set_position', 'StopHitfly', 'SwitchPhysXState')
    assert not any(x['detail'].endswith('.' + name) for name in forbidden for x in calls)
    return {
        'reference': str(runtime), 'runtime_sha256': digest,
        'payload_sha256': hashlib.sha256(payload).hexdigest(),
        'hook_body': 20, 'instruction_range_inclusive': [6593, 7142],
        'anchors': anchors,
        'source_behavior': {
            'trigger': 'one-shot triggered/armed flags; HUD release rearms and queues a new shot',
            'marker': 'FindObjectOfType(UIMapContentController, true); reflected real-position fields, fallback mark-controller.m_pos',
            'map_y': 'Uses marker Y in target-minus-origin; no terrain raycast in this block',
            'vertical': 150,
            'gravity': 'default50; abs(reflected get_FallingGravity()) accepted if5..160, otherwise default retained',
            'time': 'clamp((150+sqrt(max(0,22500-2*g*dy)))/g,2,15)',
            'xz_factor': 'clamp(2.1+D/1000,2.15,2.55)',
            'xz': 'D/time*xz_factor; not limited to100 in this block',
            'packet_time': 'clamp(time,1.8,6)',
            'direction': 'normalized horizontal target-origin, local forward if horizontal distance<=0.001',
            'send': 'Send(657,msg,2,false), result discarded',
            'stop': 'No destination feedback or explicit stop in inspected block; not proof of server landing behavior',
            'no_marker': 'Falls back to camera/local forward, time1/XZ25/Y20',
        },
        'examples_calculation_only': [model(d, dy, g) for g in (9.8, 50) for d, dy in ((100, 0), (300, 0), (100, 40))],
        'not_proven': ['actual device trajectory', 'server velocity/time handling', 'exact destination landing',
                       'post-arrival braking', 'equivalence of game versions', 'benefit of using reflection rather than exact calls'],
        'instructions': block,
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--runtime', type=Path, default=Path('/opt/okmod-1300/var/runtime.bundle'))
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    report = collect(args.runtime)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print('PASS: pinned Sophia runtime instruction anchors and map657 reference block')
    print('Read-only comparison; no deployment, no actual flight validation.')


if __name__ == '__main__':
    main()
