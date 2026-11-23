# TÀI LIỆU KIẾN TRÚC HỆ THỐNG TRẠNG THÁI & HIỆU ỨNG - US-028 ĐẾN US-035
## UML Class Diagram: Status Effect System (CC, DoT, Buff/Debuff & Sustain)

- **Mã User Story**: US-028 đến US-035
- **Các tính năng**:
  - `US-028`: Choáng (Stun) - Khóa mọi thao tác bài.
  - `US-029`: Câm lặng (Silence) - Chỉ cho phép đánh thường, khóa Skill/Ulti.
  - `US-030`: Khiêu khích (Taunt) - Ép mọi đòn đánh đơn vào bản thân.
  - `US-031`: Dễ tổn thương (Vulnerability) - Nhân hệ số % sát thương nhận vào.
  - `US-032`: Độc tố (Poison) - Trừ máu chuẩn (True Damage) đầu lượt bỏ qua Shield.
  - `US-033`: Chảy máu (Bleed) - Trừ máu chuẩn khi xuất chiêu.
  - `US-034`: Hồi máu (Heal) - Phục hồi sinh lực có chặn trần Max HP.
  - `US-035`: Thanh tẩy (Cleanse) - Quét sạch toàn bộ các Debuff/CC trên nhân vật.
- **Phân hệ**: Status Effect Engine
- **Độ ưu tiên**: CAO
- **Trạng thái**: HOÀN THÀNH

---

### 1. Kiến trúc Hướng Đối Tượng
Hệ thống sử dụng component `StatusEffectController` gắn trên mỗi `BattleUnit`:
- `ActiveStatus`: Lớp đối tượng lưu trữ loại trạng thái (`StatusEffectType`), thời gian hiệu lực (`Duration`), số tầng dồn (`Stack`) và giá trị tác động (`Value`).
- Tách biệt rõ ràng giữa logic trừ máu chuẩn qua `DamagePipeline` và logic quản lý vòng đời hiệu ứng tại các pha `OnTurnStart` và `OnTurnEnd`.

---

### 2. File Sơ Đồ Đi Kèm
- **File thiết kế StarUML**: [Antigravity_StarUML_Project.mdj](file:///c:/Users/ADMIN/Downloads/game%20unity/Docs/UML/Antigravity_StarUML_Project.mdj)
- **Hình ảnh sơ đồ PNG**: [US-028_to_035_Status_Effects.png](file:///c:/Users/ADMIN/Downloads/game%20unity/Docs/UML/US-028_to_035_Status_Effects.png)
- **Định dạng Vector SVG**: [US-028_to_035_Status_Effects.svg](file:///c:/Users/ADMIN/Downloads/game%20unity/Docs/UML/US-028_to_035_Status_Effects.svg)
