#!/usr/bin/env python3
"""Merge the official q3 codepatch (79 fixes, 10 anonymous storeys) with the
four R6 menu hooks from HBS, using append-only index relocation.

Safety properties:
- q3 bytes/tables/bodies/storeys/fixes keep their original indices and content.
- HBS bodies/tables are appended and every index-bearing operand is relocated.
- No network, channel or published artifact is touched.
"""
import gzip
import hashlib
import json
from pathlib import Path
import struct
import sys

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'tools'))
sys.path.insert(0, str(Path(__file__).resolve().parent))
import patchc
from compare_supplied_patches import inspect

OUT = Path('/tmp/opencode/q3-hbs-merge')
Q3 = Path('/tmp/opencode/user-original-patch-audit/q3.patch')
HBS = Path('/tmp/opencode/user-original-patch-audit/HBS.patch')
HBS_BUNDLE = Path('/root/assembly-csharp-patch.HBSY4R0mIV7GPFGWGCbnw1ze~2B0I~3D')

TYPE_OPS = {'Newarr', 'Castclass', 'Ldtype', 'Box', 'Isinst', 'Initobj',
            'Unbox', 'Unbox_Any', 'Constrained', 'Ldtoken'}
FIELD_OPS = {'Ldfld', 'Stfld', 'Ldflda', 'Ldsfld', 'Stsfld', 'Ldsflda'}
EXTERN_LOW16 = {'CallExtern', 'Newobj'}
EXTERN_FULL = {'Ldftn', 'Ldvirtftn'}
BODY_LOW16 = {'Call', 'Callvirt'}


def read_mref(p, r):
    return p._rm(r)


def parse_q3(payload, storey_len):
    p = patchc.Patch()
    r = patchc.R(payload)
    p.magic = r.u64()
    p.bridge = r.st()
    p.types = [r.st() for _ in range(r.i32())]
    p.bodies = []
    for _ in range(r.i32()):
        ins = [[r.u32(), r.i32()] for _ in range(r.i32())]
        n = r.i32() * 24
        eh = payload[r.p:r.p + n]
        r.skip(n)
        p.bodies.append(patchc.Body(ins, eh))
    p.extm = [read_mref(p, r) for _ in range(r.i32())]
    p.interns = [r.st() for _ in range(r.i32())]
    p.fields = []
    for _ in range(r.i32()):
        new = r.bo()
        tid = r.i32()
        name = r.st()
        extra = (r.i32(), r.i32()) if new else None
        p.fields.append(patchc.FieldRec(new, tid, name, extra))
    p.sft = [(r.i32(), r.i32()) for _ in range(r.i32())]
    p.storeys = r.i32()
    start = r.p
    p.storey_raw = payload[start:start + storey_len]
    assert len(p.storey_raw) == storey_len
    r.p = start + storey_len
    p.wm = r.st()
    p.asm = r.st()
    p.pad = r.u8()
    p.fixes = []
    for _ in range(r.i32()):
        p.fixes.append([read_mref(p, r), r.i32()])
    p.new_classes = r.i32()
    p.end = r.p
    assert r.p == len(payload), (r.p, len(payload))
    assert p.magic == struct.unpack('<Q', patchc.MAGIC)[0]
    return p


def extern_key(p, m):
    tid = p.types[m.tid]
    nums = tuple(p.types[x] for x in m.nums)
    if not m.isg:
        return (tid, m.name, (), (), nums)
    ga = tuple(p.types[x] for x in m.ga)
    items = tuple(('s', v) if k == 's' else ('i', p.types[v]) for k, v in m.items)
    return (tid, m.name, ga, items, nums)


def remap_mref(m, mt, me, mf, mi):
    out = patchc.MRef(m.isg, mt(m.tid), m.name)
    if m.isg:
        out.ga = [mt(x) for x in m.ga]
        out.items = [('s', v) if k == 's' else ('i', mt(v)) for k, v in m.items]
    out.nums = [mt(x) for x in m.nums]
    return out


