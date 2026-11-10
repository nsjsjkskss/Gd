#!/usr/bin/env python3
"""cs2patch — dich IL (tu DLL do mcs compile) sang method cua patch IFix.

Pipeline:  C# (.cs) --mcs--> DLL --dnfile/dncil--> IL --(file nay)--> patch body

Pham vi: tap lenh du cho feature cheat (nap/xuat, so hoc, goi ham game,
new object, re nhanh, chuoi, local, mang).
"""
import struct
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / 'pylibs'))
import dnfile
from dncil.cil.body import CilMethodBody
from dncil.cil.body.reader import CilMethodBodyReaderBase
import ifixc


class _Reader(CilMethodBodyReaderBase):
    def __init__(self, data):
        self.data = data
        self.offset = 0

    def read(self, n):
        b = self.data[self.offset:self.offset + n]
        self.offset += n
        return b

    def tell(self):
        return self.offset

    def seek(self, o):
        self.offset = o

    def readat(self, o, n=1):
        v = self.data[o]
        self.offset = o
        return v


# IL opcode -> ten opcode trong patch
OPMAP = {
    'nop': 'Nop', 'ret': 'Ret', 'pop': 'Pop', 'dup': 'Dup',
    'add': 'Add', 'sub': 'Sub', 'mul': 'Mul', 'div': 'Div',
    'and': 'And', 'or': 'Or', 'xor': 'Xor', 'neg': 'Neg',
    'ceq': 'Ceq', 'cgt': 'Cgt', 'clt': 'Clt', 'cgt.un': 'Cgt_Un', 'clt.un': 'Clt_Un',
    'add.ovf': 'Add_Ovf', 'sub.ovf': 'Sub_Ovf', 'mul.ovf': 'Mul_Ovf',
    'conv.i4': 'Conv_I4', 'conv.i8': 'Conv_I8', 'conv.r4': 'Conv_R4',
    'conv.r8': 'Conv_R8', 'conv.u1': 'Conv_U1', 'conv.i1': 'Conv_I1',
    'conv.u2': 'Conv_U2', 'conv.i2': 'Conv_I2', 'conv.u4': 'Conv_U4',
    'conv.u8': 'Conv_U8', 'conv.r.un': 'Conv_R_Un',
    'ldnull': 'Ldnull', 'ldlen': 'Ldlen', 'ldind.i1': 'Ldind_I1',
    'ldind.u1': 'Ldind_U1', 'ldind.i2': 'Ldind_I2', 'ldind.u2': 'Ldind_U2',
    'ldind.i4': 'Ldind_I4', 'ldind.u4': 'Ldind_U4', 'ldind.i8': 'Ldind_I8',
    'ldind.r4': 'Ldind_R4', 'ldind.r8': 'Ldind_R8', 'ldind.ref': 'Ldind_Ref',
    'stind.ref': 'Stind_Ref', 'stind.i1': 'Stind_I1', 'stind.i2': 'Stind_I2',
    'stind.i4': 'Stind_I4', 'stind.i8': 'Stind_I8', 'stind.r4': 'Stind_R4',
    'stind.r8': 'Stind_R8', 'initobj': 'Initobj', 'ldobj': 'Ldobj',
    'stobj': 'Stobj', 'cpobj': 'Cpobj', 'newarr': 'Newarr',
    'ldelema': 'Ldelema', 'ldelem.i1': 'Ldelem_I1', 'ldelem.u1': 'Ldelem_U1',
    'ldelem.i2': 'Ldelem_I2', 'ldelem.u2': 'Ldelem_U2', 'ldelem.i4': 'Ldelem_I4',
    'ldelem.u4': 'Ldelem_U4', 'ldelem.i8': 'Ldelem_I8', 'ldelem.r4': 'Ldelem_R4',
    'ldelem.r8': 'Ldelem_R8', 'ldelem.ref': 'Ldelem_Ref',
    'stelem.i1': 'Stelem_I1', 'stelem.i2': 'Stelem_I2', 'stelem.i4': 'Stelem_I4',
    'stelem.i8': 'Stelem_I8', 'stelem.r4': 'Stelem_R4', 'stelem.r8': 'Stelem_R8',
    'stelem.ref': 'Stelem_Ref', 'stelem': 'Stelem_Any',
    'box': 'Box', 'unbox': 'Unbox', 'unbox.any': 'Unbox_Any',
    'castclass': 'Castclass', 'isinst': 'Isinst', 'sizeof': 'Sizeof',
    'ldtoken': 'Ldtoken', 'switch': 'Switch', 'throw': 'Throw', 'rethrow': 'Rethrow',
    'br': 'Br', 'br.s': 'Br', 'brtrue': 'Brtrue', 'brtrue.s': 'Brtrue',
    'brfalse': 'Brfalse', 'brfalse.s': 'Brfalse',
    'beq': 'Beq', 'beq.s': 'Beq', 'bne.un': 'Bne_Un', 'bne.un.s': 'Bne_Un',
    'bgt': 'Bgt', 'bgt.s': 'Bgt', 'bgt.un': 'Bgt_Un', 'bgt.un.s': 'Bgt_Un',
    'bge': 'Bge', 'bge.s': 'Bge', 'bge.un': 'Bge_Un', 'bge.un.s': 'Bge_Un',
    'blt': 'Blt', 'blt.s': 'Blt', 'blt.un': 'Blt_Un', 'blt.un.s': 'Blt_Un',
    'ble': 'Ble', 'ble.s': 'Ble', 'ble.un': 'Ble_Un', 'ble.un.s': 'Ble_Un',
    'leave': 'Leave', 'leave.s': 'Leave', 'endfinally': 'Endfinally',
    'ldfld': 'Ldfld', 'ldflda': 'Ldflda', 'stfld': 'Stfld',
    'ldsfld': 'Ldsfld', 'ldsflda': 'Ldsflda', 'stsfld': 'Stsfld',
    'ldstr': 'Ldstr', 'call': 'Call', 'callvirt': 'Call', 'newobj': 'Newobj',
    'ldc.i4': 'Ldc_I4', 'ldc.i4.s': 'Ldc_I4', 'ldc.i4.m1': 'Ldc_I4',
    'ldc.r4': 'Ldc_R4', 'ldc.r8': 'Ldc_R8', 'ldc.i8': 'Ldc_I8',
    'ldarg': 'Ldarg', 'ldarga': 'Ldarga', 'starg': 'Starg',
    'ldloc': 'Ldloc', 'ldloca': 'Ldloca', 'stloc': 'Stloc',
    'readonly': 'Readonly', 'constrained': 'Constrained', 'volatile': 'Volatile',
}


