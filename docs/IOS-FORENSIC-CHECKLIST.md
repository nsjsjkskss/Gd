# Checklist forensic iOS — đối chiếu a4ther

Mục tiêu: biết chính xác stack OKMOD 1106 còn lộ gì nếu bị kiểm tra forensic,
dựa trên code thật của `lovelyoyk/a4ther` (scanner anti-cheat công khai), không
đoán.

Nguồn đối chiếu:

- `lovelyoyk/a4ther` — `README.md`, `a4ther-ios.js` (Scriptable, không JB),
  `a4ther.sh` (JB qua SSH).
- Regex trích trực tiếp từ `a4ther-ios.js`.
- Thiết bị tham chiếu: iPhone thật, FF App Store, proxy PAC + CA của mình.

---

## 0. Kết luận ngắn

Chỉ còn **2 thứ lộ**, cả hai đều là bản chất của MITM, không phải lỗi patch:

1. **Proxy đang bật** — `network_state` của sysdiagnose có `HTTPProxy` /
   `HTTPSProxy`; a4ther alert ngay.
2. **CA root profile** — tên hiện tại là **`mitmproxy`**, nằm trong danh sách
   keyword cheat của a4ther; và mọi CA root tự cài đều bị liệt kê.

Còn lại sạch: không jailbreak, không Substrate/Frida, không sideload, FF là app
App Store, không app cheat nào, prefs/localConfig đã dọn.

Điểm mấu chốt: **không thể vừa MITM vừa vô hình**. Nếu bị kiểm tra forensic
nghiêm túc, cách duy nhất là gỡ CA + tắt proxy + chơi sạch.

---

## 1. a4ther kiểm gì (bằng chứng)

### 1.1 Chế độ sysdiagnose (không cần JB, "mais completo")

README nói rõ nó đọc:

| File trong sysdiagnose | Nội dung bị kiểm |
|---|---|
| `summaries/Applications.txt` | toàn bộ bundle ID đã cài, đối chiếu 105 app cheat |
| `Managed_Configuration_Profiles/` | **tất cả profile**: VPN, **Proxy**, **CA Root**, DNS, MDM, Restrictions, Screen Time |
| `mobile_installation_logs/` | lịch sử cài/gỡ FF và app cheat |
| `summaries/network_state.txt` | **VPN (utun/ppp/ipsec), HTTPProxy, HTTPSProxy**, DNS, IP/domain khớp KNOWN_CHEAT_INFRA |
| `taskinfo.txt` / `ps.txt` | process đang chạy: Frida, Cycript, gdb, Substrate, TrollStore, iSH, Cydia |
| `crashes_and_spins/` | crash của FF + string injection (`DYLD_INSERT`, `libsubstrate`) |

### 1.2 Regex trích từ `a4ther-ios.js`

```js
/jailbreak|jb\.|tweakloader|substitute|substrate|trollstore|dopamine|amfid|bypass/i

/jailbroken|trollstore|sileo|cydia|frida|substrate|dopamine|palera1n/i

/esign|feather|ksign|gbox|scarlet|sideload|trollstore|cheat|hack|aimbot|
 wallhack|ffh4x|mod[._-]?menu|injector|cracked|mitmproxy|holograma|hologram/i

/khoindvn|khodivn|happymod|skibidi|esign|feather|ksign|gbox|scarlet|
 trollstore|sideload|iosgods|filza|dopamine|palera1n|unc0ver|sileo|cydia|
 zebra|altstore|appcake|appdb|tutuapp|ignition|buildstore|geranium|frida|
 cycript|substrate|ffh4x|aimbot|wallhack|cheat|hack|mod[._-]?menu|injector|
 cracked|.../i
```

Network state:

```js
/HTTPProxy[^a-zA-Z]+([0-9.]+)/i
/HTTPSProxy[^a-zA-Z]+([0-9.]+)/i
```

→ Chỉ cần proxy được cấu hình là có alert, bất kể proxy làm gì.

