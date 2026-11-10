"""IFix tables with lossless anonymous-storey preservation.

Existing storeys are kept opaque with unchanged index space. Their end is found
by a unique, fully validated manager/fixes tail, followed by a byte-exact roundtrip.
Donor storeys are deliberately unsupported: relocating them needs a full decoder.
"""
import copy
import gzip
import hashlib
from pathlib import Path
import struct
import sys

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
import patchc as P


class InvalidPatch(ValueError):
    pass


def require(ok, message):
    if not ok:
        raise InvalidPatch(message)


def digest(data):
    return hashlib.sha256(data).hexdigest()


def count(r, maximum=1000000):
    n = r.i32()
    require(0 <= n <= maximum, 'invalid table length %s' % n)
    return n


def parse(data):
    require(data.startswith(P.MAGIC), 'not IFix data')
    r, p = P.R(data), P.Patch()
    p.magic, p.bridge = r.u64(), r.st()
    p.types = [r.st() for _ in range(count(r))]
    p.bodies = []
    for _ in range(count(r)):
        instructions = [[r.u32(), r.i32()] for _ in range(count(r))]
        length = count(r, 100000) * 24
        handlers = data[r.p:r.p + length]
        require(len(handlers) == length, 'truncated exception table')
        r.skip(length)
        p.bodies.append(P.Body(instructions, handlers))
    p.extm = [p._rm(r) for _ in range(count(r))]
    p.interns = [r.st() for _ in range(count(r))]
    p.fields = []
    for _ in range(count(r)):
        new, tid, name = r.bo(), r.i32(), r.st()
        extra = (r.i32(), r.i32()) if new else None
        p.fields.append(P.FieldRec(new, tid, name, extra))
    p.sft = [(r.i32(), r.i32()) for _ in range(count(r))]
    p.storeys = count(r)
    start = r.p
    # Search only for the same wrapper-manager assembly identity as the bridge.
    require('ILFixInterfaceBridge' in p.bridge, 'unknown bridge contract')
    expected = p.bridge.replace('ILFixInterfaceBridge', 'WrappersManagerImpl', 1)
    needle, candidates = expected.encode(), []
    pos = data.find(needle, start)
    while pos >= 0:
        for at in range(max(start, pos - 5), pos):
            tail = P.R(data, at)
            try:
                wm, asm, pad = tail.st(), tail.st(), tail.u8()
                if wm != expected or not asm.startswith(', '):
                    continue
                fixes = [(p._rm(tail), tail.i32()) for _ in range(count(tail, len(p.bodies)))]
                newclasses = tail.i32()
                if newclasses != 0 or tail.p != len(data):
                    continue
                if not all(0 <= m.tid < len(p.types) and 0 <= bid < len(p.bodies)
                           for m, bid in fixes):
                    continue
                candidates.append((at, wm, asm, pad, fixes))
            except (UnicodeError, IndexError, struct.error, InvalidPatch):
                continue
        pos = data.find(needle, pos + 1)
    require(len(candidates) == 1, 'unverified/ambiguous manager tail or unsupported new classes')
    end, p.wm, p.asm, p.pad, p.fixes = candidates[0]
    p.storey_raw, p.new_classes = data[start:end], 0
    require(p.storeys > 0 or not p.storey_raw, 'unexpected zero-storey bytes')
    validate_tables(p)
    require(serialize(p) == data, 'IFix parse/write roundtrip differs')
    return p


