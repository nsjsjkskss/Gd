# Báo Cáo Đối Chiếu Nhị Phân & Giả Lập Unicorn: IPA 1 (`T74ODgYWCToR`) vs. IPA 2 (`m5d0l6e-f3701-fast.ipa` / `HunterFF.dylib`)

## 1. Tổng Quan Kết Luận Đo Trực Tiếp
Gói IPA thứ hai (`m5d0l6e-f3701-fast.ipa`, chuyển hướng tới `m5d0l6e.ipa`, dung lượng `663 MB`) **không phải bản game gốc sạch**, mà là một bản đóng gói lại (re-crack / re-pack) trực tiếp từ chính bản build `FreeFire.app` + `Penny.framework` của gói IPA thứ nhất (`T74ODgYWCToR`):
1. **Gỡ bỏ xác thực gốc của `Penny` (`pp7803/APIKey`)** và **khôi phục sẵn `154/154` trampoline bị cài bẫy** trong `UnityFramework` ngay trên đĩa.
2. **Đổi thương hiệu Telegram** được mã hóa trong `Penny` từ `t.me/pennyioshehehe` sang `t.me/xudeptraix2`.
3. **Tiêm thêm `Frameworks/Penny.framework/HunterFF.dylib` (`4.673.920 byte`)** — một thư viện bảo vệ bằng máy ảo (custom ARM64 bytecode VM) áp đặt cổng đăng nhập key mới (`https://api.hackgamefile.com/sophia/verify`) đè lên `Penny`, đồng thời dùng `33` lệnh `_memcmp` để khóa chặt các byte đã vá trong `Penny` nhằm chống người khác gỡ bỏ `HunterFF.dylib`.

| Tệp trong `Payload/FreeFire.app` | IPA 1 (`T74ODgYWCToR`) | IPA 2 (`m5d0l6e.ipa`) | `LC_UUID` | Khác biệt trước `LC_CODE_SIGNATURE` |
| :--- | :--- | :--- | :--- | :--- |
| `FreeFire` | `71.776 B` (`521d7d655e2994e1...`) | `71.776 B` (`7f74ba585adfc725...`) | `456886fef39833ed965c3c040c032f72` | `0 byte` (giống hệt 100% mã máy và dữ liệu). |
| `Frameworks/DataDomeSDK.framework/DataDomeSDK` | `319.696 B` (`942291bf7b47555e...`) | `319.696 B` (`95d75ae661ae4dc2...`) | `058758ec062035848338c1a373596ae0` | `0 byte` (giống hệt 100% mã máy và dữ liệu). |
| `Frameworks/UnityFramework.framework/UnityFramework` | `205.169.360 B` (`de080d44ace60249...`) | `205.187.296 B` (`9fc6195ad3577157...`) | `c8de7371cba73e7a9e0930b8e1b89073` | **`1.321 byte`**: `6 B` tại Mach-O header (`__LINKEDIT`), `923 B` tại `__TEXT_EXEC.__objc_tramp`, `392 B` tại `__AUTH_EXEC.__auth_stubs`. |
| `Frameworks/Penny.framework/Penny` | `216.213.056 B` (`453503914ede72d1...`) | `218.941.760 B` (`eeacd3e48b4980ea...`) | `05b0fb2a91b236b7975fb0a047585575` | **`8.486 byte`**: `5.172 B` tại vùng trống header (`0x0..0x5bd0`), `3.271 B` tại `__TEXT.__text`, `12 B` tại `__TEXT.__stubs`, `3 B` tại `__TEXT.__objc_methlist`, `22 B` tại `__TEXT.__const`, `6 B` tại `__DATA_CONST.__const`. |
| `Frameworks/Penny.framework/HunterFF.dylib` | *(Không tồn tại)* | `4.673.920 B` (`ee...`) | `970ade6095d434ffb727755db07b4f1d` | Tệp dylib mới được thêm vào IPA 2 và nạp qua `LC_LOAD_DYLIB` thứ `43` của `Penny`. |
| `Info.plist` | `10.000 B` (`d2dbad99ef970d61...`) | `10.000 B` (`f290981ab8928cfd...`) | — | `MinimumOSVersion`: `'17.0'` $\rightarrow$ `'10.0'`. |