**`mitmproxy` nằm trong danh sách keyword cheat/injector.** CA của mình đang có
subject `O=mitmproxy, CN=mitmproxy`, nên profile cài trên máy hiện tên
"mitmproxy" — khớp thẳng keyword.

---

## 2. Ai nhìn thấy gì

| Lớp kiểm tra | Thấy được | Không thấy được |
|---|---|---|
| **Game (sandbox)** | proxy settings (`CFNetworkCopySystemProxySettings`), issuer cert của kết nối nó tự mở, container của nó, `sysctl`, `canOpenURL`, dylib của chính nó | danh sách profile, root CA, app khác, ioreg, process khác |
| **sysdiagnose + Scriptable (a4ther mode 1)** | app list, **profiles (CA root, proxy)**, **HTTPProxy/HTTPSProxy**, install history, ps/taskinfo, crash strings | nội dung private của app khác (trừ log/crash) |
| **JB + SSH (a4ther mode JB)** | tất cả: trust store, ioreg HWID, process, file system | — |
| **Người kiểm tra bằng mắt** | Settings → Wi-Fi → Proxy; Settings → General → About → Certificate Trust Settings; VPN & Device Management | — |

---

## 3. Checklist 16 mục — trạng thái stack của mình

| # | Check | Cách a4ther kiểm | Trạng thái | Ghi chú |
|---:|---|---|---|---|
| 1 | Jailbreak paths (`/var/jb`, Cydia, Sileo, palera1n, Dopamine) | profile/ps + regex | **SẠCH** | máy không JB |
| 2 | Substrate / Substitute / tweakloader | ps + regex + dylib | **SẠCH** | không cài |
| 3 | Frida / Cycript / gdb | ps + port + regex | **SẠCH** | không cài |
| 4 | Sideload (TrollStore/AltStore/Esign/Feather/GBox/Scarlet) | app list + regex | **SẠCH** | FF từ App Store |
| 5 | FF bundle id + App Store signature | app list + codesign (JB) | **SẠCH** | app không bị sửa; chỉ codepatch tải về |
| 6 | App cheat đã cài | 105 bundle ID | **SẠCH** | không có |
| 7 | **Configuration Profiles** | `Managed_Configuration_Profiles/` | **LỘ** | có CA root profile |
| 8 | **Tên CA / keyword `mitmproxy`** | regex + profile name | **LỘ** | CA đang tên `mitmproxy` |
| 9 | **HTTPProxy / HTTPSProxy** | `network_state.txt` | **LỘ — không tránh được khi còn MITM** | PAC `okmod.sbs` + `proxy.okmod.sbs:1106` |
| 10 | VPN (utun/ppp/ipsec) | network_state | **SẠCH** | đang dùng proxy, không VPN |
| 11 | Process lạ | ps/taskinfo | **SẠCH** | không JB |
| 12 | HWID qua `ioreg` | JB/SSH | **SẠCH** | app không đọc được ioreg |
| 13 | Install/uninstall history | mobile_installation_logs | **SẠCH** | FF cài 1 lần từ App Store |
| 14 | Crash FF chứa `DYLD_INSERT` / `libsubstrate` | crashes_and_spins | **SẠCH** (dự kiến) | không inject dylib |
| 15 | File rác trong container FF (`localConfig.json`, prefs cheat) | (a4ther không quét, nhưng ANO có thể) | **SẠCH** | patch tự xoá localConfig + 111 key cũ |
| 16 | Hook/field lạ trong memory (IFix) | a4ther **không** quét; ANO có thể | **RỦI RO RIÊNG** | 6 hook + 333 field; không phải forensic file |

---

## 4. Findings và cách xử lý

### F1 — CA root profile tên `mitmproxy` (khớp keyword)

- Mức: cao với kiểm tra forensic/human; thấp với game (game không liệt kê root).
- Fix: `proxy/rotate-ca.sh` tạo CA mới `O=Local Device Trust,
  CN=Local Device Trust CA` (cùng key layout XenProxy cần), sau đó
  `--apply` để chuyển + restart.
