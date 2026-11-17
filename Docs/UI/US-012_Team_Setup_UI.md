# TÀI LIỆU THIẾT KẾ VÀ TRIỂN KHAI GIAO DIỆN - US-012
## UI/UX: Xây Dựng Giao Diện Kho Tướng & Sơ Đồ Lưới Phe Ta (Team Setup)

- **Mã User Story**: US-012
- **Tên User Story**: UI/UX: Xây dựng giao diện kho Tướng và sơ đồ lưới phe ta (Team Setup).
- **Phân hệ**: UI/UX & Squad Preparation
- **Độ ưu tiên**: CAO
- **Ước lượng (Hours)**: 4 giờ
- **Trạng thái**: HOÀN THÀNH

---

### 1. Mục tiêu & Yêu cầu tính năng
Màn hình **Thiết lập Đội hình (Team Setup)** cho phép người chơi chuẩn bị chiến thuật trước khi bước vào trận đấu:
1. **Sơ đồ Lưới 2 Hàng x 3 Cột phe ta**:
   - Hàng 0: **Tiền Tuyến (Frontline)** - Nơi đứng của các tướng đỡ đòn (Tanker), che chắn sát thương cho hàng sau.
   - Hàng 1: **Hậu Phương (Backline)** - Nơi an toàn cho xạ thủ, pháp sư gây sát thương chủ lực.
   - 3 Cột (Cột 1, Cột 2, Cột 3) đối xứng với lưới của phe địch.
2. **Kho Tướng khả dụng (Character Inventory / Roster)**:
   - Hiển thị danh sách thẻ Tướng đang sở hữu (Paladin, Sniper, Phù thủy...).
   - Hiển thị đầy đủ thông số cơ bản: Máu (HP), Sức mạnh (ATK), Phòng ngự (DEF), Điểm năng lượng đóng góp (AP Contribution).
   - Đánh dấu trạng thái tướng đã được triển khai lên lưới ("ĐÃ TRIỂN KHAI" / "CHỌN TƯỚNG").
3. **Thanh thông tin chiến thuật (Tactical Overview)**:
   - Theo dõi số lượng tướng xuất trận (ví dụ: `2 / 3` tướng).
   - **Tính toán năng lượng đóng góp theo thời gian thực (Real-time AP Contribution)**: Tự động cộng dồn `APContribution` của tất cả các tướng được xếp vào lưới để làm `Total Turn Cost` ban đầu khi trận đấu bắt đầu.
4. **Hệ thống điều khiển (Control Buttons)**:
   - **Tự động xếp (Auto Deploy)**: Tự động xếp các tướng trong kho vào vị trí tối ưu (tướng nhiều AP/Tank lên Frontline, tướng ít AP/DPS xuống Backline).
   - **Xóa đội hình (Clear All)**: Làm trống toàn bộ lưới để xếp lại từ đầu.
   - **Vào trận đấu (Start Battle)**: Kiểm tra điều kiện tối thiểu 1 tướng, nạp đội hình vào `GridManager` và chuyển sang trạng thái chiến đấu.

---

### 2. Sơ đồ Bố cục Giao diện (UI Layout & Wireframe)

```
+-------------------------------------------------------------------------------------------------------------+
|                                    HEADER BAR (THANH TIÊU ĐỀ & TỔNG QUAN)                                    |
| [LOGO] HỆ THỐNG THIẾT LẬP ĐỘI HÌNH           Tướng Xuất Trận: 2 / 3        Tổng Năng Lượng: 6 AP (Turn Cost) |
+----------------------------------------------------------------------+--------------------------------------+
|                     SƠ ĐỒ LƯỚI CHIẾN THUẬT PHE TA                    |           KHO TƯỚNG KHẢ DỤNG         |
|               (2 HÀNG x 3 CỘT - FRONTLINE & BACKLINE)                |        (CHARACTER ROSTER PANEL)      |
|                                                                      |                                      |
|  [HÀNG 0 - TIỀN TUYẾN / FRONTLINE]                                   | +----------------------------------+ |
|  +------------------+  +------------------+  +------------------+    | | PALADIN ÁNH SÁNG   [+5 AP] [CHỌN] | |
|  | Tiền Tuyến 1     |  | Tiền Tuyến 2     |  | Tiền Tuyến 3     |    | | Vai trò: Đỡ Đòn (Tiền Tuyến)     | |
|  | PALADIN          |  | + Trống          |  | + Trống          |    | | HP: 1800 | ATK: 20 | DEF: 80     | |
|  | HP: 1800|DEF: 80 |  | (Click để chọn)  |  | (Click để chọn)  |    | +----------------------------------+ |
|  | [+5 AP]     [X]  |  |                  |  |                  |    |                                      |
|  +------------------+  +------------------+  +------------------+    | +----------------------------------+ |
|                                                                      | | SNIPER TINH ANH    [+1 AP] [CHỌN] | |
|  [HÀNG 1 - HẬU PHƯƠNG / BACKLINE]                                    | | Vai trò: Xạ Thủ (Hậu Phương)     | |
|  +------------------+  +------------------+  +------------------+    | | HP: 950  | ATK: 85 | DEF: 15     | |
|  | Hậu Phương 1     |  | Hậu Phương 2     |  | Hậu Phương 3     |    | +----------------------------------+ |
|  | + Trống          |  | SNIPER           |  | + Trống          |    |                                      |
|  | (Click để chọn)  |  | HP: 950|ATK: 85  |  | (Click để chọn)  |    | +----------------------------------+ |
|  |                  |  | [+1 AP]     [X]  |  |                  |    | | PHÙ THỦY HƯ KHÔNG  [+2 AP] [CHỌN] | |
|  +------------------+  +------------------+  +------------------+    | | Vai trò: Pháp Sư (Hậu Phương)    | |
|                                                                      | | HP: 800  | ATK: 95 | DEF: 10     | |
|                                                                      | +----------------------------------+ |
+----------------------------------------------------------------------+--------------------------------------+
| [Thông báo: Đã xếp 'Sniper' vào ô Hậu Phương 2]           [TỰ ĐỘNG XẾP]      [XÓA ĐỘI HÌNH]     [VÀO TRẬN ĐẤU] |
+-------------------------------------------------------------------------------------------------------------+
```

