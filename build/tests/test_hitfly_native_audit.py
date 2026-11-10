"""Regression for read-only native evidence, not a flight/device simulation."""
import importlib.util
from pathlib import Path
import sys
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[2]
AUDIT = ROOT / 'buildkit/audit'
sys.path.insert(0, str(AUDIT))
spec = importlib.util.spec_from_file_location('hitfly_motion_audit', AUDIT / 'hitfly_motion_audit.py')
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)


def main():
    report = module.collect(Path('/tmp/opencode/ff-ipa-fresh/UnityFramework'))
    nested = report['nested_motion_data']
    assert nested['type_index'] == 31659
    assert len(nested['same_name_type_indices']) > 1, 'known nested short-name collision must remain explicit'
    fields = {field['name']: field for field in nested['fields']}
    assert fields['IEEIGMJLPEL']['offset'] == '0x48'
    assert fields['IEEIGMJLPEL']['type'] == 'UnityEngine.Vector3'
    assert report['regions']['kinematics_dispatch']['jump_table']['5'] == '0x568a96c'
    assert report['server_velocity_field']['offset'] == 48
    packet_fields = {field['name']: field for field in report['packet657_fields']}
    assert packet_fields['MGJNBIJNLIJ']['type'] == 'message.BJFGLKKKOAN'
    assert packet_fields['DCEMJOGHMEG']['type'] == 'System.Single'
    assert not any(field['type'] == 'message.LDCJOLOBGGH' for field in packet_fields.values())
    assert any(item['owner'] == 'COW.GamePlay.NetworkAIPawn' and item['name'] == 'StopHitfly'
               for item in report['contracts'])
    assert not any(item['owner'] == 'COW.GamePlay.Player' and item['name'] == 'StopHitfly'
                   for item in report['contracts'])
    class WrongImage:
        native = b'not-the-audited-game-image'
    with patch.object(module, 'RuntimeContract', return_value=WrongImage()):
        try:
            module.collect(Path('ignored-by-mock'))
        except ValueError as error:
            assert 'differs' in str(error)
        else:
            raise AssertionError('must not apply fixed native addresses to another binary')
    print('PASS: static audit anchors, nested-type collision, synced velocity, packet shape and wrong-image rejection')
    print('NOT TESTED: actual flight, braking, native dispatch or server acceptance')


if __name__ == '__main__':
    main()
