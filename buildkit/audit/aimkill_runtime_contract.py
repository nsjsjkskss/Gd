#!/usr/bin/env python3
"""Resolve IFix contracts from metadata names + native registration, not dump aliases."""
import hashlib
import json
import struct
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


class RuntimeContract:
    def __init__(self, binary=Path('/tmp/opencode/ff-ipa-fresh/UnityFramework')):
        self.binary = binary
        self.native = binary.read_bytes()
        self.metadata = (ROOT / 'buildkit/audit/global-metadata.dat').read_bytes()
        classes = json.loads((ROOT / 'docs/aimkill-audit-bundle/dump/ffmeta_classes.json').read_text())
        self.classes = {c['name']: c for c in classes}
        self.names = {c['tix']: c['name'] for c in classes}
        # Verified v31.1 Mach-O registration. Validate anchors on every build.
        self.types = 0xb4719a8
        self.field_offsets = 0xb7eaca8
        for index, expected in ((25633, 'COW.GamePlay.CGKJLKPMGDJ'),
                                (37478, 'COW.GamePlay.HNLIMBLIANL'),
                                (46548, 'COW.GamePlay.PIAEKELILCJ'),
                                (71887, 'UnityEngine.Vector3'),
                                (13962, 'System.Collections.Generic.List`1[System.Single]')):
            assert self.type_name(index) == expected, (index, expected)
        self.strings, = struct.unpack_from('<I', self.metadata, 24)
        self.methods, = struct.unpack_from('<I', self.metadata, 48)
        self.parameters, = struct.unpack_from('<I', self.metadata, 88)
        self.fields, = struct.unpack_from('<I', self.metadata, 96)

    def string(self, index):
        start = self.strings + index
        return self.metadata[start:self.metadata.index(b'\0', start)].decode()

    def pointer_type(self, pointer):
        data, bits = struct.unpack_from('<QI', self.native, pointer)
        kind = (bits >> 16) & 255
        if kind == 0x15:
            definition, inst = struct.unpack_from('<QQ', self.native, data)
            count, args = struct.unpack_from('<QQ', self.native, inst)
            assert 0 < count < 32
            return self.pointer_type(definition) + '[' + ','.join(
                self.pointer_type(struct.unpack_from('<Q', self.native, args + i * 8)[0])
                for i in range(count)) + ']'
        if kind in (0x1d, 0x0f):
            return self.pointer_type(data) + ('[]' if kind == 0x1d else '*')
        if kind in (0x13, 0x1e):
            return ('!' if kind == 0x13 else '!!') + str(data)
        assert data in self.names, ('unknown registration type', hex(pointer), hex(data), kind)
        return self.names[data]

    def type_name(self, index):
        pointer, = struct.unpack_from('<Q', self.native, self.types + index * 8)
        return self.pointer_type(pointer)

    def method(self, owner, name, arity):
        cls = self.classes[owner]
        matches = []
        for index in range(cls['ms'], cls['ms'] + cls['mc']):
            row = struct.unpack_from('<8I4H', self.metadata, self.methods + index * 40)
            if self.string(row[0]) != name or row[-1] != arity:
                continue
            params = []
            for i in range(arity):
                _, _, ti = struct.unpack_from('<III', self.metadata, self.parameters + (row[4] + i) * 12)
                params.append(self.type_name(ti))
            matches.append({'owner': owner, 'name': name, 'index': index,
                            'return': self.type_name(row[2]), 'params': params,
                            'static': bool(row[8] & 0x10)})
        assert len(matches) == 1, ('ambiguous/missing contract', owner, name, arity)
        return matches[0]

    def field(self, owner, name):
        cls = self.classes[owner]
        offsets, = struct.unpack_from('<Q', self.native, self.field_offsets + cls['tix'] * 8)
        for i in range(cls['fc']):
            ni, ti, _ = struct.unpack_from('<III', self.metadata, self.fields + (cls['fs'] + i) * 12)
            if self.string(ni) == name:
                offset, = struct.unpack_from('<i', self.native, offsets + i * 4)
                return {'owner': owner, 'name': name, 'type': self.type_name(ti), 'offset': offset}
        raise AssertionError(('missing field', owner, name))


CHAIN_METHODS = [
    ('COW.GamePlay.Player', 'JGIEOGCGFEP', 1),
    ('COW.GamePlay.OGKFEMJNCGM', 'get_gameObject', 0),
    ('COW.GamePlay.PlayerAttributes', 'IsInvincible', 0),
    ('COW.GamePlay.HBIBDMMOOOK', 'KBIFKMJODNA', 0),
    ('COW.GamePlay.HBIBDMMOOOK', 'NPLMKLBKJPE', 4),
    ('COW.GamePlay.IEFJGCGOMID', 'MJCENIGKEPH', 0),
    ('COW.GamePlay.IJHHJLDNMGH', 'GetId', 0),
    ('COW.GamePlay.PlayerNetwork', 'TakeDamage', 9),
    ('COW.GamePlay.HNLIMBLIANL', '.ctor', 0),
    ('COW.GamePlay.PIAEKELILCJ', '.ctor', 0),
]