---

### 3. Cấu trúc Mã nguồn (Scripts Architecture)

Hệ thống được tổ chức chuyên biệt trong namespace `Antigravity.UI` gồm 4 file chính:

1. **`Assets/Scripts/UI/TeamSlotUI.cs`**:
   - Quản lý 1 ô trong tổng số 6 ô lưới.
   - Nhận diện tọa độ `(Row, Column)` với `Row 0` (Tiền Tuyến viền cam vàng) và `Row 1` (Hậu Phương viền xanh dương).
   - Hiển thị 2 trạng thái: Trống (`EmptyStateObject`) và Đã có tướng (`OccupiedStateObject`).
   - Hỗ trợ đổi màu khi người chơi click chọn ô (`SelectedColor`).
   - Cung cấp nút `[X]` gỡ tướng nhanh.

2. **`Assets/Scripts/UI/CharacterRosterCardUI.cs`**:
   - Quản lý từng item tướng trong kho lưu trữ.
   - Hiển thị trực quan Tên tướng, Avatar, Chỉ số cơ bản (HP, ATK, DEF), Badge năng lượng (`+AP`), và tóm tắt kỹ năng.
   - Quản lý trạng thái `SetDeployed(bool)`: Làm mờ và khóa nút khi tướng đã có mặt trên lưới đội hình.

3. **`Assets/Scripts/UI/TeamSetupUI.cs`**:
   - Điều phối toàn bộ vòng đời của màn hình Team Setup.
   - Quản lý danh sách `GridSlots` (6 ô) và nạp danh sách tướng `AvailableCharacters`.
   - Cơ chế chọn ô thông minh:
     - Nếu đã chọn 1 ô lưới: Bấm thẻ tướng sẽ gán ngay vào ô đó.
     - Nếu chưa chọn ô: Tự động gán vào ô trống tối ưu (Tanker ưu tiên Tiền Tuyến, DPS ưu tiên Hậu Phương).
   - Hàm `CalculateTotalAPContribution()`: Tính toán chính xác tổng AP đầu trận theo công thức GDD.
   - Hàm `StartBattle()`: Đẩy dữ liệu vào `GridManager` và `TurnManager` để sẵn sàng chiến đấu.

4. **`Assets/Editor/TeamSetupAutoSetup.cs`**:
   - Tiện ích mở rộng Unity Editor (`Tools/Antigravity/Setup Team Setup UI (US-012)`).
   - Tự động dựng toàn bộ hệ thống Canvas UI, Panel, 6 Slots Lưới, Roster Container, Buttons, EventSystem và liên kết dữ liệu chỉ với 1 cú click.

---

### 4. Hướng dẫn Sử dụng trên Unity Editor

1. Mở dự án trong Unity 6 (phiên bản `6000.0.x` hoặc tương thích).
2. Mở Scene chính: `Assets/Scenes/SampleScene.unity`.
3. Trên thanh công cụ phía trên, chọn menu:
   **`Tools` -> `Antigravity` -> `Setup Team Setup UI (US-012)`**.
4. Toàn bộ hệ thống UI sẽ được khởi tạo hoàn chỉnh trong Scene hiện tại, tự động gán dữ liệu 3 tướng mẫu (`Paladin_Stats`, `Sniper_Stats`, `Mage_Stats`).
5. Bấm nút **Play** trong Unity để trải nghiệm trực tiếp:
   - Click vào ô lưới để chọn vị trí.
   - Click nút **CHỌN** trên thẻ tướng để đưa tướng vào ô.
   - Quan sát số lượng tướng (`x / 3`) và Tổng AP cập nhật theo thời gian thực.
   - Bấm **TỰ ĐỘNG XẾP** để máy tự xếp đội hình tối ưu.
   - Bấm **XÓA ĐỘI HÌNH** để đặt lại từ đầu.
