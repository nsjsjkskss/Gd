#!/usr/bin/env python3
import struct
import sys
import capstone


def decode_rva_tables(penny):
    m64 = 0xFFFFFFFFFFFFFFFF
    m32 = 0xFFFFFFFF
    x20 = 0x065AFA27E8371494
    x25 = 0x9E3779B97F4A7C15
    x23 = 0xBF58476D1CE4E5B9
    x8_c = 0xD6E8FEB86659FD93
    x9_c = 0x94D049BB133111EB

    def gen_mask(seed):
        x17 = (x20 ^ seed ^ x25) & m64
        x17 ^= x17 >> 30
        x17 = (x17 * x23) & m64
        x0 = x20 ^ (x17 >> 41) ^ ((x17 << 19) & m64) ^ x8_c
        x17 = (x0 + x17) & m64
        x17 ^= x17 >> 27
        x17 = (x17 * x9_c) & m64
        return ((x17 >> 32) ^ x17) & m32

    specs = [
        ("Table1", 0x63DCB1C, 15, 0xFACBFB4AF9221A85, 0xCC8F6C8),
        ("Table2", 0x63DCB58, 19, 0xEDB18FD13B040CF9, 0xCC8F740),
        ("Table3", 0x63DCBA4, 96, x23, 0xCC8F7E8),
    ]
    out = {}
    for name, off, cnt, seed, dst_bss in specs:
        arr = []
        s = seed
        for i in range(cnt):
            enc = struct.unpack("<I", penny[off + i * 4 : off + (i + 1) * 4])[0]
            rva = (enc ^ gen_mask(s)) & m32
            arr.append((i, rva, dst_bss + i * 8))
            s = (s + x23) & m64
        out[name] = arr
    return out