---

## 2. Chứng Minh Thực Nghiệm Trên `UnityFramework`: Khôi Phục `154/154` Slot Bẫy Hậu-Xác-Thực
Trong IPA 1, có tổng cộng **`155` slot** trong `UnityFramework` chứa cặp lệnh `stp d11, d10, [sp, #-0x40]! ; b #0x5632fe4` (`eb 2b bc 6d ...`):
- **`110` slot** nằm trong `__TEXT_EXEC.__objc_tramp` (`0xc388000`, file offset `0xbf44000`).
- **`45` slot** nằm trong `__AUTH_EXEC.__auth_stubs` (`0xc39c000`, file offset `0xbf58000`).

Đối chiếu với IPA 2 cho thấy:
1. **`AuthSlot 12` (`VA 0xc39c180`) là hàm gốc thật sự duy nhất của mẫu lệnh này**: Điểm đặt hook tại `UnityFramework 0x5632fe0` là `b #0xc39c180`, và lệnh tại `0x5632fe4` là `stp d9, d8, [sp, #0x10]`. Khi công cụ đóng gói của `Penny` tạo IPA 1, nó đã sao chép 8 byte fallback của `0xc39c180` (`stp d11, d10, [sp, #-0x40]! ; b #0x5632fe4`) để ghi đè làm bẫy (poisoned fallback) lên toàn bộ **`154` slot còn lại** (`110` trong `__objc_tramp` + `44` trong `__auth_stubs`).
2. **Trong IPA 2, toàn bộ `154/154` slot bẫy đã được ghi đè trên đĩa bằng lệnh prologue gốc thật sự của từng hàm kèm nhánh nhảy quay về `UnityFramework`**:
   - `110/110` slot trong `__TEXT_EXEC.__objc_tramp` được khôi phục hoàn chỉnh (ví dụ `0xc388118`: `stp x20, x19, [sp, #-0x20]! ; b #0x5968ed4`).
   - `44/44` slot bẫy trong `__AUTH_EXEC.__auth_stubs` được khôi phục hoàn chỉnh (trong đó `6` slot thuộc nhóm hook xa 12-byte nên ghi tràn thêm 4 lệnh sang các slot `udf #0` kế tiếp: `AuthSlot 17, 27, 37, 55, 57, 79`).
   - Toàn bộ danh sách 154 prologue đã khôi phục được xuất đầy đủ trong [`artifacts/ipa2_comparison_dump.txt`](artifacts/ipa2_comparison_dump.txt).

---

## 3. Phân Tích Toàn Bộ `8.486 Byte` Vá Trong `Penny.framework/Penny` (IPA 2)

### 3.1. Vô hiệu hóa `1.792` bẫy `svc` chống hook trong vùng tính năng (`0xeb000..0x219000`)
- Trong `1.881` dải lệnh thay đổi của `Penny.__TEXT.__text`, có đúng **`1.792` vị trí** là lệnh `mov w16, #1` (hoặc `mov w16, #0x1a`) nằm ngay trước lệnh `svc #<imm>` trong bộ tự kiểm tra đầu hàm (`ldr w8, #<entry> ; bl #0x2184e4 ; bl #0x2184f0 ; adr x8, #<entry> ; bl #0x20fca4`).
- IPA 2 thay cả `1.792` lệnh `mov w16, ...` bằng `b #<pc+8>` để nhảy qua lệnh `svc`, giúp các hàm trong `Penny` không tự thoát tiến trình khi bị sửa byte.

