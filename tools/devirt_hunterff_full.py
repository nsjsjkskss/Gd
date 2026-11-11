#!/usr/bin/env python3
"""
Full Static & Unicorn Micro-Execution De-Virtualizer for HunterFF.dylib:
1. Mach-O segments/sections, __mod_init_func, __interpose, __cfstring, __ustring, __objc_dictobj.
2. Custom ARM64 Bytecode VM anatomy: 4,322 VM Entry Stubs, 8,863 __text->VM branches, 8,522 VM->text resume points.
3. TLS Pinning (PennyOwnerPinnedSession & sub_4165b0): 3 SHA-256 Base64 pins, ASN.1 SPKI header, ephemeral session config.
4. Ed25519 Ref10 + SHA-512 (sub_4086f8 & sub_421714): 32-byte public key, 6-field canonical message, RFC 8032 Unicorn verification.
5. Keychain, Nonce, Sophia Verify Backend (https://api.hackgamefile.com/sophia/verify), and 45s re-verify loop.
"""
import sys
import struct
import hashlib
import lief
import capstone
import unicorn as uc
import unicorn.arm64_const as ua

HFF_DEFAULT = "/tmp/dl2/ipa/Payload/FreeFire.app/Frameworks/Penny.framework/HunterFF.dylib"
HFF_PATH = sys.argv[1] if len(sys.argv) > 1 else HFF_DEFAULT

cs = capstone.Cs(capstone.CS_ARCH_ARM64, capstone.CS_MODE_ARM)
cs.detail = True

# Pure-Python RFC 8032 Ed25519 helper for Unicorn test-vector verification
Q_ED = 2**255 - 19
L_ED = 2**252 + 27742317777372353535851937790883648493


def _inv(x: int) -> int:
    return pow(x, Q_ED - 2, Q_ED)


