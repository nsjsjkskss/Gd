#!/usr/bin/env python3
"""patchc — Patch Compiler (writer + editor + mini-assembler) cho FF IFix .bytes patches.
Round-trip: parse -> serialize phải trả về byte-identical.
"""
import os
import struct

MAGIC = bytes.fromhex('cba2b40db219a366')
# IFix.Core.Code from /root/ffdump/com.dts.freefiremax-dump.cs.
MAP_FILE = os.path.join(os.path.dirname(__file__), 'ifix-opcode-map.txt')

def load_opcodes(path=MAP_FILE):
    idx2name, name2idx = {}, {}
    for line in open(path, encoding='utf-8'):
        pp = line.strip().split()
        if len(pp) >= 2 and pp[0].lstrip('-').isdigit():
            nm = pp[1]
            if '//' in nm: nm = nm.split('//')[0].strip()
            nm = nm.rstrip(',')
            idx2name[int(pp[0])] = nm
            name2idx.setdefault(nm, int(pp[0]))
    return idx2name, name2idx

class R:
    def __init__(s, d, p=0): s.d=d; s.p=p
    def u8(s): v=s.d[s.p]; s.p+=1; return v
    def i32(s):
        v=struct.unpack_from('<i', s.d, s.p)[0]; s.p+=4; return v
    def u32(s):
        v=struct.unpack_from('<I', s.d, s.p)[0]; s.p+=4; return v
    def u64(s):
        v=struct.unpack_from('<Q', s.d, s.p)[0]; s.p+=8; return v
    def bo(s): return s.u8()!=0
    def st(s):
        b=s.u8(); n=b&0x7f; sh=7
        while b&0x80:
            b=s.u8(); n|=(b&0x7f)<<sh; sh+=7
        v=s.d[s.p:s.p+n].decode('utf-8'); s.p+=n; return v
    def skip(s,n): s.p+=n

def wv(o, n):
    while True:
        b=n&0x7f; n>>=7
        if n: o.append(b|0x80)
        else: o.append(b); break
def ws(o, s):
    b=s.encode(); wv(o,len(b)); o+=b
def wi32(o,v): o+=struct.pack('<i',v)
def wu32(o,v): o+=struct.pack('<I',v)
def wu64(o,v): o+=struct.pack('<Q',v)

class MRef:
    __slots__=('isg','tid','name','ga','items','nums')
    def __init__(self, isg=False, tid=0, name=''):
        self.isg=isg; self.tid=tid; self.name=name
        self.ga=[]; self.items=[]; self.nums=[]
    def write(self, o):
        o.append(1 if self.isg else 0); wi32(o,self.tid); ws(o,self.name)
        if self.isg:
            wi32(o,len(self.ga))
            for x in self.ga: wi32(o,x)
            wi32(o,len(self.items))
            for k,v in self.items:
                if k=='s': o.append(1); ws(o,v)
                else: o.append(0); wi32(o,v)
        else:
            wi32(o,len(self.nums))
            for x in self.nums: wi32(o,x)

class FieldRec:
    __slots__=('isnew','tid','name','extra')
    def __init__(self,isnew,tid,name,extra=None):
        self.isnew=isnew; self.tid=tid; self.name=name; self.extra=extra or (0,0)
    def write(self,o):
        o.append(1 if self.isnew else 0); wi32(o,self.tid); ws(o,self.name)
        if self.isnew: wi32(o,self.extra[0]); wi32(o,self.extra[1])

class Body:
    __slots__=('instrs','eh')
    def __init__(self, instrs=None, eh=b''):
        self.instrs=instrs if instrs is not None else []
        self.eh=eh