### 3.2. Ghi cứng hạt giống giải mã chuỗi `0x2fddd4d1` và các hằng số xác thực
- **Tại `0xb4a04` (trong `sub_b497c`)**: Thay lệnh đọc biến toàn cục `[0xcc74870]` bằng `nop ; mov w0, #0xd4d1 ; movk w0, #0x2fdd, lsl #16 ; b #0xb4a38` (ghi cứng `SEED = 0x2fddd4d1`).
- **Tại `0xb6ac8`**: Ghi cứng hằng số 64-bit `x0 = 0x1fbba9ea58632790` và nhảy thẳng tới `0xb8f08`.
- **Tại `0xb4a90`, `0xb4c48`, `0xba8a4`, `0xbb368`**: Ép nhánh kiểm tra và đặt `ret` ngay đầu hàm `0xbb368` (vô hiệu hóa màn hình đăng nhập gốc của `Penny`).
- **Tại `0x639f5cc` (stub `_NSLog`)**: Ghi đè thành `ret ; nop ; nop`.

### 3.3. Đổi liên kết Telegram được mã hóa (`0x48394` và `0x4a71c`)
- IPA 2 sửa `8 B` tại `0x63da8f8`, `14 B` tại `0x63db785..0x63db7af` và hai cặp `mov/movk w8` tại `0x48378`, `0x4a700`.
- Giải mã bằng `sub_b497c` với `SEED = 0x2fddd4d1`:
  - `0x48394` (`w2 = 0x9012403d`): `'t.me/pennyioshehehe'` $\rightarrow$ **`'t.me/xudeptraix2\x00\x00\x00'`**.
  - `0x4a71c` (`w2 = 0x2b28ae33`): `'https://t.me/pennyioshehehe'` $\rightarrow$ **`'https://t.me/xudeptraix2\x00\x00\x00'`**.

### 3.4. Chuyển hướng 9 điểm nút của `Penny` sang bảng cầu nối `__DATA.__bss` (`0xcc94280..0xcc942c0`)
IPA 2 mở rộng `__DATA.__bss` của `Penny` từ `0x44e8` lên `0x45b0` và chèn các stub điều hướng tại `0x3a00..0x3bc4` (trong vùng trống sau Mach-O header). Khi `Penny` chạy tới 9 vị trí trọng yếu, nó đọc con trỏ hàm tại `0xcc94280..0xcc942c0` do `HunterFF.dylib` nạp vào:

| Địa chỉ trong `Penny` | Stub trung gian trong `Penny` | Ô nhớ `__DATA.__bss` | Hàm đích trong `HunterFF.dylib` | Vai trò đo được qua Disassembly & Unicorn |
| :--- | :--- | :--- | :--- | :--- |
| `0xbb290` | *(trực tiếp)* | `0xcc94280` | `0x4170d4` | Chặn luồng khởi tạo UI sau `sub_b5090`, giữ lại block khởi tạo của `Penny` và hiển thị `PennyOwnerKeyViewController` của `HunterFF`. |
| `0xcd608` | `0x3a40` (prologue gốc tại `0x3bc0`) | `0xcc94288` | `0x40801c` | Chặn hàm ghi/cập nhật slot trampoline `sub_cd608`. |
| `0x3032c` (`__objc_methlist` `0x63a1078` -> `0x2f40`) | `0x3a60` (prologue gốc tại `0x3b80`) | `0xcc94290` | `0x4350b0` | Chặn hàm khởi tạo menu chính `0x3032c` cho tới khi `HunterFF` xác thực xong key. |
| `0xde6b4` | `0x3a80` (prologue gốc tại `0x3ba0`) | `0xcc94298` | `0x4352a4` | Chặn callback timer 350ms `0xde6b4` (gắn luồng IL2CPP & cài đặt methodPointer hook). |
| `0xbe8c4` | `0x3aa0` | `0xcc942a0` | `0x417150` | Trả về chuỗi hiển thị người dùng (`'hunterFF · chưa đăng nhập'` tại `0x44cf78` khi chưa đăng nhập). |
| `0xbee54` | `0x3ac0` | `0xcc942a8` | `0x41717c` | Trả về chuỗi trạng thái phụ (`0x0` khi chưa đăng nhập). |
| `0xbe19c` | `0x3ae0` | `0xcc942b0` | `0x417284` | Trả về cờ trạng thái xác thực (`0` khi chưa đăng nhập). |
| `0xcd9a8` | `0x3a00` -> `0x3b00` | `0xcc942b8` | `0x417288` | Trả về cứng timestamp hết hạn `0x6dbc2beb` (`1841048555` = `2028-05-05 03:42:35 UTC`). |
| `0xc3a84` | `0x3a10` -> `0x3b20` | `0xcc942c0` | `0x417294` | Hàm cầu nối kiểm tra đối tượng tại `0xc3a84`. |

