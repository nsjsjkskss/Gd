#!/usr/bin/env python3
"""Read-only metadata manifest for the PC AimKill damage-chain names.

This tool does not modify C# sources, IFix tables, payloads, or the proxy.  It
only records the method owner, metadata method index, raw parameter type
indices, and the corresponding dump declaration so aliasing in an obfuscated
dump is visible instead of being silently trusted.
"""
from __future__ import annotations

import argparse
import json
import re
import struct
from pathlib import Path


DEFAULT_TARGETS = [
    ("COW.GamePlay.HBIBDMMOOOK", "NPLMKLBKJPE"),
    ("COW.GamePlay.PlayerNetwork", "TakeDamage"),
    ("COW.GamePlay.Player", "JGIEOGCGFEP"),
    ("COW.GamePlay.HBIBDMMOOOK", "LHNJLFOINDO"),
    ("COW.GamePlay.HBIBDMMOOOK", "CIHHEOKFADJ"),
]


def read_pairs(data: bytes, offset: int) -> tuple[int, int]:
    return struct.unpack_from("<II", data, offset)


def metadata_index(data: bytes, offset: int, count: int, width: int = 40):
    return [struct.unpack_from("<IIiiiiIIHHHH", data, offset + i * width)
            for i in range(count)]


def read_c_string(data: bytes, start: int) -> str:
    end = data.find(b"\0", start)
    if end < 0:
        return "<unterminated>"
    return data[start:end].decode("utf-8", "replace")


def parse_metadata(path: Path, method_names: set[tuple[str, str]]):
    data = path.read_bytes()
    version = struct.unpack_from("<I", data, 4)[0]
    strings_off, _ = read_pairs(data, 24)
    methods_off, methods_size = read_pairs(data, 48)
    params_off, params_size = read_pairs(data, 56)
    type_defs_off, type_defs_size = read_pairs(data, 72)
    if version != 31 or methods_size % 40 != 0 or params_size % 12 != 0:
        raise RuntimeError(
            f"expected metadata v31/v31.1 layout; version={version}, "
            f"methods={methods_size}, params={params_size}"
        )

    classes = json.loads(Path("/root/ffdump/ffmeta_classes.json").read_text())
    class_by_name = {item["name"]: item for item in classes}
    method_rows = {}
    methods_tsv = Path("/root/ffdump/ffmeta_methods.tsv")
    for line in methods_tsv.read_text(errors="replace").splitlines():
        parts = line.split("\t")
        if len(parts) != 4:
            continue
        index, tix, assembly, name = parts
        for owner, wanted in method_names:
            short_owner = owner.split(".")[-1]
            if int(tix) == class_by_name.get(owner, {}).get("tix", -1) and name == wanted:
                method_rows[(owner, wanted, int(index))] = {
                    "method_index": int(index),
                    "type_index": int(tix),
                    "assembly": assembly,
                    "name": name,
                }

    result = []
    for owner, name in sorted(method_names):
        matches = [row for key, row in method_rows.items()
                   if key[0] == owner and key[1] == name]
        owner_meta = class_by_name.get(owner)
        for row in matches:
            raw = metadata_index(data, methods_off, methods_size // 40)[row["method_index"]]
            # v31.1 layout: name, declaring type, return type, return token,
            # parameter start, generic container, extra field, token, flags,
            # iflags, slot, parameter count.
            parameter_start = raw[4]
            parameter_count = raw[-1]
            params = []
            for i in range(parameter_count):
                name_index, token, type_index = struct.unpack_from(
                    "<III", data, params_off + (parameter_start + i) * 12
                )
                params.append({
                    "ordinal": i,
                    "name": read_c_string(data, strings_off + name_index),
                    "token": token,
                    "raw_type_index": type_index,
                })
            result.append({
                **row,
                "owner": owner,
                "owner_type_index": owner_meta["tix"] if owner_meta else None,
                "return_type_index": raw[2],
                "parameter_start": parameter_start,
                "parameter_count": parameter_count,
                "parameters": params,
            })
    return {
        "metadata": {
            "path": str(path),
            "version": version,
            "methods_offset": methods_off,
            "methods_size": methods_size,
            "parameters_offset": params_off,
            "parameters_size": params_size,
            "type_definitions_offset": type_defs_off,
            "type_definitions_size": type_defs_size,
        },
        "methods": result,
    }


def _type_header(line: str):
    match = re.match(
        r"(?:public|internal|private|protected)\s+"
        r"(?:sealed\s+|abstract\s+|static\s+)*"
        r"(?:class|struct|enum|interface)\s+([^ :{]+)",
        line.strip(),
    )
    return match.group(1) if match else None


def dump_declarations(path: Path, wanted: set[tuple[str, str]]):
    """Collect declarations only while inside the requested declaring class."""
    lines = path.read_text(errors="replace").splitlines()
    found = []
    namespace = ""
    current_type = ""
    brace_depth = 0
    class_depth = None
    requested_short = {owner.rsplit(".", 1)[-1] for owner, _ in wanted}
    for line_no, line in enumerate(lines, 1):
        ns = re.match(r"// Namespace:\s*(.*)$", line)
        if ns:
            namespace = ns.group(1).strip()
            current_type = ""
            class_depth = None
        header = _type_header(line)
        if header:
            current_type = header
            class_depth = brace_depth + (line.count("{") - line.count("}"))
        delta = line.count("{") - line.count("}")
        in_requested_class = current_type in requested_short and class_depth is not None
        if in_requested_class:
            for owner, name in wanted:
                if owner.rsplit(".", 1)[-1] != current_type:
                    continue
                if re.search(r"\b" + re.escape(name) + r"\s*\(", line):
                    found.append({
                        "line": line_no,
                        "namespace": namespace,
                        "declaring_type_seen": current_type,
                        "owner_requested": owner,
                        "method_requested": name,
                        "declaration": line.strip(),
                    })
        brace_depth += delta
        if class_depth is not None and brace_depth < class_depth:
            current_type = ""
            class_depth = None
    return found


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--metadata", type=Path,
                    default=Path("/root/okmod-1297-clean/buildkit/audit/global-metadata.dat"))
    ap.add_argument("--fresh-dump", type=Path,
                    default=Path("/tmp/opencode/dumpfresh/dump.cs"))
    ap.add_argument("--legacy-dump", type=Path,
                    default=Path("/root/ffdump/com.dts.freefiremax-dump.cs"))
    ap.add_argument("--output", type=Path,
                    default=Path("/root/okmod-1297-clean/build/aimkill-chain-manifest.json"))
    args = ap.parse_args()
    wanted = set(DEFAULT_TARGETS)
    out = parse_metadata(args.metadata, wanted)
    out["fresh_dump_declarations"] = dump_declarations(args.fresh_dump, wanted)
    out["legacy_dump_declarations"] = dump_declarations(args.legacy_dump, wanted)
    # Preserve the current dump's exact aliases separately.  This is the
    # candidate IFix spelling for this metadata generation; the legacy names
    # are retained only as a comparison, never silently substituted.
    current = {}
    for row in out["fresh_dump_declarations"]:
        key = (row["owner_requested"], row["method_requested"])
        current.setdefault(key, row["declaration"])
    out["current_dump_ifix_spelling"] = [
        {
            "owner": owner,
            "method": method,
            "declaration": current.get((owner, method)),
        }
        for owner, method in sorted(wanted)
    ]
    out["scope"] = "read-only metadata/dump audit; no IFix or payload changes"
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(out, indent=2) + "\n")
    print(json.dumps(out, indent=2))


if __name__ == "__main__":
    main()
