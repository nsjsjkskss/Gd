#!/usr/bin/env python3
import struct
import sys


def decode_penny_rva_tables(penny_path, uf_path=None):
    with open(penny_path, "rb") as f:
        penny = f.read()
    uf = None
    if uf_path:
        with open(uf_path, "rb") as f:
            uf = f.read()

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

    tables = [
        ("Table 1 (0x63dcb1c -> 0xcc8f6c8)", 0x63DCB1C, 15, 0xFACBFB4AF9221A85),
        ("Table 2 (0x63dcb58 -> 0xcc8f740)", 0x63DCB58, 19, 0xEDB18FD13B040CF9),
        ("Table 3 (0x63dcba4 -> 0xcc8f7e8)", 0x63DCBA4, 96, x23),
    ]

    for title, base_off, count, init_seed in tables:
        print(f"=== {title} ({count} entries) ===")
        seed = init_seed
        for i in range(count):
            enc = struct.unpack("<I", penny[base_off + i * 4 : base_off + (i + 1) * 4])[0]
            rva = (enc ^ gen_mask(seed)) & m32
            uf_preview = ""
            if uf is not None and rva + 8 <= len(uf):
                uf_preview = f" (uf prologue: {uf[rva:rva+8].hex()})"
            print(f"  [{i:2d}] rva={rva:#010x}{uf_preview}")
            seed = (seed + x23) & m64
        print()


if __name__ == "__main__":
    p_path = (
        sys.argv[1]
        if len(sys.argv) > 1
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/Penny.framework/Penny"
    )
    u_path = (
        sys.argv[2]
        if len(sys.argv) > 2
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/UnityFramework.framework/UnityFramework"
    )
    decode_penny_rva_tables(p_path, u_path)