- **Lưu ý thật:** đổi tên chỉ tránh khớp keyword; một CA root tự cài vẫn nằm
  trong `Managed_Configuration_Profiles` và vẫn hiện trong Certificate Trust
  Settings. Không có cách "giấu" root CA trên iOS không JB.

### F2 — Proxy đang bật (HTTPProxy/HTTPSProxy)

- Mức: cao, và **không thể tránh** khi cần MITM để nạp codepatch.
- Có một thử nghiệm đáng làm: bật proxy → mở FF → vào lobby → tắt proxy →
  chơi. Codepatch đã nạp vào memory có thể giữ nguyên trong session.
  **Chưa kiểm chứng** game có re-fetch manifest khi reconnect/next match không;
  nếu re-fetch thì cheat về official giữa trận. Phải test riêng, đừng coi là
  tính năng đã chắc.

### F3 — Domain branding `okmod.sbs`

- `okmod` / `proxy.okmod.sbs` **không** nằm trong danh sách keyword của a4ther
  (đã grep toàn file: `okmod` = null, `xenproxy` = null).
- Nhưng PAC URL hiện trong Wi-Fi settings và trong network_state; người kiểm
  tra bằng mắt đọc được. Mức thấp, không cần đổi domain.

### F4 — Codepatch/bundle flags

- Không phải forensic file trên máy; là so sánh server-side (`0x40` vs `0x243`).
- Không xử lý được nếu thiếu key 1.132.18. Giữ nguyên, đã ghi trong
  `DETECTION-AUDIT.md`.

### F5 — Hook/field trong memory

- a4ther không quét; ANO có thể quét method pointer/field table.
- Giảm bằng cách bớt hook/field (P2), không liên quan checklist file.

---

## 5. Tự kiểm bằng sysdiagnose (không cần chạy a4ther)

a4ther là script obfuscate, tải bằng one-liner, có quyền đọc sysdiagnose —
không nên chạy trên máy chính. Có thể tự làm đúng phần quan trọng:

1. iPhone: nhấn Vol+ + Vol- + Side cùng lúc 1 giây, đợi ~5 phút.
2. Settings → Privacy & Security → Analytics & Improvements → Analytics Data
   → chọn `sysdiagnose_*.tar.gz` → Share → Save to Files.
3. Trên máy tính, giải nén rồi kiểm:

```bash
tar -xzf sysdiagnose_*.tar.gz
cd sysdiagnose_*/

# 1. Proxy có bật không
grep -iE "HTTPProxy|HTTPSProxy|ProxyAutoConfig" summaries/network_state.txt

# 2. Có profile/CA lạ không
ls -la Managed_Configuration_Profiles/ 2>/dev/null
grep -ril "mitmproxy\|Local Device Trust" Managed_Configuration_Profiles/ 2>/dev/null

# 3. App cheat / sideload
grep -iE "trollstore|altstore|esign|feather|gbox|scarlet|iosgods|ffh4x|aimbot" summaries/Applications.txt

# 4. Process lạ
grep -iE "frida|cycript|substrate|cydia|sileo" taskinfo.txt ps.txt 2>/dev/null

# 5. Crash FF có string injection
grep -ril "DYLD_INSERT\|libsubstrate" crashes_and_spins/ 2>/dev/null
```

Kết quả kỳ vọng với stack hiện tại: (1) có proxy, (2) có CA, (3)–(5) trống.

---

## 6. Quy trình "clean state" trước khi bị kiểm tra

1. Settings → General → VPN & Device Management → xoá profile CA.
2. Settings → General → About → Certificate Trust Settings → tắt trust (nếu
   còn).
3. Wi-Fi → (i) → Configure Proxy → Off.
4. Xoá app FF (xoá luôn codepatch cache trong container) → cài lại từ App Store.
5. Không cài Scriptable/a4ther, không cài profile lạ, không JB.
6. Đăng nhập lại tài khoản; prefs sạch (patch cũ đã xoá localConfig/111 key).