---

## 4. Phân Tích `HunterFF.dylib` Bằng Giả Lập Unicorn Engine

### 4.1. Đánh lừa thời gian (`__DATA.__interpose` trên `_time`)
- Bảng `__DATA.__interpose` tại `0x451cc8` ánh xạ hàm `_time` về `0x40863c` (nhảy vào VM handler tại `0x39ccd8`).
- Giả lập Unicorn trên `0x40863c` xác nhận hàm này không gọi hệ thống mà **luôn trả về cứng `1794268800` (`0x6af25e80`, tức `2026-11-10 00:00:00 UTC`)**, vừa khớp với chuỗi định dạng `[TimeHook] Fake date: %s (ts=%lld)` trong `__cstring`.

### 4.2. `33` phép kiểm tra toàn vẹn `_memcmp` trong callback `0x416dec`
- Khi `dyld` nạp `Penny` (`LC_UUID: 05b0fb2a91b236b7975fb0a047585575`), callback `0x416dec` của `HunterFF.dylib` thực thi **`33` lệnh gọi `_memcmp`** để kiểm tra xem các vùng byte đã vá trong `Penny` (từ `0x2000`, `0x3a00..0x3bc4`, `0x4300`, `0x3032c`, `0x48378`, `0x4a700`, `0xba8a4`, `0xbb290`, `0xbb368`, `0xbe19c`, `0xbe8c4`, `0xbee54`, `0xcd608`, `0xde6b4` đến `0x133488`, `0x133644`) có khớp tuyệt đối với bảng dữ liệu mẫu tại `0x43ba78..0x43c824` trong `HunterFF.dylib` hay không.
- Chỉ khi cả `33/33` phép `_memcmp` đều trả về `0`, `0x416dec` mới thực hiện `12` lệnh ghi bộ nhớ: lưu base address của `Penny` (`0x452790`), địa chỉ hai thunk prologue `0x3b80` (`0x4522a8`) và `0x3ba0` (`0x452798`), rồi điền 9 con trỏ hàm vào `Penny.__DATA.__bss` (`0xcc94280..0xcc942c0`).

### 4.3. Giao thức xác thực của `HunterFF.dylib`
- Các lớp Objective-C trong `HunterFF.dylib` gồm: `PennyOwnerKeyViewController`, `PennyOwnerPinnedSession`, `HFFLoginNoticeController`, `HFFWatermarkView`.
- Khi người dùng bấm đăng nhập (`-[PennyOwnerKeyViewController login:]` tại `0x418c44`), `HunterFF.dylib` gửi yêu cầu `POST` (`Content-Type: application/json`) tới:
  - Endpoint: `https://api.hackgamefile.com/sophia/verify`
  - Trường JSON gửi đi: `package` (`"hunterff"`), `app_version` (`"1.0.0"`), `hwid`, `nonce`, `key`.
  - Trường JSON phản hồi được kiểm tra: `valid`, `code`, `user`, `expiry` / `expiresAt`, `server_time` / `serverTime`, và chữ ký Ed25519 `sig_ed2` được xác minh bằng khóa công khai 32-byte `f55a7bef05eb2fdd7580a003bf7d29c4811f27eb859b8b3d37805d3f2bfb16de`.

---

## 5. Giải Mã Sâu Kiến Trúc Máy Ảo (VM), TLS Pinning, Ed25519 & Backend (`tools/devirt_hunterff_full.py`)

