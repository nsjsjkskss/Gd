#!/usr/bin/env python3
import struct
import sys
import capstone

SEED_B497C = 0x2FDDD4D1
M32 = 0xFFFFFFFF


def dec_b497c(enc, w2):
    w8 = 0
    w9 = (SEED_B497C ^ w2) & M32
    out = bytearray()
    for b in enc:
        w17 = (w9 ^ w8) & M32
        w17 ^= w17 >> 16
        w17 = (w17 * 0x85EBCA6B) & M32
        w17 ^= w17 >> 13
        w17 = (w17 * 0xC2B2AE35) & M32
        w17 ^= w17 >> 16
        out.append(b ^ (w17 & 0xFF))
        w8 = (w8 + 0x9E3779B9) & M32
    return bytes(out)


def dump_b497c(raw, md):
    hits = []
    for off in range(0x4B40, 0xE8890, 4):
        w = struct.unpack("<I", raw[off : off + 4])[0]
        if (w & 0xFC000000) == 0x94000000:
            imm26 = w & 0x3FFFFFF
            if imm26 & (1 << 25):
                imm26 -= 1 << 26
            if off + (imm26 << 2) == 0xB497C:
                hits.append(off)

    for call_va in hits:
        start = max(0x4B40, call_va - 140)
        ins = list(md.disasm(raw[start : call_va + 4], start))
        regs = {"xzr": 0, "wzr": 0, "sp": 0x1000, "x29": 0x1800}
        mem = bytearray(0x2000)
        for i in ins:
            m = i.mnemonic
            ops = [x.strip() for x in i.op_str.split(",")]
            try:
                if m == "adrp":
                    regs[ops[0]] = int(ops[1].lstrip("#"), 16)
                elif m == "add" and len(ops) == 3 and ops[2].startswith("#"):
                    regs[ops[0]] = regs.get(ops[1], 0) + int(ops[2].lstrip("#"), 0)
                elif m == "sub" and len(ops) == 3 and ops[2].startswith("#"):
                    regs[ops[0]] = regs.get(ops[1], 0) - int(ops[2].lstrip("#"), 0)
                elif m == "mov" and len(ops) == 2:
                    val = int(ops[1].lstrip("#"), 0) & M32 if ops[1].startswith("#") else regs.get(ops[1], 0)
                    regs[ops[0]] = val
                    if ops[0].startswith("w"):
                        regs["x" + ops[0][1:]] = val & M32
                elif m == "movk" and len(ops) >= 2:
                    shift = int(ops[2].split("#")[1], 0) if (len(ops) == 3 and "lsl #" in ops[2]) else 16
                    val = int(ops[1].lstrip("#"), 0)
                    cur = regs.get(ops[0], 0) & ~(0xFFFF << shift)
                    regs[ops[0]] = cur | (val << shift)
                elif m == "ldr":
                    dst = ops[0]
                    mem_op = i.op_str.split("[", 1)[1].rstrip("]")
                    parts = [p.strip() for p in mem_op.split(",")]
                    addr = regs.get(parts[0], 0) + (int(parts[1].lstrip("#"), 0) if len(parts) > 1 else 0)
                    if 0x63D0000 <= addr < len(raw):
                        sz = 16 if dst.startswith("q") else (8 if dst.startswith(("d", "x")) else 4)
                        regs[dst] = raw[addr : addr + sz]
                elif m in ("str", "stur", "strh", "sturh", "strb", "sturb"):
                    src = ops[0]
                    mem_op = i.op_str.split("[", 1)[1].rstrip("]")
                    parts = [p.strip() for p in mem_op.split(",")]
                    addr = regs.get(parts[0], 0) + (int(parts[1].lstrip("#"), 0) if len(parts) > 1 else 0)
                    if 0 <= addr < len(mem) - 32:
                        val = regs.get(src, 0)
                        if isinstance(val, bytes):
                            mem[addr : addr + len(val)] = val
                        elif m in ("strb", "sturb"):
                            mem[addr] = val & 0xFF
                        elif m in ("strh", "sturh"):
                            struct.pack_into("<H", mem, addr, val & 0xFFFF)
                        elif src.startswith("w"):
                            struct.pack_into("<I", mem, addr, val & M32)
                        else:
                            struct.pack_into("<Q", mem, addr, val & 0xFFFFFFFFFFFFFFFF)
                elif m in ("stp", "sturp"):
                    s1, s2 = ops[0], ops[1]
                    mem_op = i.op_str.split("[", 1)[1].rstrip("]")
                    parts = [p.strip() for p in mem_op.split(",")]
                    addr = regs.get(parts[0], 0) + (int(parts[1].lstrip("#"), 0) if len(parts) > 1 else 0)
                    v1, v2 = regs.get(s1, b""), regs.get(s2, b"")
                    if isinstance(v1, bytes) and isinstance(v2, bytes) and 0 <= addr < len(mem) - 32:
                        mem[addr : addr + len(v1)] = v1
                        mem[addr + len(v1) : addr + len(v1) + len(v2)] = v2
            except Exception:
                pass
        buf_ptr = regs.get("x0", 0)
        length = regs.get("w1", 0)
        w2 = regs.get("w2", 0)
        if 0 < length < 256 and 0 <= buf_ptr < len(mem) - length:
            dec = dec_b497c(mem[buf_ptr : buf_ptr + length], w2)
            print(f"  {call_va:#07x} (len={length:2d}, w2={w2:#010x}): {dec.decode('utf-8', 'replace')!r}")


if __name__ == "__main__":
    target = (
        sys.argv[1]
        if len(sys.argv) > 1
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/Penny.framework/Penny"
    )
    with open(target, "rb") as f:
        raw_bytes = f.read()
    cs = capstone.Cs(capstone.CS_ARCH_ARM64, capstone.CS_MODE_ARM)
    dump_b497c(raw_bytes, cs)
