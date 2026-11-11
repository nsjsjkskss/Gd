#!/usr/bin/env python3
"""
Static & Unicorn Emulation Comparison Tool:
IPA 1 (/tmp/dl/ipa/Payload/FreeFire.app) vs. IPA 2 (/tmp/dl2/ipa/Payload/FreeFire.app)

Extracts:
1. File-level and Mach-O section-level diffs across FreeFire, DataDomeSDK, UnityFramework, and Penny.
2. All 154 restored UnityFramework post-auth trampoline prologues (__objc_tramp & __auth_stubs).
3. All 1,881 instruction patch ranges in Penny.__TEXT.__text, header cave wrappers (0x2000..0x3bc4),
   and modified Murmur3-encrypted strings (0x48394 / 0x4a71c).
4. HunterFF.dylib Objective-C metadata, CFStrings, and Unicorn-emulated behavior of:
   - __DATA.__interpose _time hook (0x40863c)
   - __dyld_register_func_for_add_image callback (0x416dec): 33 _memcmp integrity checks & 9 Penny __bss bridge pointers.
"""
import os
import sys
import time
import struct
import hashlib
import lief
import capstone
import unicorn as uc
import unicorn.arm64_const as ua

DIR1 = sys.argv[1] if len(sys.argv) > 1 else "/tmp/dl/ipa/Payload/FreeFire.app"
DIR2 = sys.argv[2] if len(sys.argv) > 2 else "/tmp/dl2/ipa/Payload/FreeFire.app"

cs = capstone.Cs(capstone.CS_ARCH_ARM64, capstone.CS_MODE_ARM)

SEED = 0x2FDDD4D1
M32 = 0xFFFFFFFF


def dec_b497c(enc: bytes, w2: int) -> bytes:
    w8 = 0
    w9 = (SEED ^ w2) & M32
    out = bytearray()
    for b in enc:
        w17 = (w9 ^ w8) & M32
        w17 ^= (w17 >> 16)
        w17 = (w17 * 0x85EBCA6B) & M32
        w17 ^= (w17 >> 13)
        w17 = (w17 * 0xC2B2AE35) & M32
        w17 ^= (w17 >> 16)
        out.append(b ^ (w17 & 0xFF))
        w8 = (w8 + 0x9E3779B9) & M32
    return bytes(out)


def group_ranges(offsets):
    if not offsets:
        return []
    ranges = []
    start = prev = offsets[0]
    for x in offsets[1:]:
        if x == prev + 1:
            prev = x
        else:
            ranges.append((start, prev))
            start = prev = x
    ranges.append((start, prev))
    return ranges


def disasm_str(buf: bytes, va: int) -> str:
    return " ; ".join(f"{i.address:#x}: {i.mnemonic} {i.op_str}" for i in cs.disasm(buf, va))