### 5.1. Kiến trúc Máy ảo Bytecode ARM64 (`0x007300 .. 0x404100`)
- Vùng mã máy ảo dài `0x3fce00` byte (`4.181.504 byte`) nằm ngay trước `__TEXT.__text` (`0x408000 .. 0x4357a8`).
- Trên toàn bộ `484` hàm trong `LC_FUNCTION_STARTS`, có **`8.863` nhánh nhảy `b #<vm_target>`** từ `__TEXT.__text` vào vùng VM và **`8.522` điểm nhảy quay về (`[<=VM_RESUME]`)** trong `__TEXT.__text`:
  - **`4.322` điểm vào `BytecodeVM`**: Đẩy ngữ cảnh thanh ghi `x0..x30` lên stack qua `0x560e8` / `0x2e0ab8` (`x2 = sp` là mảng thanh ghi ảo `vregs`), nạp `x0 = tbl_base`, `x3 = VIP` (con trỏ dòng bytecode) và nhảy vào 1 trong 2 trình điều phối trung tâm (**`0x223dc0`** hoặc **`0x3a2428`**).
  - **`4.541` điểm vào `ObfTramp`**: Các trampoline lệnh máy đa hình có chèn chuỗi lệnh rác trung hòa (`rev`/`rbit` kép, hoán vị cộng-trừ, ghi đè thanh ghi chết trước `ldp`).
  - **Tập lệnh Bytecode & Hybrid Native Snippets**: Opcode 16-bit tại `[x3]` (`0x002d` nạp hằng 64-bit, `0x0030` ghi vreg, `0x0037`/`0x0024` đọc vreg, `0x005c` nhảy tới đoạn mã native 5 lệnh tại `(VIP + 2) + rel64` rồi `add x3, x3, #0xa ; br x6`, `0x0007` thoát khỏi VM về `(VIP + 2) + rel64`).

### 5.2. Cơ chế TLS Pinning trong `PennyOwnerPinnedSession` (`0x41bcd4`, `0x41bfa0` & `sub_4165b0`)
- Hàm `sub_41bfb0` (`0x41c488 .. 0x41c510`) tạo `ephemeralSessionConfiguration`, xóa sạch `URLCache`, `HTTPCookieStorage`, `URLCredentialStorage`, ép `TLSMinimumSupportedProtocolVersion = 0x0303` (**TLS 1.2**) và gắn delegate `PennyOwnerPinnedSession` (`0x451ac0`).
- `-[PennyOwnerPinnedSession URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:]` (`0x41bfa0`) luôn truyền `nil` vào `completionHandler` để chặn mọi HTTP redirect.
- `-[PennyOwnerPinnedSession URLSession:didReceiveChallenge:completionHandler:]` (`0x41bcd4`) gọi **`sub_4165b0(serverTrust, host)`** (`0x4165b0`):
  1. Kiểm tra `strcmp(host, "api.hackgamefile.com") == 0` (`0x445e99`).
  2. Đánh giá chuỗi chứng chỉ chuẩn qua `SecPolicyCreateSSL(true, @"api.hackgamefile.com")` và `SecTrustEvaluateWithError`.
  3. Duyệt từng chứng chỉ trong chuỗi (`SecTrustGetCertificateCount` / `SecTrustGetCertificateAtIndex`), băm `CC_SHA256` + mã hóa Base64 (`sub_416a0c`) trên cả DER cert (`SecCertificateCopyData`) và khóa công khai SPKI (`SecCertificateCopyKey` + `SecKeyCopyExternalRepresentation`, tự ghép 24 byte ASN.1 header RSA-2048 tại `0x43ba38`: `30820122300d06092a864886f70d01010105000382010f00` nếu khóa dài `0x10e = 270` byte) rồi đối chiếu với **3 mã Pin SHA-256 Base64**:
     - **Pin 1 (`0x445eae`)**: `ByY50LFA1b/64WrZw/bMYIYEBiH1HuYabUaokVwHz3Y=`
     - **Pin 2 (`0x445edb`)**: `lrzsBiZJdvN0YHeazyjFp8/oo8Cq4RqP/O4FwL3fCMY=`
     - **Pin 3 (`0x445f08`)**: `EsfgDUB7JOgzShwSwJuq7obKoRCK7FXLITmeRtW8tDg=`

