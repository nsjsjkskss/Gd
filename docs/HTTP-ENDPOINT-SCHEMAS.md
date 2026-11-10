# Four HTTP endpoints — schemas and native call/callbacks

## Evidence and scope

Read-only metadata/native review for the supplied UnityFramework image:

- Native SHA256 `2de005ae6c9d263ddd705aedda833c29a5b27044d2f56655ecc3cf98dc7be54a`.
- Metadata SHA256 `597e77fc1dd15bac9ff75215bd328d60c6d205f5fc4a7e32cec1b37d3cc2ea5d`.
- Dump: `/tmp/opencode/aimkill-dumper-out/dump.cs`. Type aliases in that dump are
  contaminated; field/parameter types below come from native type registration.
- Wire tags come from metadata custom-attribute ranges and the Int32 argument
  to `ProtoBuf.ProtoMemberAttribute..ctor` (method index324677). They are not
  guessed from field order or in-memory offsets.
- Exact contract/signature, custom-attribute bytes, field offsets, instruction
  ranges/hashes and selected native anchors are exported in
  `docs/http-endpoint-schema-audit.json`.
- Static addresses are RVA for this image only. Functions contain IFix dispatch
  branches; native fallback analysis does not prove the effective runtime body
  cannot be overridden by a loaded patch.
- No request replay/rewrite, anti-cheat disablement, server mutation, deployment
  or service restart occurred.

Reproduce:

```sh
python3 buildkit/audit/http_endpoint_schema_audit.py --har /root/ProxyPin10-26_18_11_09.har --output docs/http-endpoint-schema-audit.json
python3 build/tests/test_http_endpoint_schema_audit.py
```

## Routing / call sites

The four native sender paths select a ServiceMessageTypeHTTP value, use its
string representation as the command, construct the request/delegate and call
the common generic `GameFacade.SendServerRequest<TRes>` body at **0x7446e8c**.
This is HTTP service routing, not the UDP gameplay opcode table.

| URL | Service ID | Sender | RVA | Response handler | RVA |
|---|---:|---|---|---|---|
|CheckHackBehavior|803|COW.UIModelSkinModCheck.RequestHackerWarningInfo(bool, LoadingType, EHttpChannel)|0x48e08cc|OnRequestCheckHackBehaviorResFinished(HttpErrorCode, object)|0x48e0dac|
|GetCreditScoreInfo|608|COW.UIModelCredit.RequestGetCreditScoreInfo(EHttpChannel)|0x43e29b4|<RequestGetCreditScoreInfo>b__84_0|0x43e5394|
|GetReportAndPenalty|835|COW.UIModelCredit.RequestGetCreditReportAndPenzalty(EHttpChannel)|0x43e3284|<RequestGetCreditReportAndPenzalty>b__89_0|0x43e57cc|
|GetMatchmakingBlacklist|27|COW.UIModelMatch.RequestMatchBlackList(ulong, bool, bool, bool)|0x46d878c|UIModelMatch closure <RequestMatchBlackList>b__0|0x46ff610|

The dump displays the last callback under a contaminated closure owner alias.
Its RVA/name are present; the exact nested closure contract was not independently
resolved by full owner identity. The other contracts are validated through
native registration, including GCommon.HttpErrorCode callback parameters.

## 1. CheckHackBehavior

### Request: proto.CSCheckHackBehaviorReq

|Tag|Field|Type|
|---:|---|---|
|1|client_info|MatchClientInfo|

`MatchClientInfo` fields:

|Tag|Field|Type|
|---:|---|---|
|1|tpsdk_str|string|
|2|start_time|uint32|
|3|file_exception|bytes|
|4|lib_result|bytes|
|5|exception_count|uint32|
|6|scan_count|uint32|
|7|native_result|bytes|
|8|client_version|string|
|9|channel_type|uint32|
|10|architecture_type|uint32|

The sender constructs and populates client_info. Fields describe client-side
information; interpreting the contents of security-result blobs would require
additional native producer/schema analysis, not just their names.

### Response: proto.CSCheckHackBehaviorRes

**Tag2**: repeated `HackWarningInfoDesc hack_warning` (not tag1).

Warning fields:1 title:string,2 content1:string,3 content2:string,4 countdown:uint32.

### Handler

`OnRequestCheckHackBehaviorResFinished` native fallback returns on nonzero
errorCode. On success, checks response type/list. Empty/invalid/no-item path
clears local `isHacker` and invokes `CheckPlayerRepairClientSuccess` (0x48e0f44).
With at least one warning, it sets local isHacker and copies the first warning's
text/countdown to `m_HackWarningInfoDesc`.

This is a client warning/repair flow in UIModelSkinModCheck. Do not generalize it
to the only anti-cheat check, a server ban decision, or a way to clear server
state. The HAR has no saved response body for this endpoint.

## 2. GetCreditScoreInfo