def _idx(x):
    """dncil tra ve Local/Parameter object -> lay index."""
    if hasattr(x, 'index'):
        return int(x.index)
    return int(x)


def _expand(op, operand):
    """Chuan hoa opcode IL -> (ten opcode patch, operand)."""
    o = op
    if o.startswith('ldarg.'):
        s = o[6:]
        return 'Ldarg', (int(s) if s.isdigit() else _idx(operand))
    if o.startswith('ldloc.'):
        s = o[6:]
        return 'Ldloc', (int(s) if s.isdigit() else _idx(operand))
    if o.startswith('stloc.'):
        s = o[6:]
        return 'Stloc', (int(s) if s.isdigit() else _idx(operand))
    if o.startswith('ldloca.'):
        s = o[7:]
        return 'Ldloca', (int(s) if s.isdigit() else _idx(operand))
    if o.startswith('ldarga.'):
        s = o[7:]
        return 'Ldarga', (int(s) if s.isdigit() else _idx(operand))
    if o.startswith('starg.'):
        s = o[6:]
        return 'Starg', (int(s) if s.isdigit() else _idx(operand))
    if o == 'ldc.i4.s':
        return 'Ldc_I4', int(operand)
    if o == 'ldc.i4.m1':
        return 'Ldc_I4', -1
    if o.startswith('ldc.i4.'):
        return 'Ldc_I4', int(o[7:])
    if o.endswith('.') and o[:-1] in OPMAP:
        o = o.rstrip('.')
    return OPMAP.get(o), operand