Sau bước này máy không còn dấu vết MITM/patch ở mức file/network. Memory thì
đã reset theo app.

---

## 7. Những thứ không nên làm

- **Không chạy a4ther** trên máy chính: script obfuscate, một phần được ship
  dưới dạng `.enc`, chạy bằng `curl | sh`. Nó đọc sysdiagnose (app list,
  profile, network, crash) — đúng loại dữ liệu không nên đưa cho code lạ.
- **Không cài "antiban profile" / VPN profile lạ** để trốn check: càng thêm
  profile càng lộ.
- **Không sideload IPA FF** hoặc cài TrollStore: đó là 3 mục alert độc lập.
- **Không jailbreak** chỉ để "ẩn profile": JB bị phát hiện chắc chắn hơn.

---

## 8. Trạng thái triển khai

- `proxy/rotate-ca.sh` đã có; chạy không tham số để sinh `proxy/conf-next`,
  `--apply` để chuyển. **Chưa apply** — cần Tris đồng ý vì mọi thiết bị phải
  cài lại CA một lần.
- `deploy.sh` đã sửa để không ghi đè CA đã xoay (chỉ symlink khi file chưa tồn
  tại).
- Checklist này chỉ ra: sau khi xoay CA, mục 8 sạch; mục 7 và 9 vẫn lộ bản
  chất MITM.


---

## 9. FairGuard — anti-cheat thật trên iOS (nguồn iOSGods + FairGuard)

Đây là phần quan trọng nhất vừa tìm được, và nó đổi cách nhìn: trên iOS, thứ
chấm điểm không phải ACE/ANO (Android) mà là **FairGuard**.

### 9.1 Nguồn

- iOSGods `topic/221481-fairguard-reversal-free-fire` (sasd1g, 10/2026):
  reverse trực tiếp `libil2cpp`/FairGuard của FF iOS 1.132.x.
- FairGuard official blog: `_posts/2023-06-15-iOS游戏反外挂方案解析.md`,
  `_posts/2026-08-27-iOS游戏加固：代码混淆、资源加密、内存保护与运行时检测.md`.

### 9.2 FairGuard chấm gì

Thread liệt kê score object getters `0x9711d80–0x97121bc` với JSON keys:

```text
nRoot, nSimu, nAccel, nDebug, nModify, nRepack, nResign, thSigMatch, defense
```

và POST `https://gin.freefiremobile.com/ios/.../create` với
`jb, fg_ver, ig1, ig2, pid, h, path, s, bd, cpu, sv, gv, dm, gm, cd` + các `n*`.

Điểm quan trọng:

- `h` = **SHA256 của `__TEXT`**. Thread nói thẳng: gửi hash của TEXT đã patch
  là ban; hash official là hằng số.
- Team ID official `98573T3W6L`, bundle `com.dts.freefireth`, CDHash
  SHA256/SHA1 + UnityFramework CDHash; `CC_MD5("com.dts.freefireth_<TEAM>")`
  tại `0x9381f90`.
- FairGuard check `GOT._syscall` / `_dlsym`: bytes `+4 == 0x31`,
  `+7 == 0x91`, cộng CFF checker `0x9457f6c` (34 callers). Hook GOT hoặc
  software breakpoint → **modifier ban**.
- Debugger `P_TRACED` cũng có thể bị tính là modifier.
- App Attest: các cmd `0xf2 0xf3 0xf4 0xf6 0xf9 0xfa`, `0xed 0xee 0x79 0x7b
  0x7c`; native probe `0x65–0x70` (bỏ 0x65/0x70 → kẹt lobby).
- `CheckHackBehavior`: `tpsdk_str` = iOS uid; `file_exception`/`lib_result` XOR
  với uid decimal; `client_version`, `channel_type=3`, `architectual_type=1`.

