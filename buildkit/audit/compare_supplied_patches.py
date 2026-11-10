#!/usr/bin/env python3
"""Read-only payload extraction/comparison; originals are never rewritten.

q3 has anonymous-storey records the custom patchc reader does not handle. Read
its common prefix and independently validate the fixes tail to exact EOF. Keep
storeys opaque rather than dropping or pretending to understand their layout.
"""
import base64
import gzip
import hashlib
import json
from pathlib import Path
import struct
import sys

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'tools'))
sys.path.insert(0, str(Path(__file__).resolve().parent))
import patchc
import UnityPy
from inspect_reference_patches import describe

OUT = Path('/tmp/opencode/user-original-patch-audit')
FILES = {
    'q3': Path('/root/assembly-csharp-patch.q3QzMLimIntmNHG7~2BbafinWQ1mU~3D'),
    'HBS': Path('/root/assembly-csharp-patch.HBSY4R0mIV7GPFGWGCbnw1ze~2B0I~3D'),
}


def extract(data):
    # This game's q3 bundle marks bit0x200 but its blocks can be read with that
    # flag cleared in an analysis-only copy (not decrypting or rewriting source).
    assert data.startswith(b'UnityFS\0')
    flags = int.from_bytes(data[46:50], 'big')
    analysis = data[:46] + (flags & ~0x200).to_bytes(4, 'big') + data[50:]
    env = UnityPy.load(analysis)
    assets = [o for o in env.objects if o.type.name == 'TextAsset']
    assert len(assets) == 1
    asset = assets[0].read()
    payload = asset.m_Script.encode('utf-8', 'surrogateescape') if isinstance(asset.m_Script, str) else bytes(asset.m_Script)
    return payload, flags


def inspect(payload):
    p = patchc.Patch()
    r = patchc.R(payload)
    p.magic = r.u64()
    assert p.magic == struct.unpack('<Q', patchc.MAGIC)[0]
    p.bridge = r.st()
    p.types = [r.st() for _ in range(r.i32())]
    p.bodies = []
    for _ in range(r.i32()):
        instructions = [[r.u32(), r.i32()] for _ in range(r.i32())]
        length = r.i32() * 24
        handlers = payload[r.p:r.p+length]
        r.skip(length)
        p.bodies.append(patchc.Body(instructions, handlers))
    p.extm = [p._rm(r) for _ in range(r.i32())]
    p.interns = [r.st() for _ in range(r.i32())]
    p.fields = []
    for _ in range(r.i32()):
        isnew, tid, name = r.bo(), r.i32(), r.st()
        extra = (r.i32(), r.i32()) if isnew else None
        p.fields.append(patchc.FieldRec(isnew, tid, name, extra))
    p.sft = [(r.i32(), r.i32()) for _ in range(r.i32())]
    storeys = r.i32()
    storeys_start = r.p
    needle = b'IFix.WrappersManagerImpl, Assembly-CSharp'
    text_at = payload.find(needle, storeys_start)
    assert text_at >= 0
    candidates = []
    for start in range(max(storeys_start, text_at-5), text_at):
        tail = patchc.R(payload, start)
        try:
            manager, assembly, pad = tail.st(), tail.st(), tail.u8()
            if not manager.startswith(needle.decode()):
                continue
            count = tail.i32()
            if not 0 <= count <= len(p.bodies):
                continue
            fixes = [(p._rm(tail), tail.i32()) for _ in range(count)]
            newclasses = tail.i32()
            if newclasses == 0 and tail.p == len(payload):
                assert all(0 <= ref.tid < len(p.types) and 0 <= bid < len(p.bodies) for ref, bid in fixes)
                candidates.append((start, manager, assembly, pad, fixes))
        except (IndexError, UnicodeDecodeError, struct.error, AssertionError):
            continue
    assert len(candidates) == 1, 'Fixes tail is ambiguous/unverified'
    start, manager, assembly, pad, p.fixes = candidates[0]
    assert storeys > 0 or start == storeys_start
    return p, {'storeys': storeys, 'storeys_opaque_bytes': start-storeys_start,
               'tail_parse_exact_eof': True, 'manager': manager, 'assembly': assembly}


def key(p, ref):
    return (p.types[ref.tid].split(',')[0], ref.name,
            tuple(p.types[i].split(',')[0] for i in ref.nums))


def main():
    OUT.mkdir(exist_ok=True)
    report, patches = {}, {}
    for label, path in FILES.items():
        data = path.read_bytes()
        actual = base64.b64encode(hashlib.sha1(data).digest()).decode()
        expected = path.name.split('.', 1)[1].replace('~2B', '+').replace('~2F', '/').replace('~3D', '=')
        assert actual == expected
        payload, flags = extract(data)
        p, extra = inspect(payload)
        patches[label] = p
        (OUT/(label+'.patch')).write_bytes(payload)
        with (OUT/(label+'.disasm.txt')).open('w') as out:
            for bid, body in enumerate(p.bodies):
                out.write('\nBODY %d INSTRUCTIONS %d EH_BYTES %d\n' % (bid, len(body.instrs), len(body.eh)))
                for ii, (c, o) in enumerate(body.instrs):
                    out.write(describe(p, ii, c, o)+'\n')
        report[label] = dict(extra, file=str(path), bytes=len(data), container_flags=hex(flags),
                             sha256=hashlib.sha256(data).hexdigest(), file_sha1_base64=actual,
                             payload_sha256=hashlib.sha256(payload).hexdigest(),
                             bodies=len(p.bodies), types=len(p.types), fields=len(p.fields),
                             fixes=[{'owner':key(p,ref)[0], 'method':ref.name,
                                     'parameters':list(key(p,ref)[2]), 'body':bid} for ref,bid in p.fixes])
    q, custom = patches['q3'], patches['HBS']
    official_keys = {key(q, ref) for ref, _ in q.fixes}
    custom_keys = {key(custom, ref) for ref, _ in custom.fixes}
    report['comparison'] = {
        'common_fixes': len(official_keys & custom_keys),
        'q3_fixes_not_registered_in_HBS': len(official_keys - custom_keys),
        'HBS_fixes_not_registered_in_q3': len(custom_keys - official_keys),
        'HBS_payload_matches_channel': False,
    }
    published = Path('/opt/okmod-sophia/var/okmod/assembly-csharp-patch.bin').read_bytes()
    if published.startswith(b'\x1f\x8b'):
        published = gzip.decompress(published)
    current, _ = extract(published)
    report['comparison']['HBS_payload_matches_channel'] = current == (OUT/'HBS.patch').read_bytes()
    (OUT/'comparison.json').write_text(json.dumps(report, indent=2))
    print(json.dumps(report['comparison'], indent=2))
    print('q3:', len(q.fixes), 'fixes;', report['q3']['storeys'], 'storeys retained opaque')
    print('HBS:', len(custom.fixes), 'fixes; bytecode matches current channel:',report['comparison']['HBS_payload_matches_channel'])


if __name__ == '__main__':
    main()