D_ED = -121665 * _inv(121666) % Q_ED
I_ED = pow(2, (Q_ED - 1) // 4, Q_ED)


def _xrecover(y: int) -> int:
    xx = (y * y - 1) * _inv(D_ED * y * y + 1)
    x = pow(xx, (Q_ED + 3) // 8, Q_ED)
    if (x * x - xx) % Q_ED != 0:
        x = (x * I_ED) % Q_ED
    if x % 2 != 0:
        x = Q_ED - x
    return x


BY_ED = 4 * _inv(5) % Q_ED
BX_ED = _xrecover(BY_ED)
B_PT = [BX_ED % Q_ED, BY_ED % Q_ED]


def _edwards(P, Q):
    x1, y1 = P
    x2, y2 = Q
    x3 = (x1 * y2 + x2 * y1) * _inv(1 + D_ED * x1 * x2 * y1 * y2)
    y3 = (y1 * y2 + x1 * x2) * _inv(1 - D_ED * x1 * x2 * y1 * y2)
    return [x3 % Q_ED, y3 % Q_ED]


def _scalarmult(P, e: int):
    if e == 0:
        return [0, 1]
    Q = _scalarmult(P, e // 2)
    Q = _edwards(Q, Q)
    if e & 1:
        Q = _edwards(Q, P)
    return Q


def _encodepoint(P) -> bytes:
    x, y = P
    return (y | ((x & 1) << 255)).to_bytes(32, "little")


def ed25519_sign_test(msg: bytes, sk: bytes):
    h = hashlib.sha512(sk).digest()
    a = 2**254 + sum(2**i * ((h[i // 8] >> (i % 8)) & 1) for i in range(3, 254))
    pk = _encodepoint(_scalarmult(B_PT, a))
    r = int.from_bytes(hashlib.sha512(h[32:64] + msg).digest(), "little")
    R = _encodepoint(_scalarmult(B_PT, r))
    k = int.from_bytes(hashlib.sha512(R + pk + msg).digest(), "little")
    S = ((r + k * a) % L_ED).to_bytes(32, "little")
    return pk, R + S


def main():
    hff = open(HFF_PATH, "rb").read()
    mh = lief.parse(HFF_PATH)

    def read_cstr(va: int) -> str:
        if va <= 0 or va >= len(hff):
            return ""
        end = hff.find(b"\x00", va)
        if end == -1 or end - va > 300:
            return ""
        return hff[va:end].decode("utf-8", "replace")

    print("=== 1. HunterFF.dylib Binary & Mach-O Layout ===")
    uuid_str = "".join(f"{b:02x}" for b in mh.uuid.uuid) if mh.has_uuid else "none"
    print(f"  Size: {len(hff)} bytes | SHA-256: {hashlib.sha256(hff).hexdigest()} | LC_UUID: {uuid_str}")
    for seg in mh.segments:
        print(
            f"  Segment {seg.name:12s} VA={seg.virtual_address:#010x} VSize={seg.virtual_size:#010x} "
            f"FOff={seg.file_offset:#010x} FSize={seg.file_size:#010x} Prot={seg.init_protection}/{seg.max_protection}"
        )
        for sec in seg.sections:
            print(f"    Section {sec.name:16s} VA={sec.virtual_address:#010x} Size={sec.size:#010x}")

    sec_cf = mh.get_section("__cfstring")
    cf_map = {}
    for off in range(sec_cf.offset, sec_cf.offset + sec_cf.size, 32):
        isa, flags, ptr, length = struct.unpack("<QQQQ", hff[off : off + 32])
        if (flags & 0xFF) == 0xD0 or (flags & 0x10):
            val = hff[ptr : ptr + length * 2].decode("utf-16le", "replace")
        else:
            val = hff[ptr : ptr + length].decode("utf-8", "replace")
        cf_map[off] = val

    print(f"\n=== 2. Custom ARM64 Bytecode VM Statistics (0x007300 .. 0x404100) ===")
    vm_jumps = 0
    for off in range(0x408000, 0x4357A8, 4):
        w = struct.unpack("<I", hff[off : off + 4])[0]
        if (w >> 26) == 0b000101:
            imm26 = w & 0x3FFFFFF
            if imm26 & (1 << 25):
                imm26 -= 1 << 26
            target = (off + (imm26 << 2)) & 0xFFFFFFFF
            if 0x007300 <= target < 0x404100:
                vm_jumps += 1

    resume_targets = set()
    for off in range(0x007300, 0x404100, 4):
        w = struct.unpack("<I", hff[off : off + 4])[0]
        if (w >> 26) == 0b000101:
            imm26 = w & 0x3FFFFFF
            if imm26 & (1 << 25):
                imm26 -= 1 << 26
            target = (off + (imm26 << 2)) & 0xFFFFFFFF
            if 0x408000 <= target < 0x4357A8:
                resume_targets.add(target)

    vm_entries = []
    for off in range(0x007300, 0x404100 - 24, 4):
        w0, w1, w2, w3, w4, w5 = struct.unpack("<6I", hff[off : off + 24])
        if (
            (w1 & 0xFFC003FF) == 0xF9400006
            and w2 == 0x8B0000C6
            and (w3 & 0xFFC003FF) == 0xF9400003
            and w4 == 0x8B000063
            and w5 == 0xD61F00C0
        ):
            rel_word = struct.unpack("<i", hff[off - 4 : off])[0]
            tbl_base = ((off - 4) + rel_word) & 0xFFFFFFFF
            h_disp = ((w1 >> 10) & 0xFFF) * 8
            bc_disp = ((w3 >> 10) & 0xFFF) * 8
            h_rel = struct.unpack("<q", hff[tbl_base + h_disp : tbl_base + h_disp + 8])[0]
            bc_rel = struct.unpack("<q", hff[tbl_base + bc_disp : tbl_base + bc_disp + 8])[0]
            h_va = (tbl_base + h_rel) & 0xFFFFFFFFFFFFFFFF
            bc_va = (tbl_base + bc_rel) & 0xFFFFFFFFFFFFFFFF
            vm_entries.append((off - 0x1C, tbl_base, h_va, bc_va))

    dispatchers = sorted(set(x[2] for x in vm_entries))
    print(f"  Total functions in LC_FUNCTION_STARTS: {len(mh.function_starts.functions)}")
    print(f"  Branches from __TEXT.__text into VM region (0x7300..0x404100): {vm_jumps}")
    print(f"  VM Bytecode Entry Stubs (BytecodeVM): {len(vm_entries)}")
    print(f"  Central VM Dispatchers: {', '.join(hex(d) for d in dispatchers)}")
    print(f"  Unique VM->text resume addresses in __TEXT.__text: {len(resume_targets)}")

    print("\n=== 3. TLS Pinning Configuration (PennyOwnerPinnedSession & sub_4165b0) ===")
    print(f"  Pinned Host (0x445e99 / CFString@0x44cef8): {read_cstr(0x445e99)!r}")
    for idx, addr in enumerate([0x445EAE, 0x445EDB, 0x445F08], 1):
        print(f"  SHA-256 Pin {idx} @ {addr:#08x}: {read_cstr(addr)!r}")
    print(f"  RSA-2048 SPKI ASN.1 Header @ 0x43ba38 (24B): {hff[0x43ba38:0x43ba50].hex()}")

    print("\n=== 4. Backend Error Dictionary (__objc_dictobj @ 0x44d980 / __objc_arraydata @ 0x44d900) ===")
    keys = [cf_map[struct.unpack("<Q", hff[0x44D900 + i * 8 : 0x44D908 + i * 8])[0]] for i in range(8)]
    vals = [cf_map[struct.unpack("<Q", hff[0x44D940 + i * 8 : 0x44D948 + i * 8])[0]] for i in range(8)]
    for k, v in zip(keys, vals):
        print(f"  {k!r:16s} -> {v!r}")

    print("\n=== 5. Ed25519 Ref10 + SHA-512 Unicorn Verification (sub_421714) ===")
    print(f"  Embedded Server Ed25519 Public Key @ 0x444952: {read_cstr(0x444952)}")
    print(f"  Group Order L @ 0x4447e0 (32B): {hff[0x4447e0:0x444800].hex()}")
    print("  Canonical Message Format (built in sub_4086f8 @ 0x408b9c..0x408db8):")
    print("    <valid_bit>|<user>|<expiry>|<hwid>|<nonce>|<server_time>")

    sk = b"hunterff_test_secret_key_32bytes"
    msg = b"1|aero_user|2028-05-05 03:42:35|HWID-123456|0123456789abcdef0123456789abcdef|1794268800"
    pk, sig = ed25519_sign_test(msg, sk)

    mu = uc.Uc(uc.UC_ARCH_ARM64, uc.UC_MODE_ARM)
    mu.mem_map(0x0, 0x500000)
    mu.mem_write(0x0, hff[:0x458000])
    STACK_BASE = 0x70000000
    STACK_SIZE = 0x200000
    mu.mem_map(STACK_BASE, STACK_SIZE)
    STUB_BASE = 0x80000000
    mu.mem_map(STUB_BASE, 0x100000)
    DATA_BUF = 0x90000000
    mu.mem_map(DATA_BUF, 0x100000)
    cpacr = mu.reg_read(ua.UC_ARM64_REG_CPACR_EL1)
    mu.reg_write(ua.UC_ARM64_REG_CPACR_EL1, cpacr | 0x300000)

    stub_to_sym = {}
    sym_to_stub = {}
    idx = 0
    for b_entry in mh.bindings:
        sym = b_entry.symbol.name if b_entry.has_symbol else f"bind_{b_entry.address:#x}"
        if sym not in sym_to_stub:
            s_va = STUB_BASE + idx * 8
            mu.mem_write(s_va, b"\xc0\x03\x5f\xd6")
            stub_to_sym[s_va] = sym
            sym_to_stub[sym] = s_va
            idx += 1
        mu.mem_write(b_entry.address, struct.pack("<Q", sym_to_stub[sym]))

    def hook_stubs(uc_e, addr, sz, ud):
        if addr in stub_to_sym:
            sym = stub_to_sym[addr]
            x0 = uc_e.reg_read(ua.UC_ARM64_REG_X0)
            x1 = uc_e.reg_read(ua.UC_ARM64_REG_X1)
            x2 = uc_e.reg_read(ua.UC_ARM64_REG_X2)
            if sym in ("_memcpy", "_memmove"):
                uc_e.mem_write(x0, bytes(uc_e.mem_read(x1, x2)))
            elif sym == "_bzero":
                uc_e.mem_write(x0, b"\x00" * x1)
            elif sym == "_memset":
                uc_e.mem_write(x0, bytes([x1 & 0xFF]) * x2)

    mu.hook_add(uc.UC_HOOK_CODE, hook_stubs, begin=STUB_BASE, end=STUB_BASE + 0x20000)

    def verify_sig_in_unicorn(sig_b: bytes, msg_b: bytes, pk_b: bytes) -> int:
        sig_va = DATA_BUF + 0x1000
        pk_va = DATA_BUF + 0x2000
        msg_va = DATA_BUF + 0x3000
        mu.mem_write(sig_va, sig_b)
        mu.mem_write(pk_va, pk_b)
        mu.mem_write(msg_va, msg_b)
        sp = STACK_BASE + STACK_SIZE - 0x10000
        ret_va = STUB_BASE + 0xF0000
        mu.mem_write(ret_va, b"\x00\x00\x00\x14")
        mu.reg_write(ua.UC_ARM64_REG_SP, sp)
        mu.reg_write(ua.UC_ARM64_REG_X0, sig_va)
        mu.reg_write(ua.UC_ARM64_REG_X1, msg_va)
        mu.reg_write(ua.UC_ARM64_REG_X2, len(msg_b))
        mu.reg_write(ua.UC_ARM64_REG_X3, pk_va)
        mu.reg_write(ua.UC_ARM64_REG_LR, ret_va)
        mu.emu_start(0x421714, ret_va, timeout=20 * uc.UC_SECOND_SCALE, count=0)
        return mu.reg_read(ua.UC_ARM64_REG_X0)

    r_ok = verify_sig_in_unicorn(sig, msg, pk)
    r_bad = verify_sig_in_unicorn(sig, msg + b"x", pk)
    print(f"  sub_421714(valid RFC 8032 signature)   -> {r_ok} (expected 1)")
    print(f"  sub_421714(tampered canonical message) -> {r_bad} (expected 0)")


if __name__ == "__main__":
    main()
