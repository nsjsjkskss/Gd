#!/usr/bin/env python3
"""Hard core guard for OKMOD codepatch candidates.

This is intentionally conservative: a candidate must keep the q3 prefix, keep
the R8 renderer architecture, and must not make the old DrawEsp body reachable
again. It is a release gate, not a style checker.
"""
import argparse
import gzip
import hashlib
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import patchc
from q3update import format as F

KIT = Path(__file__).resolve().parents[1]
DEFAULT_BASELINE = KIT / 'patches' / 'baseline.patch'
DEFAULT_BASE = KIT / 'patches' / 'menuonly.patch'
DEFAULT_REPORT = KIT.parent / 'build' / 'build-aim-report.json'
FORBIDDEN_BODIES = set(range(319, 330))   # old HUD DrawEsp + old helper cluster


def parse_candidate(path):
    packed = Path(path).read_bytes()
    raw = F.extract(packed) if packed.startswith(b'\x1f\x8b') else packed
    if not raw.startswith(patchc.MAGIC):
        raw = F.extract(packed)
    return F.parse(raw), packed


def body_equal(a, b):
    return a.instrs == b.instrs and a.eh == b.eh


def fix_key(p, ref):
    return (p.types[ref.tid], ref.name, tuple(ref.nums), ref.isg)


def closure(p, roots):
    seen, todo = set(), [int(x) for x in roots]
    while todo:
        bid = todo.pop()
        if bid in seen or not (0 <= bid < len(p.bodies)):
            continue
        seen.add(bid)
        for code, value in p.bodies[bid].instrs:
            if p.op_name(code) == 'Call':
                todo.append(value & 65535)
    return seen


