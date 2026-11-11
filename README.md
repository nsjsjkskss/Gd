# Static Analysis Repository: `Penny.framework` & `UnityFramework`

Kho lưu trữ này chứa báo cáo kỹ thuật, công cụ đọc cấu trúc tĩnh (read-only static analysis tools) và các bảng dữ liệu trích xuất từ `Penny.framework/Penny` cùng các phân vùng cầu nối trong `UnityFramework`.

> **Lưu ý:** Kho lưu trữ chỉ phục vụ mục đích phân tích tĩnh và kiểm tra hành vi/bảo mật; không chứa tệp IPA gốc hay bất kỳ bản vá (patch) bỏ qua cơ chế xác thực nào.

## Cấu trúc Kho lưu trữ

### 1. Báo cáo Kỹ thuật
- [`STATIC_ANALYSIS_REPORT.md`](STATIC_ANALYSIS_REPORT.md): Báo cáo phân tích tĩnh toàn diện gồm 12 mục (Cấu trúc phân vùng Mach-O, 73 lớp Objective-C, cơ chế giải mã chuỗi `0xb497c` & `0xec874`, đối chiếu kiến trúc SDK `pp7803/APIKey` v6.0.5–v6.1.0, luồng khởi tạo `+load` & `__mod_init_func`, cơ chế UUID/UDID & Reset Guest, luồng Auth & SSE, cấu trúc 5 tab Menu `_UISceneLayoutRenderController`, 4 lớp bảo vệ Anti-Tamper/Anti-Hook, bản đồ 100% vùng mã `__TEXT.__text` và ánh xạ cầu nối `UnityFramework`).
- [`IPA2_HUNTERFF_COMPARISON.md`](IPA2_HUNTERFF_COMPARISON.md): Báo cáo đối chiếu nhị phân, giải mã VM (`VMP_DEVIRT`), TLS Pinning, Ed25519 và giả lập Unicorn giữa gói IPA gốc (`T74ODgYWCToR`) và gói IPA thứ hai (`m5d0l6e-f3701-fast.ipa` / `HunterFF.dylib`).

### 2. Bộ Công cụ Phân tích Tĩnh (`tools/`)
- [`tools/macho_inspect.py`](tools/macho_inspect.py): Đọc header, load commands, segments và sections của tệp Mach-O 64-bit.
- [`tools/objc_dump.py`](tools/objc_dump.py): Trích xuất toàn bộ 73 lớp Objective-C, phương thức (`+`/`-`), địa chỉ `IMP` và biến thực thể (`ivar`).
- [`tools/inspect_trampolines.py`](tools/inspect_trampolines.py): Giải mã 3 bảng địa chỉ RVA (`0x63dcb1c`, `0x63dcb58`, `0x63dcba4`) và kiểm tra prologue tương ứng trong `UnityFramework`.
- [`tools/map_uf_bridges.py`](tools/map_uf_bridges.py): Đối chiếu 130 địa chỉ RVA của `Penny` với 139 điểm nhảy tĩnh trong `UnityFramework`, 155 trampoline stub trong `__TEXT_EXEC` / `__AUTH_EXEC`, 44 hàm `orig_thunk` (`0xdb5d0 .. 0xdc280`) và lập hồ sơ mật độ lệnh cho toàn bộ `3.77 MB` vùng `__TEXT.__text`.
- [`tools/decrypt_strings.py`](tools/decrypt_strings.py): Giải mã tĩnh toàn bộ chuỗi cấu hình/UI qua hàm dòng `0xb497c` (Murmur3-32 stream) và hàm XOR `0xec874`.
- [`tools/dump_symbol_table.py`](tools/dump_symbol_table.py): Giải mã bảng 37 hàm hệ thống và hàm nội bộ ẩn qua trình phân giải `sub_b5218` / `sub_b5310` (`0x63dbb98`).
- [`tools/compare_ipa2_hunterff.py`](tools/compare_ipa2_hunterff.py): Công cụ đối chiếu trực tiếp hai bản IPA, trích xuất `154` prologue `UnityFramework` đã khôi phục, phân loại `1.881` vùng vá trong `Penny` và giả lập Unicorn cho `HunterFF.dylib`.
- [`tools/devirt_hunterff_full.py`](tools/devirt_hunterff_full.py): Công cụ khử ảo hóa máy ảo bytecode ARM64 (`VMP_DEVIRT`), trích xuất cấu hình TLS Pinning, định dạng chuỗi chuẩn hóa Ed25519 và kiểm chứng hàm `sub_421714` bằng Unicorn Engine.

### 3. Dữ liệu Trích xuất (`artifacts/`)
- [`artifacts/objc_classes.txt`](artifacts/objc_classes.txt): Danh sách đầy đủ 73 lớp Objective-C cùng địa chỉ hàm thực thi (`IMP`) và `ivar`.
- [`artifacts/rva_tables.txt`](artifacts/rva_tables.txt): Bảng 130 mục RVA đã giải mã từ `Penny`.
- [`artifacts/uf_bridge_map.txt`](artifacts/uf_bridge_map.txt): Bản đồ ánh xạ chi tiết giữa `Penny` và `UnityFramework` kèm thống kê vùng mã `__TEXT.__text`.
- [`artifacts/b497c_decrypted.txt`](artifacts/b497c_decrypted.txt): Toàn bộ các chuỗi cấu hình và nhãn giao diện 5 tab menu giải mã từ các lệnh gọi `bl #0xb497c`.
- [`artifacts/ec874_strings.txt`](artifacts/ec874_strings.txt): Các chuỗi log trạng thái (`[UIVERIFY]`, `[SSE]`, `[UI-Transition]`, `[UI-Glass]`) giải mã từ các lệnh gọi `bl #0xec874`.
- [`artifacts/b5218_symbols.txt`](artifacts/b5218_symbols.txt): Danh mục 37 mục trong bảng phân giải symbol ẩn `sub_b5218`.
- [`artifacts/ipa2_comparison_dump.txt`](artifacts/ipa2_comparison_dump.txt): Bản xuất đầy đủ `154` prologue `UnityFramework` được khôi phục trong IPA 2, danh sách các điểm vá hàm trong `Penny`, và kết quả giả lập Unicorn của `HunterFF.dylib`.
- [`artifacts/hunterff_devirt_analysis.txt`](artifacts/hunterff_devirt_analysis.txt): Bản xuất kết quả phân tích VM, 3 mã pin TLS SHA-256, từ điển lỗi backend và kiểm chứng vector chữ ký Ed25519 RFC 8032 trên Unicorn Engine.
