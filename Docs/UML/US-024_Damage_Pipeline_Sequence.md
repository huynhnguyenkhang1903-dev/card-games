# TÀI LIỆU ĐẶC TẢ QUY TRÌNH XỬ LÝ SÁT THƯƠNG - US-024
## UML Sequence Diagram: Chuỗi Xử Lý Sát Thương, Khiên & Tử Trận

- **Mã User Story**: US-024
- **Tên User Story**: Damage Pipeline: Viết logic sát thương trừ vào Shield trước, Overkill mới trừ vào HP.
- **Các US liên quan**: US-023 (Trừ DEF), US-025 (Sự kiện tử trận), US-026 (Khóa xám 3 thẻ bài), US-027 (Reset Giáp ảo End Turn).
- **Phân hệ**: Combat Engine / Damage Pipeline
- **Độ ưu tiên**: CAO
- **Trạng thái**: HOÀN THÀNH

---

### 1. Giới thiệu & Mục tiêu
Tài liệu này đặc tả quy trình tuần tự (Sequence Flow) của hệ thống tính toán và áp dụng sát thương trong **Antigravity**. Hệ thống phân tách rõ ràng giữa sát thương vật lý/phép thông thường và sát thương chuẩn (True Damage), đồng thời đảm bảo lớp Giáp ảo (Shield) luôn hấp thụ sát thương trước khi Máu (HP) bị tổn hại.

---

### 2. Các thành phần tham gia (Lifelines)
1. **Attacker (BattleUnit)**: Đơn vị xuất chiêu hoặc nguồn gây sát thương.
2. **DamagePipeline (Static Class)**: Bộ điều phối tính toán trung tâm.
3. **TargetUnit (BattleUnit)**: Mục tiêu chịu sát thương.
4. **StatusEffectController**: Bộ quản lý hiệu ứng (kiểm tra Vulnerability).
5. **GridManager**: Quản lý lưới 2x3 (giải phóng ô khi đơn vị tử trận).
6. **CardHandUI**: Quản lý giao diện bài (khóa xám 3 thẻ bài khi chủ nhân chết).

---

### 3. Chuỗi thông điệp tuần tự (Message Flow)
1. `Attacker -> DamagePipeline`: `ApplyDamage(target, rawDamage, isTrueDamage)`
2. `DamagePipeline -> StatusEffectController`: `GetVulnerabilityMultiplier()`
3. `DamagePipeline`: Tính toán `Actual Damage = Mathf.Max(1, rawDamage - targetDEF) * vulnMultiplier`.
4. `DamagePipeline -> TargetUnit`: Trừ vào `CurrentShield` trước.
5. `DamagePipeline -> TargetUnit`: Nếu còn dư sát thương (Overkill), trừ tiếp vào `CurrentHP`.
6. `DamagePipeline`: Kiểm tra `TargetUnit.IsDead == true`:
   - Bắn sự kiện `OnUnitDeath` -> `GridManager.ReleaseSlot(row, col)`.
   - Bắn sự kiện `OnCardGrayOutRequired` -> `CardHandUI.GrayOutCards()`.
   - Gọi `TargetUnit.Die()`.

---

### 4. File Sơ Đồ Đi Kèm
- **File thiết kế StarUML**: [Antigravity_StarUML_Project.mdj](file:///c:/Users/ADMIN/Downloads/game%20unity/Docs/UML/Antigravity_StarUML_Project.mdj)
- **Hình ảnh sơ đồ PNG**: [US-024_Damage_Pipeline_Sequence.png](file:///c:/Users/ADMIN/Downloads/game%20unity/Docs/UML/US-024_Damage_Pipeline_Sequence.png)
- **Định dạng Vector SVG**: [US-024_Damage_Pipeline_Sequence.svg](file:///c:/Users/ADMIN/Downloads/game%20unity/Docs/UML/US-024_Damage_Pipeline_Sequence.svg)
