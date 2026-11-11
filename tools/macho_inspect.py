#!/usr/bin/env python3
import struct
import sys

LC = {
    0x19: "SEGMENT_64",
    0xC: "LOAD_DYLIB",
    0x18 | 0x80000000: "LOAD_WEAK_DYLIB",
    0xD: "ID_DYLIB",
    0x2C: "ENCRYPTION_INFO_64",
    0x1B: "UUID",
    0x32: "BUILD_VERSION",
    0x1D: "CODE_SIGNATURE",
    0x1C | 0x80000000: "RPATH",
    0x80000028: "MAIN",
    0x1F | 0x80000000: "REEXPORT_DYLIB",
}


def inspect_macho64(f, base, show_sections=False):
    f.seek(base)
    magic, cpu, sub, ftype, ncmds, sizecmds, flags, _ = struct.unpack("<8I", f.read(32))
    print(f"  cputype={cpu:#x} subtype={sub:#x} filetype={ftype} ncmds={ncmds} flags={flags:#x}")
    off = base + 32
    for _ in range(ncmds):
        f.seek(off)
        cmd, size = struct.unpack("<2I", f.read(8))
        body = f.read(size - 8)
        name = LC.get(cmd)
        if name == "SEGMENT_64":
            seg = body[:16].rstrip(b"\0").decode("utf-8", "replace")
            vmaddr, vmsize, fileoff, filesize, maxp, initp, nsects, fl = struct.unpack("<4Q4I", body[16:64])
            print(
                f"  SEG {seg:<16} vm={vmaddr:#x} vmsize={vmsize:#x} "
                f"fileoff={fileoff:#x} filesize={filesize:#x} nsects={nsects}"
            )
            if show_sections:
                for i in range(nsects):
                    s = body[64 + i * 80 : 64 + (i + 1) * 80]
                    sn = s[:16].rstrip(b"\0").decode("utf-8", "replace")
                    addr, sz, so = struct.unpack("<2QI", s[32:52])
                    print(f"      SEC {sn:<18} addr={addr:#x} size={sz:#x} off={so:#x}")
        elif name in ("LOAD_DYLIB", "LOAD_WEAK_DYLIB", "ID_DYLIB", "REEXPORT_DYLIB"):
            so = struct.unpack("<I", body[:4])[0]
            dylib_name = body[so - 8 :].split(b"\0")[0].decode("utf-8", "replace")
            print(f"  {name:<16} {dylib_name}")
        elif name == "RPATH":
            so = struct.unpack("<I", body[:4])[0]
            rpath = body[so - 8 :].split(b"\0")[0].decode("utf-8", "replace")
            print(f"  RPATH            {rpath}")
        elif name == "ENCRYPTION_INFO_64":
            co, cs, cid = struct.unpack("<3I", body[:12])
            print(f"  ENCRYPTION       cryptoff={co:#x} cryptsize={cs:#x} cryptid={cid}")
        elif name == "UUID":
            print(f"  UUID             {body[:16].hex()}")
        elif name == "BUILD_VERSION":
            plat, minos, sdk, _ = struct.unpack("<4I", body[:16])
            fmt_ver = lambda x: f"{x >> 16}.{(x >> 8) & 255}.{x & 255}"
            print(f"  BUILD_VERSION    platform={plat} minos={fmt_ver(minos)} sdk={fmt_ver(sdk)}")
        elif name == "CODE_SIGNATURE":
            do, ds = struct.unpack("<2I", body[:8])
            print(f"  CODE_SIGNATURE   off={do:#x} size={ds:#x}")
        off += size


def parse_file(path, show_sections=False):
    with open(path, "rb") as f:
        magic = struct.unpack("<I", f.read(4))[0]
        print(f"== {path}")
        if magic in (0xBEBAFECA, 0xCAFEBABE):
            f.seek(4)
            n = struct.unpack(">I", f.read(4))[0]
            archs = [struct.unpack(">5I", f.read(20)) for _ in range(n)]
            print("  FAT archs:", [(hex(a[0]), hex(a[1]), a[2], a[3]) for a in archs])
            for a in archs:
                inspect_macho64(f, a[2], show_sections=show_sections)
        elif magic == 0xFEEDFACF:
            inspect_macho64(f, 0, show_sections=show_sections)
        else:
            print("  not Mach-O 64, magic", hex(magic))


if __name__ == "__main__":
    show_sec = "-s" in sys.argv
    targets = [p for p in sys.argv[1:] if p != "-s"]
    if not targets:
        print(f"Usage: {sys.argv[0]} [-s] <macho-path> [...]")
        sys.exit(1)
    for target in targets:
        parse_file(target, show_sections=show_sec)
