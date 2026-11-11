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

## 7. Đối chiếu Kiến trúc với SDK `pp7803/APIKey` (v6.0.5 – v6.1.0)
Đối chiếu tài liệu công khai của `https://github.com/pp7803/APIKey` (`APIKey 6.1` của tác giả Phát Phạm / `@pdp7803`) với kết quả đo đạc tĩnh trên `Penny` cho thấy `Penny` liên kết tĩnh biến thể **Full (`libPPAPIKey_full.a` / `libPPAPIKey_custom_full.a`)** dành cho gói VIP3:

1. **Kiến trúc tách biệt Tweak và Core qua C Bridge:**
   - Tài liệu `APIKey 6.0+` định nghĩa các hàm C Bridge gồm `setTokenC(const char *)`, `setENC(int)`, `setVerC(const char *)`, `loadingC(void (^execute)(void))` và `packageData(void (^completion)(id data))`.
   - Phần mã của tweak đăng ký callback khởi chạy (qua `CFNotificationCenterAddObserver` với `UIApplicationDidFinishLaunchingNotification`) và truyền completion block `execute` vào `loadingC`. Khối lệnh tạo giao diện chức năng chỉ được gọi sau khi `loadingC` hoàn tất xác thực với máy chủ.

2. **Đối chiếu hệ thống Theme và Lớp Giao diện (`_UIKite*`, `_UIPaneHUD*`, `_UIFlow*`):**
   - Các bản phát hành `APIKey 6.0.1` đến `6.1.0` cung cấp 13 theme (`ANDROID`, `CST`, `GLASS`, `HACKER`, `JG`, `LINUX`, `MAC`, `MBP`, `MINECRAFT`, `NEWYEAR2026`, `PGAM`, `SCL`, `XP`) cùng chế độ nhập key `UIAL` (UIKit Alert).
   - Các chuỗi giải mã được trong `Penny` khớp trực tiếp với lịch sử thay đổi của `APIKey`:
     - Chuỗi `[UIVERIFY] phase=%@ modalChrome=%d dimOverlay=%lu dimModal=%lu dimJg=%lu hide=%d loadingDim=%d enterkeyLive=%d keepDim=%d toast=%d anyHud=%d uialAlert=%p sclAlert=%p activeBlockModal=%p ppwinVisible=%d` (tại `0xef900`) tương ứng với cơ chế kiểm tra trạng thái modal/dim overlay được bổ sung ở bản `v6.0.5`.
     - Chuỗi `[UI-Transition] SCL auto -> enterkey handoff begin | session=%ld` (tại `0x209b98`) và lớp `_UIKiteKeyHandoff` tương ứng với luồng chuyển tiếp từ màn hình kiểm tra key (`SCL`) sang màn hình nhập key (`ENTERKEY`).
     - Chuỗi `[UI-Glass] contact btn label=%@ url=%@` (tại `0x112a3c`) và lớp `_UIKiteGlassKeyView` tương ứng với theme **Liquid Glass (`GLASS`)** được tinh chỉnh ở bản `v6.0.2` – `v6.0.4`.
     - Lớp `_UIKiteSSEC` và các chuỗi `[SSE] connected http=%d`, `[SSE] event pk_thongbao=%d debstatus=%d hide=%d k_thongbao=%d k_khoamay=%d k_hethan=%d` (tại `0x12c464 .. 0x12d5b4`) là kênh nhận sự kiện thời gian thực từ portal quản lý khóa.
     - Chuỗi `PPWindow` và `PPHideScreenShowView hitTest: miss subviews, returning nil (pass-through)` (tại `0xf1a30`) tương ứng với tính năng ẩn lớp phủ (`pp_isHide`).

3. **Đối chiếu cơ chế `Anti-Hex` (`PPAPIKey Hash Generator`):**
   - Tài liệu `pp7803/APIKey` mô tả hai cấp độ bảo vệ: **Anti-Hex Semi** (khai báo `IDF` và `Signature` hash trên Dashboard `Package Hash`) và **Anti-Hex Full** (đưa `Dylib` qua ứng dụng `PPAPIKey Hash Generator` dành cho VIP3).
   - Trên tệp nhị phân `Penny`, lớp bảo vệ `Anti-Hex Full` thể hiện qua:
     - Hai section làm phình kích thước chứa dữ liệu ngẫu nhiên/làm rối: `__TEXT.__objc_stubs` (`96 MB`) và `__TEXT.__swift5_types` (`100 MB`), cùng các thông tin `LC_FUNCTION_STARTS` và `LC_DATA_IN_CODE` đặt lệch khỏi `__LINKEDIT`.
     - Section `__DATA.__ppguard` (`va = 0xcc85500`, `16 byte`).
     - Các hàm băm kiểm tra toàn vẹn vùng mã (`sub_62a90`, `sub_b5100`) kết hợp khóa từ phản hồi máy chủ (`packagedata`) và các nhánh bẫy xóa thanh ghi `x29`, `x30`, `sp` trước khi gọi `brk #1` (`sub_cdad8`).

## 8. Kiểm kê Công cụ Phân tích trong Môi trường Thực thi
Kết quả kiểm tra trực tiếp trên môi trường container Linux (`x86_64`) hiện tại đối với danh sách công cụ:

| Nhóm công cụ | Công cụ có sẵn và đã sử dụng | Công cụ không cài đặt sẵn hoặc phụ thuộc macOS/iOS |
| :--- | :--- | :--- |
| **Đọc cấu trúc Mach-O & Disassembly** | `lief` (Python), `capstone` (Python), `/usr/bin/objdump`, cùng bộ parser Mach-O / Objective-C tự viết (`tools/macho_inspect.py`, `tools/objc_dump.py`). | `r2` / `radare2` (`r2ghidra`), `ghidra`, `llvm-objdump`, `llvm-otool`, `jtool2`, `ipsw`, `dsdump`, `ktool` (`k2l`), `class-dump-z` / `class-dump-ng`, `Hopper Disassembler`, `Mach0kit`, `Malimite`. |
| **Giả lập CPU & Symbolic Execution** | `unicorn` (Python `arm64` emulator). | `qiling`, `angr`, `triton`, `miasm`, `dynamorio`. |
| **Gỡ lỗi động & Tracing** | Không khả dụng cho tệp Mach-O `arm64` iOS trên kernel Linux không có thiết bị iOS thực. | `lldb`, `gdb` + `pwndbg`, `rr`, `debugserver`, `objection`, `strace`, `bpftrace`. |
| **Nhận diện mẫu & Deobfuscation** | Script giải mã tĩnh chuyên biệt theo thuật toán đo được từ mã máy (`0xb497c` Murmur3-32 stream và `0xec874` XOR). | `capa`, `Detect It Easy (diec)`, `yara`, `floss`, `D810G`, `VMP_DEVIRT`, `bindiff`, `kaitaistruct`. |

**Lý do kỹ thuật ưu tiên dùng `lief` + `capstone` + parser Python tùy chỉnh trên bản build này:**
- Tệp `Penny` bị công cụ `Anti-Hex` chèn gần `196 MB` dữ liệu nhiễu vào `__objc_stubs` và `__swift5_types`, đồng thời làm lệch header `LC_FUNCTION_STARTS` và `LC_DATA_IN_CODE`. Các công cụ tự động quét toàn bộ tệp hoặc phụ thuộc vào `LC_FUNCTION_STARTS` sẽ bị treo hoặc báo lỗi phân tích, trong khi parser Python đọc trực tiếp đúng địa chỉ ảo (`va`) của `__objc_classlist` (`0xcc61500`) và `__text` (`0x4b40 .. 0x39f19c`).

