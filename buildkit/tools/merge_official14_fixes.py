import gzip, sys, struct, hashlib, base64
from pathlib import Path
sys.path.insert(0, '/root/okmod-1297-clean/buildkit/tools')
sys.path.insert(0, '/root/okmod-1297-clean/buildkit/audit')
import patchc, UnityPy
from merge_q3_hbs import parse_q3, serialize, extern_key, remap_mref, TYPE_OPS, FIELD_OPS, EXTERN_LOW16, EXTERN_FULL, BODY_LOW16

R1225 = '/tmp/opencode/360-field-r1225/okmod-R8-AIM-ESP.bin'
UP14 = '/tmp/opencode/up14patch.bin'
OUT = '/tmp/opencode/okmod-R12.31-official14fix.bin'
CONTAINER = '/root/assembly-csharp-patch.HBSY4R0mIV7GPFGWGCbnw1ze~2B0I~3D'

def load_bundle(path):
    b = Path(path).read_bytes()
    raw = gzip.decompress(b)
    m, size, payload = patchc.split_bundle(raw)
    return parse_q3(payload, 580), raw

def fix_key(p, ref):
    return (p.types[ref.tid], ref.name, tuple(p.types[x] for x in ref.nums), ref.isg)

q, q_raw = load_bundle(R1225)
h, h_raw = load_bundle(UP14)
qkeys = {fix_key(q, m) for m, b in q.fixes}
new_fixes = [(m, b) for m, b in h.fixes if fix_key(h, m) not in qkeys]
assert len(new_fixes) == 14, len(new_fixes)
print('new official .14 fixes:', len(new_fixes))

# Transitive body closure for new fixes.
selected = set()
todo = [b for m, b in new_fixes]
while todo:
    bid = todo.pop()
    if bid in selected:
        continue
    selected.add(bid)
    for code, op in h.bodies[bid].instrs:
        name = h.op_name(code)
        if name in ('Call', 'Callvirt'):
            target = op & 0xffff
            if target not in selected:
                todo.append(target)
print('selected official bodies:', len(selected))

# Type map.
types = list(q.types)
tmap = {}
for i, t in enumerate(types):
    tmap.setdefault(t, i)
def mt(i):
    assert 0 <= i < len(h.types), i
    t = h.types[i]
    if t not in tmap:
        tmap[t] = len(types)
        types.append(t)
    return tmap[t]

# Extern map.
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
    return emap[extern_key(h, h.extm[i])]

# Field map.
fields = list(q.fields)
fmap = {}
for i, f in enumerate(fields):
    fmap.setdefault((q.types[f.tid], f.name), i)
for f in h.fields:
    k = (h.types[f.tid], f.name)
    if k in fmap:
        continue
    extra = None
    if f.isnew:
        mid = f.extra[1]
        # Initializer bodies are only valid if we append them; otherwise -1.
        extra = (mt(f.extra[0]), mid if mid < 0 else -1)
    fmap[k] = len(fields)
    fields.append(patchc.FieldRec(f.isnew, mt(f.tid), f.name, extra))
def mf(i):
    assert 0 <= i < len(h.fields), i
    k = (h.types[h.fields[i].tid], h.fields[i].name)
    assert k in fmap, k
    return fmap[k]

# Intern strings.
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

# Append selected bodies.
order = sorted(selected)
offset = len(q.bodies)
bmap = {bid: offset + idx for idx, bid in enumerate(order)}
bodies = list(q.bodies)
for bid in order:
    body = h.bodies[bid]
    ins = []
    for code, operand in body.instrs:
        name = h.op_name(code)
        if name in TYPE_OPS:
            if operand >= 0: operand = mt(operand)
        elif name in FIELD_OPS:
            if operand >= 0: operand = mf(operand)
        elif name == 'Ldstr':
            operand = mi(operand)
        elif name in BODY_LOW16:
            argc, target = operand >> 16, operand & 0xffff
            assert target in bmap, (bid, target)
            operand = (argc << 16) | bmap[target]
        elif name in EXTERN_LOW16:
            argc, ref = operand >> 16, operand & 0xffff
            operand = (argc << 16) | me(ref)
        elif name in EXTERN_FULL:
            operand = me(operand)
        elif name in ('Newanon', 'Callvirtvirt', 'Ldvirtftn2'):
            raise AssertionError(('storey operand unsupported', bid, name))
        ins.append([code, operand])
    eh = bytearray()
    for i in range(len(body.eh) // 24):
        rec = list(struct.unpack_from('<6i', body.eh, i * 24))
        if rec[0] == 0 and rec[1] >= 0:
            rec[1] = mt(rec[1])
        eh += struct.pack('<6i', *rec)
    bodies.append(patchc.Body(ins, bytes(eh)))

# sft: official .14 entries are q3-equivalent and their cctors use anonymous
# storeys from the .14 storey table; R12.25 already carries the equivalent
# q3 sft entries. Keep q's table, do not append .14 cctors.
sft = list(q.sft)

# Fixes: q all + 14 official additions.
fixes = [[m, b] for m, b in q.fixes]
for m, b in new_fixes:
    assert b in bmap, b
    fixes.append([remap_mref(m, mt, me, mf, mi), bmap[b]])

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
merged.new_classes = q.new_classes
# Identify this build; menu strings are in q interns.
for i, s in enumerate(merged.interns):
    if 'R12.25' in s:
        merged.interns[i] = s.replace('R12.25', 'R12.31')

blob = serialize(merged)
assert len(blob) > 0
check = parse_q3(blob, 580)
assert len(check.fixes) == 103, len(check.fixes)
assert len(check.bodies) == 484 + len(order), (len(check.bodies), 484, len(order))

# Pack into the same unencrypted HBSY container used by all working builds.
env = UnityPy.load(CONTAINER)
assets = [o for o in env.objects if o.type.name == 'TextAsset']
assert len(assets) == 1
a = assets[0].read()
a.m_Script = blob.decode('utf-8', 'surrogateescape')
a.save()
bundle = bytes(env.file.save(packer='original'))
packed = gzip.compress(bundle, 9, mtime=0)
Path(OUT).write_bytes(packed)
raw_sha = base64.b64encode(hashlib.sha1(bundle).digest()).decode()
pack_sha = base64.b64encode(hashlib.sha1(packed).digest()).decode()
print('OUTPUT', OUT)
print('bundle', len(bundle), 'packed', len(packed))
print('raw_sha1', raw_sha)
print('pack_sha1', pack_sha)
print('row', f'codepatch/assembly-csharp-patch,{raw_sha},{len(bundle)},0,{pack_sha},{len(packed)},True,0')
print('sha256', hashlib.sha256(packed).hexdigest())
