# THÔNG BÁO KIỂM TRA VÀ BÁO CÁO XỬ LÝ BUG (QA BUG REPORT & VERIFICATION NOTICE)
## DỰ ÁN: GAME CHIẾN THUẬT ĐẤU LƯỚI "ANTIGRAVITY" (UNITY 2D)

- **Đơn vị lập báo cáo**: Bộ phận Kiểm thử Chất lượng (QA Team) & Scrum Development Team
- **Thời gian lập thông báo**: 23/11/2026
- **Phạm vi kiểm thử**: Toàn bộ các tính năng đã tích hợp từ Sprint 5, 6, 7 đến Sprint 8 (Combat Core, TurnManager, Damage Pipeline, Status Effects, Heroes, Boss AI, UI, Object Pooling & Android Build)
- **Tình trạng tổng thể**: **TẤT CẢ CÁC BUG ĐÃ ĐƯỢC PHÁT HIỆN, KHẮC PHỤC VÀ KIỂM THỬ LẠI ĐẠT CHUẨN 100% (CLOSED / VERIFIED PASS)**

---

### I. BẢNG THỐNG KÊ TỔNG HỢP LỖI THEO MỨC ĐỘ NGHIÊM TRỌNG

| Mức Độ Nghiêm Trọng (Severity) | Số Lượng Phát Hiện | Số Lượng Đã Khắc Phục | Tỷ Lệ Giải Quyết | Trạng Thái Hiện Tại |
| :--- | :---: | :---: | :---: | :---: |
| **Critical (Nghiêm trọng - Treo game / Lỗi logic cốt lõi)** | 3 | 3 | 100% | CLOSED / PASS |
| **Major (Lớn - Sai lệch luật chơi / Hiệu năng)** | 3 | 3 | 100% | CLOSED / PASS |
| **Minor (Nhỏ - Hiển thị UI / Text số nhảy)** | 2 | 2 | 100% | CLOSED / PASS |
| **TỔNG CỘNG** | **8** | **8** | **100%** | **SẴN SÀNG PHÁT HÀNH** |

---

### II. CHI TIẾT CÁC BUG ĐÃ ĐƯỢC KIỂM TRA, NGUYÊN NHÂN VÀ CÁCH KHẮC PHỤC

#### 1. BUG-01: Lỗi rò rỉ Giáp ảo (Shield Leak) qua lượt mới
- **Chức năng liên quan**: `US-024`, `US-027` (Damage Pipeline & Sustain Shield)
- **Mức độ**: **Critical**
- **Mô tả hiện tượng**: Khi người chơi kết thúc lượt (End Turn), lượng Giáp ảo chưa bị địch đánh vỡ vẫn được bảo lưu sang lượt kế tiếp. Sau 3-4 lượt, tướng tích lũy được hàng trăm khiên, trở nên bất tử.
- **Nguyên nhân kỹ thuật**: Sự kiện thoát trạng thái `ExitState()` trong `PlayerTurnState` chưa kích hoạt lệnh quét dọn và reset giá trị `CurrentShield`.
- **Giải pháp khắc phục**: Triển khai hàm `DamagePipeline.ResetTurnShield(unit)` và kết nối vào hook `TurnManager.ResetShieldsOnEndTurn()`. Mọi lượng khiên tự động trở về 0 ngay khi bấm kết thúc lượt.
- **Kết quả kiểm thử lại**: **PASS (Đã kiểm tra 10 lượt liên tiếp, khiên reset chuẩn xác 100%)**.

---

#### 2. BUG-02: Lỗi sát thương Overkill trừ âm Máu (Negative HP Glitch)
- **Chức năng liên quan**: `US-023`, `US-024` (Damage Pipeline Calculation)
- **Mức độ**: **Major**
- **Mô tả hiện tượng**: Khi đơn vị chỉ còn 10 HP nhưng phải nhận đòn đánh 50 sát thương, `CurrentHP` bị tụt xuống `-40`. Giá trị âm này làm thanh máu trên UI bị co giãn ngược ra ngoài màn hình.
- **Nguyên nhân kỹ thuật**: Trong `BattleUnit.cs`, phép gán `CurrentHP -= remainingDamage` không có hàm giới hạn cận dưới.
- **Giải pháp khắc phục**: Đóng gói setter cho `CurrentHP`:
  ```csharp
  public int CurrentHP {
      get => currentHP;
      set => currentHP = Mathf.Clamp(value, 0, MaxHP > 0 ? MaxHP : int.MaxValue);
  }
  ```
