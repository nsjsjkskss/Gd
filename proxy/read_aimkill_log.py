#!/usr/bin/env python3
"""Decode the dedicated nginx AimKill JSONL access log."""
import argparse
import json
from collections import deque
from pathlib import Path
from urllib.parse import parse_qs


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--path", type=Path, default=Path("/var/log/nginx/okmod-aimkill.jsonl"))
    parser.add_argument("-n", type=int, default=10)
    args = parser.parse_args()
    if args.n < 1:
        parser.error("-n must be positive")
    with args.path.open(encoding="utf-8") as source:
        for line in deque(source, maxlen=args.n):
            row = json.loads(line)
            query = parse_qs(row.pop("args"), keep_blank_values=True)
            row.update({key: values[-1] for key, values in query.items()})
            log = row.pop("d", "")
            print(json.dumps(row, ensure_ascii=False))
            print(log)


if __name__ == "__main__":
    main()
