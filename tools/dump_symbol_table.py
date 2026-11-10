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

    # Disassemble sub_b973c (0xb973c..0xb97e4)
    print("\n=== sub_b973c Disasm (0xb973c .. 0xb97e4) ===")
    for insn in md.disasm(raw[0xB973C:0xB97E4], 0xB973C):
        print(f"  {insn.address:#08x}: {insn.mnemonic:8s} {insn.op_str}")

    # Emulate +[_UISceneBootstrapLoader load] env-var decryption using Unicorn
    import unicorn as uc
    import unicorn.arm64_const as uca

    mu = uc.Uc(uc.UC_ARCH_ARM64, uc.UC_MODE_ARM)
    # Map code 0xe4000..0xe6000, const 0x63dd000..0x63de000, data 0xcc74000..0xcc90000, stack 0x70000000..0x70010000
    mu.mem_map(0xE4000, 0x2000)
    mu.mem_write(0xE4000, raw[0xE4000:0xE6000])
    mu.mem_map(0x63DD000, 0x2000)
    mu.mem_write(0x63DD000, raw[0x63DD000:0x63DF000])
    mu.mem_map(0xCC74000, 0x1C000)
    mu.mem_write(0xCC74000, raw[0xCC74000:0xCC88000])
    mu.mem_map(0x70000000, 0x10000)
    sp_val = 0x70008000
    mu.reg_write(uca.UC_ARM64_REG_SP, sp_val)
    mu.reg_write(uca.UC_ARM64_REG_X8, 0)
    # Enable FP/SIMD (CPACR_EL1)
    cpacr = mu.reg_read(uca.UC_ARM64_REG_CPACR_EL1)
    mu.reg_write(uca.UC_ARM64_REG_CPACR_EL1, cpacr | (3 << 20))

    mu.emu_start(0xE4DBC, 0xE5190)
    x0_env = mu.reg_read(uca.UC_ARM64_REG_X0)
    env_name = mu.mem_read(x0_env, 48).split(b"\0")[0].decode("utf-8", "replace")

    mu.emu_start(0xE5198, 0xE54B8)
    x1_val = mu.reg_read(uca.UC_ARM64_REG_X1)
    env_val = mu.mem_read(x1_val, 48).split(b"\0")[0].decode("utf-8", "replace")
    print(f"\n=== +[_UISceneBootstrapLoader load] Decrypted setenv(name, value, 1) ===")
    print(f"  env_name  = {env_name!r}")
    print(f"  env_value = {env_val!r}")

    # Find all calls to sub_b973c (0xb973c) in Penny
    print("\n=== All Calls to sub_b973c (case-insensitive FNV1a-64 + SplitMix64 -> 32-bit hash) ===")
    for off in range(0x4B40, 0xE8890, 4):
        w = struct.unpack("<I", raw[off : off + 4])[0]
        if (w & 0xFC000000) == 0x94000000:
            imm26 = w & 0x3FFFFFF
            if imm26 & (1 << 25):
                imm26 -= 1 << 26
            if off + (imm26 << 2) == 0xB973C:
                print(f"  call sub_b973c @ {off:#08x}:")
                for insn in md.disasm(raw[off - 32 : off + 48], off - 32):
                    print(f"    {insn.address:#08x}: {insn.mnemonic:8s} {insn.op_str}")


if __name__ == "__main__":
    main()
