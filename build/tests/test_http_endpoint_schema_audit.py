"""Verify offline native schema/callback evidence, not HTTP rewrite behavior."""
from pathlib import Path
import sys
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT / 'buildkit/audit'))
import http_endpoint_schema_audit as audit


def main():
    report = audit.collect(Path('/tmp/opencode/ff-ipa-fresh/UnityFramework'),
                           Path('/root/ProxyPin10-26_18_11_09.har'))
    schemas = report['schemas']
    def tags(owner):
        return {f['name']: f['tag'] for f in schemas[owner]['fields']}
    assert tags('proto.CSCheckHackBehaviorRes') == {'hack_warning': 2}
    credit = tags('proto.CSGetCreditScoreInfoRes')
    assert credit['credit_score'] == 2 and credit['weekly_match_cnt'] == 8
    assert 7 not in credit.values()
    penalty = tags('proto.CreditPenaltyInfo')
    assert penalty['punish_ts'] == 4 and penalty['is_punishing'] == 9
    assert tags('proto.CreditPunishOtherInfo')['match_mode'] == 5
    assert not schemas['proto.Empty']['fields']
    assert report['endpoints']['GetMatchmakingBlacklist']['request'] == 'proto.Empty'
    assert {k:v['service_id'] for k,v in report['endpoints'].items()} == {
        'CheckHackBehavior': 803, 'GetCreditScoreInfo': 608,
        'GetReportAndPenalty': 835, 'GetMatchmakingBlacklist': 27}
    values = {row['name']: row['value'] for row in report['captured_credit_score']['values']}
    assert values['credit_score'] == 96
    assert values['account_id'] == '<redacted account ID>'
    assert report['captured_credit_score']['bytes'] == 24
    class WrongImage:
        native = b'wrong image'
    with patch.object(audit, 'RuntimeContract', return_value=WrongImage()):
        try:
            audit.collect(Path('mock'))
        except ValueError as error:
            assert 'differs' in str(error)
        else:
            raise AssertionError('Fixed address audit must reject a different binary')
    print('PASS:14 metadata schemas, non-sequential wire tags, four native sender/callback anchors, redacted HAR credit decode and wrong-image rejection')
    print('No traffic replay, rewrite or server-state modification tested.')


if __name__ == '__main__':
    main()
