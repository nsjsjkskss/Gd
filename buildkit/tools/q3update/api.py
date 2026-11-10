"""Dump-backed dependency gates, including exact overload and hook ID checks."""
import re
from pathlib import Path

from .format import FIELD_OPS, TYPE_OPS, InvalidPatch, body_indices, closure, digest, require


def typename(full):
    # Assembly-qualified generic names contain commas inside [[...]].
    depth = 0
    for i, c in enumerate(full):
        if c == '[': depth += 1
        elif c == ']': depth -= 1
        elif c == ',' and depth == 0: return full[:i]
    return full


def short(full):
    name = typename(full)
    return name.rsplit('.', 1)[-1]


ALIASES = {'void': 'Void', 'bool': 'Boolean', 'int': 'Int32', 'long': 'Int64',
           'float': 'Single', 'double': 'Double', 'string': 'String', 'object': 'Object',
           'byte': 'Byte', 'short': 'Int16', 'uint': 'UInt32', 'ulong': 'UInt64'}


def split_params(args):
    depth, start = 0, 0
    for i, char in enumerate(args):
        if char in '[<': depth += 1
        elif char in ']>': depth -= 1
        elif char == ',' and depth == 0:
            yield args[start:i]
            start = i + 1
    if args:
        yield args[start:]


class ApiDump:
    def __init__(self, path):
        raw = Path(path).read_bytes()
        self.sha256 = digest(raw)
        self.types, self.methods, self.fields, self.ids, self.opcodes = set(), {}, {}, {}, {}
        namespace, owner = '', ''
        for line in raw.decode().splitlines():
            if line.startswith('// Namespace:'):
                namespace = line.partition(':')[2].strip(); owner = ''
            decl = re.match(r'^(?:public|private|internal|protected) (?:(?:static|sealed|abstract|readonly) )*(?:class|struct|interface|enum) ([^ :]+)', line)
            if decl:
                owner = (namespace + '.' + decl.group(1)).lstrip('.')
                self.types.add(owner)
            if not owner:
                continue
            ident = re.search(r'public const IDMAP\d+ (\S+) = (\d+);', line)
            if ident: self.ids[ident.group(1)] = int(ident.group(2))
            if owner == 'IFix.Core.Code':
                op = re.search(r'public const Code (\w+) = (\d+);', line)
                if op: self.opcodes[op.group(1)] = int(op.group(2))
            method = re.match(r'\s+(?:public|private|protected|internal)\s+(.+?)\s+([\w.<>]+)\(([^)]*)\) \{ \}', line)
            if method:
                prefix, name, args = method.groups()
                params = []
                for arg in split_params(args):
                    words = arg.strip().split()
                    require(len(words) >= 2, 'unrecognized dump parameter: ' + arg)
                    typ = words[-2]
                    if words[0] in ('ref', 'out', 'in'): typ += '&'
                    params.append(ALIASES.get(typ, typ))
                self.methods.setdefault((owner, name, tuple(params)), []).append({
                    'static': 'static' in prefix.split(), 'generic_return': prefix.split()[-1] == 'T'})
            field = re.match(r'\s+(?:public|private|internal|protected)\s+(?:(?:static|readonly)\s+)*(\S+) (\S+); // 0x', line)
            if field:
                self.fields[(owner, field.group(2))] = field.group(1)

    def require_type(self, full):
        name = typename(full)
        while name.endswith(('[]', '&')):
            name = name[:-2] if name.endswith('[]') else name[:-1]
        # Constructed generic owners use the definition in the dump. Concrete
        # generic arguments are separately included in the donor type table.
        definition = name.split('[[', 1)[0]
        require(definition in self.types, 'type absent in game dump: ' + definition)

    def method(self, p, ref):
        owner = typename(p.types[ref.tid]).split('[[', 1)[0]
        params = tuple(short(p.types[i]) for i in ref.nums)
        if ref.isg:
            params = tuple(v if k == 's' else short(p.types[v]) for k, v in ref.items)
            require(not params and len(ref.ga) == 1, 'unsupported generic method proof')
        key = (owner, ref.name, params)
        matches = self.methods.get(key, [])
        require(len(matches) == 1, 'missing/ambiguous method signature: ' + repr(key))
        require(not ref.isg or matches[0]['generic_return'], 'generic contract changed: ' + repr(key))
        return matches[0]

    def hook(self, p, ref):
        self.method(p, ref)
        owner = typename(p.types[ref.tid])
        stem = owner.replace('.', '-') + '-' + ref.name
        matches = [(name, value) for name, value in self.ids.items()
                   if name.startswith(stem) and name[len(stem):].isdigit()]
        require(len(matches) == 1, 'ambiguous/missing hook IDMAP: ' + stem)
        return {'owner': owner, 'method': ref.name, 'idmap': matches[0][0], 'slot': matches[0][1]}


def check(p, hooks, api):
    bodies = closure(p, [bid for _, bid in hooks])
    refs, fields, types, ops = set(), set(), set(), set()
    for bid in bodies:
        for _, name, value in body_indices(p, bid):
            ops.add(name)
            if name in ('CallExtern', 'Newobj'): refs.add(value & 65535)
            elif name in ('Ldftn', 'Ldvirtftn'): refs.add(value)
            elif name in FIELD_OPS: fields.add(value)
            elif name in TYPE_OPS and value >= 0: types.add(value)
        import struct
        for at in range(0, len(p.bodies[bid].eh), 24):
            rec = struct.unpack_from('<6i', p.bodies[bid].eh, at)
            if rec[0] == 0 and rec[1] >= 0: types.add(rec[1])
    for i in refs:
        ref = p.extm[i]
        api.method(p, ref)
        types.update([ref.tid] + ref.nums + ref.ga + [v for k, v in ref.items if k == 'i'])
    for i in fields:
        f = p.fields[i]; types.add(f.tid)
        if f.isnew:
            types.add(f.extra[0])
            require((typename(p.types[f.tid]), f.name) not in api.fields,
                    'runtime field now collides with real game field: ' + f.name)
        else:
            actual = api.fields.get((typename(p.types[f.tid]), f.name))
            require(actual is not None, 'field missing: ' + f.name)
    for i in types:
        api.require_type(p.types[i])
    for op in ops:
        require(api.opcodes.get(op) == p.op_code(op), 'IFix opcode map changed: ' + op)
    for bid in bodies:
        for _, name, value in body_indices(p, bid):
            if name not in ('CallExtern', 'Newobj'): continue
            ref = p.extm[value & 65535]
            contract = api.method(p, ref)
            param_count = len(ref.items) if ref.isg else len(ref.nums)
            expected = param_count + (0 if name == 'Newobj' or contract['static'] else 1)
            require(value >> 16 == expected, 'argument/static ABI changed: ' + ref.name)
    hooks_report = [api.hook(p, ref) for ref, _ in hooks]
    require(len({h['slot'] for h in hooks_report}) == len(hooks_report), 'hook IDMAP slot collision')
    return {'bodies': len(bodies), 'external_signatures': len(refs), 'fields': len(fields),
            'types': len(types), 'opcodes': len(ops), 'hooks': hooks_report,
            'limitation': 'dump signature/IDMAP validation; native AOT invocation and device runtime remain untested'}
