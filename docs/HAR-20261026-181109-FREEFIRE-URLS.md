# Other Free Fire / Garena URLs in the capture

This supplements `HAR-20261026-181109-ANALYSIS.md`. Endpoint counts below remove
18 duplicate HAR records. Full path-only inventory, times, status and query-key
names (not values/tokens) are in `har-20261026-181109-analysis.json` under
`freefire_endpoints`. No request was replayed and no live service was changed.

137 unique requests across10 game-related hosts:

| Host | Requests | Response status | Median / maximum HAR duration |
|---|---:|---|---:|
|clientbp.ppmainecoonghj.com|102|all200|118 /640ms|
|vnevent.ggblueshark.com|16|all200|154.5 /415ms|
|100067.msdk.garena.com|7|all200|109 /156ms|
|100067.connect.garena.com|3|all200|91 /222ms|
|loginbp.ppmainecoonghj.com|2|all200|190.5 /200ms|
|core-gmc.freefiremobile.com|2|200,206|148.5 /160ms|
|gin.freefiremobile.com|2|200,902|105 /135ms|
|dl.gmc.freefiremobile.com|1|200|103ms|
|version.ggwhitehawk.com|1|200|118ms|
|cdn.wildflamestudio.com|1|403|132ms|

Durations are ProxyPin HAR elapsed values, not in-game ping, server computation
time or proof that gameplay transport has no latency.

## Version / route configuration

`version.ggwhitehawk.com/live/ver.php` returns200 and JSON advertising:

- remote_version1.132.13, latest_release_versionOB55, multi_regionVN.
- hotupdate endpoint `https://core-gmc.freefiremobile.com/live/ABHotUpdates/`.
- asset CDN `https://dl.gmc.freefiremobile.com/live/ABHotUpdates/`.
- login server `https://loginbp.ppmainecoonghj.com/`.
- diagnostics `https://vnnetwork.ggblueshark.com/` and
  `https://networkselftest.ff.garena.com/api/`.

These diagnostics URLs appear in configuration, not as captured requests to
those hosts. Do not infer that they were contacted. The same applies to several
patchnote/billboard URLs advertised in version JSON.

## Login and account service

- `loginbp.ppmainecoonghj.com/MajorLogin`: POST200,1209 reported response bytes.
- `/Ping`: POST200. Together with subsequent client API traffic this supports a
  successful HTTP login flow; it does not expose or validate gameplay UDP state.
- Garena Connect `/oauth/token/inspect`:200, JSON contains identity/scope/expiry
  fields. Values and token query parameters are excluded from the report.
- `/api/v1/game/local-requirement` and `/user-info`:200, JSON code/data envelope.
  HTTP success alone does not establish the meaning of each business code.

## Main game API: clientbp.ppmainecoonghj.com

102 distinct POST paths, all HTTP200. Main groups:

- Login/config: GetLoginData, LoginGetDesc (109046bytes), LoginGetSplash,
  LoginGetAccountInfo, LoginGetProfile.
- Player/social: GetFriend, GetFriendRequestList, GetPlatformProfile,
  GetPlatformFriends, GetPlayerPersonalShow, GetMailList, GetPetList.
- Inventory/appearance: GetBackpack, GetPresetLoadoutInfo, GetLoadoutSchemeDesc,
  GetAccountCollectionCustomItems, GetWeaponExpInfo, GetWeaponSkinUpgradeGratitude,
  GetLegendClothesDesc, GetAvatarSkillOverWrite.
- Ranking/match: GetBRRankingInfoEx, GetPlayerRankingInfoEx,
  GetCSRankingTaskList, GetAPIMatchPool, AccountMatchStats, GetMatchmakingBlacklist.
- Events/shop: GetLuckyWheelDesc, GetDiscountStore, GetVirtualBrandDesc,
  GetHyperBookInfo/Setting, GetFestivalAttendanceInfo, GetSharedGachaList.
- UGC/bot/config: GetWorkshopSwitch, GetWorkshopUGCOpeningInfo, GetAIBotAgentDesc,
  GetLabSetting, GetModeStatsConfig.
- Report/score: CheckHackBehavior, GetReportAndPenalty,
  GetCreditScoreInfo, GetCreditScoreDesc, ReportEventPushInfo.

The CheckHackBehavior and GetReportAndPenalty calls are ordinary observed API
requests. Their presence is not evidence of a detection or ban. For endpoints
whose HAR content is absent/empty (`size=-1`), the application result cannot be
decoded from this capture. Binary response bytes were not schema-decoded here;
HTTP200 is not proof of every business operation succeeding.

The slowest API request in this group is GetQuickMsgSettings640ms; another
GetHyperBookInfo348ms, GetWorkshopUGCOpeningInfo321ms, GetAttendance and
GetBRRankingInfoEx310ms. No timeout/5xx is recorded in this group.

## CDN / optional resources

- Main iOS1.132.13 fileinfo:200; codepatch206 complete300829bytes. Hashes/sizes
  match, as documented in the companion analysis.
- Optional avatar832 fileinfo from dl.gmc:200,7969bytes. This request is
  separate from the main codepatch manifest.
- Splash PNG from cdn.wildflamestudio:403, XML AccessDenied. This is an asset
  permission/availability failure in the capture, not evidence that codepatch,
  login, HitFly or ESP failed because of it.

## Feature / flag delivery

- `gin.freefiremobile.com/flagSortIos/deliveryFlagIos`: POST200,48-character
  opaque response. Contents are not interpreted as decoded feature flags.
- `/featureSortIos/deliveryFeatureIos`: POST902, body
  `PR_FEATURE_VERSION_DUPLICATE`. Nonstandard status/application message; it
  indicates the feature-version duplicate condition reported by the endpoint.
  It is not a demonstrated ban/disconnect and does not prove a bad patch.

## Telemetry / commerce SDK

- `vnevent.ggblueshark.com/LogEvent`:16 POSTs, all200. No event-body schema was
  decoded, so these responses are only evidence of HTTP transport success.
- Garena MSDK pricing:5 requests200; event/config:1 request200; rebates:1
  request200. These are SDK/event/commerce requests, not movement controls.
- Other captured Firebase, AppsFlyer and Facebook hosts belong to SDK/analytics
  paths, not the HitFly endpoint. Crashlytics upload200 does not by itself prove
  a new crash. Facebook/Apple TLS errors do not establish a Free Fire CDN failure.

## Gameplay limits of this HAR

There is no decoded opcode657 request, native flight instruction execution,
per-shot authoritative damage ACK or per-ID ESP frame output in these URLs.
The custom `/aimlog` requests provide client diagnostic observations, not game
server confirmations. Endpoint names alone cannot reveal server-side handling
of movement, anti-cheat or damage.