class Conv:
    def __init__(self, base_payload: bytes = None, dll: str = None, kit=None,
                 runtime_lowering=True):
        self.k = kit if kit is not None else ifixc.Kit(base_payload)
        self.p = self.k.p
        self.pe = dnfile.dnPE(dll)
        self.methods = {}          # ten method -> MethodDef row
        self._own_calls = []       # (vi tri lenh Call, ten method)
        self.runtime_lowering = runtime_lowering
        for m in self.pe.net.mdtables.MethodDef.rows:
            if getattr(m, 'Rva', 0):
                self.methods[str(m.Name)] = m

    # ---------- resolve ----------
    def type_id(self, fullname):
        fullname = str(fullname)
        for i, t in enumerate(self.p.types):
            if t.split(',')[0].endswith(fullname):
                return i
        return self.type_index_for(fullname)

    def ext_for(self, type_fullname, mname, nargs=None):
        """Tim extern theo (type, ten, SO tham so).

        Neu da co trong patch (reference) -> tra ve (index, True) de dung nguyen `nums` cua ho
        (chuan xac 100%, tranh sai type nhu TextAnchor/Color).
        """
        type_fullname = str(type_fullname); mname = str(mname)
        tid = self.type_id(type_fullname)
        for i, mm in enumerate(self.p.extm):
            if mm.tid == tid and mm.name == mname and (nargs is None or len(mm.nums) == nargs):
                return i, True
        idx = self.p.add_method_ref(tid, mname)
        return idx, False

    def set_nums(self, ext_idx, token):
        types = self._sig_types(token)
        nums = [self.type_index_for(t) for t in types]
        self.p.extm[ext_idx].nums = nums

    def fld_for(self, type_fullname, fname):
        type_fullname = str(type_fullname); fname = str(fname)
        tid = self.type_id(type_fullname)
        i = self.p.find_field(tid, fname)
        if i < 0:
            # Runtime storage for fields declared only in our compile-time stubs.
            if type_fullname == 'COW.GamePlay.SceneEditBoxSelectTool' and fname.startswith(('OkEsp', 'OkAim', 'OkUi')):
                field_types = {
                    'OkEspBlueLines': 'UnityEngine.LineRenderer[]',
                    'OkEspGrenadeLines': 'UnityEngine.LineRenderer[]',
                    'OkEspLineMaterial': 'UnityEngine.Material',
                    'OkEspEffectFrame': 'System.Int32',
                    'OkAimTarget': 'COW.GamePlay.Player',
                    'OkAimOwner': 'COW.GamePlay.Player',
                    'OkAimMatch': 'COW.GamePlay.JMAGGLCNGIG',
                    'OkAimFrame': 'System.Int32',
                    'OkAimStatus': 'System.Int32',
                    'OkAimSilentFrame': 'System.Int32',
                    'OkAimSilentAppliedFrame': 'System.Int32',
                    'OkAimSilentStatus': 'System.Int32',
                    'OkAimSilentCalls': 'System.Int32',
                    'OkAimSilentWrites': 'System.Int32',
                    'OkAimHit': 'COW.GamePlay.CGKJLKPMGDJ',
                    'OkAimOldObject': 'UnityEngine.GameObject',
                    'OkAimAppliedObject': 'UnityEngine.GameObject',
                    'OkAimOldCollider': 'UnityEngine.Collider',
                    'OkAimAppliedCollider': 'UnityEngine.Collider',
                    'OkAimOldPoint': 'UnityEngine.Vector3',
                    'OkAimOldNormal': 'UnityEngine.Vector3',
                    'OkAimOldDirection': 'UnityEngine.Vector3',
                    'OkAimAppliedPoint': 'UnityEngine.Vector3',
                    'OkAimAppliedDirection': 'UnityEngine.Vector3',
                    'OkAimOldGroup': 'COW.GamePlay.LKOOALMKJND',
                    'OkAimAppliedGroup': 'COW.GamePlay.LKOOALMKJND',
                    'OkAimAppliedIgnore': 'System.Boolean',
                    'OkAimOldIgnore': 'System.Boolean',
                    'OkAimOldBlocked': 'System.Boolean',
                    'OkUiActive': 'System.Boolean',
                    'OkUiStyles': 'UnityEngine.GUIStyle[]',
                    'OkUiTouch': 'System.Boolean',
                    'OkUiMoved': 'System.Boolean',
                    'OkUiFinger': 'System.Int32',
                    'OkUiTouchFrame': 'System.Int32',
                    'OkUiX': 'System.Single',
                    'OkUiY': 'System.Single',
                    'OkUiPhase': 'System.Int32',
                    'OkUiSlider': 'System.Int32',
                    'OkUiScroll': 'System.Single',
                }
                type_index = self.type_index_for(field_types[fname])
                self.p.fields.append(ifixc.P.FieldRec(True, tid, fname, (type_index, -1)))
                i = len(self.p.fields) - 1
            else:
                i = self.p.add_field(tid, fname, isnew=False)
        return i

    @staticmethod
    def _tok2int(token):
        try:
            return int(token)
        except TypeError:
            import re as _re
            m2 = _re.search(r'0x([0-9A-Fa-f]+)', repr(token))
            if not m2:
                raise KeyError('token khong doc duoc: %r' % (token,))
            return int(m2.group(1), 16)

    def resolve_member(self, token):
        """token -> (kind, info) voi kind = 'method'|'field'|'string'|'type'"""
        tok = self._tok2int(token)
        table = tok >> 24
        rid = tok & 0xFFFFFF
        if table == 0x70:      # UserString
            row = self.pe.net.user_strings.get(rid)
            v = getattr(row, 'value', None)
            if v is None:
                v = str(row)
            return 'string', str(v)
        if table == 0x0A:      # MemberRef -> method
            row = self.pe.net.mdtables.MemberRef.rows[rid - 1]
            owner = self._owner_name(row.Class)
            return 'method', (owner, row.Name)
        if table == 0x06:      # MethodDef -> method cua chinh minh
            row = self.pe.net.mdtables.MethodDef.rows[rid - 1]
            return 'ownmethod', str(row.Name)
        if table == 0x04:      # Field
            row = self.pe.net.mdtables.Field.rows[rid - 1]
            return 'field', (None, row.Name)
        if table == 0x0A or table == 0x01:
            return 'raw', tok
        raise KeyError('token chua ho tro: %#x' % tok)

    def _owner_name(self, tabled):
        try:
            tname = str(getattr(tabled.table, 'name', '') or '')
            if tname in ('TypeRef', 'TypeDef'):
                ns = str(getattr(tabled.row, 'TypeNamespace', '') or '')
                nm = str(getattr(tabled.row, 'TypeName', '') or '')
                return (ns + '.' + nm).lstrip('.')
            return tname
        except Exception:
            return str(getattr(tabled.table, 'name', '?'))

    # ---------- convert 1 method ----------
    def _local_specs(self, body):
        token = getattr(body, 'local_var_sig_tok', None)
        if token is None:
            return []
        value = self._tok2int(token)
        if value == 0:
            return []
        if value >> 24 != 0x11:
            raise ValueError('Invalid local signature token')
        sig = bytes(self.pe.net.mdtables.StandAloneSig.rows[(value & 0xffffff) - 1].Signature.value)
        if not sig or sig[0] != 0x07:
            raise ValueError('Invalid local signature calling convention')
        count, offset = self._comp(sig, 1)
        result = []
        for _ in range(count):
            value_type = sig[offset] == 0x11
            name, offset = self._sig_type(sig, offset)
            result.append((name, value_type))
        if offset != len(sig):
            raise ValueError('Unsupported trailing local signature data')
        return result

    def convert(self, name):
        mdef = self.methods[name]
        body = CilMethodBody(_Reader(self.pe.get_data(mdef.Rva, 65536)))
        m = self.k.method(name)
        sig = bytes(mdef.Signature.value)
        si = 1
        if sig[0] & 0x10:
            _, si = self._comp(sig, si)
        _, si = self._comp(sig, si)
        returns_value = sig[si] != 0x01
        local_specs = self._local_specs(body)
        nl = len(local_specs)
        m.code[0][1] = (nl << 16) | max(8, body.max_stack)
        lowering = getattr(self, 'runtime_lowering', True)
        if not lowering and body.exception_handlers:
            raise ValueError('Exception clauses require runtime lowering; refusing to drop them')
        if lowering:
            # InjectFix's translator initializes nonprimitive struct locals even
            # when CIL contains no explicit Initobj (CodeTranslator.cs:1230-1261).
            for slot, (local_type, value_type) in enumerate(local_specs):
                if value_type:
                    m._e('Ldloca', slot)
                    m._e('Initobj', self.type_index_for(local_type))
        # nhan cho cac diem nhay
        def is_branch(opname):
            return (opname.startswith('br') or opname.startswith('beq')
                    or opname.startswith('bne') or opname.startswith('bgt')
                    or opname.startswith('bge') or opname.startswith('blt')
                    or opname.startswith('ble') or opname in ('leave', 'leave.s'))
        targets = set()
        for ins in body.instructions:
            o = getattr(ins.opcode, 'name', str(ins.opcode))
            if is_branch(o):
                targets.add(int(ins.operand))
        off2idx = {}
        raw = []
        for ins in body.instructions:
            o = getattr(ins.opcode, 'name', str(ins.opcode))
            patch_op, operand = _expand(o, getattr(ins, 'operand', None))
            raw.append([o, patch_op, operand, ins.offset])
        cursor = len(m.code)
        for o, patch_op, operand, off in raw:
            off2idx[off] = cursor
            if patch_op == 'Switch':
                raise ValueError('Switch lowering is not implemented; refusing invalid bytecode')
            cursor += 0 if patch_op == 'Constrained' else (2 if patch_op == 'Leave' else 1)
        end_offset = body.header_size + body.code_size
        off2idx[end_offset] = cursor
        handlers = []
        for handler in body.exception_handlers:
            kind = int(handler.exception_type)
            if kind not in (0, 2):
                raise ValueError('Only catch/finally supported; filter/fault require separate lowering')
            catch_id = self.type_from_token(handler.catch_type) if kind == 0 else -1
            handlers.append([
                kind, catch_id,
                off2idx[body.header_size + handler.try_start],
                off2idx[body.header_size + handler.try_end],
                off2idx[body.header_size + handler.handler_start],
                off2idx[body.header_size + handler.handler_end],
            ])

        def enclosing_finally(position):
            matches = [(i, h) for i, h in enumerate(handlers)
                       if h[0] == 2 and h[2] <= position < h[3]]
            return min(matches, key=lambda pair: pair[1][3] - pair[1][2]) if matches else (-1, None)

        # emit
        constrained_owner = None
        for i, (o, patch_op, operand, off) in enumerate(raw):
            if len(m.code) != off2idx[off]:
                raise ValueError('Instruction layout mismatch at IL offset %d' % off)
            if off in targets:
                m.label('#off%d' % off)   # label phai dang ky NGAY LUC emit toi lenh nay
            if patch_op is None:
                raise KeyError('opcode chua map: ' + o)
            if patch_op == 'Constrained':
                constrained_owner = self._name_from_token(self._tok2int(operand))
                continue
            if patch_op == 'Leave':
                target = off2idx[int(operand)]
                _, handler = enclosing_finally(len(m.code))
                if handler is None or handler[2] <= target < handler[3]:
                    m._e('Br', target - len(m.code))
                    m.nop()
                else:
                    # IFix Leave stores an absolute destination, then executes
                    # the next instruction; explicitly branch to the finally.
                    m._e('Leave', target)
                    m._e('Br', handler[4] - len(m.code))
            elif patch_op == 'Endfinally':
                index, _ = enclosing_finally(len(m.code))
                m._e('Endfinally', index)
            elif is_branch(o):
                m._jump(patch_op, '#off%d' % int(operand))
            elif o == 'ldstr':
                kind, s = self.resolve_member(operand)
                m.ldstr(s)
            elif o == 'ret':
                m._e('Ret', 1 if returns_value else 0)
            elif o in ('call', 'callvirt'):
                kind, info = self.resolve_member(operand)
                if kind == 'ownmethod':
                    m._own_calls.append((len(m.code), str(info)))
                    n, has_this = self._sig_args(operand)
                    m.call((n + (1 if has_this else 0)) << 16)
                else:
                    owner, mname = info
                    if owner == 'System.Type' and str(mname) == 'GetTypeFromHandle':
                        # IFix Ldtype already pushes System.Type, not RuntimeTypeHandle.
                        m.nop()
                        continue
                    if constrained_owner is not None:
                        owner = constrained_owner
                        constrained_owner = None
                    n, has_this = self._sig_args(operand)
                    idx, existed = self.ext_for(owner, mname, n)
                    if not existed:
                        self.set_nums(idx, operand)
                    m.callext(idx, n + (1 if has_this else 0))
            elif o == 'newobj':
                kind, info = self.resolve_member(operand)
                owner, mname = info
                n, _ht = self._sig_args(operand)
                idx, existed = self.ext_for(owner, '.ctor', n)
                if not existed:
                    self.set_nums(idx, operand)
                m.newobj(idx, n)
            elif o in ('ldfld', 'stfld', 'ldsfld', 'stsfld', 'ldflda', 'ldsflda'):
                kind, info = self.resolve_member(operand)
                owner, fname = info
                if owner is None:
                    m._e(patch_op, 0)
                else:
                    m._e(patch_op, self.fld_for(owner, fname))
            elif patch_op in ('Ldc_I4', 'Ldc_I8'):
                m._e(patch_op, int(operand))
            elif patch_op == 'Ldc_R4':
                m.ldr4(float(operand))
            elif patch_op == 'Ldc_R8':
                import struct as _s
                m._e('Ldc_R8', _s.unpack('<Q', _s.pack('<d', float(operand)))[0])
            elif patch_op == 'Ldc_R4':
                m.ldr4(float(operand))
            elif patch_op in ('Ldarg', 'Ldloc', 'Stloc', 'Ldarga', 'Ldloca', 'Starg'):
                m._e(patch_op, _idx(operand))
            elif patch_op == 'Ldtoken':
                tok = self._tok2int(operand)
                if tok >> 24 not in (0x01, 0x02):
                    raise KeyError('ldtoken chi ho tro typeof: %#x' % tok)
                m._e('Ldtype', self.type_from_token(operand))
            elif patch_op in ('Box', 'Unbox', 'Unbox_Any', 'Initobj',
                              'Newarr', 'Castclass', 'Isinst', 'Sizeof', 'Ldobj', 'Stobj',
                              'Cpobj', 'Ldelema', 'Mkrefany', 'Refanyval', 'Constrained'):
                # cac lenh nay nhan TOKEN TYPE -> doi sang index type trong patch
                m._e(patch_op, self.type_from_token(operand))
            else:
                m._e(patch_op, int(operand) if isinstance(operand, int) else 0)
        if len(m.code) != cursor:
            raise ValueError('Final instruction layout mismatch')
        m.eh = b''.join(struct.pack('<6i', *handler) for handler in handlers)
        return m

    def convert_all(self):
        """Dich tat ca method cua minh, roi va operand Call noi bo (two-pass)."""
        names = [n for n in self.methods if n != '.ctor']
        out = {}
        own = {}
        for n in names:
            m = self.convert(n)
            own[n] = m
            out[n] = m
        # pass 2: commit de biet body id, roi va operand
        ids = {}
        for n, m in out.items():
            ids[n] = self.k.commit(m)
        fixed = 0
        for n, m in out.items():
            for idx, (pos, target) in enumerate(m._own_calls):
                if target in ids:
                    nargs = self.p.bodies[ids[n]].instrs[pos][1] & 0xFFFF0000
                    self.p.set_operand(ids[n], pos, nargs | ids[target])
                    fixed += 1
        print(f'[cs2patch] dich {len(out)} method | va {fixed} lenh Call noi bo')
        return ids

    def _sig_args(self, token):
        """Dem so tham so cua MemberRef tu signature (kind: 'method'|'field')."""
        tok = self._tok2int(token); rid = tok & 0xFFFFFF
        tbl = self.pe.net.mdtables
        try:
            if (tok >> 24) == 0x0A:
                row = tbl.MemberRef.rows[rid - 1]
            elif (tok >> 24) == 0x06:
                row = tbl.MethodDef.rows[rid - 1]
            else:
                return 0, False
            sg = row.Signature
            try:
                sig = bytes(sg.value) if hasattr(sg, 'value') else bytes(sg or b'')
            except Exception:
                sig = b''
        except Exception:
            return 0, False
        if not sig:
            return 0, False
        cc = sig[0]
        has_this = bool(cc & 0x20)
        i = 1
        # generic param count (compressed) neu co
        if cc & 0x10:
            _, i = self._comp(sig, i)
        nparams, i = self._comp(sig, i)
        return nparams, has_this

    def _sig_types(self, token):
        """Tra ve danh sach ten type tham so (khong ke 'this')."""
        tok = self._tok2int(token); rid = tok & 0xFFFFFF
        tbl = self.pe.net.mdtables
        try:
            row = tbl.MemberRef.rows[rid - 1] if (tok >> 24) == 0x0A else tbl.MethodDef.rows[rid - 1]
            sg = row.Signature
            b = bytes(sg.value) if hasattr(sg, 'value') else bytes(sg or b'')
        except Exception:
            return []
        if not b:
            return []
        i = 1
        if b[0] & 0x10:
            _, i = self._comp(b, i)
        n, i = self._comp(b, i)
        _, i = self._sig_type(b, i)
        out = []
        for _ in range(n):
            name, i = self._sig_type(b, i)
            out.append(name)
        return out

    def _sig_type(self, b, i):
        elem = {0x01: 'System.Void', 0x02: 'System.Boolean', 0x03: 'System.Char',
                0x04: 'System.SByte', 0x05: 'System.Byte', 0x06: 'System.Int16',
                0x07: 'System.UInt16', 0x08: 'System.Int32', 0x09: 'System.UInt32',
                0x0A: 'System.Int64', 0x0B: 'System.UInt64', 0x0C: 'System.Single',
                0x0D: 'System.Double', 0x0E: 'System.String', 0x18: 'System.IntPtr',
                0x19: 'System.UIntPtr', 0x1C: 'System.Object'}
        et = b[i]; i += 1
        if et in elem:
            return elem[et], i
        if et in (0x11, 0x12):
            coded, i = self._comp(b, i)
            table = {0: 0x02, 1: 0x01, 2: 0x1B}[coded & 3]
            return self._name_from_token((table << 24) | (coded >> 2)), i
        if et in (0x0F, 0x10, 0x1D):
            name, i = self._sig_type(b, i)
            return name + {0x0F: '*', 0x10: '&', 0x1D: '[]'}[et], i
        if et == 0x15:  # GENERICINST, e.g. List<Collider> getter return type.
            name, i = self._sig_type(b, i)
            count, i = self._comp(b, i)
            args = []
            for _ in range(count):
                arg, i = self._sig_type(b, i)
                args.append(arg)
            return name + '[' + ','.join(args) + ']', i
        raise ValueError('unsupported signature element %#x' % et)

    def type_from_token(self, token):
        """Token type (TypeRef/TypeDef) -> index type trong patch."""
        try:
            tok = self._tok2int(token)
        except Exception:
            return 0
        t = tok >> 24
        rid = tok & 0xFFFFFF
        try:
            tbl = self.pe.net.mdtables
            if t == 0x01:
                r = tbl.TypeRef.rows[rid - 1]
            elif t == 0x02:
                r = tbl.TypeDef.rows[rid - 1]
            else:
                return 0
            ns = str(getattr(r, 'TypeNamespace', '') or '')
            nm = str(getattr(r, 'TypeName', '') or '')
            return self.type_index_for((ns + '.' + nm).lstrip('.'))
        except Exception:
            return 0

    def _name_from_token(self, tok):
        tbl = self.pe.net.mdtables
        t = tok >> 24; rid = tok & 0xFFFFFF
        try:
            if t == 0x01:
                r = tbl.TypeRef.rows[rid - 1]
            elif t == 0x02:
                r = tbl.TypeDef.rows[rid - 1]
            else:
                return 'System.Object'
            ns = str(getattr(r, 'TypeNamespace', '') or '')
            nm = str(getattr(r, 'TypeName', '') or '')
            return (ns + '.' + nm).lstrip('.')
        except Exception:
            return 'System.Object'

    def type_index_for(self, name):
        """Tim/tao type index trong patch theo ten."""
        for i, t in enumerate(self.p.types):
            if t.split(',')[0].endswith(name):
                return i
        # Array types belong to the element's assembly, not a guessed default.
        if name.endswith('[]'):
            element_index = self.type_index_for(name[:-2])
            element, assembly = self.p.types[element_index].split(',', 1)
            self.p.types.append(element + '[],' + assembly)
            return len(self.p.types) - 1
        if name.startswith('System.'):
            full = name + ', mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
        elif name.startswith('UnityEngine.'):
            module = 'UnityEngine.CoreModule'
            if name in ('UnityEngine.Input', 'UnityEngine.Touch', 'UnityEngine.TouchPhase'):
                module = 'UnityEngine.InputLegacyModule'
            elif name in ('UnityEngine.GUI', 'UnityEngine.GUIStyle', 'UnityEngine.GUIStyleState',
                          'UnityEngine.GUISkin', 'UnityEngine.Event'):
                module = 'UnityEngine.IMGUIModule'
            elif name in ('UnityEngine.Networking.UnityWebRequest',
                          'UnityEngine.Networking.UnityWebRequestAsyncOperation'):
                # Verified against the current global-metadata image table:
                # image 24 -> UnityEngine.UnityWebRequestModule.dll. Naming the
                # wrong module here is exactly what made R11.90 reject the patch.
                module = 'UnityEngine.UnityWebRequestModule'
            full = name + ', ' + module + ', Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
        else:
            full = name + ', Assembly-CSharp, Version=0.86.0.518, Culture=neutral, PublicKeyToken=null'
        self.p.types.append(full)
        return len(self.p.types) - 1

    @staticmethod
    def _comp(b, i):
        # ECMA-335 compressed unsigned integer (big-endian, not LEB128).
        x = b[i]; i += 1
        if x < 0x80:
            return x, i
        if x < 0xC0:
            return ((x & 0x3F) << 8) | b[i], i + 1
        if x < 0xE0:
            return ((x & 0x1F) << 24) | int.from_bytes(b[i:i + 3], 'big'), i + 3
        raise ValueError('invalid compressed signature integer')