## 9. Phân tích Chuyên sâu Luồng Khởi tạo (`__mod_init_func` & `+load`)
Khi `Penny` được nạp vào tiến trình, hệ thống khởi tạo chạy theo hai giai đoạn tĩnh:

1. **Giai đoạn `+load` của Objective-C (`__objc_nlclslist` tại `0xcc61748`):**
   - `+[_UISceneBootstrapLoader load]` (`imp = 0xe4d34`):
     - Đầu hàm (`0xe4d64 .. 0xe4db8`) kiểm tra trạng thái toàn cục tại `0xcc74880` và `0xcc8f6b0`. Nếu phát hiện trạng thái bất thường, hàm nhảy tới `0xe4da8` để xóa `x29`, `x30`, `sp` về `0` và gọi `brk #1`.
     - Tiếp theo (`0xe4dbc .. 0xe4e90`), hàm nạp vector 16 byte từ `0x63dd2f0` lên stack và thiết lập các hằng số khởi tạo cho bộ đệm trạng thái tại `0xcc8f000`.
   - `+[_UIPaneHUDBox load]` (`imp = 0x219150`):
     - Đăng ký các observer với `NSNotificationCenter.defaultCenter` cho các sự kiện bàn phím (`UIKeyboardWillChangeFrameNotification`, `UIKeyboardDidChangeFrameNotification`, `UIKeyboardDidHideNotification`) để căn chỉnh vị trí hộp thoại nhập key / HUD.

2. **Giai đoạn Constructor C++ (`__mod_init_func` tại `0xcc4c490`, gồm 9 hàm):**
   - `init_0` (`0xad7c8`): Khởi tạo vùng nhớ toàn cục tại `0xcc8f5a0 .. 0xcc8f5f8`, gán hệ số tỷ lệ float `1.0f` (`0x3f800000`) tại `0xcc8f5c0` và đăng ký các hàm dọn dẹp qua `__cxa_atexit` (`0x570c`, `0x5710`).
   - `init_1` (`0xe8828`): Gọi `dispatch_time` và `dispatch_after` trên `dispatch_main_q` (`0xcc4c2a0`) với block toàn cục tại `0xcc4d168` để trì hoãn việc gắn giao diện điều khiển vào cửa sổ chính sau khi cảnh (scene) của ứng dụng hoạt động.
   - `init_2` (`0x14a140`) đến `init_7` (`0x14b0a0`): Nhóm hàm khởi tạo của lõi `PPAPIKey` có gắn lớp tự kiểm tra mã lệnh (self-check) ở đầu hàm:
     - Đọc lệnh đầu tiên của chính hàm đó (`ldr w8, #<func_va>`) và kiểm tra qua `sub_2184e4`, `sub_2184f0`, `sub_20fca4` để phát hiện breakpoint phần mềm (`brk`) hoặc lệnh nhảy (`b` / `ldr x16, #8; br x16`) do công cụ hook chèn vào.
     - Nếu phát hiện đầu hàm bị sửa đổi, mã máy nạp `mov w16, #1` (`SYS_exit`) kèm lệnh `svc` (`#0xefc4`, `#0xd7f3`, `#0xc71`, `#0xc857`) để thoát tiến trình ngay lập tức ở mức syscall mà không đi qua hàm thư viện C (`exit`).
   - `init_8` (`0x218f5c`): Kiểm tra toàn vẹn tương tự ở đầu hàm (`0x218f60 .. 0x218fa4`), sau đó nạp hằng số `UIBackgroundTaskInvalid` vào `0xcc91248`.

3. **Kiểm tra chống Hook trên các phương thức C Bridge (`0x130dec .. 0x131214`):**
   - Các hàm cầu nối như `fn_130dec`, `fn_130e7c` (`setVer:`), `fn_131014` đều gọi `objc_getClass("_UIKites")` và `method_getImplementation` để đối chiếu địa chỉ hàm thực thi thực tế với địa chỉ tĩnh (`adr x8, #0x130e7c`). Nếu con trỏ IMP của Objective-C bị swizzle sang địa chỉ khác, hàm thực thi `mov w16, #1; svc #0x851b` để dừng tiến trình.

## 10. Cấu trúc Bảng Địa chỉ RVA Mã hóa và Trampoline trong `UnityFramework`
Phân tích tĩnh vùng hằng số `0x63dcb1c .. 0x63dcd24` trong `Penny` và 4 phân vùng cuối trong `UnityFramework` cho thấy cơ chế liên kết giữa hai mô-đun:

1. **Ba bảng địa chỉ RVA mã hóa trong `Penny` (`0x63dcb1c`, `0x63dcb58`, `0x63dcba4`):**
   - Sử dụng hàm tạo mặt nạ 64-bit sang 32-bit (với các hằng số trộn `0x065afa27e8371494`, `0x9e3779b97f4a7c15`, `0xbf58476d1ce4e5b9`, `0xd6e8feb86659fd93`, `0x94d049bb133111eb`), `Penny` giải mã 3 bảng địa chỉ tương đối (RVA) trỏ vào đầu các hàm bên trong `UnityFramework` (đều bắt đầu bằng prologue chuẩn của ARM64 như `stp x29, x30, ...` hoặc `stp x22, x21, ...`):
     - **Bảng 1 (`0x63dcb1c` -> giải mã ra `0xcc8f6c8`):** Gồm **15 mục** RVA (ví dụ: `0x021e8d20`, `0x015cb050`, `0x04644998`, `0x04630914`, `0x08de4fe0`, `0x05624598`, ..., `0x06ceadc4`).
     - **Bảng 2 (`0x63dcb58` -> giải mã ra `0xcc8f740`):** Gồm **19 mục** RVA (ví dụ: `0x020f2d94`, `0x021ebd24`, `0x046447cc`, `0x046448cc`, `0x086ff118`, ..., `0x034225c4`).
     - **Bảng 3 (`0x63dcba4` -> giải mã ra `0xcc8f7e8`):** Gồm **96 mục** RVA (từ `0x044b7528` đến `0x07737840`).

2. **Cấu trúc Trampoline Stubs trong `UnityFramework` (`__TEXT_EXEC` & `__AUTH_EXEC`):**
   - Phân vùng `__TEXT_EXEC` (`va = 0xc388000`, `foff = 0xbf44000`) chứa **110 trampoline stub** căn chỉnh theo bước `0x40` byte (từ `0xc388100` đến `0xc389c40`).
   - Phân vùng `__AUTH_EXEC` (`va = 0xc39c000`, `foff = 0xbf58000`) chứa **45 trampoline stub** căn chỉnh theo bước `0x40` byte (từ `0xc39c100` đến `0xc39cc00`).
   - Mỗi stub đều tuân theo cùng một mẫu lệnh máy 8 lệnh:
     ```armasm
     ldr  x17, #<slot_va>        ; Đọc giá trị con trỏ đã mã hóa trong __DATA_DIRTY (0xc398xxx) hoặc __AUTH_DATA (0xc3a0xxx)
     cbz  x17, #<fallback_label> ; Nếu slot bằng 0 (chưa khởi tạo), nhảy sang nhánh fallback
     ldr  x16, #<slot_va + 8>    ; Đọc khóa XOR 64-bit tại slot kế tiếp
     eor  x17, x17, x16          ; Giải mã con trỏ hàm bằng phép XOR
     ror  x17, x17, #<imm_rot>   ; Xoay phải bit (mỗi stub có số bit xoay #imm_rot riêng biệt)
     br   x17                    ; Nhảy tới địa chỉ hàm đích sau khi giải mã
     <fallback_label>:
     stp  d11, d10, [sp, #-0x40]!
     b    #0x5632fe4             ; Nhánh mặc định khi slot chưa được nạp dữ liệu
     ```
   - Trong tệp tĩnh trên đĩa, các vùng dữ liệu `__DATA_DIRTY` (`0xc398100 ..`) và `__AUTH_DATA` (`0xc3a0080 ..`) tại các địa chỉ `<slot_va>` đều đang mang giá trị `0`, cho thấy các cặp giá trị `(con_trỏ_mã_hóa, khóa_XOR)` chỉ được ghi vào bộ nhớ ở thời điểm chạy (runtime).

