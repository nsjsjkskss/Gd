# Staged q3 update pipeline

`q3_update.py`: discover official codepatch changes, validate the pinned menu
against the supplied game's API dump, merge without shifting official indices,
run preservation and upstream VM checks, and write a **test candidate only**.
There is no publish command. The hourly systemd service has read-only access to
the live OKMOD directory and can only write under `var/q3-update/`.

## Current bindings

- Content feed: `/ios/1.132.11/fileinfo`.
- App build: `1.132.1/2019121207`.
- Pinned donor: user-tested R8-Batch-v1, SHA-256
  `cec815c513cd87279af8bc954119512bb33fb0ab1d3f7e75eaa646e24040c435`.
- Local baseline q3: 138 bodies, 79 fixes, 10 storeys. These counts are read from
  each new official file and are **not** fixed in the merger.
- API dump, metadata, donor, opcode map and VM runner are pinned by SHA-256.

## Run manually

```sh
export PYTHONPATH=/opt/ilvenv/lib/python3.12/site-packages
python3 /opt/okmod-sophia/tools/q3_update.py check
python3 /opt/okmod-sophia/tools/q3_update.py run
# Build the current official q3 too, even when unchanged:
python3 /opt/okmod-sophia/tools/q3_update.py run --force
```

Results: `/opt/okmod-sophia/var/q3-update/latest.json` and timestamped `runs/`.
`last-candidate.json` points to the latest successful staged build. An unchanged
feed does not rebuild or replace that candidate. A blocked run records the error
and exits nonzero. Check scheduled status with:

```sh
systemctl status q3-update.timer
journalctl -u q3-update.service --no-pager -n 40
```

## Gates

1. Fetch current configured official feed; require one codepatch row and validate
   compressed/raw SHA-1 and byte sizes. SHA-1 here checks the manifest transport
   identity; it is not a vendor signature.
2. Parse IFix and require exact serialization roundtrip. Preserve original
   anonymous-storey bytes, static slots, loader identities and new-field metadata.
   Nonzero new-class sections/ambiguous tails are unsupported and block.
3. Follow the pinned donor's two hooks and their internal calls/field initializers;
   only the reachable graph is imported. Negative initializer sentinel -1 stays -1.
   Duplicate official/menu hook target or dynamic field name conflicts block.
4. Check donor dependencies against the bound API dump: 124 exact signatures,
   static/argument ABI, fields, types, hook IDMAP slots and opcode values on the
   current donor. These counts are report data, not hardcoded acceptance limits.
5. Append remapped helper code and used tables; official types, methods, fields,
   strings, bodies/EH, fixes and raw storeys stay identical and at the same indices.
   Validate every imported instruction/EH reference and bundle extraction roundtrip.
6. Export the resulting HUD graph to the pinned upstream VM runner. Behavioral,
   exception, GC, mesh/fallback and stack tests must pass before candidate ready.
7. Write `candidate-test.bin`, checksums, `report.json`, `relocation.json`, VM log
   and device-test instructions. Mark `device_test=pending`, `published=false`.

The compact initial candidate is ~44KB instead of ~264KB because it carries only
37 reachable R8 helpers plus the official base. It has not been device-tested and
is not interchangeable with the live artifact merely because VM tests pass.

## Game app/build update versus hotfix update

The hourly monitor watches **the configured content version**, not an inferred
new IPA version. A changed q3 within this version can produce a staged candidate
against the existing API binding. A codepatch alone cannot prove whether the
installed app/native APIs changed. When the game app/content version changes,
obtain a matching dump/metadata and initialize a new config/root (do not just
edit the version string). Mismatched `api_binding` blocks building.

```sh
python3 /opt/okmod-sophia/tools/q3_update.py --config /path/new-version/config.json init \
  --donor /path/tested-menu.bin --official /path/new-official.bin \
  --api-dump /path/new-game-dump.cs --metadata /path/new-global-metadata.dat \
  --vm-runner /path/VmAudit.exe --version NEW.CONTENT.VERSION \
  --app-build APP.VERSION/BUILD --platform ios
```

The importer requires the same bridge/assembly contract. If names/assembly/opcode
or hook signatures change, updating the menu/native bindings is a separate task;
the pipeline does not guess replacements. Dump validation is not a native AOT or
on-device API execution test. Storeys are preserved as opaque bytes, not relocated.

## Live release

Review the candidate report, test the exact hash on the target device (initial
menu/touch, real-player ESP, respawn/lobby transitions, heap/GC and visuals), then
make a separate explicit release decision. No scheduled job publishes, restarts
the game service or changes ProxyPin configuration.