def extern_owners(p, bodies):
    owners = set()
    for bid in bodies:
        for code, value in p.bodies[bid].instrs:
            if p.op_name(code) in ('CallExtern', 'Newobj'):
                index = value & 65535
                if 0 <= index < len(p.extm):
                    owners.add(p.types[p.extm[index].tid].split(',')[0])
    return owners


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--candidate', required=True)
    ap.add_argument('--report', default=str(DEFAULT_REPORT))
    ap.add_argument('--baseline', default=str(DEFAULT_BASELINE))
    ap.add_argument('--base', default=str(DEFAULT_BASE))
    ap.add_argument('--mode', choices=('r8', 'r8-aim'), default='r8-aim')
    ap.add_argument('--profile', choices=('full', 'nohook'), default='full')
    ap.add_argument('--forbidden', default='319-329')
    ap.add_argument('--json', action='store_true')
    args = ap.parse_args()

    q3 = F.parse(Path(args.baseline).read_bytes())
    base = F.parse(Path(args.base).read_bytes())
    cand, packed = parse_candidate(args.candidate)
    report = json.loads(Path(args.report).read_text()) if Path(args.report).is_file() else {}
    helpers = {k: int(v) for k, v in report.get('helpers', {}).items()}
    checks, failures = [], []

    def check(name, ok, detail=''):
        checks.append({'check': name, 'ok': bool(ok), 'detail': detail})
        if not ok:
            failures.append(name)

    # 1. Official q3 prefix is immutable.
    check('q3 bodies unchanged',
          len(cand.bodies) >= len(q3.bodies) and
          all(body_equal(cand.bodies[i], q3.bodies[i]) for i in range(len(q3.bodies))),
          '%d/%d' % (len(q3.bodies), len(cand.bodies)))
    check('q3 fixes unchanged',
          len(cand.fixes) >= len(q3.fixes) and
          all(fix_key(cand, cand.fixes[i][0]) == fix_key(q3, q3.fixes[i][0]) and
              cand.fixes[i][1] == q3.fixes[i][1]
              for i in range(len(q3.fixes))),
          '%d/%d' % (len(q3.fixes), len(cand.fixes)))
    check('q3 storeys unchanged',
          cand.storeys == q3.storeys and cand.storey_raw == q3.storey_raw,
          '%d storeys, %d bytes' % (cand.storeys, len(cand.storey_raw)))
    check('q3 type/intern/sft prefix',
          cand.types[:len(q3.types)] == q3.types and
          cand.interns[:len(q3.interns)] == q3.interns and cand.sft == q3.sft)

    # 2. R8 menu-only base is the only append point.
    body_diffs = [i for i in range(min(len(cand.bodies), len(base.bodies)))
                  if not body_equal(cand.bodies[i], base.bodies[i])]
    check('R8 body prefix unchanged (known redirects allowed)',
          len(cand.bodies) >= len(base.bodies) and
          all(i in (319, 329) for i in body_diffs),
          'diffs=%s bodies=%d/%d' % (body_diffs, len(base.bodies), len(cand.bodies)))
    # UI/diagnostic strings may be renamed in the intern table. The q3 intern
    # prefix is already checked above; the R8 base type table is the hard part.
    check('R8 type prefix unchanged',
          cand.types[:len(base.types)] == base.types)
    check('R8 storey prefix',
          cand.storeys == base.storeys and cand.storey_raw == base.storey_raw)

    # Base fixes keep their hook identities; OnGUI may be redirected.
    base_ok = True
    for i, (ref, bid) in enumerate(base.fixes):
        if i >= len(cand.fixes):
            base_ok = False; break
        if (ref.name == 'OnGUI'):
            continue
        if fix_key(cand, cand.fixes[i][0]) != fix_key(base, ref) or cand.fixes[i][1] != bid:
            base_ok = False; break
    check('R8 fix identities preserved (OnGUI redirect allowed)', base_ok)

    # 3. Old core must not be reachable from OnGUI.
    ongui = next(((m, b) for m, b in cand.fixes if m.name == 'OnGUI'), None)
    check('OnGUI fix present', ongui is not None)
    reach = closure(cand, [ongui[1]]) if ongui else set()
    # Body 319 is the old DrawEsp slot. It may remain as a small redirect shim
    # (stable R8) or be completely unreachable (new HUD). It must never remain
    # the old per-player loop.
    old_body = 319
    old_reachable = old_body in reach
    shim = False
    if old_reachable and len(cand.bodies) > old_body:
        ins = cand.bodies[old_body].instrs
        ops = [cand.op_name(c) for c, v in ins]
        calls = [v & 65535 for c, v in ins if cand.op_name(c) == 'Call']
        shim = (len(ins) <= 32 and 'Ret' in ops and
                len(calls) == 1 and 'DrawEsp' in helpers and
                calls[0] == helpers['DrawEsp'])
    check('old DrawEsp is shimmed or unreachable',
          (not old_reachable) or shim,
          'reachable=%s shim=%s' % (old_reachable, shim))

    if 'DrawEsp' in helpers:
        check('new DrawEsp reachable', helpers['DrawEsp'] in reach,
              'id=%d' % helpers['DrawEsp'])
        check('new DrawEsp is not old body', helpers['DrawEsp'] != old_body)

    # The old full-source graph has these helpers reachable. Their presence in
    # a report means the old per-frame DrawEsp path was compiled back in.
    forbidden_helpers = {'DrawTeammate', 'SetEffectLine', 'DisableLines',
                         'DrawLine', 'DrawBox'}
    present_old = sorted(forbidden_helpers & set(helpers))
    check('old helper graph absent', not present_old, str(present_old))

    required = {'DrawEsp', 'Render', 'DrawBatch', 'Sample'}
    if args.mode == 'r8-aim':
        required |= {'Hud'}
        if args.profile == 'full':
            required |= {'SilentAimingInfo', 'ScatterRate'}
    missing = sorted(required - set(helpers))
    check('R8 helper graph complete', not missing, str(missing))

    # 4. Cache renderer must be pure Unity/System.
    for name in ('Render', 'DrawBatch'):
        if name not in helpers:
            check('%s helper id present' % name, False)
            continue
        ids = closure(cand, [helpers[name]])
        owners = extern_owners(cand, ids)
        cow = sorted(o for o in owners if o.startswith('COW.'))
        check('%s has no COW.* calls' % name, not cow, str(cow))
        check('%s does not call Sample' % name,
              'Sample' not in helpers or helpers['Sample'] not in ids)
        check('%s avoids old core cluster' % name, not (ids & FORBIDDEN_BODIES))

    # 5. AIM mode contract.
    risky = {'get_LastAimingInfoFromWeapon', 'GetScatterRate',
             'get_ShowDamageNum', 'get_CustomGravity',
             'get_RisingGravity', 'get_FallingGravity'}
    if args.mode == 'r8-aim' and args.profile == 'full':
        sil = next(((m, b) for m, b in cand.fixes
                    if m.name == 'get_LastAimingInfoFromWeapon'), None)
        check('Silent getter hook present', sil is not None)
        if sil and 'SilentAimingInfo' in helpers:
            check('Silent hook points at new body',
                  sil[1] == helpers['SilentAimingInfo'],
                  '%d/%d' % (sil[1], helpers['SilentAimingInfo']))
    if args.profile == 'nohook':
        present = sorted(m.name for m, b in cand.fixes if m.name in risky)
        check('nohook profile adds no risky hooks', not present, str(present))
        for field in ('Zx3de8320', 'Zx5b0b2a5', 'Zx8c4b660', 'Zxe63a668'):
            check('advanced ESP field %s' % field,
                  any(f.name == field and f.isnew for f in cand.fields))

    # 6. No forbidden fix bodies anywhere in the fix table except known old
    # bodies that are unreachable (they remain in the table for compatibility).
    check('fix table size sane', len(cand.fixes) in (len(base.fixes), len(base.fixes) + 1, len(base.fixes) + 2, len(base.fixes) + 3, len(base.fixes) + 4, len(base.fixes) + 5, len(base.fixes) + 6, len(base.fixes) + 7, len(base.fixes) + 8, len(base.fixes) + 9, len(base.fixes) + 10, len(base.fixes) + 11, len(base.fixes) + 12),
          '%d base %d candidate' % (len(base.fixes), len(cand.fixes)))

    result = {
        'candidate': str(Path(args.candidate)),
        'sha256': hashlib.sha256(packed).hexdigest(),
        'bodies': len(cand.bodies), 'fixes': len(cand.fixes),
        'storeys': cand.storeys, 'mode': args.mode,
        'ok': not failures, 'checks': checks, 'failures': failures,
    }
    if args.json:
        print(json.dumps(result, indent=2))
    else:
        for c in checks:
            print(('PASS ' if c['ok'] else 'FAIL ') + c['check'] +
                  ((' | ' + c['detail']) if c['detail'] else ''))
        print(('PASS ' if not failures else 'FAIL ') + 'CORE GUARD: ' +
              (str(len(checks)) + ' checks' if not failures else ','.join(failures)))
    return 0 if not failures else 1


if __name__ == '__main__':
    raise SystemExit(main())