ENGINE_METHODS = [
    ('message.EOBFCLNNIJH', '.ctor', 0),
    ('COW.GameFacade', 'Send', 4),
    ('COW.GamePlay.Player', 'StartFiring', 1),
    ('COW.GamePlay.Player', 'StopFire', 1),
    ('COW.GamePlay.Player', 'SetAimRotation', 2),
    ('COW.GamePlay.Player', 'GetAimRotation', 0),
    ('COW.GamePlay.HBIBDMMOOOK', 'LHNJLFOINDO', 1),
    ('COW.GamePlay.HBIBDMMOOOK', 'LDKCJEMDAMC', 0),
    ('COW.GamePlay.HBIBDMMOOOK', 'BDHMJJIADFG', 0),
    ('COW.GamePlay.HBIBDMMOOOK', 'KENANOGLPJO', 0),
    ('COW.GamePlay.HBIBDMMOOOK', 'KBIFKMJODNA', 0),
    ('COW.GamePlay.IEFJGCGOMID', 'MJCENIGKEPH', 0),
    ('COW.GamePlay.PlayerAttributes', 'IsInvincible', 0),
]


def check_compiled(converter, runtime):
    """Verify compiled MemberRefs (including field/return types) against registration."""
    relevant = {owner for owner, _, _ in CHAIN_METHODS + ENGINE_METHODS}
    relevant.add('COW.GamePlay.IOKEALMHBKN')
    for rid, ref in enumerate(converter.pe.net.mdtables.MemberRef.rows, 1):
        owner = converter._owner_name(ref.Class)
        name = str(ref.Name)
        if owner not in relevant:
            continue
        sig = bytes(ref.Signature.value)
        if sig[0] == 0x06:
            if owner not in ('COW.GamePlay.HNLIMBLIANL', 'COW.GamePlay.PIAEKELILCJ',
                             'COW.GamePlay.IOKEALMHBKN', 'message.EOBFCLNNIJH') and name not in ('PIILKNEHNLB', 'ODFDGMOCJLL'):
                continue
            actual, end = converter._sig_type(sig, 1)
            assert end == len(sig)
            expected = runtime.field(owner, name)['type']
            assert actual == expected, ('compiled field mismatch', owner, name, actual, expected)
        else:
            token = 0x0a000000 | rid
            arity, has_this = converter._sig_args(token)
            if (owner, name, arity) not in CHAIN_METHODS + ENGINE_METHODS:
                continue
            contract = runtime.method(owner, name, arity)
            i = 1
            if sig[0] & 0x10:
                _, i = converter._comp(sig, i)
            _, i = converter._comp(sig, i)
            ret, _ = converter._sig_type(sig, i)
            assert ret == contract['return'], ('return type mismatch', owner, name, ret, contract['return'])
            assert converter._sig_types(token) == contract['params'], ('compiled parameters mismatch', owner, name)
            assert has_this != contract['static'], ('static mismatch', owner, name)


def check_patch(candidate, runtime, specs=None):
    """Exact parameter signatures and every new chain field, including enum/uint."""
    methods = []
    for owner, name, arity in (CHAIN_METHODS if specs is None else specs):
        contract = runtime.method(owner, name, arity)
        refs = [ref for ref in candidate.extm
                if candidate.types[ref.tid].split(',')[0] == owner and ref.name == name
                and len(ref.nums) == arity]
        assert len(refs) == 1, ('chain extern missing', owner, name)
        actual = [canonical_type(candidate.types[i]) for i in refs[0].nums]
        assert actual == contract['params'], (owner, name, actual, contract['params'])
        methods.append(contract)
    fields = []
    owners = ('COW.GamePlay.HNLIMBLIANL', 'COW.GamePlay.PIAEKELILCJ', 'COW.GamePlay.IOKEALMHBKN', 'message.EOBFCLNNIJH')
    for field in candidate.fields:
        owner = candidate.types[field.tid].split(',')[0]
        if owner in owners or (owner == 'COW.GamePlay.HBIBDMMOOOK' and field.name == 'PIILKNEHNLB') or (owner == 'COW.GamePlay.IEFJGCGOMID' and field.name == 'ODFDGMOCJLL'):
            assert not field.isnew
            fields.append(runtime.field(owner, field.name))
    if specs is None:
        assert fields
    return {'methods': methods, 'fields': fields,
            'native_sha256': hashlib.sha256(runtime.native).hexdigest(),
            'metadata_sha256': hashlib.sha256(runtime.metadata).hexdigest()}


def canonical_type(full):
    # Parse only the constructed List<float> used by the chain; reject surprises.
    if full.startswith('System.Collections.Generic.List`1[['):
        assert full.startswith('System.Collections.Generic.List`1[[System.Single, mscorlib,')
        return 'System.Collections.Generic.List`1[System.Single]'
    return full.split(',')[0]


if __name__ == '__main__':
    r = RuntimeContract()
    print(json.dumps([r.method(*spec) for spec in CHAIN_METHODS], indent=2))
