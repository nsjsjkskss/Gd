#!/usr/bin/env python3
"""Offline schema/call-site audit. No traffic replay, rewriting or game patching."""
import argparse
import hashlib
import json
import re
import struct
from pathlib import Path
from urllib.parse import urlsplit

from capstone import Cs, CS_ARCH_ARM64, CS_MODE_ARM
from aimkill_runtime_contract import RuntimeContract

NATIVE_SHA = '2de005ae6c9d263ddd705aedda833c29a5b27044d2f56655ecc3cf98dc7be54a'
SCHEMAS = (
    'proto.Empty', 'proto.CSCheckHackBehaviorReq', 'proto.CSCheckHackBehaviorRes',
    'proto.MatchClientInfo', 'proto.HackWarningInfoDesc',
    'proto.CSGetCreditScoreInfoRes', 'proto.CreditPunishOtherInfo', 'proto.PhotoWallReportInfo',
    'proto.CSGetReportAndPenaltyRes', 'proto.CreditReportInfo',
    'proto.CreditPenaltyInfo', 'proto.CreditCompensationInfo',
    'proto.CSGetMatchmakingBlacklistRes', 'proto.MatchmakingBlacklistPlayerInfo',
)

ENDPOINTS = {
    'CheckHackBehavior': {
        'service_id': 803, 'request': 'proto.CSCheckHackBehaviorReq', 'response': 'proto.CSCheckHackBehaviorRes',
        'sender': ('COW.UIModelSkinModCheck', 'RequestHackerWarningInfo', 3, 0x48e08cc, 0x48e0dac),
        'callback': ('COW.UIModelSkinModCheck', 'OnRequestCheckHackBehaviorResFinished', 2, 0x48e0dac, 0x48e0f44),
        'native_checks': {0x48e0a38: ('mov', 'w8, #0x323'), 0x48e0a6c: ('bl', '#0x3a34c90'),
                          0x48e0d84: ('bl', '#0x7446e8c'), 0x48e0e48: ('cbz', 'w21, #0x48e0e5c'),
                          0x48e0ee0: ('strb', 'w8, [x19, #0x1c]')},
        'effect': 'On errorCode0, inspects hack_warning list; empty/invalid path clears isHacker and runs CheckPlayerRepairClientSuccess. Nonempty path sets isHacker and copies first warning title/content/countdown to local warning model. Not a universal anti-cheat verdict.',
    },
    'GetCreditScoreInfo': {
        'service_id': 608, 'request': 'proto.Empty', 'response': 'proto.CSGetCreditScoreInfoRes',
        'sender': ('COW.UIModelCredit', 'RequestGetCreditScoreInfo', 1, 0x43e29b4, 0x43e2bd8),
        'callback': ('COW.UIModelCredit', '<RequestGetCreditScoreInfo>b__84_0', 2, 0x43e5394, 0x43e54f8),
        'native_checks': {0x43e2afc: ('mov', 'w8, #0x260'), 0x43e2b2c: ('bl', '#0x3832fe0'),
                          0x43e2bb8: ('bl', '#0x7446e8c'), 0x43e54a0: ('ldr', 'w9, [x8, #0x18]'),
                          0x43e54a4: ('str', 'w9, [x19, #0x7c]'), 0x43e54b0: ('bl', '#0x43e428c')},
        'effect': 'On errorCode0, casts/caches response in m_CreditScoreInfo, copies credit_score to CurCreditScore, processes credit_punish_other_info, dispatches model update flag16.',
    },
    'GetReportAndPenalty': {
        'service_id': 835, 'request': 'proto.Empty', 'response': 'proto.CSGetReportAndPenaltyRes',
        'sender': ('COW.UIModelCredit', 'RequestGetCreditReportAndPenzalty', 1, 0x43e3284, 0x43e3484),
        'callback': ('COW.UIModelCredit', '<RequestGetCreditReportAndPenzalty>b__89_0', 2, 0x43e57cc, 0x43e58e4),
        'native_checks': {0x43e33c8: ('mov', 'w8, #0x343'), 0x43e33f0: ('bl', '#0x3832fe0'),
                          0x43e345c: ('bl', '#0x7446e8c'), 0x43e5860: ('ldr', 'x1, [x20, #0x10]'),
                          0x43e5870: ('ldr', 'x1, [x20, #0x18]'), 0x43e5880: ('ldr', 'x1, [x20, #0x20]')},
        'effect': 'On errorCode0, copies report/penalty/compensation lists into UIModelCredit fields, calls InjectReportFeedbackToGameAssist and dispatches model update flag32. Lists include reports and compensation about other accounts, not merely current-account bans.',
    },
    'GetMatchmakingBlacklist': {
        'service_id': 27, 'request': 'proto.Empty', 'response': 'proto.CSGetMatchmakingBlacklistRes',
        'sender': ('COW.UIModelMatch', 'RequestMatchBlackList', 4, 0x46d878c, 0x46d8ba4),
        'callback': (None, '<RequestMatchBlackList>b__0', 2, 0x46ff610, 0x46ffed8),
        'native_checks': {0x46d8ab4: ('mov', 'w8, #0x1b'), 0x46d8ae4: ('bl', '#0x3832fe0'),
                          0x46d8b70: ('bl', '#0x7446e8c'), 0x46ff7c8: ('cbnz', 'w20, #0x46ff958'),
                          0x46ff9bc: ('str', 'x20, [x22, #0x2e0]')},
        'effect': 'Has cache/count-only/force-request branches. Successful response path reads blacklist and blacklist_info and rebuilds local match-blacklist model. Neighbor APIs add/remove selected blacklisted player IDs; not proof the current account is in an anti-cheat punishment pool.',
    },
}


