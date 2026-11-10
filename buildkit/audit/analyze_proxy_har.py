#!/usr/bin/env python3
"""Offline HAR audit. Excludes tokens, headers and request bodies from reports."""
import argparse
import base64
import collections
import gzip
import hashlib
import json
import math
import re
from pathlib import Path
from urllib.parse import parse_qs, urlsplit


def fields(text):
    return dict(re.findall(r'(\w+)=([^\s]*)', text))


def vector(text):
    try:
        result = [float(x) for x in text.split(',')]
        return result if len(result) == 3 and all(math.isfinite(x) for x in result) else None
    except (AttributeError, ValueError):
        return None


def body_bytes(content):
    text = content.get('text', '')
    if content.get('encoding') == 'base64':
        return base64.b64decode(text, validate=True)
    # ProxyPin stores raw byte-valued characters, without encoding=base64.
    return text.encode('latin1')


def analyze(path):
    data = path.read_bytes()
    entries = json.loads(data)['log']['entries']
    seen, unique = set(), []
    for index, e in enumerate(entries):
        key = (e.get('startedDateTime'), e['request']['method'], e['request']['url'])
        if key not in seen:
            seen.add(key)
            unique.append((index, e))
    logs, patches, manifests, errors = [], [], [], []
    game_hosts = collections.defaultdict(list)
    for index, e in unique:
        request, response = e['request'], e['response']
        u = urlsplit(request['url'])
        content = response.get('content', {})
        if any(part in (u.hostname or '') for part in
               ('freefire', 'garena', 'ggwhitehawk', 'ggblueshark', 'ppmaine', 'wildflame')):
            game_hosts[u.hostname].append({'entry': index, 'at': e['startedDateTime'],
                                          'method': request['method'], 'path': u.path,
                                          'status': response['status'], 'time_ms': e.get('time'),
                                          'response_size_reported': content.get('size'),
                                          'query_keys': sorted(parse_qs(u.query))})
        if 'gameassetbundles/codepatch/assembly-csharp-patch.' in u.path:
            packed = body_bytes(content)
            raw = gzip.decompress(packed)
            header_map = {h['name'].lower(): h['value'] for h in response.get('headers', [])}
            patches.append({'entry': index, 'at': e['startedDateTime'], 'path': u.path,
                            'status': response['status'], 'bytes': len(packed),
                            'sha256': hashlib.sha256(packed).hexdigest(),
                            'packed_sha1': base64.b64encode(hashlib.sha1(packed).digest()).decode(),
                            'raw_sha1': base64.b64encode(hashlib.sha1(raw).digest()).decode(),
                            'raw_bytes': len(raw), 'gzip_valid': True,
                            'content_range': header_map.get('content-range')})
        if u.path.endswith('/fileinfo') and '/optional/' not in u.path:
            rows = [row for row in content.get('text', '').splitlines() if row.startswith('codepatch/')]
            manifests.append({'entry': index, 'at': e['startedDateTime'], 'rows': rows})
        if u.path == '/aimlog':
            q = parse_qs(u.query)
            text = q.get('d', [''])[0]
            lines = text.splitlines()
            hf = next((line for line in lines if line.startswith('hfMap status=')), '')
            target = next((line for line in lines if line.startswith('hfMap target=')), '')
            selected = {}
            for line in lines:
                values = fields(line)
                for name in ('shotStarts', 'shotStops', 'shotFault', 'probeOk', 'probeFail',
                             'probeMixed', 'manualAims', 'dispatched', 'calls'):
                    if name in values:
                        selected[name] = values[name]
            log_errors = [line for line in lines if line.startswith(('error=', 'cleanupError=', 'combatError='))
                          and line.split('=', 1)[1]]
            logs.append({'entry': index, 'at': e['startedDateTime'],
                         'tag': q.get('t', [''])[0], 'version': q.get('v', [''])[0],
                         'http_status': response['status'], 'hf': fields(hf),
                         'target': fields(target), 'counters': selected,
                         'errors': log_errors})
        if response['status'] not in (200, 204, 206):
            # Only known diagnostic messages are included, never arbitrary bodies.
            t = content.get('text', '')
            diagnosis = ('TLS handshake failure' if 'HandshakeException' in t else
                         'AccessDenied' if '<Code>AccessDenied</Code>' in t else
                         'PR_FEATURE_VERSION_DUPLICATE' if 'PR_FEATURE_VERSION_DUPLICATE' in t else
                         'unclassified')
            errors.append({'entry': index, 'at': e['startedDateTime'], 'host': u.hostname,
                           'path': u.path, 'status': response['status'], 'diagnosis': diagnosis})
    logs.sort(key=lambda row: row['at'])
    groups = collections.defaultdict(list)
    for row in logs:
        h = row['hf']
        if h.get('attempts') not in (None, '0') and h.get('reason') != 'queued':
            groups[h['attempts']].append(row)
    flights = []
    for attempt, samples in groups.items():
        first = samples[0]
        terminal = next((row for row in samples if row['hf'].get('obs') in ('3', '4', '5', '6', '7')), None)
        last = terminal or samples[-1]
        h = first['hf']
        summary = {'attempt': int(attempt), 'first_observed_at': first['at'],
                   'last_or_terminal_at': last['at'],
                   'hover_first_sample': h.get('hover'),
                   'hover_changed_during_samples': len({row['hf'].get('hover') for row in samples
                                                      if terminal is None or row['at'] <= terminal['at']}) > 1,
                   'gravity': h.get('gravity'), 'scale': h.get('scale'),
                   'packet_xz': h.get('xz'), 'packet_y': h.get('vy'),
                   'packet_time': h.get('t'), 'estimated_time': h.get('estimate'),
                   'terminal_observation': last['hf'].get('obs'),
                   'terminal_reason': last['hf'].get('reason'),
                   'reported_rise': last['hf'].get('rise'),
                   'reported_nearest_xz': last['hf'].get('nearestXZ'),
                   'reported_final_or_last_xz': last['hf'].get('missXZ')}
        origin = vector(first['target'].get('origin'))
        target = vector(first['target'].get('target'))
        position = vector(last['hf'].get('last'))
        if origin and target and position:
            dx, dz = target[0] - origin[0], target[2] - origin[2]
            distance = math.hypot(dx, dz)
            if distance:
                along = (position[0] - origin[0]) * dx / distance + (position[2] - origin[2]) * dz / distance
                cross = -(position[0] - origin[0]) * dz / distance + (position[2] - origin[2]) * dx / distance
                summary.update({'start_distance_xz': round(distance, 2),
                                'along_travel_xz': round(along, 2),
                                'along_shortfall_xz': round(distance - along, 2),
                                'cross_error_xz': round(cross, 2),
                                'vertical_error_vs_marker': round(position[1] - target[1], 2)})
        flights.append(summary)
    for patch in patches:
        row = f"codepatch/assembly-csharp-patch,{patch['raw_sha1']},{patch['raw_bytes']},0,{patch['packed_sha1']},{patch['bytes']},True,0"
        patch['manifest_matches'] = any(row in m['rows'] for m in manifests)
    return {'source': str(path), 'source_bytes': len(data), 'source_sha256': hashlib.sha256(data).hexdigest(),
            'entries': len(entries), 'unique_requests': len(unique), 'duplicate_entries': len(entries) - len(unique),
            'first_utc': min(e['startedDateTime'] for e in entries),
            'last_utc': max(e['startedDateTime'] for e in entries),
            'status_all': dict(collections.Counter(str(e['response']['status']) for e in entries)),
            'status_unique': dict(collections.Counter(str(e['response']['status']) for _, e in unique)),
            'patches': patches, 'manifests': manifests, 'errors': errors,
            'freefire_endpoints': dict(game_hosts),
            'telemetry_count': len(logs),
            'telemetry_versions': dict(collections.Counter(row['version'] for row in logs)),
            'flights': sorted(flights, key=lambda row: row['attempt']),
            'telemetry': logs,
            'limitations': ['HAR does not expose opcode657 game UDP traffic or authoritative server ACK.',
                            'Client observation label landed is not evidence of the selected surface, grounded accuracy or attribution.',
                            'No frame timings, player-ID ESP cache records or renderer outputs are captured.',
                            'Duplicate HAR entries are deduplicated by timestamp, method and full URL for statistics only.',
                            'Tokens, query values except diagnostic telemetry, headers and request bodies are excluded from this report.']}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('har', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    report = analyze(args.har)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n')
    print(json.dumps({key: report[key] for key in ('source_bytes', 'entries', 'unique_requests',
                                                'duplicate_entries', 'status_unique', 'patches', 'flights')}, indent=2))


if __name__ == '__main__':
    main()