Request: **proto.Empty** (constructor0x3832fe0). Outer HTTP/auth/encryption data
may still exist; Empty refers only to this protobuf request message.

Response: **proto.CSGetCreditScoreInfoRes**:

|Tag|Field|Type|
|---:|---|---|
|1|account_id|uint64|
|2|credit_score|uint32|
|3|is_init|bool|
|4|reward_state|RewardState enum|
|5|periodic_summary_like_cnt|uint32|
|6|periodic_summary_illegal_cnt|uint32|
|8|weekly_match_cnt|uint32|
|9|periodic_summary_start_time|int64|
|10|periodic_summary_end_time|int64|
|11|periodic_summary_level|SummaryLevel enum|
|12|credit_punish_other_info|repeated CreditPunishOtherInfo|
|13|photo_wall_report_info|PhotoWallReportInfo|

No field7 in this metadata schema. `CreditPunishOtherInfo`:1 punish_type:uint32,
2 sub_type:uint64,3 punish_until:int64,**5** match_mode:uint32.
PhotoWallReportInfo:1 reports_today:uint32.

### Handler

On success, callback caches response at `m_CreditScoreInfo`, copies credit_score
to `CurCreditScore`, invokes `ProcessCreditPunishOtherInfo`, and sends local
model update flag16. Verified native reads response+0x18 and writes model+0x7c.
These are in-memory offsets, not protobuf tags.

### Captured response

The24-byte HAR response parses completely against these wire tags:

- tag1 account_id: **redacted**.
- tag2 credit_score: **96**.
- tag9 periodic_summary_start_time:1793009628.
- tag10 periodic_summary_end_time:1793268828.
- tag11 periodic_summary_level:2 (enum name mapping not exported here).
- tag13 photo_wall_report_info:zero-length message.

This is the value in the captured response at10:51:45.246646UTC, not a claim
about the account's current state. Absent fields are not independent explicit
server assurances of no restrictions.

## 3. GetReportAndPenalty

Request: **proto.Empty**. Response: **proto.CSGetReportAndPenaltyRes**:

|Tag|Field (exact metadata spelling)|Type|
|---:|---|---|
|1|credit_repoter_list|repeated CreditReportInfo|
|2|credit_penalty_list|repeated CreditPenaltyInfo|
|3|credit_compensation_list|repeated CreditCompensationInfo|

Nested field mappings:

- CreditReportInfo:1 nickname:string,2 status:uint32,3 report_time:int64,
  4 report_type:uint32,5 unique_id:uint64,6 be_reported_id:uint64.
- CreditPenaltyInfo:1 punish_type:uint32,**4** punish_ts:int64,5 duration:int64,
  6 ban_type:string,7 hacker_account_id:uint64,8 hacker_nickname:string,
  9 is_punishing:bool. Fields2/3 are absent in this schema.
- CreditCompensationInfo:1 game_mode:uint32,2 amount:uint32,3 ts:int64,
  4 punish_account_id:uint64,5 punish_nickname:string,6 is_punishing:bool.

### Handler

On success callback copies the three response lists into local report/penalty/
compensation lists, invokes `InjectReportFeedbackToGameAssist` and sends local
model update flag32. This endpoint includes feedback/compensation about reports
and other accounts; it is not merely a current-user banned flag. HAR has no
saved response body, so cannot say those lists were empty or contained penalties.

## 4. GetMatchmakingBlacklist

Request: **proto.Empty**. Response: **proto.CSGetMatchmakingBlacklistRes**:

|Tag|Field|Type|
|---:|---|---|
|1|blacklist|repeated MatchmakingBlacklistPlayerInfo|
|2|blacklist_info|AccountInfoBasicBundleRes|

MatchmakingBlacklistPlayerInfo:1 blacklisted_id:uint64,2 blacklisted_time:int64.
AccountInfoBasicBundleRes contains an infos list; full AccountInfoBasic schema
is outside this subset.

### Caller/handler semantics

Sender has cached-list/count-only/force-request branches. Callback success path
checks response/list/profile-info, rebuilds model `m_MatchBlackList`, and updates
model/UI. Related native functions in the same UIModelMatch:

- `IsPlayerInBlackList(ulong)`0x46d908c.
- `RequestAddMatchBlackListPlayer(ulong, uint, uint, ulong)`0x46d95fc.
- `RequestRemoveMatchBlackListPlayer(ulong[])`0x46d9898.
- `ProcessExpireMatchBlackList()`0x46d8ba4.

Together with per-player blacklisted IDs/times, this establishes a player
matchmaking-blocklist management flow. It must not be conflated with unrelated
ban-account/anti-cheat blacklist types that also exist in the dump. The HAR's
missing saved body does not establish an empty blacklist.

## Remaining scope

The audit reconstructs14 message schemas including nested types and all four
native sender/handler anchor sets. It does not reconstruct the entire HTTP
wrapper, ciphertext formats or server enforcement. No response rewrite has
been implemented or tested. Client caching/UI changes do not establish server
state changes.