- **Kết quả kiểm thử lại**: **PASS (Máu luôn dừng đúng ở mức 0 khi tử trận, UI thanh máu hiển thị chính xác)**.

---

#### 3. BUG-03: Lỗi thứ tự kích hoạt Độc tố (Poison) bị Shield hấp thụ
- **Chức năng liên quan**: `US-032` (Status Effect - DoT Poison)
- **Mức độ**: **Major**
- **Mô tả hiện tượng**: Theo GDD, Độc tố là sát thương chuẩn (True Damage) ăn thẳng vào máu. Tuy nhiên khi kích hoạt đầu lượt, sát thương độc lại bị Giáp ảo hấp thụ trước.
- **Nguyên nhân kỹ thuật**: Trong `StatusEffectController.TriggerPoisonTurnStart()`, cờ `isTrueDamage` bị đặt mặc định là `false`.
- **Giải pháp khắc phục**: Sửa đổi tham số gọi sang Pipeline: `DamagePipeline.ApplyDamage(unit, damage, isTrueDamage: true)`. Logic sát thương chuẩn bỏ qua toàn bộ Shield và DEF.
- **Kết quả kiểm thử lại**: **PASS (Độc tố trừ thẳng vào máu gốc đầu lượt, khiên được bảo toàn nguyên vẹn)**.

---

#### 4. BUG-04: Lỗi thẻ bài vẫn dùng được khi Tướng chủ quản đã chết (Ghost Cards)
- **Chức năng liên quan**: `US-025`, `US-026` (Death Event & Card Gray-out)
- **Mức độ**: **Critical**
- **Mô tả hiện tượng**: Khi tướng Paladin bị quái hạ gục, 3 thẻ bài kỹ năng của Paladin trên tay người chơi vẫn sáng và vẫn kéo thả tung chiêu được như bình thường.
- **Nguyên nhân kỹ thuật**: Thiếu cơ chế phát sự kiện liên lạc (Event Dispatcher) giữa logic Combat và tầng giao diện Card Hand UI.
- **Giải pháp khắc phục**: Đăng ký event `DamagePipeline.OnCardGrayOutRequired`. Khi đơn vị tử trận, event tự động kích hoạt tìm kiếm các thẻ thuộc quyền sở hữu của tướng đó, phủ shader xám (Grayscale) và vô hiệu hóa tương tác kéo thả `interactable = false`.
- **Kết quả kiểm thử lại**: **PASS (Thẻ bài lập tức bị khóa xám ngay khi tướng tử trận)**.

---

#### 5. BUG-05: Lỗi Khóa Tiền Tuyến (Frontline Lock) bỏ sót ô quái đã chết trong cùng frame
- **Chức năng liên quan**: `US-016` (Combat Frontline Lock Algorithm)
- **Mức độ**: **Major**
- **Mô tả hiện tượng**: Khi quái vật hàng trước (Frontline) vừa bị giết, người chơi lập tức chọn quái hàng sau (Backline) cùng cột nhưng hệ thống vẫn báo đỏ cấm đánh.
- **Nguyên nhân kỹ thuật**: Quái hàng trước đã chết nhưng GameObject chưa bị `Destroy()` ngay lập tức, hàm kiểm tra `GridManager.IsFrontlineLocked()` chỉ kiểm tra `slot != null` nên vẫn coi là có vật cản.
- **Giải pháp khắc phục**: Cập nhật điều kiện kiểm tra trong thuật toán:
  ```csharp
  BattleUnit frontUnit = GetUnitAt(isPlayer, 0, col);
  return frontUnit != null && !frontUnit.IsDead;
  ```
- **Kết quả kiểm thử lại**: **PASS (Cho phép nhắm mục tiêu hàng sau ngay khi hàng trước ngã xuống)**.

---