FairGuard official docs xác nhận các nhóm: anti-debug (ptrace, syscall, sysctl,
exception), anti-jailbreak (app/file/quyền thư mục), anti-resign (so signature
nhúng với runtime), anti-modifier, runtime detection (signature, Team ID,
Bundle ID, file integrity, rootless, debugger, dylib lạ, Frida, **hook hàm
quan trọng**), integrity resource khi load (AssetBundle/metadata).

### 9.3 Map vào stack của mình

| FairGuard check | Ý nghĩa | Stack hiện tại |
|---|---|---|
| `nRoot` | jailbreak | **SẠCH** — không JB |
| `nSimu` | simulator | **SẠCH** — máy thật |
| `nAccel` | cảm biến/emulator | **SẠCH** |
| `nDebug` | debugger `P_TRACED` | **SẠCH** |
| `nModify` | sửa runtime / hook | **CHƯA RÕ** — IFix patch là sửa runtime; đây là rủi ro chính |
| `nRepack` | đóng gói lại | **SẠCH** — app App Store |
| `nResign` | ký lại | **SẠCH** |
| `thSigMatch` | Team ID / Bundle / signature | **SẠCH** — 98573T3W6L |
| `h = SHA256(__TEXT)` | hash Mach-O | **SẠCH** — mình không sửa file app; codepatch nằm ngoài bundle |
| GOT `_syscall`/`_dlsym` | hook GOT | **SẠCH** — IFix không hook GOT |
| App Attest (`0xf2…`) | attestation | **SẠCH** — gin đi trực tiếp, PAC chỉ CDN |
| `CheckHackBehavior` | telemetry | **SẠCH** — đi trực tiếp, không sửa |

### 9.4 Kết luận từ FairGuard

- Cách mình đang làm (app App Store nguyên vẹn + codepatch) **tránh đúng** các
  thứ FairGuard chấm nặng: `nRepack`, `nResign`, `thSigMatch`, `h(__TEXT)`,
  `nRoot`, `nDebug`.
- Sideload IPA mod (iOSGods/Platinmods) là **sai hoàn toàn** với FairGuard: ký
  lại → `nResign`/`thSigMatch`/`h` lệch → ban.
- Rủi ro còn lại duy nhất: **`nModify`** — FairGuard nói có detect "hook hàm
  quan trọng". IFix patch thay body method; 6 hook + 333 field là bề mặt cần
  theo dõi. Chưa có bằng chứng nó bắt được IFix, nhưng cũng chưa có bằng chứng
  nó không bắt.
- Vì vậy: giữ hook tối thiểu (đã có `assert_hooks_clean`), không thêm hook vào
  hàm nhạy cảm, và ưu tiên tắt AimKill — đó là thứ tạo ra hành vi bất thường
  rõ nhất, độc lập với FairGuard.


### 9.5 Bổ sung từ blog chính thức FairGuard

- **Hot-update AB**: FairGuard mã hoá AssetBundle cả bản trong app lẫn bản tải
  về, và nói rõ "resource integrity khi load" là một lớp bảo vệ. Codepatch của
  mình là resource tải về; hiện nó vẫn load được, nghĩa là hoặc nhánh này
  không bị check, hoặc manifest client-side đã thoả. **Chưa rõ server có so
  hash official không.**
- **HybridCLR DLL**: FairGuard hỗ trợ mã hoá toàn bộ DLL và từng function, giải
  mã on-demand; kèm `global-metadata.dat` encryption/obfuscation. Đây là lý do
  việc re-encrypt bundle của mình bị chặn ở metadata 1.132.18.
- **AI aim detection**: FairGuard công bố model "time-space dual-modal" dùng
  quỹ đạo xoay góc nhìn, độ trễ quyết định bắn, phổ micro-jitter; claim 96.5%
  detect, <0.3% false positive. Đây là lý do AimKill/Aimbot là rủi ro hành vi
  số 1, độc lập với mọi thứ file/string.

Chi tiết + nguồn: `docs/RESEARCH-2026-11.md`.
