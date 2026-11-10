# Báo Cáo Phân Tích Tĩnh: `Penny.framework/Penny` & `UnityFramework`

## 1. Phạm vi và Giới hạn
- Báo cáo này ghi nhận kết quả phân tích tĩnh (static analysis) trên tệp nhị phân `Penny` và `UnityFramework` trích xuất từ gói IPA kiểm tra.
- Không thực hiện việc chỉnh sửa nhị phân (binary patching) để bỏ qua cơ chế xác thực khóa (login/key authentication) hoặc kích hoạt trái phép các chức năng trong ứng dụng.

## 2. Cấu trúc Phân vùng Mach-O (`Penny`)
Tệp thực thi `Frameworks/Penny.framework/Penny` là thư viện động Mach-O kiến trúc `arm64` (`cputype = 0x100000c`, `imagebase = 0x0`).

| Segment / Section | Địa chỉ ảo (VA) | Kích thước (Size) | File Offset | Ghi chú đo đạc |
| :--- | :--- | :--- | :--- | :--- |
| `__TEXT.__text` | `0x4b40` | `0x39a65c` | `0x4b40` | Mã máy thực thi chính của thư viện. |
| `__TEXT.__objc_stubs` | `0x39f1a0` | `0x6000000` (96 MB) | `0x39f1a0` | 24.576 trang 4 KB chứa dữ liệu khác không (làm phình kích thước / gây nhiễu phân tích). |
| `__TEXT.__const` | `0x63da000` | `0x318dc0` | `0x63da000` | Bảng hằng số và dữ liệu cấu hình mã hóa. |
| `__TEXT.__cstring` | `0x66f2dc0` | `0x131ddb` | `0x66f2dc0` | Chuỗi C và các khối byte mã hóa XOR. |
| `__TEXT.__swift5_types` | `0x6834550` | `0x6400008` (100 MB) | `0x6834550` | 25.600 trang 4 KB chứa dữ liệu khác không. |
| `__DATA_CONST.__mod_init_func` | `0xcc4c490` | `0x48` | `0xcc4c490` | 9 hàm khởi tạo: `0xad7c8`, `0xe8828`, `0x14a140`, `0x14abe0`, `0x14ad90`, `0x14ae94`, `0x14af9c`, `0x14b0a0`, `0x218f5c`. |
| `__DATA_CONST.__objc_nlclslist` | `0xcc61748` | `0x10` | `0xcc61748` | 2 lớp có `+load`: `0xcc73420` (`_UISceneBootstrapLoader`) và `0xcc73ec0` (`_UIPaneHUDBox`). |
| `__DATA.__ppguard` | `0xcc85500` | `0x10` | `0xcc85500` | Vùng dữ liệu kiểm tra riêng dài 16 byte. |

### Các phân vùng bổ sung trong `UnityFramework`
Tại cuối tệp `Frameworks/UnityFramework.framework/UnityFramework` (từ file offset `0xbf44000`) xuất hiện 4 phân vùng:
- `__TEXT_EXEC`: `va = 0xc388000`, `foff = 0xbf44000`, `size = 0x10000` (vùng có dữ liệu khác không: `0x0 .. 0x89f4`).
- `__DATA_DIRTY`: `va = 0xc398000`, `foff = 0xbf54000`, `size = 0x4000` (vùng có dữ liệu khác không: `0x0 .. 0x80`).
- `__AUTH_EXEC`: `va = 0xc39c000`, `foff = 0xbf58000`, `size = 0x4000` (vùng có dữ liệu khác không: `0x0 .. 0xc20`).
- `__AUTH_DATA`: `va = 0xc3a0000`, `foff = 0xbf5c000`, `size = 0x4000` (vùng có dữ liệu khác không: `0x0 .. 0x80`).

## 3. Phân loại Hệ thống Lớp Objective-C (`Penny`)
Bảng `__objc_classlist` (`va = 0xcc61500`, kích thước `0x248`) chứa 73 lớp Objective-C:
1. **Nhóm khởi tạo và dựng bố cục giao diện:**
   - `_UISceneBootstrapLoader` (`0xcc73420`): Nằm trong `__objc_nlclslist`, được gọi khi nạp thư viện.
   - `_UISceneLayoutRenderController` (`0xcc73060`): Chứa các phương thức đọc/ghi cấu hình (`_readTraitConfigurationArchive` tại `0x2be54`, `_writeTraitConfigurationArchive` tại `0x2aab8`) và dựng giao diện (`_constructOverlaySheetContainer` tại `0x32c8c`, `_constructNavigationRailView` tại `0x33578`, `_buildTraitSectionLayout_01`..`_04` tại `0x3a6b8`..`0x3d9bc`).