class Patch:
    def __init__(self, data=None):
        self.idx2name, self.name2idx = load_opcodes()
        brs = ('Br','Brtrue','Brfalse','Beq','Bge','Bgt','Ble','Blt','Bne_Un',
               'Bge_Un','Bgt_Un','Ble_Un','Blt_Un','Leave')
        self.BRANCH = {self.name2idx[n] for n in brs if n in self.name2idx}
        if data is not None: self._parse(data)

    def _parse(self, data):
        r=R(data)
        self.magic=r.u64(); self.bridge=r.st()
        self.types=[r.st() for _ in range(r.i32())]
        mc=r.i32(); self.bodies=[]
        for _ in range(mc):
            cs=r.i32()
            instrs=[[r.u32(), r.i32()] for _ in range(cs)]
            ehc=r.i32()
            eh=bytes(data[r.p:r.p+ehc*24]); r.skip(ehc*24)
            self.bodies.append(Body(instrs, eh))
        self.extm=[self._rm(r) for _ in range(r.i32())]
        self.interns=[r.st() for _ in range(r.i32())]
        self.fields=[]
        for _ in range(r.i32()):
            isnew=r.bo(); tid=r.i32(); nm=r.st()
            extra=(r.i32(), r.i32()) if isnew else None
            self.fields.append(FieldRec(isnew,tid,nm,extra))
        self.sft=[(r.i32(), r.i32()) for _ in range(r.i32())]
        self.storeys=r.i32()
        self.wm=r.st(); self.asm=r.st(); self.pad=r.u8()
        self.fixes=[]
        for _ in range(r.i32()):
            mr=self._rm(r); bid=r.i32(); self.fixes.append([mr,bid])
        self.new_classes=r.i32()
        self.end=r.p
        # MinDo/newer format: co the co section phu o cuoi (giu nguyen raw de round-trip)
        self.tail = bytes(data[self.end:])
        assert self.magic==struct.unpack('<Q', MAGIC)[0]
        assert len(self.tail)<4096, f"trailing too long {len(self.tail)}"

    def _rm(self, r):
        m=MRef(); m.isg=r.bo(); m.tid=r.i32(); m.name=r.st()
        if m.isg:
            gc=r.i32(); m.ga=[r.i32() for _ in range(gc)]
            pc=r.i32()
            for _ in range(pc):
                if r.bo(): m.items.append(('s', r.st()))
                else: m.items.append(('i', r.i32()))
        else:
            pc=r.i32(); m.nums=[r.i32() for _ in range(pc)]
        return m

    def serialize(self):
        o=bytearray()
        wu64(o,self.magic); ws(o,self.bridge)
        wi32(o,len(self.types))
        for t in self.types: ws(o,t)
        wi32(o,len(self.bodies))
        for b in self.bodies:
            wi32(o,len(b.instrs))
            for c,x in b.instrs: wu32(o,c); wi32(o,x)
            wi32(o,len(b.eh)//24); o+=b.eh
        wi32(o,len(self.extm))
        for m in self.extm: m.write(o)
        wi32(o,len(self.interns))
        for t in self.interns: ws(o,t)
        wi32(o,len(self.fields))
        for f in self.fields: f.write(o)
        wi32(o,len(self.sft))
        for a,b2 in self.sft: wi32(o,a); wi32(o,b2)
        wi32(o,self.storeys)
        ws(o,self.wm); ws(o,self.asm); o.append(self.pad)
        wi32(o,len(self.fixes))
        for mr,bid in self.fixes: mr.write(o); wi32(o,bid)
        wi32(o,self.new_classes)
        if self.tail: o+=self.tail
        return bytes(o)

    # ---------- API thêm/sửa ----------
    def op_name(self,code): return self.idx2name.get(code,'?%d?'%code)
    def op_code(self,name): return self.name2idx[name]
    def add_string(self,s): self.interns.append(s); return len(self.interns)-1
    def add_type(self,full): self.types.append(full); return len(self.types)-1
    def find_type(self,prefix):
        for i,t in enumerate(self.types):
            if t.startswith(prefix): return i
        return -1
    def add_method_ref(self,tid,name): self.extm.append(MRef(False,tid,name)); return len(self.extm)-1
    def find_method_ref(self,tid,name):
        for i,m in enumerate(self.extm):
            if m.tid==tid and m.name==name: return i
        return -1
    def add_field(self, tid, name, isnew=False, extra=None):
        self.fields.append(FieldRec(isnew, tid, name, extra))
        return len(self.fields)-1
    def find_field(self, tid, name):
        for i,f in enumerate(self.fields):
            if f.tid==tid and f.name==name: return i
        return -1
    def set_operand(self,bi,ii,val): self.bodies[bi].instrs[ii][1]=val

    def insert_instrs(self, bi, at, items):
        """items = list [code, operand]; nếu là lệnh nhảy, operand = target TUYỆT ĐỐI (chỉ số lệnh cuối cùng)."""
        b=self.bodies[bi]; orig=b.instrs; k=len(items)
        if b.eh:
            for i in range(len(b.eh)//24):
                rec=struct.unpack_from('<6i', b.eh, i*24)
                for v in rec[2:6]:
                    assert 0<=v<=len(orig), f'EH record lạ: {rec}'
        targets={}
        for i,(c,o) in enumerate(orig):
            if c in self.BRANCH: targets[i]=i+o
        new=[list(x) for x in orig[:at]]+[list(x) for x in items]+[list(x) for x in orig[at:]]
        for i in targets:
            t=targets[i]; t2=t if t<at else t+k
            ni=i if i<at else i+k
            new[ni]=[orig[i][0], t2-ni]
        for m in range(k):
            c,o=new[at+m]
            if c in self.BRANCH: new[at+m]=[c,o-(at+m)]
        neweh=bytearray()
        for i in range(len(b.eh)//24):
            rec=list(struct.unpack_from('<6i', b.eh, i*24))
            for j in (2,4):
                if rec[j]>=at: rec[j]+=k
            for j in (3,5):
                if rec[j]>at: rec[j]+=k
            neweh+=struct.pack('<6i',*rec)
        b.eh=bytes(neweh); b.instrs=new

    def append_instrs(self, bi, items):
        self.insert_instrs(bi, len(self.bodies[bi].instrs), items)

def split_bundle(bundle):
    m=bundle.find(MAGIC)
    assert m>0
    size=struct.unpack_from('<I', bundle, m-4)[0]
    return m,size, bundle[m:m+size]

def f32(v): return struct.unpack('<i', struct.pack('<f', v))[0]
