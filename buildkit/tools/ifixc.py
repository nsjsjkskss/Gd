#!/usr/bin/env python3
"""ifixc — bo build patch IFix cua rieng minh (thay cho viec duc tay bytecode).

Y tuong: lay 1 patch lam "khuon" (co san bang type/extern/field cua game),
roi THEM method cua minh bang mot mini-assembler co nhan (label), hook vao
method game qua bang `fixes`. Tat ca deu kiem tra duoc truoc khi xuat file.

Dung:
    from ifixc import Kit
    k = Kit(open('base.bin','rb').read())          # doc payload tu bundle (da gunzip)
    m = k.method('MyDraw')                          # tao method moi
    m.ldstr('OKMOD').call_extern_void(GUI_LABEL, [rect, text, style]) ...
"""
import struct
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import patchc as P


class Method:
    """Mini-assembler co label: emit tung lenh, label giai quyet sau."""

    def __init__(self, kit, name):
        self.kit = kit
        self.name = name
        # MOI method phai mo dau bang StackSpace (khai bao frame) - neu thieu,
        # runtime IFix se loai ca patch (game mat menu)
        self.code = [[kit.p.op_code('StackSpace'), kit.stack_space]]  # [[opcode, operand]]
        self.labels = {}        # name -> index
        self.patches = []       # (instr_index, label_name)
        self._own_calls = []    # (instr_index, ten method noi bo)

    # ---------- helpers ----------
    def _e(self, opname, operand=0):
        self.code.append([self.kit.p.op_code(opname), operand])
        return self

    def label(self, name):
        self.labels[name] = len(self.code)
        return self

    def _jump(self, opname, label):
        self.code.append([self.kit.p.op_code(opname), 0])
        self.patches.append((len(self.code) - 1, label))
        return self

    # ---------- loads / stores ----------
    def nop(self):            return self._e('Nop')
    def pop(self):            return self._e('Pop')
    def dup(self):            return self._e('Dup')
    def ret(self):            return self._e('Ret')
    def ldarg(self, i):       return self._e('Ldarg', i)
    def ldloc(self, i):       return self._e('Ldloc', i)
    def stloc(self, i):       return self._e('Stloc', i)
    def ldloca(self, i):      return self._e('Ldloca', i)
    def ldsfld(self, i):      return self._e('Ldsfld', i)
    def stsfld(self, i):      return self._e('Stsfld', i)
    def ldfld(self, i):       return self._e('Ldfld', i)
    def stfld(self, i):       return self._e('Stfld', i)
    def ldflda(self, i):      return self._e('Ldflda', i)
    def ldnull(self):         return self._e('Ldnull')
    def ldstr(self, s):
        """s co the la chuoi (tu intern) hoac index intern co san."""
        idx = s if isinstance(s, int) else self.kit.intern(s)
        return self._e('Ldstr', idx)
    def ldi4(self, v):        return self._e('Ldc_I4', v)
    def ldr4(self, v):        return self._e('Ldc_R4', P.f32(v))
    def conv_r4(self):        return self._e('Conv_R4')
    def conv_i4(self):        return self._e('Conv_I4')
    def add(self):            return self._e('Add')
    def sub(self):            return self._e('Sub')
    def mul(self):            return self._e('Mul')
    def div(self):            return self._e('Div')
    def and_(self):           return self._e('And')
    def or_(self):            return self._e('Or')
    def ceq(self):            return self._e('Ceq')
    def box(self, tid):       return self._e('Box', tid)
    def newarr(self, tid):    return self._e('Newarr', tid)
    def ldlen(self):          return self._e('Ldlen')
    def stelem_i1(self):      return self._e('Stelem_I1')
    def ldelem_i1(self):      return self._e('Ldelem_I1')

    # ---------- calls ----------
    def call(self, body_id):
        return self._e('Call', body_id)

    def callext(self, ext_index, nargs):
        """CallExtern voi operand = (nargs<<16)|index (dung nhu patch goc)."""
        return self._e('CallExtern', (nargs << 16) | ext_index)

    def newobj(self, ext_index, nargs=0):
        return self._e('Newobj', (nargs << 16) | ext_index)

    # ---------- branches ----------
    def br(self, label):      return self._jump('Br', label)
    def brtrue(self, label):  return self._jump('Brtrue', label)
    def brfalse(self, label): return self._jump('Brfalse', label)
    def beq(self, label):     return self._jump('Beq', label)
    def bne(self, label):     return self._jump('Bne_Un', label)
    def blt(self, label):     return self._jump('Blt', label)
    def ble(self, label):     return self._jump('Ble', label)
    def bgt(self, label):     return self._jump('Bgt', label)
    def bge(self, label):     return self._jump('Bge', label)

    def finish(self):
        """Giai quyet label -> operand tuong doi, tra ve body."""
        # StackSpace = (so locals << 16) | maxstack  -> phai khop so local THUC TE dung
        p = self.kit.p
        locs = [o for c, o in self.code if p.op_name(c) in ('Ldloc', 'Stloc', 'Ldloca')]
        nloc = (max(locs) + 1) if locs else 0
        if self.code and p.op_name(self.code[0][0]) == 'StackSpace':
            self.code[0][1] = (nloc << 16) | (self.code[0][1] & 0xFFFF)
        for idx, name in self.patches:
            if name not in self.labels:
                raise KeyError('label khong ton tai: ' + name)
            self.code[idx][1] = self.labels[name] - idx
        body = P.Body([[c, o] for c, o in self.code], getattr(self, 'eh', b''))
        return body