def main():
    print("=== 1. Binary SHA-256 & Pre-CodeSignature Diff Summary ===")
    for rel in [
        "FreeFire",
        "Frameworks/DataDomeSDK.framework/DataDomeSDK",
        "Frameworks/UnityFramework.framework/UnityFramework",
        "Frameworks/Penny.framework/Penny",
    ]:
        p1 = os.path.join(DIR1, rel)
        p2 = os.path.join(DIR2, rel)
        b1 = open(p1, "rb").read()
        b2 = open(p2, "rb").read()
        m1 = lief.parse(p1)
        m2 = lief.parse(p2)
        uuid1 = "".join(f"{x:02x}" for x in m1.uuid.uuid) if m1.has_uuid else "none"
        uuid2 = "".join(f"{x:02x}" for x in m2.uuid.uuid) if m2.has_uuid else "none"
        pre_cs = min(m1.code_signature.data_offset, m2.code_signature.data_offset)
        pre_diff = sum(1 for i in range(pre_cs) if b1[i] != b2[i])
        print(
            f"  {rel}: size {len(b1)} vs {len(b2)} | UUID {uuid1} == {uuid2} | pre-CS diff bytes (0..{pre_cs:#x}): {pre_diff}"
        )

    uf1 = open(os.path.join(DIR1, "Frameworks/UnityFramework.framework/UnityFramework"), "rb").read()
    uf2 = open(os.path.join(DIR2, "Frameworks/UnityFramework.framework/UnityFramework"), "rb").read()

    print("\n=== 2. UnityFramework Restored Trampoline Slots (IPA1 Trap -> IPA2 Original Prologue) ===")
    restored_tramp = []
    for idx in range(1024):
        off = 0xBF44000 + idx * 0x40
        va = 0xC388000 + idx * 0x40
        if uf1[off + 0x18 : off + 0x1C] == bytes.fromhex("eb2bbc6d") and uf1[off : off + 0x40] != uf2[off : off + 0x40]:
            restored_tramp.append((idx, va, disasm_str(uf2[off + 0x18 : off + 0x30], va + 0x18)))
    print(f"__TEXT_EXEC.__objc_tramp restored slots: {len(restored_tramp)} / 110")
    for idx, va, s2 in restored_tramp:
        print(f"  [Tramp {idx:3d}] stub={va:#x} fallback={va+0x18:#x} -> {s2}")

    restored_auth = []
    for idx in range(512):
        off = 0xBF58000 + idx * 0x20
        va = 0xC39C000 + idx * 0x20
        if uf1[off + 0x18 : off + 0x1C] == bytes.fromhex("eb2bbc6d") and uf1[off : off + 0x20] != uf2[off : off + 0x20]:
            span = 0x28 if uf2[off + 0x20 : off + 0x24] != b"\x00\x00\x00\x00" else 0x08
            restored_auth.append((idx, va, disasm_str(uf2[off + 0x18 : off + 0x18 + span], va + 0x18)))
    print(f"\n__AUTH_EXEC.__auth_stubs restored slots: {len(restored_auth)} / 44 (plus 1 genuine prologue at 0xc39c180 = 45)")
    for idx, va, s2 in restored_auth:
        print(f"  [Auth  {idx:3d}] stub={va:#x} fallback={va+0x18:#x} -> {s2}")

    p1 = open(os.path.join(DIR1, "Frameworks/Penny.framework/Penny"), "rb").read()
    p2 = open(os.path.join(DIR2, "Frameworks/Penny.framework/Penny"), "rb").read()

    print("\n=== 3. Penny.__TEXT.__text Patch Classification & Decrypted Telegram Strings ===")
    for label, buf, w8_1, w8_2 in [
        ("IPA1", p1, 0x001D7D32, 0x00B81D47),
        ("IPA2", p2, 0x00781557, 0x00DD7522),
    ]:
        enc1 = buf[0x63DB780:0x63DB790] + struct.pack("<I", w8_1)[:3]
        enc2 = buf[0x63DB7A0:0x63DB7B0] + buf[0x63DA8F8:0x63DA900] + struct.pack("<I", w8_2)[:3]
        print(f"  {label} 0x48394: {dec_b497c(enc1, 0x9012403D)!r}")
        print(f"  {label} 0x4a71c: {dec_b497c(enc2, 0x2B28AE33)!r}")

    text_diffs = [i for i in range(0x5BD0, 0x3A022C) if p1[i] != p2[i]]
    text_ranges = group_ranges(text_diffs)
    c_svc = 0
    non_svc = []
    for st, en in text_ranges:
        ast = st & ~3
        aen = (en + 4) & ~3
        ins1 = list(cs.disasm(p1[ast:aen], ast))
        ins2 = list(cs.disasm(p2[ast:aen], ast))
        if len(ins1) == 1 and len(ins2) == 1 and ins1[0].mnemonic == "mov" and ins2[0].mnemonic == "b":
            nxt = list(cs.disasm(p1[aen : aen + 4], aen))
            if nxt and nxt[0].mnemonic == "svc":
                c_svc += 1
                continue
        non_svc.append((ast, aen, disasm_str(p1[ast:aen], ast), disasm_str(p2[ast:aen], ast)))

    print(f"  Total Penny.__TEXT.__text modified ranges: {len(text_ranges)}")
    print(f"    - Inline anti-hook/svc trap bypasses (mov w16, #1/#0x1a -> b #<pc+8>): {c_svc}")
    print(f"    - Functional / auth / trace-wrapper patches: {len(non_svc)}")
    for ast, aen, s1, s2 in non_svc:
        print(f"      @ {ast:#x} ({aen-ast}B): [{s1}]  ==>  [{s2}]")

    print("\n=== 4. HunterFF.dylib Unicorn Emulation (_time Interpose, 0x416dec _memcmp Table, and 9 Penny Bridges) ===")
    hff_path = os.path.join(DIR2, "Frameworks/Penny.framework/HunterFF.dylib")
    hff = open(hff_path, "rb").read()
    mh = lief.parse(hff_path)

    mu = uc.Uc(uc.UC_ARCH_ARM64, uc.UC_MODE_ARM)
    mu.mem_map(0x0, 0x500000)
    mu.mem_write(0x0, hff[:0x458000])

    PENNY_BASE = 0x100000000
    mu.mem_map(PENNY_BASE, 0xD000000)
    mu.mem_write(PENNY_BASE, p2[:0xCC88000])

    STACK_BASE = 0x70000000
    STACK_SIZE = 0x200000
    mu.mem_map(STACK_BASE, STACK_SIZE)
    STUB_BASE = 0x80000000
    mu.mem_map(STUB_BASE, 0x100000)
    cpacr = mu.reg_read(ua.UC_ARM64_REG_CPACR_EL1)
    mu.reg_write(ua.UC_ARM64_REG_CPACR_EL1, cpacr | 0x300000)

    stub_to_sym = {}
    sym_to_stub = {}
    idx = 0
    for b in mh.bindings:
        sym = b.symbol.name if b.has_symbol else f"bind_{b.address:#x}"
        if sym not in sym_to_stub:
            s_va = STUB_BASE + idx * 8
            mu.mem_write(s_va, b"\xc0\x03\x5f\xd6")
            stub_to_sym[s_va] = sym
            sym_to_stub[sym] = s_va
            idx += 1
        mu.mem_write(b.address, struct.pack("<Q", sym_to_stub[sym]))

    memcmps = []
    non_stack_writes = []

    def hook_w(uc_e, acc, addr, sz, val, ud):
        if not (STACK_BASE <= addr < STACK_BASE + STACK_SIZE):
            non_stack_writes.append((uc_e.reg_read(ua.UC_ARM64_REG_PC), addr, sz, val))

    def hook_code(uc_e, addr, sz, ud):
        if addr in stub_to_sym:
            sym = stub_to_sym[addr]
            x0 = uc_e.reg_read(ua.UC_ARM64_REG_X0)
            x1 = uc_e.reg_read(ua.UC_ARM64_REG_X1)
            x2 = uc_e.reg_read(ua.UC_ARM64_REG_X2)
            if sym == "_memcmp":
                b1 = bytes(uc_e.mem_read(x0, x2))
                b2 = bytes(uc_e.mem_read(x1, x2))
                res = 0 if b1 == b2 else 1
                memcmps.append((x0 - PENNY_BASE, x1, x2, res == 0))
                uc_e.reg_write(ua.UC_ARM64_REG_X0, res)

    mu.hook_add(uc.UC_HOOK_MEM_WRITE, hook_w)
    mu.hook_add(uc.UC_HOOK_CODE, hook_code, begin=STUB_BASE, end=STUB_BASE + 0x10000)

    def run_fn(fn_va, x0=0, x1=0, max_ins=8000000):
        memcmps.clear()
        non_stack_writes.clear()
        sp = STACK_BASE + STACK_SIZE - 0x10000
        ret_va = STUB_BASE + 0xF000
        mu.mem_write(ret_va, b"\x00\x00\x00\x14")
        mu.reg_write(ua.UC_ARM64_REG_SP, sp)
        mu.reg_write(ua.UC_ARM64_REG_X0, x0)
        mu.reg_write(ua.UC_ARM64_REG_X1, x1)
        mu.reg_write(ua.UC_ARM64_REG_LR, ret_va)
        mu.emu_start(fn_va, ret_va, count=max_ins)
        return mu.reg_read(ua.UC_ARM64_REG_X0)

    t_res = run_fn(0x40863C, STUB_BASE + 0x20000)
    print(f"  _time interpose (0x40863c) -> {t_res} ({t_res:#x}, UTC={time.strftime('%Y-%m-%d %H:%M:%S', time.gmtime(t_res))})")

    run_fn(0x416DEC, PENNY_BASE, PENNY_BASE)
    print(f"  0x416dec _memcmp integrity checks: {len(memcmps)}")
    for i, (rva, hff_off, sz, ok) in enumerate(memcmps):
        print(f"    [{i:2d}] Penny RVA {rva:#x} == HunterFF {hff_off:#x} ({sz} bytes) -> match={ok}")
    print(f"  0x416dec memory writes ({len(non_stack_writes)}):")
    for pc, addr, sz, val in non_stack_writes:
        target = f"Penny+{addr-PENNY_BASE:#x}" if addr >= PENNY_BASE else f"HunterFF+{addr:#x}"
        print(f"    PC={pc:#x} -> {target} (sz={sz}) = {val:#x}")


if __name__ == "__main__":
    main()
