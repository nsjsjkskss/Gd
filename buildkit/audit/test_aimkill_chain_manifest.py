#!/usr/bin/env python3
"""Regression checks for the read-only AimKill chain manifest."""
import json
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
MANIFEST = ROOT / "build" / "aimkill-chain-manifest.json"


def main():
    data = json.loads(MANIFEST.read_text())
    methods = {(m["owner"], m["name"]): m for m in data["methods"]}
    expected = {
        ("COW.GamePlay.HBIBDMMOOOK", "CIHHEOKFADJ"): 3,
        ("COW.GamePlay.HBIBDMMOOOK", "LHNJLFOINDO"): 1,
        ("COW.GamePlay.HBIBDMMOOOK", "NPLMKLBKJPE"): 4,
        ("COW.GamePlay.Player", "JGIEOGCGFEP"): 1,
        ("COW.GamePlay.PlayerNetwork", "TakeDamage"): 9,
    }
    assert set(methods) == set(expected)
    for key, arity in expected.items():
        row = methods[key]
        assert row["parameter_count"] == arity
        assert len(row["parameters"]) == arity
        assert all(isinstance(item["raw_type_index"], int)
                   for item in row["parameters"])
    assert data["metadata"]["version"] == 31
    assert data["fresh_dump_declarations"]
    assert data["legacy_dump_declarations"]
    print("PASS: AimKill source/metadata manifest has 5 exact method entries")


if __name__ == "__main__":
    main()