## 11. Phân tích Toàn diện 5 Hệ thống Cốt lõi: Luồng Thực thi, UUID/UDID, Auth, Menu và Anti-Tamper

### 11.1. Luồng Thực thi Tổng thể (End-to-End Execution Flow)
1. **Giai đoạn nạp Dylib (`dyld` Image Load):**
   - Runtime Objective-C gọi `+[_UISceneBootstrapLoader load]` (`0xe4d34`) và `+[_UIPaneHUDBox load]` (`0x219150`).
   - Trong `+[_UISceneBootstrapLoader load]`:
     - Kiểm tra các cờ trạng thái toàn cục (`0xcc74880`, `0xcc8f6b0`); nếu phát hiện bất thường sẽ nhảy tới nhánh xóa stack/frame (`0xe4da8`) và thực thi `brk #1`.
     - Giải mã biến môi trường bằng máy trạng thái trên stack (`0xe4dbc .. 0xe5494`, xác minh bằng trình giả lập Unicorn `arm64`): gọi `getenv("_UISceneTraitBootToken")` (`0xe5190`), nếu chưa tồn tại thì giải mã giá trị `"1"` và gọi `setenv("_UISceneTraitBootToken", "1", 1)` (`0xe54b8`) để đảm bảo luồng khởi tạo chỉ chạy một lần duy nhất trong tiến trình.
     - Gọi `[_UIContextAssertionCoordinator _configureSystemContextAssertions]` (`0xe54cc`) và đặt lịch `dispatch_after` trên `dispatch_main_q` với block toàn cục tại `0xcc4d0e8` (`0xe5540`).
   - Tiếp theo, `dyld` chạy 9 hàm trong `__mod_init_func` (`0xcc4c490`): khởi tạo các biến toàn cục (`0xad7c8`), đặt lịch gắn ViewController (`0xe8828`), và chạy các hàm tự kiểm tra đầu hàm của SDK `PPAPIKey` (`0x14a140 .. 0x218f5c`).

2. **Giai đoạn kích hoạt Xác thực và Khởi tạo Giao diện (`0xe5540 .. 0xe5800` & `0xba000 .. 0xbdbdc`):**
   - Block `0xcc4d0e8` (`0xe5540`) gọi `+[_UIContextAssertionCoordinator _performDeferredTraitSyncWithCompletion:]` (`0xbb224`) với completion block `0xcc4d108` (`0xe55b4`).
   - `0xbb224` chuyển thực thi về main thread và gọi block `0xbb368`:
     1. Dùng `sub_b5218` lấy địa chỉ 4 hàm C Bridge nội bộ: `0x130dec` (`setTokenC` wrapper, mã `0x375a`/`0x375b`), `0x131014` (`setENC` wrapper, mã `0x3758`), `0x133484` (`loadingC` wrapper, mã `0x3759`), `0x133640` (`packageData` wrapper, mã `0x375e`).
     2. Quét 19 lệnh đầu của từng hàm C Bridge (`0xbb530 .. 0xbc198`) để kiểm tra chống hook.
     3. Giải mã **Package Token dài 100 ký tự** từ 4 con trỏ tại `0xcc4d078` kết hợp `0x63dd250` và 4 opcode đầu của C Bridge (`0xbbcf4 .. 0xbbda8`):
        `qAQerzzfWtOxTWsLTmNgBaUXwXaeealpNJhbipLLIyZgrQsVpEfjByqDrWGpRtOZRnFPgTTiyTxpLDcGRkOhpZJABxFBLsBhChjE`
     4. Giải mã chuỗi phiên bản từ `0x63dbca8` (`0xbbdb4 .. 0xbbdec`): `"1.132.1"`.
     5. Gọi tuần tự: `setTokenC("<100-char token>")` (`0xbbe64`), `setENC(0)` (`0xbbe74`), `setVerC("1.132.1")` (`0xbbe88`), tính băm FNV-1a 64-bit của cả token + version lưu vào `[0xcc8f6a8] = 0x527fd45f68599f17` (`0xbc1e4`), xóa sạch chuỗi token/version trên stack (`0xbc1e8 .. 0xbc218`), rồi gọi `loadingC` (`0x133484`) với completion block `0xbdeb8` (`0xbc268`).
   - Khi `loadingC` xác thực key thành công, nó gọi `0xbdeb8`, hàm này lấy `0x133640` (`packageData` wrapper) và gọi kèm callback `0xbdfb8(id data)` (`0xbdf88`).
   - `0xbdfb8` chuyển `data` vào `sub_bc2f8(data, lr)` (`0xbe038`):
     - Trong `sub_bc2f8`, `Penny` quét danh sách dylib đang nạp để phát hiện các chuỗi giải mã bằng `sub_b5090` (`seed = 0x4031cc15`): `'frida'` (`0xbc668`), `'gadget'` (`0xbc6f8`), `'cycript'` (`0xbc77c`), `'ffz.dylib'` (`0xbc808`).
     - Giải mã gói `packageData` (các khóa JSON `'packageData'`, `'package_data'`, `'data'`, `'value'`, `'content'`, `'skin_field'` tại `0xe0c68 .. 0xe0e24`) và lưu vào `[0xcc90ad0]`.
     - Tại `0xbdb38 .. 0xbdbbc`, giải mã bảng nhảy 7 mục tại `0xcc4d040` và gọi `sub_cdad8` (`0xcdad8`) cùng `sub_cf1a4` (`0xcf1a4`).
   - Nếu `sub_bc2f8` trả về `1`, `0xbdfb8` (`0xbe054 .. 0xbe0a4`) đẩy block `0xbe0dc` lên `dispatch_main_q` để gọi completion block ban đầu `0xe55b4`.
   - Tại `0xe55b4 .. 0xe57ec`, `Penny` kiểm tra lại chữ ký `sub_62a90 == 0x8842428c`, đăng ký observer cho `UIApplicationDidBecomeActiveNotification`, và gọi `[_UISceneBootstrapLoader _installHUDControlView]` (`0xe6e7c`) trên main thread để gắn nút điều khiển nổi (`_UIDragInteractionContainerView`) và bộ nhận diện cử chỉ chạm đa điểm (`_UISystemMultitouchEventRecognizer`) vào cửa sổ chính.
   - Sau khi xác thực và nhận dữ liệu gói (`packageData`), luồng callback tại `0xbdb38 .. 0xbdbbc` giải mã bảng nhảy 7 mục tại `0xcc4d040` (`0xbc518`, `0xbcab8`, `0xbcf98`, `0xbc904`, `0xbdb7c`, `0xbcb30`, `0xbd2b4`) để nạp cấu hình vào `0xcc90ad0`, sau đó gọi tuần tự các hàm hậu xác thực: `sub_cdad8` (`0xcdad8`), `sub_cf1a4` (`0xcf1a4`), `sub_d47a0`, `sub_d6ac8`, `sub_d7a10`, `sub_d8fc4`, `sub_db5d0`.
   - Khi hoàn tất, `[_UISceneBootstrapLoader _installHUDControlView]` (`0xe6e7c`) được gọi trên main thread để gắn nút điều khiển nổi (`_UIDragInteractionContainerView`) và bộ nhận diện cử chỉ chạm đa điểm (`_UISystemMultitouchEventRecognizer`) vào cửa sổ chính, đồng thời tạo `NSTimer` lặp lại mỗi `2.0` giây (`0xe700c`) gọi `_synchronizeWindowGestureRecognizers` để giữ giao diện luôn nằm trên cùng.