def serialize(p):
    out = bytearray()
    P.wu64(out, p.magic); P.ws(out, p.bridge)
    P.wi32(out, len(p.types))
    for value in p.types:
        P.ws(out, value)
    P.wi32(out, len(p.bodies))
    for b in p.bodies:
        P.wi32(out, len(b.instrs))
        for code, value in b.instrs:
            P.wu32(out, code); P.wi32(out, value)
        P.wi32(out, len(b.eh) // 24); out.extend(b.eh)
    P.wi32(out, len(p.extm))
    for ref in p.extm:
        ref.write(out)
    P.wi32(out, len(p.interns))
    for value in p.interns:
        P.ws(out, value)
    P.wi32(out, len(p.fields))
    for f in p.fields:
        f.write(out)
    P.wi32(out, len(p.sft))
    for typ, ctor in p.sft:
        P.wi32(out, typ); P.wi32(out, ctor)
    P.wi32(out, p.storeys); out.extend(p.storey_raw)
    P.ws(out, p.wm); P.ws(out, p.asm); out.append(p.pad)
    P.wi32(out, len(p.fixes))
    for ref, bid in p.fixes:
        ref.write(out); P.wi32(out, bid)
    P.wi32(out, p.new_classes)
    return bytes(out)


def validate_tables(p):
    def typ(i):
        require(0 <= i < len(p.types), 'type index out of range')
    for m in p.extm + [m for m, _ in p.fixes]:
        typ(m.tid)
        for i in m.nums + m.ga + [v for k, v in m.items if k == 'i']:
            typ(i)
    for f in p.fields:
        typ(f.tid)
        if f.isnew:
            typ(f.extra[0])
            require(f.extra[1] == -1 or 0 <= f.extra[1] < len(p.bodies), 'invalid field initializer')
    for t, ctor in p.sft:
        typ(t)
        require(ctor == -1 or 0 <= ctor < len(p.bodies), 'invalid static initializer')
    for b in p.bodies:
        require(bool(b.instrs) and p.op_name(b.instrs[0][0]) == 'StackSpace', 'missing StackSpace')
        for at in range(0, len(b.eh), 24):
            kind, ct, ts, te, hs, he = struct.unpack_from('<6i', b.eh, at)
            require(kind in (0, 1, 2, 4), 'unknown EH kind')
            if kind == 0 and ct >= 0:
                typ(ct)
            require(0 <= ts <= te <= len(b.instrs) and 0 <= hs <= he <= len(b.instrs), 'invalid EH ranges')


def extract(packed):
    import UnityPy
    raw = gzip.decompress(packed) if packed.startswith(b'\x1f\x8b') else packed
    if raw.startswith(P.MAGIC):
        return raw
    require(raw.startswith(b'UnityFS\0'), 'expected UnityFS bundle')
    # Clear the known game-specific 0x200 format bit only in an analysis copy.
    flags = int.from_bytes(raw[46:50], 'big')
    analysis = raw[:46] + (flags & ~0x200).to_bytes(4, 'big') + raw[50:]
    env = UnityPy.load(analysis)
    assets = [o for o in env.objects if o.type.name == 'TextAsset']
    require(len(assets) == 1, 'bundle must have exactly one TextAsset')
    value = assets[0].read().m_Script
    return value.encode('utf-8', 'surrogateescape') if isinstance(value, str) else bytes(value)


def package(container, payload):
    import UnityPy
    raw = gzip.decompress(container) if container.startswith(b'\x1f\x8b') else container
    require(raw.startswith(b'UnityFS\0'), 'donor container is not UnityFS')
    flags = int.from_bytes(raw[46:50], 'big')
    require(not flags & 0x200, 'unverified donor packing flags')
    env = UnityPy.load(raw)
    assets = [o for o in env.objects if o.type.name == 'TextAsset']
    require(len(assets) == 1, 'expected one container TextAsset')
    asset = assets[0].read()
    asset.m_Script = payload.decode('utf-8', 'surrogateescape'); asset.save()
    packed = gzip.compress(bytes(env.file.save(packer='original')), 9, mtime=0)
    require(extract(packed) == payload, 'bundle roundtrip mismatch')
    return packed


TYPE_OPS = {'Newarr', 'Castclass', 'Ldtype', 'Box', 'Isinst', 'Initobj', 'Unbox',
            'Unbox_Any', 'Constrained', 'Ldtoken', 'Ldelema', 'Ldobj', 'Stobj', 'Cpobj', 'Sizeof'}
FIELD_OPS = {'Ldfld', 'Stfld', 'Ldflda', 'Ldsfld', 'Stsfld', 'Ldsflda'}


def refbytes(ref):
    result = bytearray(); ref.write(result); return bytes(result)


def remap_ref(ref, types):
    result = copy.deepcopy(ref)
    result.tid = types(ref.tid)
    result.nums = [types(i) for i in ref.nums]
    result.ga = [types(i) for i in ref.ga]
    result.items = [(k, types(v) if k == 'i' else v) for k, v in ref.items]
    return result


def body_indices(p, bid):
    """Walk opcodes without interpreting inline 64-bit literal words as code."""
    i, body = 0, p.bodies[bid]
    while i < len(body.instrs):
        code, value = body.instrs[i]
        name = p.op_name(code)
        require(not name.startswith('?'), 'unknown donor opcode')
        require(name not in ('Switch', 'Newanon', 'Callvirtvirt', 'Ldvirtftn2', 'Jmp'),
                'unsupported donor relocation opcode ' + name)
        yield i, name, value
        i += 2 if name in ('Ldc_I8', 'Ldc_R8') else 1
        require(i <= len(body.instrs), 'truncated inline literal')


def closure(p, roots):
    seen, todo = set(), list(roots)
    while todo:
        bid = todo.pop()
        require(0 <= bid < len(p.bodies), 'invalid donor body reference')
        if bid in seen:
            continue
        seen.add(bid)
        for _, name, value in body_indices(p, bid):
            if name in ('Call', 'Callvirt'):
                todo.append(value & 65535)
            elif name in FIELD_OPS:
                require(0 <= value < len(p.fields), 'donor anonymous/static-slot field unsupported')
                f = p.fields[value]
                if f.isnew and f.extra[1] >= 0:
                    todo.append(f.extra[1])
    return sorted(seen)


def merge(base, donor, hooks):
    require(base.bridge == donor.bridge and base.wm == donor.wm and base.asm == donor.asm,
            'bridge/assembly contract changed')
    # Only reachable donor bodies/tables are imported; donor q3 data is not.
    graph = closure(donor, [bid for _, bid in hooks])
    out = copy.deepcopy(base)
    bodymap = {bid: len(base.bodies) + i for i, bid in enumerate(graph)}
    require(len(base.bodies) + len(graph) < 65536, '16-bit body id overflow')
    typecache = {name: i for i, name in reversed(list(enumerate(out.types)))}
    externcache = {refbytes(m): i for i, m in reversed(list(enumerate(out.extm)))}
    stringcache = {s: i for i, s in reversed(list(enumerate(out.interns)))}
    fieldcache = {(f.tid, f.name): i for i, f in enumerate(out.fields)}
    tm, em, fm, sm = {}, {}, {}, {}

    def types(i):
        require(0 <= i < len(donor.types), 'bad donor type')
        if i not in tm:
            name = donor.types[i]
            if name not in typecache:
                typecache[name] = len(out.types); out.types.append(name)
            tm[i] = typecache[name]
        return tm[i]

    def external(i):
        require(0 <= i < len(donor.extm), 'bad donor external')
        if i not in em:
            m = remap_ref(donor.extm[i], types); key = refbytes(m)
            if key not in externcache:
                externcache[key] = len(out.extm); out.extm.append(m)
            em[i] = externcache[key]
        require(em[i] < 65536, '16-bit extern overflow')
        return em[i]

    def fields(i):
        if i not in fm:
            f = copy.deepcopy(donor.fields[i]); f.tid = types(f.tid)
            if f.isnew:
                f.extra = (types(f.extra[0]), bodymap[f.extra[1]] if f.extra[1] >= 0 else -1)
            key = (f.tid, f.name)
            if key in fieldcache:
                old = out.fields[fieldcache[key]]
                require(not f.isnew and not old.isnew, 'official/donor dynamic field collision: ' + f.name)
            else:
                fieldcache[key] = len(out.fields); out.fields.append(f)
            fm[i] = fieldcache[key]
        return fm[i]

    def strings(i):
        if i not in sm:
            s = donor.interns[i]
            if s not in stringcache:
                stringcache[s] = len(out.interns); out.interns.append(s)
            sm[i] = stringcache[s]
        return sm[i]

    for bid in graph:
        body = copy.deepcopy(donor.bodies[bid])
        for at, name, value in body_indices(donor, bid):
            if name in TYPE_OPS and value >= 0:
                value = types(value)
            elif name in FIELD_OPS:
                value = fields(value)
            elif name == 'Ldstr':
                value = strings(value)
            elif name in ('Call', 'Callvirt'):
                value = (value & 0xffff0000) | bodymap[value & 65535]
            elif name in ('CallExtern', 'Newobj'):
                value = (value & 0xffff0000) | external(value & 65535)
            elif name in ('Ldftn', 'Ldvirtftn'):
                value = external(value)
            body.instrs[at][1] = value
        handlers = bytearray()
        for at in range(0, len(body.eh), 24):
            rec = list(struct.unpack_from('<6i', body.eh, at))
            if rec[0] == 0 and rec[1] >= 0:
                rec[1] = types(rec[1])
            handlers.extend(struct.pack('<6i', *rec))
        body.eh = bytes(handlers); out.bodies.append(body)
    existing = {refbytes(m) for m, _ in base.fixes}
    for ref, bid in hooks:
        m = remap_ref(ref, types)
        require(refbytes(m) not in existing, 'official hotfix overlaps menu hook: ' + m.name)
        out.fixes.append((m, bodymap[bid]))
    result = parse(serialize(out))
    verify_preserved(base, result)
    # Verify every used operand against its symbolic source/expected mapping.
    # Do not invert deduplicated tables: multiple source indices can alias one.
    for bid in graph:
        restored = result.bodies[bodymap[bid]]
        require(len(restored.instrs) == len(donor.bodies[bid].instrs), 'instruction count changed')
        for at, name, value in body_indices(donor, bid):
            new = restored.instrs[at][1]
            expected = value
            if name in TYPE_OPS and value >= 0: expected = tm[value]
            elif name in FIELD_OPS: expected = fm[value]
            elif name == 'Ldstr': expected = sm[value]
            elif name in ('Call', 'Callvirt'): expected = (value & 0xffff0000) | bodymap[value & 65535]
            elif name in ('CallExtern', 'Newobj'): expected = (value & 0xffff0000) | em[value & 65535]
            elif name in ('Ldftn', 'Ldvirtftn'): expected = em[value]
            require(new == expected and restored.instrs[at][0] == donor.bodies[bid].instrs[at][0],
                    'instruction relocation mismatch')
            if name in ('Ldc_I8', 'Ldc_R8'):
                require(restored.instrs[at + 1] == donor.bodies[bid].instrs[at + 1], 'inline literal modified')
            if name in ('Br', 'Brtrue', 'Brfalse', 'Beq', 'Bge', 'Bgt', 'Ble', 'Blt',
                        'Bne_Un', 'Bge_Un', 'Bgt_Un', 'Ble_Un', 'Blt_Un'):
                require(0 <= at + value < len(restored.instrs), 'donor branch out of range')
            elif name == 'Leave':
                require(0 <= value < len(restored.instrs), 'donor continuation out of range')
        for at in range(0, len(restored.eh), 24):
            before = list(struct.unpack_from('<6i', donor.bodies[bid].eh, at))
            after = list(struct.unpack_from('<6i', restored.eh, at))
            if before[0] == 0 and before[1] >= 0: before[1] = tm[before[1]]
            require(before == after, 'exception metadata relocation mismatch')
    for source, dest in fm.items():
        f, g = donor.fields[source], result.fields[dest]
        require((g.tid, g.name, g.isnew) == (tm[f.tid], f.name, f.isnew), 'field relocation mismatch')
        if f.isnew:
            require(g.extra == (tm[f.extra[0]], bodymap[f.extra[1]] if f.extra[1] >= 0 else -1),
                    'field initializer relocation mismatch')
    return result, {'imported_bodies': graph, 'body_map': bodymap, 'type_map': tm,
                    'extern_map': em, 'field_map': fm, 'string_map': sm,
                    'official': inventory(base), 'candidate': inventory(result)}


def verify_preserved(base, merged):
    require(merged.types[:len(base.types)] == base.types, 'official types changed')
    require(merged.interns[:len(base.interns)] == base.interns, 'official strings changed')
    require(merged.sft == base.sft and merged.storeys == base.storeys and
            merged.storey_raw == base.storey_raw, 'official storeys/static initializers changed')
    require((merged.bridge, merged.wm, merged.asm, merged.pad) ==
            (base.bridge, base.wm, base.asm, base.pad), 'loader identity changed')
    for a, b in zip(base.bodies, merged.bodies):
        require(a.instrs == b.instrs and a.eh == b.eh, 'official body/EH changed')
    require(len(merged.bodies) >= len(base.bodies), 'official bodies removed')
    require([refbytes(m) for m in merged.extm[:len(base.extm)]] ==
            [refbytes(m) for m in base.extm], 'official externs changed')
    for a, b in zip(base.fields, merged.fields):
        require((a.isnew, a.tid, a.name, a.extra) == (b.isnew, b.tid, b.name, b.extra), 'official field changed')
    require(len(merged.fields) >= len(base.fields), 'official fields removed')
    require([(refbytes(m), bid) for m, bid in merged.fixes[:len(base.fixes)]] ==
            [(refbytes(m), bid) for m, bid in base.fixes], 'official fixes changed')


def inventory(p):
    return {'bodies': len(p.bodies), 'types': len(p.types), 'externs': len(p.extm),
            'fields': len(p.fields), 'strings': len(p.interns), 'static_slots': len(p.sft),
            'storeys': p.storeys, 'storey_bytes': len(p.storey_raw), 'fixes': len(p.fixes)}


def export_vm(p, entry, path):
    graph = [entry] + [b for b in closure(p, [entry]) if b != entry]
    bodymap = {bid: i for i, bid in enumerate(graph)}
    refs, fields, bodies = {}, {}, []
    for bid in graph:
        code = []
        for c, value in p.bodies[bid].instrs:
            name = p.op_name(c)
            if name in ('Call', 'Callvirt'):
                value = (value & 0xffff0000) | bodymap[value & 65535]
            elif name in ('CallExtern', 'Newobj'):
                ix = value & 65535; refs.setdefault(ix, len(refs))
                value = (value & 0xffff0000) | refs[ix]
            elif name in FIELD_OPS:
                fields.setdefault(value, len(fields)); value = fields[value]
            code.append([name, value])
        bodies.append({'name': 'body' + str(bid), 'code': code,
                       'handlers': [list(struct.unpack_from('<6i', p.bodies[bid].eh, i))
                                    for i in range(0, len(p.bodies[bid].eh), 24)]})
    import json
    manifest = {
        'types': [t.split(',')[0] for t in p.types], 'strings': p.interns, 'bodies': bodies,
        'externs': [{'owner': p.types[p.extm[i].tid].split(',')[0], 'name': p.extm[i].name,
                     'params': [p.types[t].split(',')[0] for t in p.extm[i].nums]} for i in refs],
        'fields': [{'owner': p.types[p.fields[i].tid].split(',')[0], 'name': p.fields[i].name,
                    'isnew': p.fields[i].isnew,
                    'type': p.types[p.fields[i].extra[0]].split(',')[0] if p.fields[i].isnew else None,
                    'initializer': p.fields[i].extra[1] if p.fields[i].isnew else -1} for i in fields],
    }
    Path(path).write_text(json.dumps(manifest, indent=2))
