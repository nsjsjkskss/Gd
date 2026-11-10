#!/usr/bin/env python3
import struct
import sys
import capstone
import lief


def main():
    path = (
        sys.argv[1]
        if len(sys.argv) > 1
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/Penny.framework/Penny"
    )
    with open(path, "rb") as f:
        raw = f.read()

    binary = lief.parse(path)
    md = capstone.Cs(capstone.CS_ARCH_ARM64, capstone.CS_MODE_ARM)
    md.detail = True

    got_labels = {}
    for b in binary.bindings:
        if b.symbol and b.address:
            got_labels[b.address] = b.symbol.name

    h = 0x811C9DC5
    for ch in b"unityframework":
        h = ((h ^ ch) * 0x01000193) & 0xFFFFFFFF
    print(f"FNV1a('unityframework') ^ 0xa5c39f17 = {h ^ 0xA5C39F17:#010x} (expected 0xea7fd991)\n")

    print("=== sub_b5218 / sub_b5310 Obfuscated Symbol Resolver Table (37 entries) ===")
    for idx in range(0x25):
        arg = 0x3700 | (idx ^ 0x5A)
        rel = struct.unpack("<i", raw[0x63DBB98 + idx * 4 : 0x63DBB9C + idx * 4])[0]
        target = 0xB5334 + rel
        ins = list(md.disasm(raw[target : target + 16], target))
        desc = "; ".join(f"{i.mnemonic} {i.op_str}" for i in ins[:3])
        resolved = ""
        if ins and ins[0].mnemonic == "adr":
            fn = ins[0].operands[1].imm
            resolved = f"internal_func_{fn:#x}"
        elif len(ins) >= 2 and ins[0].mnemonic == "adrp" and ins[1].mnemonic == "ldr":
            ptr = ins[0].operands[1].imm + ins[1].operands[1].mem.disp
            resolved = f"GOT[{ptr:#x}] -> {got_labels.get(ptr, 'unknown')}"
        print(f"  idx={idx:2d} | id={arg:#06x} | case_va={target:#08x} | {resolved:<42s} | {desc}")

    # Also decode the 19-entry IL2CPP export hash table at 0x63dcd28 used by sub_d9e44 (0xd9e44..0xda378)
    print("\n=== sub_d9e44 / sub_da2e0 Disasm (0xda180 .. 0xda2e8) ===")
    for insn in md.disasm(raw[0xDA180:0xDA2E8], 0xDA180):
        print(f"  {insn.address:#08x}: {insn.mnemonic:8s} {insn.op_str}")

    m64 = 0xFFFFFFFFFFFFFFFF

    def hash_da2e0(sym_bytes):
        if not sym_bytes:
            x8 = 0x6A2A169E036C9F3A
        else:
            x8 = 0xCBF29CE484222325
            for b in sym_bytes:
                x8 = ((x8 ^ b) * 0x100000001B3) & m64
            x8 = (x8 + 0x9E3779B97F4A7C15) & m64
        x8 ^= x8 >> 30
        x8 = (x8 * 0xBF58476D1CE4E5B9) & m64
        x8 ^= x8 >> 27
        x8 = (x8 * 0x94D049BB133111EB) & m64
        x8 ^= x8 >> 31
        return x8

    uf_path = "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/UnityFramework.framework/UnityFramework"
    uf_bin = lief.parse(uf_path)
    il2cpp_hashes = {}
    for s in uf_bin.symbols:
        cname = s.name.lstrip("_")
        if cname.startswith("il2cpp_"):
            il2cpp_hashes[hash_da2e0(cname.encode("utf-8"))] = (cname, s.value)

    table_19 = [
        struct.unpack("<Q", raw[0x63DCD28 + i * 8 : 0x63DCD30 + i * 8])[0]
        for i in range(19)
    ]
    # Find the 64-bit mask x15 = hash_da2e0(sym_0) ^ table_19[0]
    x15_mask = 0
    for h_cand in il2cpp_hashes:
        mask = h_cand ^ table_19[0]
        if sum(1 for v in table_19 if (v ^ mask) in il2cpp_hashes) == 19:
            x15_mask = mask
            break

    print(f"\n=== sub_d9e44 IL2CPP Export Table @ 0x63dcd28 (19 entries, mask={x15_mask:#018x}) ===")
    for i, val in enumerate(table_19):
        match = il2cpp_hashes.get(val ^ x15_mask)
        if match:
            print(f"  idx={i:2d} ({i:#04x}) | hash_enc={val:#018x} | uf_va={match[1]:#010x} | {match[0]}")
        else:
            print(f"  idx={i:2d} ({i:#04x}) | hash_enc={val:#018x} | UNMATCHED")


if __name__ == "__main__":
    main()
