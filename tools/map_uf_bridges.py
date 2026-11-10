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
            ins_prev = list(md.disasm(uf[rva - 4 : rva + 8], rva - 4))
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


if __name__ == "__main__":
    main()