def serialize(p):
    o = bytearray()
    patchc.wu64(o, p.magic)
    patchc.ws(o, p.bridge)
    patchc.wi32(o, len(p.types))
    for t in p.types:
        patchc.ws(o, t)
    patchc.wi32(o, len(p.bodies))
    for b in p.bodies:
        patchc.wi32(o, len(b.instrs))
        for c, x in b.instrs:
            patchc.wu32(o, c)
            patchc.wi32(o, x)
        patchc.wi32(o, len(b.eh) // 24)
        o += b.eh
    patchc.wi32(o, len(p.extm))
    for m in p.extm:
        m.write(o)
    patchc.wi32(o, len(p.interns))
    for s in p.interns:
        patchc.ws(o, s)
    patchc.wi32(o, len(p.fields))
    for f in p.fields:
        f.write(o)
    patchc.wi32(o, len(p.sft))
    for a, b in p.sft:
        patchc.wi32(o, a)
        patchc.wi32(o, b)
    patchc.wi32(o, p.storeys)
    o += p.storey_raw
    patchc.ws(o, p.wm)
    patchc.ws(o, p.asm)
    o.append(p.pad)
    patchc.wi32(o, len(p.fixes))
    for mr, bid in p.fixes:
        mr.write(o)
        patchc.wi32(o, bid)
    patchc.wi32(o, p.new_classes)
    return bytes(o)


def main():
    OUT.mkdir(exist_ok=True)
    q3_payload = Q3.read_bytes()
    hbs_payload = HBS.read_bytes()
    _, qx = inspect(q3_payload)
    q = parse_q3(q3_payload, qx['storeys_opaque_bytes'])
    h = patchc.Patch(hbs_payload)
    assert q.storeys == 10 and h.storeys == 0
    assert all(0 <= b < len(q.bodies) for _, b in q.fixes)
    assert all(0 <= b < len(h.bodies) for _, b in h.fixes)

    # ---- type table ----
    types = list(q.types)
    tmap = {}
    for i, t in enumerate(types):
        tmap.setdefault(t, i)
    for t in h.types:
        if t not in tmap:
            tmap[t] = len(types)
            types.append(t)
    assert len(types) == len(q.types) + len(set(h.types) - set(q.types))

    def mt(i):
        assert 0 <= i < len(h.types), i
        return tmap[h.types[i]]

    # ---- extern methods ----
    extm = list(q.extm)
    emap = {}
    for i, m in enumerate(extm):
        emap.setdefault(extern_key(q, m), i)
    for m in h.extm:
        k = extern_key(h, m)
        if k not in emap:
            emap[k] = len(extm)
            extm.append(remap_mref(m, mt, emap, None, None))

    def me(i):
        assert 0 <= i < len(h.extm), i
        k = extern_key(h, h.extm[i])
        assert k in emap, k
        return emap[k]

    # ---- fields ----
    fields = list(q.fields)
    fmap = {}
    for i, f in enumerate(fields):
        fmap.setdefault((q.types[f.tid], f.name), i)
    for f in h.fields:
        k = (h.types[f.tid], f.name)
        if k in fmap:
            old = fields[fmap[k]]
            if old.isnew and f.isnew:
                assert q.types[old.extra[0]] == h.types[f.extra[0]], (k, 'field type mismatch')
            continue
        # -1 means "no initializer body". It must NOT be relocated.
        extra = None
        if f.isnew:
            mid = f.extra[1]
            extra = (mt(f.extra[0]), mid + len(q.bodies) if mid >= 0 else -1)
        fmap[k] = len(fields)
        fields.append(patchc.FieldRec(f.isnew, mt(f.tid), f.name, extra))

    def mf(i):
        assert 0 <= i < len(h.fields), i
        k = (h.types[h.fields[i].tid], h.fields[i].name)
        assert k in fmap, k
        return fmap[k]

    # ---- intern strings ----
    interns = list(q.interns)
    imap = {}
    for i, s in enumerate(interns):
        imap.setdefault(s, i)
    for s in h.interns:
        if s not in imap:
            imap[s] = len(interns)
            interns.append(s)

    def mi(i):
        assert 0 <= i < len(h.interns), i
        return imap[h.interns[i]]

    # ---- bodies ----
    offset = len(q.bodies)
    bodies = list(q.bodies)
    for bid, body in enumerate(h.bodies):
        ins = []
        for code, operand in body.instrs:
            name = h.op_name(code)
            assert not name.startswith('?'), ('unknown opcode', code, bid)
            if name in TYPE_OPS:
                if operand >= 0:
                    operand = mt(operand)
            elif name in FIELD_OPS:
                if operand >= 0:
                    operand = mf(operand)
            elif name == 'Ldstr':
                operand = mi(operand)
            elif name in BODY_LOW16:
                argc, target = operand >> 16, operand & 0xffff
                assert 0 <= target < len(h.bodies), (bid, name, target)
                operand = (argc << 16) | (target + offset)
            elif name in EXTERN_LOW16:
                argc, ref = operand >> 16, operand & 0xffff
                operand = (argc << 16) | me(ref)
            elif name in EXTERN_FULL:
                operand = me(operand)
            elif name in ('Callvirtvirt', 'Ldvirtftn2'):
                assert False, ('anonymous-storey operand in HBS', bid, name)
            elif name == 'Newanon':
                assert False, ('Newanon in HBS', bid)
            ins.append([code, operand])
        eh = bytearray()
        for i in range(len(body.eh) // 24):
            rec = list(struct.unpack_from('<6i', body.eh, i * 24))
            if rec[0] == 0 and rec[1] >= 0:
                rec[1] = mt(rec[1])
            eh += struct.pack('<6i', *rec)
        bodies.append(patchc.Body(ins, bytes(eh)))

    # ---- static field types / storeys / tail ----
    sft = list(q.sft)
    for tid, cctor in h.sft:
        sft.append((mt(tid), cctor + offset))
    assert h.storeys == 0

    # ---- fixes ----
    # q3 indices are unchanged by append-only merge, so its fix refs stay as-is.
    fixes = [[mr, bid] for mr, bid in q.fixes]
    for mr, bid in h.fixes:
        fixes.append([remap_mref(mr, mt, me, mf, mi), bid + offset])

    merged = patchc.Patch()
    merged.magic = q.magic
    merged.bridge = q.bridge
    merged.types = types
    merged.bodies = bodies
    merged.extm = extm
    merged.interns = interns
    merged.fields = fields
    merged.sft = sft
    merged.storeys = q.storeys
    merged.storey_raw = q.storey_raw
    merged.wm = q.wm
    merged.asm = q.asm
    merged.pad = q.pad
    merged.fixes = fixes
    merged.new_classes = 0
    blob = serialize(merged)
    (OUT / 'merged.patch').write_bytes(blob)

    # ---------------- verification ----------------
    m = parse_q3(blob, len(q.storey_raw))
    assert m.magic == q.magic and m.bridge == q.bridge
    assert m.types[:len(q.types)] == q.types
    assert len(m.bodies) == len(q.bodies) + len(h.bodies)
    for i, (a, b) in enumerate(zip(q.bodies, m.bodies)):
        assert a.instrs == b.instrs and a.eh == b.eh, ('q3 body changed', i)
    assert m.storeys == 10 and m.storey_raw == q.storey_raw
    assert m.wm == q.wm and m.asm == q.asm and m.pad == q.pad
    assert len(m.fixes) == 83
    for i, ((amr, ab), (bmr, bb)) in enumerate(zip(q.fixes, m.fixes)):
        assert ab == bb and extern_key(q, amr) == extern_key(m, bmr), ('q3 fix changed', i)
    for j, ((hmr, hb), (mmr, mb)) in enumerate(zip(h.fixes, m.fixes[len(q.fixes):])):
        assert mb == hb + offset, ('fix body', j)
        assert extern_key(h, hmr) == extern_key(m, mmr), ('fix symbol', j)
    assert m.extm[:len(q.extm)] and all(
        extern_key(q, a) == extern_key(m, b) for a, b in zip(q.extm, m.extm[:len(q.extm)]))

    # HBS bodies: every relocated operand must resolve to the same symbol
    for bid, body in enumerate(h.bodies):
        mb = m.bodies[offset + bid]
        assert len(body.instrs) == len(mb.instrs)
        for k, ((hc, ho), (mc, mo)) in enumerate(zip(body.instrs, mb.instrs)):
            assert hc == mc
            name = h.op_name(hc)
            if name in TYPE_OPS:
                assert mo == ho if ho < 0 else m.types[mo] == h.types[ho], (bid, k, name)
            elif name in FIELD_OPS:
                if ho >= 0:
                    a, b = h.fields[ho], m.fields[mo]
                    assert (h.types[a.tid], a.name) == (m.types[b.tid], b.name), (bid, k, name)
                else:
                    assert mo == ho, (bid, k, name)
            elif name == 'Ldstr':
                assert m.interns[mo] == h.interns[ho], (bid, k)
            elif name in BODY_LOW16:
                assert (mo & 0xffff) == (ho & 0xffff) + offset, (bid, k)
                assert (mo >> 16) == (ho >> 16)
            elif name in EXTERN_LOW16:
                assert (mo >> 16) == (ho >> 16)
                assert extern_key(h, h.extm[ho & 0xffff]) == extern_key(m, m.extm[mo & 0xffff])
            elif name in EXTERN_FULL:
                assert extern_key(h, h.extm[ho]) == extern_key(m, m.extm[mo])
            else:
                assert mo == ho, (bid, k, name, ho, mo)
        # EH catches
        for k in range(len(body.eh) // 24):
            a = list(struct.unpack_from('<6i', body.eh, k * 24))
            b = list(struct.unpack_from('<6i', mb.eh, k * 24))
            assert a[0] == b[0] and a[2:] == b[2:]
            if a[0] == 0 and a[1] >= 0:
                assert h.types[a[1]] == m.types[b[1]]
            else:
                assert a[1] == b[1]

    # HBS fields: relocated type/method ids, -1 initializer preserved
    q3_field_keys = {(q.types[f.tid], f.name): f for f in q.fields}
    for hf in h.fields:
        key = (h.types[hf.tid], hf.name)
        mf_rec = m.fields[fmap[key]]
        assert m.types[mf_rec.tid] == key[0] and mf_rec.name == key[1]
        if hf.isnew:
            assert mf_rec.isnew
            assert m.types[mf_rec.extra[0]] == h.types[hf.extra[0]]
            expected = hf.extra[1] + offset if hf.extra[1] >= 0 else -1
            assert mf_rec.extra[1] == expected, (key, hf.extra, mf_rec.extra)
        elif key in q3_field_keys:
            assert not mf_rec.isnew
    assert all(f.extra[1] == -1 for f in m.fields if f.isnew and f.name.startswith('Mz'))

    # range checks on every body
    for bid, body in enumerate(m.bodies):
        for code, operand in body.instrs:
            name = m.op_name(code)
            if name in TYPE_OPS and operand >= 0:
                assert operand < len(m.types), (bid, name)
            elif name in FIELD_OPS and operand >= 0:
                assert operand < len(m.fields), (bid, name)
            elif name == 'Ldstr':
                assert operand < len(m.interns), (bid, name)
            elif name in BODY_LOW16:
                assert (operand & 0xffff) < len(m.bodies), (bid, name)
            elif name in EXTERN_LOW16:
                assert (operand & 0xffff) < len(m.extm), (bid, name)
            elif name in EXTERN_FULL:
                assert operand < len(m.extm), (bid, name)

    report = {
        'q3_bytes': len(q3_payload), 'hbs_bytes': len(hbs_payload),
        'merged_bytes': len(blob), 'merged_sha256': hashlib.sha256(blob).hexdigest(),
        'bodies': len(m.bodies), 'types': len(m.types), 'externs': len(m.extm),
        'fields': len(m.fields), 'interns': len(m.interns), 'sft': len(m.sft),
        'storeys': m.storeys, 'storey_bytes': len(m.storey_raw),
        'fixes': len(m.fixes), 'new_classes': m.new_classes,
        'q3_fixes_preserved': 79, 'hbs_fixes_relocated': 4,
        'verification': 'all q3 prefix + q3 bodies + 79 fixes unchanged; '
                        'all HBS operands/EH/fields/fixes resolve to same symbols',
    }
    (OUT / 'merge-report.json').write_text(json.dumps(report, indent=2))
    print(json.dumps(report, indent=2))


if __name__ == '__main__':
    main()
