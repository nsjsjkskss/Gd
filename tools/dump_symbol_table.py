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


if __name__ == "__main__":
    main()