---

### 11.2. Hệ thống Định danh Thiết bị (UUID / UDID) và Reset Guest
Tệp nhị phân `Penny` kết hợp hai cơ chế định danh thiết bị riêng biệt:

1. **Định danh phần cứng / phiên bản của SDK `PPAPIKey` (`-[_UIKites getDeviceID]` tại `0x131adc` và `_UIKiteUDIDView`):**
   - **Mobileconfig Enrollment (`getudid`):** Khi thiết bị chưa có UDID gắn với khóa, lớp `_UIKiteUDIDView` hiển thị hộp thoại yêu cầu lấy UDID và mở đường dẫn `https://udid3.ppdeveloper.xyz/getudid` (trong `SafariServices` / `UIApplication openURL:`) để lấy UDID phần cứng rồi truy vấn `https://api6.ppdeveloper.xyz/api/v1/udid/find` và `https://api6.ppdeveloper.xyz/api/v1/udid/uuid_key`.
   - **Fallback `identifierForVendor` (`0xda420 .. 0xda560`):** Giải mã động tên lớp `UIDevice` (`0xda440`), selector `currentDevice` (`0xda484`), `identifierForVendor` (`0xda4d4`) và `UUIDString` (`0xda524`) bằng hàm `sub_b497c`, sau đó gọi chuỗi `[[[UIDevice currentDevice] identifierForVendor] UUIDString]` để lấy chuỗi UUID chuẩn 36 ký tự (có dấu gạch ngang `-` tại các vị trí `8, 13, 18, 23` được kiểm tra tại `0xda404 .. 0xda418`).
   - **Lưu trữ bền vững trong Keychain (`0x5e150 .. 0x5e3b0`):** Sử dụng `SecItemCopyMatching` / `SecItemAdd` / `SecItemUpdate` với `kSecClassGenericPassword`, `kSecAttrService` và `kSecAttrAccount` dưới các khóa giải mã bằng `sub_b497c`:
     - `com.dts.freefireth` (`0x5e1a0`)
     - `pp.device.seed` (`0x5e21c`)
     - `PP-DEVICEID-v1` (`0x5e290`)
     - `PP-VUUID-v1` (`0x5e2f4`)

2. **Cơ chế Xóa Tài khoản Khách (`ResetGuest` tại `0x5e334 .. 0x5f800`):**
   - Khi người dùng kích hoạt mục `Xóa Khách (Reset Guest)` trên menu:
     - Xóa các mục Keychain (`SecItemDelete`) của tài khoản guest trong ứng dụng.
     - Xóa các tệp lưu dữ liệu định danh khách trong thư mục `Documents` (`0xba1c8`), bao gồm 8 tên tệp được giải mã tại `0x5e868 .. 0x5ead4`: `guest.dat`, `guest_id.dat`, `guest_token.dat`, `guest_account.dat`, `garena_guest.dat`, `guest_uuid.dat`, `guest_info.json`, `guest_auth.dat`.

---

### 11.3. Hệ thống Xác thực (`Auth`), Giao tiếp Máy chủ và Luồng `SSE`
1. **Lớp Quản lý Xác thực `_UIKites` (`0xcc73a10`):**
   - Là tên đã làm rối (obfuscated name) của lớp `PPAPIKey`, cung cấp các phương thức: `+[shared]` (`0x12f7cc`), `-[setToken:]` (`0x12f940`), `-[setEN:]` (`0x130d04`), `-[setVer:]` (`0x130e7c`), `-[getDeviceKey]` (`0x13110c`), `-[getKeyExpire]` (`0x131608`), `-[getKeyAmount]` (`0x13185c`), `-[getDeviceID]` (`0x131adc`), `-[getAppBundle]` (`0x131cd4`), `-[exitKey:]` (`0x131e34`), `-[copyKey:]` (`0x132de4`), `-[packageData:]` (`0x132fb4`), `-[loading:]` (`0x133214`).
   - Bộ kiểm tra phiên cục bộ `_UISceneSessionActivationValidator` (`0xbf5a0`) đối chiếu chuỗi phiên bản `"1.0.0"` (`0xbf690`) và giải mã dữ liệu cấu hình phiên qua `sub_b56cc` (`0xbf758`).

2. **Các Endpoint Giao tiếp Mạng:**
   - `https://api6.ppdeveloper.xyz/api/v2/package` & `https://api6.ppdeveloper.xyz/api/v2/package/packagedata`: Xác thực token của gói (package token), kiểm tra chữ ký băm Anti-Hex và tải gói dữ liệu cấu hình/khóa giải mã (`ui_pkg_data` tại `0x1330ec`).
   - `https://api6.ppdeveloper.xyz/api/v3/key`: Kiểm tra license key do người dùng nhập qua `_UIKiteKeyView` hoặc `_UIKiteGlassKeyView`, kiểm tra hạn dùng (`Expires`) và số lượng thiết bị (`Quota`).
   - `https://api6.ppdeveloper.xyz/api/v1/udid/uuid_key` & `https://api6.ppdeveloper.xyz/api/v1/udid/find`: Đối chiếu liên kết giữa UDID/UUID thiết bị và license key.

3. **Kênh Điều khiển Thời gian thực `SSE` (`_UIKiteSSEC` tại `0xcc739c0`):**
   - Sau khi xác thực thành công, `_UIKiteSSEC` mở kết nối Server-Sent Events giữ liên lạc liên tục với máy chủ (`[SSE] connected http=%d` tại `0x12c464`).
   - Lắng nghe sự kiện điều khiển trực tiếp (`[SSE] event pk_thongbao=%d debstatus=%d hide=%d k_thongbao=%d k_khoamay=%d k_hethan=%d` tại `0x12d5b4`), cho phép máy chủ từ xa:
     - Đẩy thông báo (`pk_thongbao`, `k_thongbao`).
     - Ẩn/hiện giao diện tức thì (`hide` -> `[SSE] shouldHideView changed -> %d (live apply)` tại `0x12d4fc`).
     - Khóa thiết bị (`k_khoamay`), báo hết hạn key (`k_hethan`) hoặc vô hiệu hóa gói (`debstatus`) ngay khi đang chạy.

---

### 11.4. Hệ thống Giao diện Menu (`_UISceneLayoutRenderController`)
Giao diện điều khiển chính do lớp `_UISceneLayoutRenderController` (`0xcc73060`) quản lý, được mở thông qua nút nổi `_UIDragInteractionContainerView` (`0xe2914`, lưu tọa độ `JHDragView_X`, `JHDragView_Y`) hoặc cử chỉ chạm đa điểm `_UISystemMultitouchEventRecognizer` (`0xe4458`).