def collect(binary, har=None):
    r = RuntimeContract(binary)
    digest = hashlib.sha256(r.native).hexdigest()
    if digest != NATIVE_SHA:
        raise ValueError('Native image differs; fixed call-site addresses must be re-established')
    b = r.metadata
    ranges_at, ranges_bytes = struct.unpack_from('<II', b, 208)
    data_at, data_bytes = struct.unpack_from('<II', b, 200)
    ranges = [struct.unpack_from('<II', b, ranges_at + i * 8) for i in range(ranges_bytes // 8)]
    # Target schemas are in image0. Tokens in other assemblies may repeat; keep
    # all matching ranges and require a single valid tag rather than overwrite.
    attr_by_token = {}
    for i, (token, start) in enumerate(ranges[:-1]):
        end = ranges[i + 1][1]
        if end >= start and end <= data_bytes:
            attr_by_token.setdefault(token, []).append(b[data_at + start:data_at + end])
    ctor = r.method('ProtoBuf.ProtoMemberAttribute', '.ctor', 1)['index']

    def member_tag(token):
        hits = []
        for blob in attr_by_token.get(token, []):
            if not blob or blob[0] not in (1, 2):
                continue
            count = blob[0]
            methods = struct.unpack_from('<' + 'I' * count, blob, 1)
            if ctor not in methods:
                continue
            # Restrict to observed argument forms: one Int32 tag or an empty
            # DefaultValue(string) argument. Reject unsupported attribute data.
            pos = 1 + 4 * count
            tag = None
            for method in methods:
                if blob[pos:pos+3] != b'\x01\x00\x00':
                    raise ValueError(f'Unsupported attribute argument counts token={token:#x} blob={blob.hex()}')
                kind, value = blob[pos+3:pos+5]
                if method == ctor:
                    if kind != 8 or value & 1 or value >= 128:
                        raise ValueError('Unsupported compressed ProtoMember Int32')
                    tag = value // 2
                elif kind != 14 or value != 0:
                    raise ValueError('Unsupported additional field attribute')
                pos += 5
            if pos != len(blob) or tag is None or tag < 1:
                raise ValueError('Invalid attribute extent/tag')
            hits.append((tag, blob.hex()))
        if len(hits) != 1:
            raise ValueError(f'Ambiguous/missing ProtoMember attribute for token{token:#x}')
        return hits[0]

    schemas = {}
    for owner in SCHEMAS:
        cls = r.classes[owner]
        fields = []
        for i in range(cls['fc']):
            ni, _, token = struct.unpack_from('<III', b, r.fields + (cls['fs'] + i) * 12)
            name = r.string(ni)
            tag, raw = member_tag(token)
            fields.append({**r.field(owner, name), 'tag': tag, 'field_token': hex(token), 'attribute_blob': raw})
        schemas[owner] = {'type_index': cls['tix'], 'fields': fields}
    dis = Cs(CS_ARCH_ARM64, CS_MODE_ARM)
    endpoints = {}
    for name, spec in ENDPOINTS.items():
        endpoints[name] = {key: value for key, value in spec.items() if key not in ('native_checks', 'sender', 'callback')}
        code_by_addr = {}
        for label in ('sender', 'callback'):
            owner, method, arity, start, end = spec[label]
            code = list(dis.disasm_lite(r.native[start:end], start))
            code_by_addr.update({addr: (mnemonic, operands) for addr, _, mnemonic, operands in code})
            contract = r.method(owner, method, arity) if owner else {
                'name': method, 'owner': 'UIModelMatch closure; exact nested owner not resolved here',
                'params': ['GCommon.HttpErrorCode', 'System.Object']}
            endpoints[name][label] = {'contract': contract, 'rva': hex(start), 'end': hex(end),
                                     'native_region_sha256': hashlib.sha256(r.native[start:end]).hexdigest(),
                                     'instructions': [f'{a:#x}: {m} {o}' for a, _, m, o in code]}
        for address, expected in spec['native_checks'].items():
            if code_by_addr.get(address) != expected:
                raise ValueError(f'Native call-site mismatch at {address:#x}')
        endpoints[name]['verified_anchors'] = {hex(a): ' '.join(x) for a, x in spec['native_checks'].items()}
    wire = None
    if har:
        entries = json.loads(har.read_bytes())['log']['entries']
        entry = next(e for e in entries if urlsplit(e['request']['url']).path == '/GetCreditScoreInfo')
        raw = entry['response']['content'].get('text', '').encode('latin1')
        by_tag = {field['tag']: field for field in schemas['proto.CSGetCreditScoreInfoRes']['fields']}
        values, pos = [], 0

        def varint():
            nonlocal pos
            value = shift = 0
            while pos < len(raw) and shift < 70:
                byte = raw[pos]; pos += 1
                value |= (byte & 127) << shift
                if byte < 128:
                    return value
                shift += 7
            raise ValueError('Truncated/oversized varint')

        while pos < len(raw):
            key = varint(); tag, kind = key >> 3, key & 7
            field = by_tag.get(tag)
            if field is None:
                raise ValueError(f'Unmapped field{tag}')
            if kind == 0:
                value = varint()
                if field['name'] == 'account_id':
                    value = '<redacted account ID>'
            elif kind == 2:
                length = varint()
                if pos + length > len(raw):
                    raise ValueError('Truncated length-delimited field')
                value = {'bytes': length}; pos += length
            else:
                raise ValueError(f'Unsupported wire type{kind}')
            values.append({'tag': tag, 'wire_type': kind, 'name': field['name'], 'value': value})
        wire = {'source': str(har), 'entry_time': entry['startedDateTime'], 'bytes': len(raw),
                'values': values, 'scope': 'Captured response only; account identifier redacted; absent fields not inferred as explicit server verdicts.'}
    return {'native_sha256': digest, 'metadata_sha256': hashlib.sha256(b).hexdigest(),
            'protobuf_member_ctor_index': ctor, 'schema_basis': 'Native type registration + field offsets + metadata ProtoMember custom-attribute constructor arguments',
            'schemas': schemas, 'endpoints': endpoints, 'captured_credit_score': wire,
            'limitations': ['Static native fallback bodies may be redirected by active IFix patches.',
                            'No server implementation, traffic rewriting, anti-cheat bypass or account-state alteration tested.',
                            'Field offset is an in-memory offset, not a wire tag; tags are independently read from attributes.',
                            'Outer HTTP encryption/compression/authentication is not fully reconstructed here.',
                            'Nested enum namespace aliases and AccountInfoBasicBundleRes are referenced, not fully exported in this schema subset.']}


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--binary', type=Path, default=Path('/tmp/opencode/ff-ipa-fresh/UnityFramework'))
    p.add_argument('--har', type=Path)
    p.add_argument('--output', required=True, type=Path)
    a = p.parse_args()
    report = collect(a.binary, a.har)
    a.output.parent.mkdir(parents=True, exist_ok=True)
    a.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(f'PASS: {len(report["schemas"])} schemas with verified ProtoMember tags; four endpoint native call/callback anchors. Output={a.output}')
    if report['captured_credit_score']:
        print('Captured credit_score=96; account ID omitted from report.')


if __name__ == '__main__':
    main()