class Kit:
    """Bao quanh patchc de them method/field/intern va hook."""

    # gia tri StackSpace cho method moi sinh (cac method nho khong local trong
    # patch goc deu dung 8)
    stack_space = 8

    def __init__(self, payload: bytes):
        self.p = P.Patch(payload)
        self._intern = {s: i for i, s in enumerate(self.p.interns)}

    # ---------- intern / field / extern ----------
    def intern(self, s):
        if s in self._intern:
            return self._intern[s]
        self.p.interns.append(s)
        self._intern[s] = len(self.p.interns) - 1
        return self._intern[s]

    def field(self, tid, name, isnew=False):
        i = self.p.find_field(tid, name)
        if i < 0:
            i = self.p.add_field(tid, name, isnew=isnew)
        return i

    def extern(self, type_prefix, name):
        """Tim extern theo (type prefix, name); them moi neu chua co."""
        for i, mm in enumerate(self.p.extm):
            t = self.p.types[mm.tid] if 0 <= mm.tid < len(self.p.types) else ''
            if mm.name == name and t.split(',')[0].endswith(type_prefix):
                return i
        tid = next((i for i, t in enumerate(self.p.types)
                    if t.split(',')[0].endswith(type_prefix)), -1)
        if tid < 0:
            raise KeyError('khong co type: ' + type_prefix)
        return self.p.add_method_ref(tid, name)

    def type_id(self, full_prefix):
        for i, t in enumerate(self.p.types):
            if t.split(',')[0].endswith(full_prefix):
                return i
        raise KeyError('khong co type: ' + full_prefix)

    # ---------- method ----------
    def method(self, name):
        """Tao method moi: body duoc them cuoi bang, tra ve (Method, body_id)."""
        m = Method(self, name)
        m._body_id = len(self.p.bodies)      # se duoc phat khi commit
        return m

    def commit(self, m: Method):
        self.p.bodies.append(m.finish())
        return len(self.p.bodies) - 1

    def hook(self, game_type, game_method, body_id, nums=()):
        """Gan method cua minh thay cho 1 method game (bang fixes)."""
        tid = self.type_id(game_type)
        mr = P.MRef(False, tid, game_method)
        mr.nums = list(nums)
        self.p.extm.append(mr)
        self.p.fixes.append([mr, body_id])

    def serialize(self):
        return self.p.serialize()