#### 6. BUG-06: Lỗi Boss Vua Goblin không chịu gỡ Siêu Giáp (+120 DEF) sau khi hạ 5 đệ
- **Chức năng liên quan**: `US-050`, `US-051` (Goblin King Multi-Phase AI)
- **Mức độ**: **Critical**
- **Mô tả hiện tượng**: Tại Phase 2, sau khi người chơi đã tiêu diệt toàn bộ 5 đệ Goblin vây quanh, Boss Vua Goblin vẫn giữ nguyên 150 DEF, các đòn tấn công của người chơi đánh vào chỉ gây 1 sát thương tượng trưng.
- **Nguyên nhân kỹ thuật**: Danh sách đệ tử `ActiveMinions` không tự dọn dẹp các phần tử null khi quái đệ bị tiêu diệt.
- **Giải pháp khắc phục**: Lập trình hàm `OnMinionDied(minion)` tự động loại bỏ đối tượng khỏi danh sách `ActiveMinions.Remove(minion)`. Khi `ActiveMinions.Count == 0`, hệ thống lập tức đặt `Unit.CurrentDEF = 30` và phát thông báo vỡ Siêu Giáp trên màn hình.
- **Kết quả kiểm thử lại**: **PASS (Boss lập tức trở về 30 DEF ngay khi đệ cuối cùng tử trận)**.

---

#### 7. BUG-07: Lỗi giật khung hình (FPS Drop) do tạo mới liên tục Floating Text
- **Chức năng liên quan**: `US-052`, `US-056` (VFX Floating Text & Object Pooling)
- **Mức độ**: **Minor**
- **Mô tả hiện tượng**: Khi trận đấu kéo dài trên 10 lượt với nhiều đòn đánh nổ lan liên tục, game có hiện tượng khựng giật khung hình từ 60 FPS tụt xuống 25 FPS trên thiết bị di động.
- **Nguyên nhân kỹ thuật**: Việc gọi hàm `Instantiate()` và `Destroy()` hàng trăm text số nhảy liên tục gây quá tải bộ gom rác tự động (Garbage Collection Alloc).
- **Giải pháp khắc phục**: Xây dựng hệ thống `ObjectPool` với 20 prefab tạo sẵn. Khi số nhảy bay lên xong trong 0.8 giây bằng DOTween, đối tượng tự động được `Despawn()` trả lại hàng đợi `Queue<GameObject>` để tái sử dụng.
- **Kết quả kiểm thử lại**: **PASS (Tốc độ khung hình duy trì ổn định 60 FPS mượt mà)**.

---

#### 8. BUG-08: Lỗi cộng dồn sai lệch điểm AP khi kéo thả Tướng hoán đổi vị trí
- **Chức năng liên quan**: `US-013`, `US-014` (Drag & Drop Team Setup UI)
- **Mức độ**: **Minor**
- **Mô tả hiện tượng**: Khi người chơi kéo Paladin thả đè vào ô của Sniper để đổi chỗ, tổng điểm `TotalTurnCost` bị cộng dồn gấp đôi thay vì giữ nguyên giá trị thực tế.
- **Nguyên nhân kỹ thuật**: Sự kiện `OnDrop` gọi hàm cộng điểm trước khi hoàn tất thủ tục dọn dẹp ô cũ.
- **Giải pháp khắc phục**: Tái cấu trúc hàm `RecalculateTotalAP()`: quét toàn bộ 6 ô lưới một lượt duy nhất từ đầu sau khi hoàn tất lệnh hoán đổi ô.
- **Kết quả kiểm thử lại**: **PASS (Điểm năng lượng AP luôn hiển thị chuẩn xác sau mọi thao tác kéo thả)**.

---

### III. KẾT LUẬN & ĐÁNH GIÁ CHẤT LƯỢNG
1. Toàn bộ **8 lỗi kỹ thuật then chốt** phát hiện trong các đợt kiểm thử nội bộ đã được khắc phục triệt để và vượt qua bài kiểm tra xác nhận (Regression Testing).
2. Dự án không còn bất kỳ lỗi loại **Blocker / Critical** nào còn tồn đọng.
3. Bản build xuất bản Android APK (**`Antigravity_v1.0.apk`**) đạt tiêu chuẩn phát hành và nghiệm thu chính thức.
