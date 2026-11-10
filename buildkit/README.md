# OKMOD local build kit

Everything needed to build `/root/okmod-sophia/cs/r8-aim` without `/tmp/opencode`
or `/opt/okmod-sophia`:

```text
tools/       cs2patch, ifixc, patchc, q3update, check_core_rules, core_guard
pylibs/      dncil, dnfile, pefile, peutils, ordlookup (IL reader)
patches/     menuonly.patch, merged.patch, baseline.patch
audit/       merge_q3_hbs.py, q3.patch, global-metadata.dat
r6-lite/     stubs.cs (stub source used by build.py)
```

Build + guard + publish (copy-only, keeps a backup of the current patch):

```bash
cd /root/okmod-sophia/cs/r8-aim
./deploy.sh
```

Build only:

```bash
cd /root/okmod-sophia/cs/r8-aim
python3 build.py
```

Guard only:

```bash
../buildkit/tools/core_guard.sh \
  --candidate okmod-R8-AIM-ESP.bin \
  --report build-aim-report.json \
  --mode r8-aim
```

The running proxy remains at `/opt/okmod-1297` (patch.bin, addon, CA); the
source and build dependencies now live entirely under
`/root/okmod-sophia/cs`.
