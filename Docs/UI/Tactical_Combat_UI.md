# TÀI LIỆU HƯỚNG DẪN TÍCH HỢP GIAO DIỆN TRẬN ĐẤU CHIẾN THUẬT (TACTICAL COMBAT HUD)
## TÍCH HỢP GIAO DIỆN COMBAT TỪ BẢN THIẾT KẾ FIGMA VÀO UNITY

---

### 1. Tổng quan giao diện (Figma Screen 3 - Tactical Combat)
Giao diện trận đấu chiến thuật được thiết kế theo phong cách Dark Fantasy hiện đại với các khu vực chức năng chính:

1. **Top Header Bar (Thanh thông tin đỉnh)**:
   - **Tốc độ trận đấu & Tự động**: Nút `1X SPEED / 2X SPEED` và `AUTO: ON / OFF`.
   - **Huy hiệu Lượt đấu (Encounter Badge)**: Hiển thị số Turn hiện tại (`ENCOUNTER TURN 1`), tự động đồng bộ từ `TurnManager`.
   - **Thanh Điểm Hành Động (Action Points - AP)**: Hiển thị số lượng AP hiện có (`8 / 10 ACTION POINTS`) kèm thanh 10 khối năng lượng phát sáng màu xanh cyan neon.
   - **Quản lý Bộ bài & Tiện ích**: `DECK: 18`, Nút Tạm dừng `||`, Nút Cài đặt `⚙`.

2. **Khu vực Sàn Đấu (Arena Overlays & Floating Units HUD)**:
   - **Phe Ta (Allied Formation - Bên Trái)**:
     - Tướng `Lyra (Lv.40) RANGER` (HP: 1,420 / 1,420, Buff: FOCUS +20%).
     - Tướng `Valerius (Lv.42) TANK` (HP: 2,840 / 2,840, Giáp ảo: `+650 AEGIS`).
     - Các ô trống đánh dấu vị trí: `BACK - BTM`, `FRONT - BTM`.
   - **Phe Địch (Enemy Formation - Bên Phải)**:
     - Quái vật: `Goblin Raider` (STRIKE - 120 DMG), `Shieldbearer` (PIERCE - Taunt), `Goblin Shaman` (POISON - 40/T).
     - Trùm phụ bản: `WARLORD GORZOG` (STAGE BOSS, HP: 5,200 / 6,200, Ý định: `INTENT: DREAD CLEAVE [AOE] 180 DMG`).

3. **Bottom Hand Cards Dock (Băng Thẻ Bài Kỹ Năng & Điều Khiển)**:
   - **Tab Nhận diện Tướng**: `VALERIUS • PALADIN` (Viền vàng) và `LYRA • RANGER` (Viền xanh dương).
   - **6 Lá bài kỹ năng**:
     - `Shield Slam`: Cost 2 AP | SKILL | Gây 220 ST vật lý. 60% làm CHOÁNG 1 lượt.
     - `Holy Bastion`: Cost 3 AP | GUARD | Tạo 450 Giáp Aegis toàn đội. Xóa 1 hiệu ứng xấu.
     - `Solar Wrath`: Cost 5 AP | ULTIMATE | Thiêu đốt toàn bộ kẻ địch, gây 680 ST thánh quang.
     - `Quick Shot`: Cost 1 AP | PIERCE | Bắn tỉa gây 160 ST xuyên giáp. Bốc thêm 1 lá bài.
     - `Poison Volley`: Cost 2 AP | DEBUFF | Hồi chiêu 1T | Gây 40 Độc/lượt trong 3 lượt lên toàn bộ hàng trước.
     - `Rain of Arrows`: Cost 4 AP | ULTIMATE | Mưa tên gây 520 ST, bỏ qua 50% phòng ngự mục tiêu.
   - **Tactical Belt (Thanh Bổ Trợ Tức Thời)**: 4 nút phép bổ trợ: `MANA`, `PURGE`, `EVADE`, `RALLY`.
   - **End Turn Button**: Nút bấm kích thước lớn, viền vàng kim loại với icon đồng hồ cát đỏ, văn bản `END TURN / HOLD TO PASS`.

---

### 2. Cấu trúc Mã nguồn đã triển khai (Implemented Scripts)

- **`Assets/Scripts/UI/CombatCardUI.cs`**:
  Quản lý hiển thị và tương tác cho từng lá bài: Cost kim cương, Name, Type, Description, Cooldown overlay, kiểm tra số dư AP và hiệu ứng phóng to khi hover chuột.
- **`Assets/Scripts/UI/CombatUnitHUDUI.cs`**:
  Quản lý thanh máu HP, giáp ảo Aegis Shield, ý định tấn công Intent của kẻ địch và thông tin Tướng.
- **`Assets/Scripts/UI/CombatHUDUI.cs`**:
  Bộ điều phối trung tâm của toàn bộ màn hình Combat, kết nối logic `TurnManager`, `GridManager`, quản lý Action Points, tốc độ game, bốc bài, và kích hoạt lượt địch khi bấm `End Turn`.
- **`Assets/Editor/CombatHUDAutoSetup.cs`**:
  Công cụ mở rộng Unity Editor giúp tự động tạo Canvas và gắn toàn bộ linh kiện UI hoàn chỉnh chỉ với 1 click.

---

### 3. Cách kích hoạt và Test trong Unity Editor

1. Mở Unity Editor (Project: `game unity`).
2. Mở Scene trận đấu: **`Assets/Scenes/CombatScene.unity`** (hoặc bất kỳ Scene nào bạn muốn đặt UI).
3. Trên thanh công cụ trên cùng của Unity Editor, chọn:
   ```
   Tools > Antigravity > Setup Tactical Combat UI (Figma)
   ```
4. Hệ thống sẽ tự động khởi tạo cụm Canvas **`CombatHUDCanvas`** chuẩn 1920x1080 với đầy đủ tất cả các thành phần theo đúng thiết kế Figma.
5. Nhấn nút **Play (▶)** để test tương tác trực tiếp:
   - Click vào các thẻ bài kỹ năng: AP sẽ tự động trừ đi, thẻ hồi chiêu sẽ hiển thị `CD: 1T`, thông báo chiến đấu sẽ hiện góc dưới.
   - Bấm các nút bổ trợ `MANA` (+2 AP), `PURGE`, `EVADE`, `RALLY` trên Tactical Belt.
   - Bấm nút **END TURN**: Hệ thống sẽ chuyển lượt sang phe địch, sau đó tự động bắt đầu Turn mới với đầy đủ 10 Action Points.