def main():
    penny_path = (
        sys.argv[1]
        if len(sys.argv) > 1
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/Penny.framework/Penny"
    )
    uf_path = (
        sys.argv[2]
        if len(sys.argv) > 2
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/UnityFramework.framework/UnityFramework"
    )
    with open(penny_path, "rb") as f:
        penny = f.read()
    with open(uf_path, "rb") as f:
        uf = f.read()

    md = capstone.Cs(capstone.CS_ARCH_ARM64, capstone.CS_MODE_ARM)
    tables = decode_rva_tables(penny)

    for tname in ("Table1", "Table2", "Table3"):
        entries = tables[tname]
        print(f"=== {tname} ({len(entries)} entries) ===")
        for idx, rva, bss_slot in entries:
            ins_prev = list(md.disasm(uf[rva - 16 : rva + 8], rva - 16))
            dis_str = "; ".join(f"{x.address:#x}: {x.mnemonic} {x.op_str}" for x in ins_prev)
            # If rva-4 is a direct branch to __TEXT_EXEC or __AUTH_EXEC, decode the stub!
            w_prev = struct.unpack("<I", uf[rva - 4 : rva])[0]
            stub_info = ""
            if (w_prev >> 26) == 0b000101:
                imm26 = w_prev & 0x3FFFFFF
                if imm26 & (1 << 25):
                    imm26 -= 1 << 26
                stub_va = (rva - 4) + (imm26 << 2)
                if 0xC388000 <= stub_va < 0xC398000:
                    s_off = 0xBF44000 + (stub_va - 0xC388000)
                    s_ins = list(md.disasm(uf[s_off : s_off + 0x20], stub_va))
                    stub_info = f" -> __TEXT_EXEC stub@{stub_va:#x} [{'; '.join(f'{x.mnemonic} {x.op_str}' for x in s_ins[:7])}]"
                elif 0xC39C000 <= stub_va < 0xC3A0000:
                    s_off = 0xBF58000 + (stub_va - 0xC39C000)
                    s_ins = list(md.disasm(uf[s_off : s_off + 0x20], stub_va))
                    stub_info = f" -> __AUTH_EXEC stub@{stub_va:#x} [{'; '.join(f'{x.mnemonic} {x.op_str}' for x in s_ins[:7])}]"
            elif w_prev == 0xD61F0200 and len(ins_prev) >= 3:
                # 3-instruction long branch at rva-12..rva: adrp x16, #page; add x16, x16, #off; br x16
                if ins_prev[1].mnemonic == "adrp" and ins_prev[2].mnemonic == "add":
                    page = int(ins_prev[1].op_str.split("#")[1], 16)
                    off = int(ins_prev[2].op_str.split("#")[1], 16)
                    stub_va = page + off
                    if 0xC388000 <= stub_va < 0xC398000:
                        s_off = 0xBF44000 + (stub_va - 0xC388000)
                        s_ins = list(md.disasm(uf[s_off : s_off + 0x20], stub_va))
                        stub_info = f" -> __TEXT_EXEC (12B long-branch) stub@{stub_va:#x} [{'; '.join(f'{x.mnemonic} {x.op_str}' for x in s_ins[:7])}]"
                    elif 0xC39C000 <= stub_va < 0xC3A0000:
                        s_off = 0xBF58000 + (stub_va - 0xC39C000)
                        s_ins = list(md.disasm(uf[s_off : s_off + 0x20], stub_va))
                        stub_info = f" -> __AUTH_EXEC (12B long-branch) stub@{stub_va:#x} [{'; '.join(f'{x.mnemonic} {x.op_str}' for x in s_ins[:7])}]"
            print(f"  [{idx:2d}] rva={rva:#010x} bss={bss_slot:#x} | {dis_str}{stub_info}")
        print()

    print("=== Penny Orig-Call Thunks (0xdb5d0 .. 0xdc230) ===")
    cur = 0xDB5D0
    thunk_idx = 0
    while cur < 0xDC230:
        ins = list(md.disasm(penny[cur : cur + 0x5C], cur))
        if not ins:
            break
        # Find the ldr x17, [x17, #off] or ldr x17, [x17] instruction and displaced prologue instruction before br x17
        slot_str = ""
        displaced = ""
        end_addr = cur + 0x5C
        for k, x in enumerate(ins):
            if x.mnemonic == "ldr" and x.op_str.startswith("x17, [x17"):
                slot_str = x.op_str
            if x.mnemonic == "br" and x.op_str == "x17" and k > 0:
                displaced = f"{ins[k-1].mnemonic} {ins[k-1].op_str}"
            if x.mnemonic == "brk":
                end_addr = x.address + 4
                break
        print(f"  thunk[{thunk_idx:2d}] @ {cur:#08x} .. {end_addr:#08x}: slot=({slot_str}) displaced=({displaced})")
        cur = end_addr
        thunk_idx += 1

    # Scan all of UnityFramework __TEXT.__text (0x4000..0xa19aeb4) for B/BL targeting __TEXT_EXEC or __AUTH_EXEC
    import array

    words = array.array("I")
    words.frombytes(uf[0x4000:0xA19AEB4])
    text_exec_branches = []
    auth_exec_branches = []
    text_exec_long = []
    auth_exec_long = []
    for i, w in enumerate(words):
        op = w >> 26
        if op == 0b000101 or op == 0b100101:
            imm26 = w & 0x3FFFFFF
            if imm26 & (1 << 25):
                imm26 -= 1 << 26
            pc = 0x4000 + (i << 2)
            target = pc + (imm26 << 2)
            if 0xC388000 <= target < 0xC398000:
                text_exec_branches.append((pc, target))
            elif 0xC39C000 <= target < 0xC3A0000:
                auth_exec_branches.append((pc, target))
        elif w == 0xD61F0200 and i >= 2:
            w_adrp = words[i - 2]
            w_add = words[i - 1]
            if (w_adrp & 0x9F00001F) == 0x90000010 and (w_add & 0xFFC003FF) == 0x91000210:
                immlo = (w_adrp >> 29) & 3
                immhi = (w_adrp >> 5) & 0x7FFFF
                imm = (immhi << 2) | immlo
                if imm & (1 << 20):
                    imm -= 1 << 21
                pc_adrp = 0x4000 + ((i - 2) << 2)
                page = (pc_adrp & ~0xFFF) + (imm << 12)
                imm12 = (w_add >> 10) & 0xFFF
                target = page + imm12
                pc_br = 0x4000 + (i << 2)
                if 0xC388000 <= target < 0xC398000:
                    text_exec_long.append((pc_br, target))
                elif 0xC39C000 <= target < 0xC3A0000:
                    auth_exec_long.append((pc_br, target))

    print(
        f"\n=== Summary of UnityFramework __TEXT.__text Static Branches ==="
        f"\n  Direct B/BL to __TEXT_EXEC: {len(text_exec_branches)}, 12B ADRP+ADD+BR to __TEXT_EXEC: {len(text_exec_long)} (Total: {len(text_exec_branches) + len(text_exec_long)} / 110 stubs)"
        f"\n  Direct B/BL to __AUTH_EXEC: {len(auth_exec_branches)}, 12B ADRP+ADD+BR to __AUTH_EXEC: {len(auth_exec_long)} (Total: {len(auth_exec_branches) + len(auth_exec_long)} / 45 stubs)"
    )

    # Decode 31-entry jump table in sub_de414 (0x63dbc2c)
    print("\n=== sub_de414 Jump Table @ 0x63dbc2c (31 entries) ===")
    for idx in range(31):
        rel = struct.unpack("<i", penny[0x63DBC2C + idx * 4 : 0x63DBC30 + idx * 4])[0]
        case_va = (0xDE430 + rel) & 0xFFFFFFFF
        c_ins = list(md.disasm(penny[case_va : case_va + 12], case_va))
        c_str = "; ".join(f"{x.mnemonic} {x.op_str}" for x in c_ins)
        print(f"  case[{idx:2d}] -> {case_va:#08x}: {c_str}")
    rva_set = {rva for t in tables.values() for _, rva, _ in t}
    unmatched_text = [(pc, tgt) for pc, tgt in text_exec_branches if (pc + 4) not in rva_set]
    unmatched_auth = [(pc, tgt) for pc, tgt in auth_exec_branches if (pc + 4) not in rva_set]
    print(f"  Unmatched __TEXT_EXEC branches (pc+4 not in Table1..3): {len(unmatched_text)}")
    for pc, tgt in unmatched_text:
        print(f"    pc={pc:#010x} (rva={pc+4:#010x}) -> {tgt:#010x}")
    print(f"  Unmatched __AUTH_EXEC branches (pc+4 not in Table1..3): {len(unmatched_auth)}")
    for pc, tgt in unmatched_auth:
        print(f"    pc={pc:#010x} (rva={pc+4:#010x}) -> {tgt:#010x}")

    # Profile Penny __TEXT.__text from 0x4b40 to 0x39f19c by 64KB blocks
    print("\n=== Penny __TEXT.__text Profile (0x4b40 .. 0x39f19c) ===")
    for blk_start in range(0x4B40, 0x39F19C, 0x20000):
        blk_end = min(0x39F19C, blk_start + 0x20000)
        chunk = penny[blk_start:blk_end]
        nz = len(chunk) - chunk.count(0)
        # Count RET (0xd65f03c0) and BRK #1 (0xd4200020) and UDF #0 (0x00000000)
        w_arr = array.array("I")
        w_arr.frombytes(chunk[: len(chunk) & ~3])
        rets = w_arr.count(0xD65F03C0)
        brks = w_arr.count(0xD4200020)
        svcs = sum(1 for x in w_arr if (x & 0xFFE0001F) == 0xD4000001)
        print(
            f"  {blk_start:#08x}..{blk_end:#08x}: nonzero={nz:#07x} "
            f"RET={rets:4d} BRK#1={brks:4d} SVC={svcs:3d}"
        )

    # Disassemble sample functions in high __text (0x35da20, 0x382300, 0x39ec8c)
    print("\n=== High __TEXT.__text Sample Disassembly (0x35da20, 0x382300, 0x39ec8c) ===")
    for name, va, sz in [
        ("sub_de414_to_de800", 0xDE414, 0x3EC),
        ("sub_d48a0_to_d5400", 0xD48A0, 0x360),
    ]:
        print(f"--- {name} @ {va:#x} ---")
        for insn in md.disasm(penny[va : va + sz], va):
            print(f"  {insn.address:#08x}: {insn.mnemonic:8s} {insn.op_str}")

    # Extract all ADR instructions inside 0xcf930 .. 0xdb5d0
    print("\n=== All Handler ADR Instructions in 0xcf930 .. 0xdb5d0 ===")
    adr_handlers = []
    for insn in md.disasm(penny[0xCF930:0xDB5D0], 0xCF930):
        if insn.mnemonic == "adr":
            adr_handlers.append((insn.address, insn.op_str))
    print(f"Total ADR instructions in 0xcf930..0xdb5d0: {len(adr_handlers)}")
    for addr, op_s in adr_handlers:
        print(f"  {addr:#08x}: adr {op_s}")

    print("\n=== Strings in 0x66e0000..0x6700000 referenced by 0x220000..0x39ec8c ===")
    for off in range(0x66EA000, 0x66F3000, 64):
        s = penny[off : off + 64]
        printable = "".join(chr(b) if 32 <= b < 127 else "." for b in s)
        if any(32 <= b < 127 for b in s):
            print(f"  {off:#08x}: {printable}")


if __name__ == "__main__":
    main()
