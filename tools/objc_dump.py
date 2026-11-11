#!/usr/bin/env python3
import struct
import sys


def dump_objc_metadata(path):
    with open(path, "rb") as f:
        raw = f.read()

    def r64(va):
        return struct.unpack("<Q", raw[va : va + 8])[0] & 0xFFFFFFFFFFFF

    def r32(va):
        return struct.unpack("<I", raw[va : va + 4])[0]

    def r32s(va):
        return struct.unpack("<i", raw[va : va + 4])[0]

    def cstr(va):
        if va == 0 or va >= len(raw):
            return ""
        end = raw.find(b"\0", va)
        return raw[va:end].decode("utf-8", "replace")

    def dump_methods(mlist_va, prefix="-"):
        if not mlist_va:
            return
        entsize_flags = r32(mlist_va)
        count = r32(mlist_va + 4)
        is_small = (entsize_flags & 0x80000000) != 0
        entsize = entsize_flags & 0xFFFF
        cur = mlist_va + 8
        for _ in range(count):
            if is_small:
                name_off = r32s(cur)
                types_off = r32s(cur + 4)
                imp_off = r32s(cur + 8)
                name_ptr = cur + name_off
                if 0x6600000 <= name_ptr < 0x6900000:
                    mname = cstr(name_ptr)
                else:
                    mname = cstr(r64(name_ptr))
                mtypes = cstr(cur + 4 + types_off)
                imp = (cur + 8 + imp_off) & 0xFFFFFFFFFFFFFFFF
            else:
                mname = cstr(r64(cur))
                mtypes = cstr(r64(cur + 8))
                imp = r64(cur + 16)
            print(f"    {prefix}[{mname}] imp={imp:#x} ({mtypes})")
            cur += entsize

    def dump_class(cls_va):
        isa = r64(cls_va)
        ro = r64(cls_va + 32) & ~7
        if not ro or ro >= len(raw):
            return
        flags = r32(ro)
        name_va = r64(ro + 24)
        base_methods = r64(ro + 32)
        base_ivars = r64(ro + 48)
        cname = cstr(name_va)
        print(f"CLASS {cname} @ {cls_va:#x} (ro={ro:#x} flags={flags:#x})")
        if isa and isa < len(raw):
            meta_ro = r64(isa + 32) & ~7
            if meta_ro and meta_ro < len(raw):
                meta_methods = r64(meta_ro + 32)
                dump_methods(meta_methods, "+")
        dump_methods(base_methods, "-")
        if base_ivars:
            entsize = r32(base_ivars)
            icount = r32(base_ivars + 4)
            cur = base_ivars + 8
            for _ in range(icount):
                off_ptr = r64(cur)
                iname = cstr(r64(cur + 8))
                itype = cstr(r64(cur + 16))
                off_val = r32(off_ptr) if off_ptr else 0
                print(f"    IVAR +{off_val:#x} {iname} : {itype}")
                cur += entsize

    # __objc_classlist va=0xcc61500 sz=0x248
    for off in range(0xCC61500, 0xCC61500 + 0x248, 8):
        dump_class(r64(off))


if __name__ == "__main__":
    target = (
        sys.argv[1]
        if len(sys.argv) > 1
        else "/tmp/dl/ipa/Payload/FreeFire.app/Frameworks/Penny.framework/Penny"
    )
    dump_objc_metadata(target)