1. **Bố cục 5 Tab Điều hướng (`_constructNavigationRailView` tại `0x33578` & `_populateViewportForSectionIndex:` tại `0x39ad0`):**
   - **Tab 1 — `AIMBOT` / `AIM` (`_buildTraitSectionLayout_01:width:padding:` tại `0x3a6b8` & `_03` tại `0x3c3ac`):**
     - Công tắc: `Bật Aimbot` (`Aimbot`), `Aim Line` (`AimLine`), `Hiện Vòng FOV` (`ShowFOV`), `Bỏ Qua Bot` (`IgnoreBot`), `Bỏ Qua Gục` (`IgnoreKnocked2`).
     - Thanh trượt (`_appendContinuousValueRow:...` tại `0x4ce1c`): `Bán Kính FOV` (`AimFov`), `Độ Dày FOV` (`AimbotFOVThickness`).
     - Bộ chọn phân đoạn:
       - `Điều Kiện Kích Hoạt` (`AimWhen`): `Luôn Luôn` (`Always`), `Khi Bắn` (`Firing`), `Khi Ngắm` (`Scope`).
       - `Vị Trí Ngắm` (`Target`): `Đầu` (`Head`), `Cổ` (`Neck`), `Thân` (`Body`).
       - `Ưu Tiên Mục Tiêu` (`TargetPriority`): `Gần Nhất` (`Closest`), `Máu Thấp` (`Lowest HP`), `Trúng Đầu` (`Headshot`).
       - `Màu Vòng FOV` (`AimbotFOVColor`): `Branco`, `Vermelho`, `Azul`, `Verde`, `Rosa`.
   - **Tab 2 — `ESP` (`_buildTraitSectionLayout_02:width:padding:` tại `0x3a9dc`):**
     - Công tắc: `Bật ESP` (`Enable`), `Khung ESP` (`Box`), `Đường Kẻ ESP` (`Lines`), `Bộ Xương` (`Skeleton`), `Thanh Máu` (`Health`), `Khoảng Cách` (`Distance`), `Tên ESP` (`Name`), `Viền ESP` (`Outline`), `Số Lượng Địch` (`EnemyCount` — hiển thị chuỗi `ENEMIES: %d (%d K)` tại `0x1f210`), `Hiệu Ứng Phát Sáng` (`Glow`).
     - Bộ chọn màu (`_appendTintSwatchControlAtY:...` tại `0x44900`): `ESPLineColor`, `ESPBoxColor`, `ESPSkeletonColor`, `ESPNameColor`.
   - **Tab 3 & 4 — `KHÁC` / `OTHER` (`_buildTraitSectionLayout_04:width:padding:` tại `0x3d9bc`):**
     - `Fast Switch` (`Troca Rápida` — kèm nút nổi riêng lưu tọa độ `FSW_%@_X`, `FSW_%@_Y` và các khóa `qsw_send`, `qsw_dame`, `qsw_keo` tại `0xaf304 .. 0xb1f60`).
     - `Chế Độ Stream` (`StreamMode` — ẩn lớp phủ khi quay màn hình / chụp ảnh bằng thuộc tính `disableUpdateMask` / secure container).
     - `Xóa Khách (Reset Guest)` (`ResetGuest`).
     - Các tính năng có hộp thoại cảnh báo xác nhận (`⚠️ CẢNH BÁO: Chức năng nguy hiểm, bạn có chắc muốn kích hoạt?` tại `0x4ebc4 .. 0x4f7f4`): `Aim Silent` (`AimSilent`), `Thay Đạn Nhanh` (`FastReload`).
     - Nhóm mở khóa ngoại hình cục bộ (`0x3e308 .. 0x413bc`): `Mở Khóa Súng Lv7 (Evo Gun)` (`UnlockGunLv7`), `Mở Khóa Skin Nắm Đấm` (`UnlockFistSkin`), `Mở Khóa Trang Phục & Combo Đồ` (`UnlockOutfit` với 5 gói combo cài sẵn tại `0x72be8 .. 0x72fd8` và 8 vị trí bộ phận `0..7` từ `Mũ/Tóc` đến `Phụ Kiện`), `Đổi Hành Động Hello -> Emote Lv7` (`UnlockEmoteLv7`).
   - **Tab 5 — `THÔNG TIN` / `INFO` (`_buildTraitSectionLayout_05:width:padding:` tại `0x472a8`):**
     - Hiển thị `FPS`, `Thời Gian` (`Time`), `Thiết Bị` (`Device`), `Hệ Điều Hành` (`iOS`), `Tên Máy` (`Name`), `UDID`, `Key`, `Trạng Thái` (`Đã Kích Hoạt` / `Chưa Kích Hoạt`), `Hạn Dùng` (`Expires`), liên kết hỗ trợ (`https://t.me/pennyioshehehe`) và bộ chọn `Ngôn Ngữ` (`Tiếng Việt` / `English`) cùng nút chuyển chế độ Sáng/Tối (`ModMenuDarkMode`).

2. **Cơ chế Lưu trữ Cấu hình Menu (`_writeTraitConfigurationArchive` tại `0x2aab8` & `_readTraitConfigurationArchive` tại `0x2be54`):**
   - Lưu song song vào tệp `Documents/menu_settings.txt` (`0xa7824`), `NSUserDefaults` (`ModMenuCurrentTab`, `ModMenuDarkMode`, `ModMenuLanguage`, `ModMenuPanelX`, `ModMenuPanelY`) và mục Keychain `keychain://com.apple.uikit.traitcollection.storage/UITraitArchivePayloadV2` (`0x2aa44`).

---

### 11.5. Hệ thống Bảo vệ Đa tầng (`Anti-Tamper`, `Anti-Hook`, `Symbol Obfuscation`)
1. **Trình Phân giải Symbol Ẩn (`sub_b5218` / `sub_b5310`):**
   - Thay vì gọi trực tiếp các hàm hệ thống nhạy cảm qua bảng PLT/stubs thông thường, `Penny` dùng `sub_b5218(uint32_t id)` với 37 mã định danh (`0x3740 .. 0x377e`) và làm rối địa chỉ trả về bằng phép nhân `0xdf442d22ce4859b9` kết hợp XOR `0xbf58476d1ce4e5b9` (`0xb54f8 .. 0xb5530`).
   - Danh mục các hàm được ẩn qua `sub_b5218`:
     - **Thao tác bộ nhớ Mach VM:** `vm_read_overwrite` (`0x3753`), `vm_write` (`0x3750`), `vm_protect` (`0x3751`), `vm_deallocate` (`0x3756`), `vm_region_recurse_64` (`0x3757`).
     - **Kiểm soát Thread & Exception Port (chống debug):** `task_threads` (`0x3754`), `thread_get_state` (`0x3755`), `thread_set_state` (`0x374a`), `task_set_state` (`0x374b`), `task_set_exception_ports` (`0x3748`).
     - **Mật mã & Keychain:** `CCCrypt` (`0x3749`), `CCCryptorCreateWithMode` (`0x374e`), `CCCryptorUpdate` (`0x374f`), `CCCryptorRelease` (`0x374c`), `SecItemCopyMatching` (`0x374d`), `SecItemAdd` (`0x3742`), `SecItemUpdate` (`0x3743`), `SecItemDelete` (`0x3740`).
     - **Kiểm tra Image & Hệ thống:** `_dyld_image_count` (`0x3741`), `_dyld_get_image_name` (`0x3746` — đối chiếu FNV hash của `'unityframework'` XOR `0xa5c39f17 == 0xea7fd991`), `_dyld_get_image_header` (`0x3747`), `_dyld_get_image_vmaddr_slide` (`0x3744`), `_dyld_register_func_for_add_image` (`0x3745`), `sysctl` (`0x377a`), `sysctlbyname` (`0x377b`).

2. **Kiểm tra Toàn vẹn Mã máy và Ràng buộc Khóa (Code-Integrity Binding):**
   - Mọi hàm trọng yếu (như `sub_9290` tại `0x9318 .. 0x93c8`, `sub_b5100`, `sub_b9994` tại `0xb9ad4 .. 0xb9b68`) đều đọc trực tiếp 4 byte lệnh máy đầu tiên tại 4 địa chỉ cố định của các hàm C Bridge (`0x12fccc`, `0x133484`, `0x133640`, `0x130dec`), trộn với giá trị tại `0xcc74890` (`0x0000034e89abcdef`) và bảng trạng thái xác thực tại `0xcc8faf0` để tính ra offset hoặc khóa giải mã tiếp theo. Nếu bất kỳ hàm nào trong 4 hàm trên bị đặt breakpoint hoặc patch ở đầu hàm, kết quả tính địa chỉ sẽ trỏ sang vùng nhớ sai và làm hỏng luồng thực thi mà không cần báo lỗi rõ ràng.