### 5.3. Cơ chế Xác minh Chữ ký Ed25519 (`sub_4086f8` & `sub_421714`)
- Trong `sub_4086f8` (`0x4086f8 .. 0x409348`), sau khi kiểm tra `|device_time - server_time| <= 300.0` giây (`0x40891c..0x408930`), hàm giải mã `sig_ed2` (hex 128 ký tự $\rightarrow$ 64 byte) và khóa công khai Ed25519 tại **`0x444952`** (`f55a7bef05eb2fdd7580a003bf7d29c4811f27eb859b8b3d37805d3f2bfb16de` $\rightarrow$ 32 byte).
- **Định dạng Chuỗi Chuẩn Hóa (Canonical Message) 6 trường (`0x408b9c .. 0x408db8`)**:
  ```text
  <valid_bit>|<user>|<expiry>|<hwid>|<nonce>|<server_time>
  ```
  Trong đó `<valid_bit>` là `"1"` hoặc `"0"`, `<hwid>` lấy từ Keychain (`net.hunterff.penny` / `device`), `<nonce>` là chuỗi hex 32 ký tự sinh từ `arc4random_buf(16)`, và `<server_time>` là chuỗi số nguyên `std::to_string((long long)server_time)`.
- **Hàm xác minh Ed25519 Ref10 + SHA-512 (`sub_421714` tại `0x421714 .. 0x421840`)**:
  - Kiểm tra $S < L$ với hằng số bậc nhóm $L$ tại `0x4447e0` (`edd3f55c1a631258d69cf7a2def9de1400000000000000000000000000000010`).
  - Giải nén điểm khóa công khai $-A$ qua `sub_41f5e0` (`ge_frombytes_negate_vartime`).
  - Tính `SHA-512(R || A || msg)` qua `sub_42104c`, `sub_421098`, `sub_42154c`, rút gọn modulo $L$ qua `sub_41fe58` (`sc_reduce`), tính $R_{\text{calc}} = [S]B + [k](-A)$ qua `sub_41e6f0` (`ge_double_scalarmult_vartime` + 77 hàm bảng điểm cơ sở `0x421844..0x434eac`), nén điểm qua `sub_41f7f4` (`ge_tobytes`) và so sánh thời gian hằng 32 byte bằng tập lệnh SIMD NEON (`cmeq` + `mvn` + `orn` + `umaxv` tại `0x421818..0x42183c`).
  - Giả lập Unicorn trên `sub_421714` với vector chữ ký chuẩn RFC 8032 xác nhận hàm trả về `1` khi chữ ký hợp lệ và `0` khi thông điệp bị sửa đổi.

### 5.4. Keychain, Chu kỳ Re-verify 45 Giây & Watermark (`0x41895c .. 0x41bc68`)
- **Keychain (`0x41895c` & `0x418fdc`)**: Lưu dưới `kSecAttrService = "net.hunterff.penny"` (`0x44cff8`) với 2 tài khoản `"device"` (mã thiết bị `UUIDString`) và `"key"` (license key đã trim và `uppercaseString`).
- **Timeout & Re-verify**: Mỗi phiên xác thực có hạn chờ tối đa `90.0` giây theo `mach_absolute_time` (`0x40a3e8`). Sau khi đăng nhập, `HunterFF.dylib` đặt lịch kiểm tra lại key mỗi **`45` giây** (`0xa7a358200 ns` tại `0x419ad4`, `0x419bb8`) và mỗi khi ứng dụng vào tiền cảnh (`UIApplicationDidBecomeActiveNotification`).
- **Watermark (`init[3]` tại `0x41b888`)**: Quét cửa sổ hoạt động mỗi `1.0` giây để gắn `HFFWatermarkView` (`'xumod_crack'` với `CAGradientLayer` 7 màu cầu vồng).