2. **Nhóm `_UIKite*` (32 lớp):**
   - Bao gồm `_UIKiteKeyHandoff`, `_UIKiteKeyView`, `_UIKiteGlassKeyView`, `_UIKiteUDIDView`, `_UIKiteDenyPane`, `_UIKiteSSEC`, `_UIKiteRequest`, `_UIKiteToks`, `_UIKitePresentationToken`, `_UIKiteToastPanel`, `_UIKiteBannerWindw`, `_UIKiteOverlayPassWindw`, v.v.
   - Đảm nhiệm giao diện nhập khóa, kiểm tra UDID và duy trì luồng sự kiện Server-Sent Events (SSE).
3. **Nhóm `_UIPaneHUD*` (14 lớp):**
   - Bao gồm `_UIPaneHUDBox` (`0xcc73ec0`), `_UIPaneHUDIndicatorBoxView`, `_UIPaneHUDRingIndicatorBoxView`, v.v., hiển thị thông báo trạng thái.
4. **Nhóm `_UIFlow*` (12 lớp):**
   - Bao gồm `_UIFlowWindowRootVCController`, `_UIFlowAlert`, `_UIFlowAlertBuilder`, `_UIFlowSwitch`, `_UIFlowBt`, `_UIFlowTimerBox`.

## 4. Cơ chế Làm rối Chuỗi và Danh mục Endpoint Mạng
1. **Giải mã dòng tại `0xb497c`:**
   - Sử dụng phép trộn bit 32-bit (hằng số nhân `0x85ebca6b` và `0xc2b2ae35`, bước cộng `0x9e3779b9`) kết hợp `seed` tính từ mã máy để giải mã các khóa cấu hình giao diện, tên tệp (`menu_settings.txt`, `guest_info.json`, `guest_auth.dat`) và định danh lưu trữ (`keychain://com.apple.uikit.traitcollection.storage/UITraitArchivePayloadV2`).
2. **Giải mã XOR một byte (quanh `0xec874` và nhóm `_UIKite*`):**
   - Giải mã các chuỗi log kiểm tra trạng thái giao diện (`[UIVERIFY] ...`, `[SSE] ...`, `[UI-Transition] ...`) và các đường dẫn API máy chủ:
     - `https://api6.ppdeveloper.xyz/api/v2/package/packagedata`
     - `https://api6.ppdeveloper.xyz/api/v2/package`
     - `https://api6.ppdeveloper.xyz/api/v3/key`
     - `https://api6.ppdeveloper.xyz/api/v1/udid/uuid_key`
     - `https://api6.ppdeveloper.xyz/api/v1/udid/find`
     - `https://udid3.ppdeveloper.xyz/getudid`

## 5. Cơ chế Kiểm tra Toàn vẹn (Integrity / Anti-Tamper)
- **Khóa giải mã phụ thuộc nội dung nhị phân:** Hàm `sub_b5100` đọc các word 32-bit tại các offset cố định (`0x0`, `0x12fccc`, `0x133484`, `0x133640`, `0x130dec`) và kết hợp với hàm băm vùng mã tại `sub_62a90` để tạo `seed` giải mã. Bất kỳ thay đổi byte nào trên vùng được băm sẽ làm sai lệch kết quả giải mã lúc chạy.
- **Bẫy làm sập tiến trình (Crash Traps):** Tại các hàm hậu xử lý như `sub_cdad8` (`0xcdad8 .. 0xcdd28`), mã máy kiểm tra các cờ trạng thái tại `0xcc74880`, `0xcc8f6b0`, `0xcc8f6c0` rồi dùng `csel` + `br x16` nhảy vào khối lệnh xóa `x29`, `x30`, `sp` về `0` trước khi thực thi `brk #1` / `udf #0` nếu phát hiện trạng thái không hợp lệ.

## 6. Trạng thái Xác minh
- Toàn bộ thông tin trên được đo trực tiếp từ tệp nhị phân tĩnh.
- Chưa xác minh hành vi lúc chạy (runtime) trên thiết bị thực tế và chưa có bản IPA gốc (clean build) cùng phiên bản để đối chiếu nhị phân (binary diff) đối với phần mã chính của `UnityFramework`.