3. **Kiểm tra Chữ ký Vùng nhớ và Cờ Độc (`Poison Flags` tại `sub_b5534` & `sub_cdad8`):**
   - Hàm `sub_b5534` (`0xb5534 .. 0xb5658`) đọc 3 biến 64-bit tại `0xcc74870`, `0xcc8f6b8`, `0xcc74878`, trộn với `0xca4a91b8985a3c96` và `0xbf58476d1ce4e5b9`, rồi gọi hàm băm `sub_62a90(..., 0x31415927)` để so sánh kết quả với hằng số `0x8842428c`.
   - Nếu giá trị băm không khớp `0x8842428c` (`0xb5614`), hàm không làm sập ngay tại chỗ mà bật **cờ độc (poison bits)**: gán bit `0x8000` vào `[0xcc8f6b0]`, gán bit dấu `0x80000000` vào `[0xcc74880]` và xóa `[0xcc8f6c0]` về `0` (`0xb563c .. 0xb5654`).
   - Khi các cờ độc này bị bật, bất kỳ hàm giao diện hoặc hàm hậu xử lý nào chạy sau đó (như `+[_UISceneBootstrapLoader load]`, `_installHUDControlView`, `sub_cdad8`) khi kiểm tra `tbz w8, #0x1f` và `cbz w9` sẽ nhảy vào bẫy xóa `x29 = 0`, `x30 = 0`, `sp = 0` rồi thực thi `brk #1` / `udf #0`.

4. **Kiểm tra Chống Hook / Anti-Swizzle kết hợp Syscall Trực tiếp:**
   - Các hàm trong `PPAPIKey` kiểm tra `method_getImplementation` so với địa chỉ hàm tĩnh và quét opcode đầu hàm để tìm lệnh nhảy/breakpoint (`sub_2184e4`, `sub_2184f0`, `sub_20fca4`). Khi phát hiện hook, chúng gọi trực tiếp `mov w16, #1; svc #<imm>` (`SYS_exit`) để thoát tiến trình ngay lập tức.

---

## 12. Bản đồ Phân vùng Đầy đủ của `__TEXT.__text` (`0x4b40 .. 0x39f19c`) và Liên kết `UnityFramework`

### 12.1. Bản đồ Địa chỉ Toàn bộ `3.77 MB` Mã thực thi (`__TEXT.__text` trong `Penny`)
Phân tích toàn bộ dải `0x4b40 .. 0x39f19c` (xem chi tiết tại `artifacts/uf_bridge_map.txt`) chia không gian mã máy của `Penny` thành 5 phân khu rõ rệt:

| Dải địa chỉ (VA) | Mật độ `BRK #1` / `SVC` | Thành phần và Chức năng Đo đạc |
| :--- | :--- | :--- |
| `0x004b40 .. 0x0e8890` | `1.387` lệnh `brk #1`, `16` lệnh `svc` | **Mã thực thi của Tweak & Giao diện Menu:** Chứa các hàm hook/tính năng (`0x4b40 .. 0x2a490`), lớp giao diện menu `_UISceneLayoutRenderController` (`0x2a498 .. 0x60b00`), hàm băm toàn vẹn `sub_62a90`, hàm giải mã chuỗi `sub_b497c`, bảng ẩn symbol `sub_b5218`, luồng khởi tạo xác thực (`0xb9994 .. 0xbdbdc`), các hàm hậu xác thực (`0xcdad8 .. 0xdb5d0`), 44 hàm `orig_thunk` (`0xdb5d0 .. 0xdc280`), và `_UISceneBootstrapLoader` (`0xe4d34 .. 0xe8890`). |
| `0x0e8890 .. 0x218f5c` | `39` lệnh `brk #1`, `2.495` lệnh `svc` | **Lõi SDK `PPAPIKey` (`libPPAPIKey_full.a`):** Chứa hàm giải mã XOR `sub_ec874`, 32 lớp `_UIKite*` (`_UIKites`, `_UIKiteKeyView`, `_UIKiteGlassKeyView`, `_UIKiteUDIDView`, `_UIKiteSSEC`, ...), 12 lớp `_UIFlow*`, các hàm kiểm tra chống swizzle/inline hook và thoát trực tiếp qua `mov w16, #1; svc #<imm>`. |
| `0x218f5c .. 0x224b40` | `0` lệnh `brk #1`, `98` lệnh `svc` | **Hệ thống HUD (`_UIPaneHUD*`):** Chứa 14 lớp `_UIPaneHUDBox` và các lớp con biểu diễn vòng xoay tiến trình / thông báo trạng thái. |
| `0x224b40 .. 0x39ec8c` | `1` lệnh `brk #1`, `0` lệnh `svc` | **Thư viện Mật mã & TLS Tĩnh (BoringSSL / OpenSSL `libcrypto` + `libssl`):** Được liên kết tĩnh vào `libPPAPIKey_full.a` để thực hiện mã hóa tầng truyền tải độc lập và xác minh chữ ký số X.509 / ChaCha20-Poly1305 (`ChaCha20 for ARMv8, CRYPTOGAMS` tại `0x66f2d40`) cùng trạng thái bảo vệ tại `__DATA.__ppguard` (`0xcc85000`). |
| `0x39ec8c .. 0x39f19c` | `12` lệnh `brk #1` / `udf #0` | **Trạm Xử lý Cờ Độc Trung tâm (`sub_39ec8c` Poison Sink):** Được gọi khi phát hiện vi phạm toàn vẹn (`bl #0x39ec8c` tại `0xb566c`, `0x39ee50`, ...). Ghi đè hằng số báo lỗi `0xdead10cc58632790` vào `[0xcc74870]` và `[0xcc74878]`, ghi `0xdead10cd` vào `[0xcc74890]`, xóa `[0xcc8f6b8]` và `[0xcc8f6c0]` về `0`, bật bit `0x80000003` tại `[0xcc74880]`, sau đó xóa toàn bộ thanh ghi `x0, x1, x29, x30, sp` về `0` và gọi `brk #1`. |

### 12.2. Liên kết Trực tiếp giữa 130 RVA của `Penny` và 155 Điểm Nhảy Tĩnh trong `UnityFramework`
Quét toàn bộ vùng `__TEXT.__text` của `UnityFramework` (`0x4000 .. 0xa19aeb4`) và đối chiếu với 3 bảng RVA trong `Penny` (`artifacts/uf_bridge_map.txt`) xác nhận cơ chế nối kết hai chiều:

1. **Quy luật 4-byte (`b #<stub>`) so với 12-byte (`adrp x16; add x16; br x16`) — Khớp tuyệt đối `155/155` Stubs:**
   - Trong kiến trúc ARM64, lệnh nhảy trực tiếp `b #<imm26>` có tầm nhảy tối đa `±128 MB` (`±0x8000000`). Do `__TEXT_EXEC` nằm ở `0xc388000` và `__AUTH_EXEC` nằm ở `0xc39c000`, mọi hàm trong `UnityFramework` có địa chỉ `< 0x4388000` đều nằm ngoài tầm `128 MB` của lệnh `b`.
   - Vì vậy:
     - **Với các hàm tại địa chỉ `>= 0x4388000` (139 vị trí):** Ghi đè **1 lệnh (4 byte)** tại `rva - 4` (`func_entry + 0`) bằng `b #<stub_va>`, gồm **100 nhánh** nhảy vào `__TEXT_EXEC` (`96` từ `Table 3`, `1` từ `Table 1`, `3` từ `Table 2`) và **39 nhánh** nhảy vào `__AUTH_EXEC` (`12` từ `Table 1`, `8` từ `Table 2`, và `19` nhánh tại `0x5632f7c..0x6c00b70`).
     - **Với các hàm tại địa chỉ `< 0x4388000` (16 vị trí):** Ghi đè **3 lệnh (12 byte)** tại `rva - 12 .. rva` (`func_entry + 0 .. +8`) bằng cụm `adrp x16, #<stub_page>; add x16, x16, #<stub_off>; br x16`, gồm **10 nhánh** nhảy vào `__TEXT_EXEC` (`t1[0..1]`, `t2[0..1]` và `6` nhánh tại `0x21ec318..0x41b2c10`) và **6 nhánh** nhảy vào `__AUTH_EXEC` (`t2[5..6]`, `t2[15..18]`).
     - **Tổng cộng:** `__TEXT_EXEC` có đúng `100 + 10 = 110 / 110` stub được trỏ tới; `__AUTH_EXEC` có đúng `39 + 6 = 45 / 45` stub được trỏ tới (**tổng `155 / 155` stub khớp 100%, không dư một stub nào**).

2. **Hai giai đoạn nạp Slot `__DATA_DIRTY` (`0xc398080`) & `__AUTH_DATA` (`0xc3a0080`):**
   - **Giai đoạn 1 — Khởi tạo Passthrough Trước Xác thực (`+[_UIContextAssertionCoordinator _configureSystemContextAssertions]` tại `0xba558 .. 0xbaf38` -> `sub_cf930`):**
     - Ngay khi nạp thư viện (`+[_UISceneBootstrapLoader load]` -> `0xe54cc`), `sub_ba558` đọc hạt giống 64-bit `0x065afa27e8371494` từ header 128 byte đầu phân vùng `UnityFramework.__TEXT_EXEC` (`0xc388004 .. 0xc38800c`, xác minh qua `sub_cf358` tại `0xcf400 .. 0xcf494`), giải mã 3 bảng RVA vào `0xcc8f6c8`, `0xcc8f740`, `0xcc8f7e8` và gọi `sub_cf930(uf_base, 0xc398080, 0xc3a0080, 0x065afa27e8371494)` (`0xbaf38`).
     - `sub_cf930` giải mã chỉ số slot `(w9 & 0xff) * 16` và số bit xoay `w24` từ bảng `0x63dbcb0` / `0x63dbd1e`, rồi gọi `sub_cd608` để ghi cặp `(rol(handler_va, w24) ^ key, key)` vào `__DATA_DIRTY` / `__AUTH_DATA`. Khi stub chạy `eor x17, x17, x16; ror x17, x17, #<rot>`, khóa ngẫu nhiên `key` tự triệt tiêu và nhảy tới `handler_va`. Ở giai đoạn chưa xác thực này, đa số `handler_va` được trỏ vào các **hàm `orig_thunk` chuyển tiếp (`0xdb7d4 .. 0xdc108`)** và stub trả về `0` (`0xde3ec`) để `UnityFramework` chạy qua lệnh gốc bình thường mà không rơi vào nhánh fallback giả `b #0x5632fe4`.
   - **Giai đoạn 2 — Kích hoạt Handler Tính năng Sau Xác thực (`sub_cdad8`, `sub_cf1a4`, `sub_de414` tại `0x63dbc2c`):**
     - Sau khi xác thực thành công (`0xbdbb0 .. 0xbdbbc`), `sub_cdad8` và `sub_cf1a4` kích hoạt bảng điều phối 31 mục `sub_de414` (`0xde414 .. 0xde5f8`, jump table tại `0x63dbc2c`) và các hàm `sub_d8fc4`, `sub_d7a10`, `sub_d6ac8` để chuyển hướng các slot sang các hàm xử lý tính năng thực sự (`0xd48d8 .. 0xd6904`, `0xdb578`, ...).

3. **Cơ chế Gọi Hàm Gốc qua 44 `Orig-Call Thunks` trong `Penny` (`0xdb5d0 .. 0xdc280`):**
   - Khi hàm xử lý trong `Penny` cần gọi lại hàm gốc của `UnityFramework` (vốn đã bị ghi đè lệnh đầu tiên tại `rva - 4` hoặc 3 lệnh đầu tại `rva - 12`), nó gọi qua dải thunk tại `0xdb5d0 .. 0xdc280`.
   - Mỗi thunk thực hiện:
     1. Kiểm tra bit dấu của `[0xcc74880]` (`tbnz w17, #0x1f`) và ghi hằng số `0x7c3a4f9b` vào `[0xcc74880]`.
     2. Đọc con trỏ đã mã hóa từ bảng BSS `0xcc8f6c8` (`Table 1`) hoặc `0xcc8f740` (`Table 2`) (do `sub_ba558` nạp tại `0xbad34 .. 0xbae60`).
     3. Giải mã địa chỉ `rva` bằng cách XOR với địa chỉ hiện tại cộng hằng số (`adr x16, #...; add x16, x16, #...`) và khóa phiên 64-bit tại `[0xcc74888]`.
     4. Thực thi **1 lệnh prologue bị ghi đè** (đối với nhánh 4 byte tại `rva - 4`) hoặc **3 lệnh prologue bị ghi đè** (đối với nhánh 12 byte tại `rva - 12`) ngay trước khi thực hiện `br x17` để nhảy vào `rva` trong `UnityFramework`.

### 12.3. Bộ Phân giải 19 Hàm `il2cpp_*` (`sub_d9e44`), Luồng Timer 350ms (`0xde6b4`) và Quét Chống Hook `UnityFramework` (`sub_d47a0`)
1. **Bảng 19 Hàm Export `il2cpp_*` tại `0x63dcd28` (`sub_d9e44` & `sub_da2e0`):**
   - Hàm `sub_d9e44(uint32_t idx)` (`0xd9e44 .. 0xda2dc`, với `idx = 0..18`) duyệt bảng symbol `LC_SYMTAB` của `UnityFramework`, lọc các symbol bắt đầu bằng `"il2cpp_"` (`0xda228 .. 0xda248`), băm tên symbol qua `sub_da2e0` (FNV-1a 64-bit `basis = 0xcbf29ce484222325`, `prime = 0x100000001b3` + SplitMix64) rồi XOR với mặt nạ `0x376d7964fb120108` để khớp với 19 mục trong bảng `0x63dcd28` (lưu cache đã mã hóa vào `0xcc90ce8`):
     - `[0] il2cpp_domain_get` (`0x0a10fefc`)
     - `[1] il2cpp_thread_attach` (`0x0a11043c`)
     - `[2] il2cpp_runtime_class_init` (`0x0a110408`)
     - `[3] il2cpp_object_new` (`0x0a1103b8`)
     - `[4] il2cpp_array_new` (`0x0a10f98c`)
     - `[5] il2cpp_class_from_name` (`0x0a10f9d8`)
     - `[6] il2cpp_get_corlib` (`0x0a10f96c`)
     - `[7] il2cpp_gchandle_new` (`0x0a110204`)
     - `[8] il2cpp_gc_wbarrier_set_field` (`0x0a110278`)
     - `[9] il2cpp_string_new` (`0x0a110420`)
     - `[10] il2cpp_class_get_method_from_name` (`0x0a10fa04`)
     - `[11] il2cpp_runtime_invoke` (`0x0a110404`)
     - `[12] il2cpp_class_get_methods` (`0x0a10f9fc`)
     - `[13] il2cpp_method_get_name` (`0x0a1102e0`)
     - `[14] il2cpp_domain_get_assemblies` (`0x0a10ff08`)
     - `[15] il2cpp_assembly_get_image` (`0x0a10f9a0`)
     - `[16] il2cpp_class_get_field_from_name` (`0x0a10f9f8`)
     - `[17] il2cpp_field_static_set_value` (`0x0a110128`)
     - `[18] il2cpp_field_static_get_value` (`0x0a110124`)

2. **Luồng GCD Timer Định kỳ 350ms (`sub_cf1a4` -> `0xde6b4`) và Hook `MethodInfo->methodPointer` (`sub_d6ac8`):**
   - Tại `sub_cf1a4` (`0xcf2b8 .. 0xcf340`), `Penny` khởi tạo một `dispatch_source_t` kiểu timer lặp lại mỗi `350 ms` (`0x14dc9380` ns) gọi vào `0xde6b4`.
   - Hàm `0xde6b4` gọi `il2cpp_domain_get()` (`idx = 0`) và `il2cpp_thread_attach(domain)` (`idx = 1`), sau đó chạy `sub_d7a10` và `sub_d6ac8`.
   - Trong `sub_d6ac8` (`0xd6ed8 .. 0xd7008`), `Penny` dùng `il2cpp_class_get_methods` (`idx = 12`) và `il2cpp_method_get_name` (`idx = 13`), băm tên phương thức qua `sub_b973c` để tìm 2 phương thức có mã băm `0x16113cae` và `0x2e70e851`, lưu `method->methodPointer` gốc vào `[0xcc90e38]` / `[0xcc90e48]` và ghi đè `method->methodPointer` thành `0xd6904` / `0xdb578`.
   - Trong `sub_d9b24` (`0xd9b24 .. 0xd9d88`), `Penny` lấy UUID 36 ký tự từ `identifierForVendor` (`sub_da37c`), băm FNV-1a thành chuỗi hex 64 ký tự tại `0xcc90d80`, tạo đối tượng `Il2CppString*` qua `il2cpp_string_new` (`idx = 9`), ghim vào bộ thu gom rác qua `il2cpp_gchandle_new(..., 1)` (`idx = 7`) và lưu tại `[0xcc90ce0]`.

3. **Bộ Quét Chống Hook trên `UnityFramework` (`sub_d47a0`) và Kiểm tra Địa chỉ Trả về `LR` (`0xd4a0c .. 0xd4a2c`):**
   - Hàm `sub_d47a0` (`0xd47a0 .. 0xd48b8`) đọc mã máy của `UnityFramework` tại `rva - 4` và 4 lệnh tiếp theo từ `rva` để kiểm tra xem có bị công cụ bên thứ ba chèn `brk` (`0xd4200000`), `hlt` (`0xd4400000`) hoặc inline hook `ldr x16/x17, #8; br x16/x17` (`0x58000050` / `0xd61f0200`) hay không; nếu phát hiện sẽ kích hoạt cờ độc (`0xdead0020`).
   - Các hàm xử lý tính năng tại `0xd48d8 .. 0xd64d8` đều kiểm tra thanh ghi `x30` (`LR & 0x7fffffffffff`) phải nằm trong phạm vi bộ nhớ `[uf_base, uf_base + 0x0d000000)` của `UnityFramework` (`0xd4a0c .. 0xd4a2c`), từ chối thực thi nếu được gọi từ ngoài `UnityFramework`.
@@
3. **Bộ Quét Chống Hook trên `UnityFramework` (`sub_d47a0`) và Kiểm tra Địa chỉ Trả về `LR` (`0xd4a0c .. 0xd4a2c`):**
   - Hàm `sub_d47a0` (`0xd47a0 .. 0xd48b8`) đọc mã máy của `UnityFramework` tại `rva - 4` và 4 lệnh tiếp theo từ `rva` để kiểm tra xem có bị công cụ bên thứ ba chèn `brk` (`0xd4200000`), `hlt` (`0xd4400000`) hoặc inline hook `ldr x16/x17, #8; br x16/x17` (`0x58000050` / `0xd61f0200`) hay không; nếu phát hiện sẽ kích hoạt cờ độc (`0xdead0020`).
   - Các hàm xử lý tính năng tại `0xd48d8 .. 0xd64d8` đều kiểm tra thanh ghi `x30` (`LR & 0x7fffffffffff`) phải nằm trong phạm vi bộ nhớ `[uf_base, uf_base + 0x0d000000)` của `UnityFramework` (`0xd4a0c .. 0xd4a2c`), từ chối thực thi nếu được gọi từ ngoài `UnityFramework`.
+
+---
+
+## 13. Đối Chiếu Thực Nghiệm Với Bản Đóng Gói Lại `m5d0l6e-f3701-fast.ipa` (`HunterFF.dylib`)
+
+Chi tiết toàn bộ phép đối chiếu nhị phân và kết quả giả lập Unicorn giữa gói IPA 1 (`T74ODgYWCToR`) và gói IPA 2 (`m5d0l6e-f3701-fast.ipa`) được trình bày trong [`IPA2_HUNTERFF_COMPARISON.md`](IPA2_HUNTERFF_COMPARISON.md) và [`artifacts/ipa2_comparison_dump.txt`](artifacts/ipa2_comparison_dump.txt):
+
+1. **Cùng một bản build gốc (`LC_UUID` trùng khớp 100%)**:
+   - `FreeFire` (`456886fe...`) và `DataDomeSDK` (`058758ec...`) giống hệt `100%` trước `LC_CODE_SIGNATURE`.
+   - `UnityFramework` (`c8de7371...`) giữ nguyên `100%` phân vùng `__TEXT.__text`, chỉ khác `1.321 byte` tại header và hai phân vùng trampoline `__TEXT_EXEC.__objc_tramp` (`923 B`) / `__AUTH_EXEC.__auth_stubs` (`392 B`).
+2. **Khôi phục tĩnh `154/154` prologue bị cài bẫy trong `UnityFramework`**:
+   - Trong `155` slot mang lệnh `stp d11, d10, [sp, #-0x40]! ; b #0x5632fe4` của IPA 1, `AuthSlot 12` (`0xc39c180`) là prologue thật của hàm `0x5632fe0`, còn `154` slot kia (`110` trong `__objc_tramp` + `44` trong `__auth_stubs`) là bẫy. Trong IPA 2, cả `154/154` slot này đã được ghi đè sẵn trên đĩa bằng lệnh prologue gốc và nhánh nhảy quay lại từng hàm trong `UnityFramework`.
+3. **Sửa đổi `8.486 byte` trong `Penny` và tiêm `HunterFF.dylib`**:
+   - IPA 2 vá tự động `1.792` lệnh `mov w16, #1/#0x1a` trước `svc` thành `b #<pc+8>`, ghi cứng `SEED = 0x2fddd4d1` tại `0xb4a04` và `x0 = 0x1fbba9ea58632790` tại `0xb6ac8`, đổi chuỗi Telegram mã hóa tại `0x48394`/`0x4a71c` sang `t.me/xudeptraix2`, và chuyển hướng 9 điểm nút của `Penny` (`0xbb290`, `0xcd608`, `0x3032c`, `0xde6b4`, `0xbe8c4`, `0xbee54`, `0xbe19c`, `0xcd9a8`, `0xc3a84`) qua bảng con trỏ `__DATA.__bss` (`0xcc94280..0xcc942c0`) do `HunterFF.dylib` kiểm soát.
+   - Giả lập Unicorn trên `HunterFF.dylib` xác nhận hàm `_time` interpose (`0x40863c`) trả về cứng `1794268800` (`2026-11-10 00:00:00 UTC`), và callback `0x416dec` kiểm tra `33` vùng byte trong `Penny` bằng `_memcmp` trước khi ghi 9 con trỏ cầu nối vào `0xcc94280..0xcc942c0` để yêu cầu xác thực key qua `https://api.hackgamefile.com/sophia/verify`.
