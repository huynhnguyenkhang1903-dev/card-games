# **PHẦN 1: THIẾT KẾ CƠ CHẾ GAME**

# **CHƯƠNG 1: Các định nghĩa về game**

## **1\. HỆ THỐNG CHỈ SỐ CƠ BẢN (CHARACTER STATS)**

Đây là các chỉ số nền tảng gắn liền với bản thân nhân vật trước khi bước vào trận đấu.

* **Base HP (Máu / Sinh lực cơ bản):** Điểm sinh mệnh nền tảng của nhân vật. Khi HP tụt về 0, nhân vật bị hạ gục và loại bỏ khỏi bàn cờ.  
* **Base ATK (Tấn công cơ bản):** Chỉ số sức mạnh gốc. Sát thương thực tế khi đánh ra một thẻ bài thường là sự kết hợp giữa sát thương ghi trên bản thân thẻ bài đó cộng dồn (hoặc nhân) với Base ATK của nhân vật đang thi triển.  
* **Base DEF (Phòng thủ cơ bản):** Chỉ số chống chịu mặc định giúp giảm thiểu sát thương nhận vào từ các đòn tấn công vật lý của đối phương. DEF thường không có tác dụng giảm sát thương DoT (Sát thương theo thời gian).  
* **Action Point (AP) Contribution / Năng lượng đóng góp:** Lượng điểm Cost (Tài nguyên hành động) mà mỗi nhân vật tự động cung cấp vào quỹ tài nguyên chung của toàn đội hình khi bắt đầu mỗi lượt đấu.

**2\. CƠ CHẾ TÀI NGUYÊN (COST & ACTION POINT)**  
Hệ thống quản lý năng lượng để sử dụng thẻ bài (chiêu thức) trong mỗi lượt đấu.

* **Total Turn Cost (Tổng quỹ năng lượng):** Tổng số điểm Cost người chơi nhận được ở mỗi lượt. Bằng tổng chỉ số *AP Contribution* của tất cả nhân vật đang sống sót trên sân. Nếu một nhân vật bị hạ gục, quỹ điểm Cost ở các lượt tiếp theo của đội sẽ bị sụt giảm tương ứng.  
* **Card Cost (Chi phí thẻ bài):** Số điểm năng lượng trích từ Tổng quỹ năng lượng (Total Turn Cost) cần thiết để sử dụng một thẻ bài.  
* **Turn Action Cost (Năng lượng hành động mỗi lượt):** Lượng tài nguyên mặc định được hệ thống làm mới và cung cấp cho người chơi ở đầu mỗi lượt đấu (Start of Turn) để chi trả Cost cho các thẻ bài.

## **3\. CƠ CHẾ PHẠM VI CHIÊU THỨC VÀ ĐỘI HÌNH (GRID & POSITIONING)**

Hệ thống này được thiết kế dựa trên sơ đồ lưới **2 Hàng x 3 Cột (Tổng 6 ô mỗi bên)**, gồm Hàng trước (Frontline) và Hàng sau (Backline).

* **Frontline Lock (Khóa Tiền tuyến):** Các đòn đánh đơn mục tiêu thông thường bắt buộc phải nhắm vào nhân vật đứng ở Hàng trước. Nếu Hàng trước của cột đó còn người, nhân vật ở Hàng sau sẽ được bảo vệ khỏi đòn đánh thường.  
* **Pierce (Xuyên thấu / Sát thương dọc cột):** Đòn tấn công đánh trúng mục tiêu ở Hàng trước và tự động đâm xuyên, gây sát thương cho mục tiêu đứng ở Hàng sau trên cùng một Cột (Tối đa 2 mục tiêu).  
* **Blast / Cleave (Lan rộng / Sát thương ngang hàng):** Đòn tấn công đánh vào một mục tiêu chính và tạo sóng xung kích lan sang 2 ô liền kề (Trái/Phải) trên cùng một Hàng ngang (Tối đa 3 mục tiêu).  
* **Displace / Reposition (Dịch chuyển vị trí):** Các hiệu ứng thay đổi cấu trúc đội hình:  
  * *Push (Đẩy):* Đẩy mục tiêu từ Hàng trước lùi xuống Hàng sau (nếu ô sau trống).  
  * *Pull (Kéo):* Kéo mục tiêu từ Hàng sau lên Hàng trước.  
  * *Swap (Hoán đổi):* Tráo đổi vị trí của 2 nhân vật trên sân.

## **4\. CƠ CHẾ KHỐNG CHẾ (CROWD CONTROL \- CC)**

Các hiệu ứng dùng để tước quyền hành động hoặc cản trở chiến thuật của đối phương.

* **Stun (Choáng):** Vô hiệu hóa hoàn toàn nhân vật. Nhân vật không thể thực hiện hành động. Các thẻ bài thuộc về nhân vật này sẽ bị mờ đi và không thể sử dụng.  
* **Taunt / Provoke (Khiêu khích):** Phá vỡ quy tắc Frontline Lock. Buộc mọi thẻ bài tấn công đơn mục tiêu của đối phương phải nhắm vào nhân vật đang kích hoạt Khiêu khích, bất kể nhân vật đó đứng ở đâu.  
* **Silence (Câm lặng):** Khóa thẻ bài Kỹ năng (Skill/Spell) hoặc Tuyệt kỹ (Ultimate). Nhân vật bị câm lặng chỉ được phép dùng các thẻ Tấn công (Attack) vật lý cơ bản.  
* **Root / Immobilize (Trói chân / Cấm di chuyển):** Nhân vật không thể bị dịch chuyển bởi các hiệu ứng Push (Đẩy), Pull (Kéo) hay Swap (Hoán đổi).  
* **Blind (Mù):** Các đòn tấn công của nhân vật bị mù có xác suất bị trượt (Miss), không gây sát thương và hủy bỏ mọi hiệu ứng đính kèm của thẻ bài đó.

## **5\. CƠ CHẾ HỖ TRỢ VÀ DUY TRÌ (BUFF / SUSTAIN)**

Các hiệu ứng có lợi dùng để bảo vệ hoặc tăng cường sức mạnh cho đội hình.

* **Shield / Block (Khiên ảo / Đỡ đòn):** Một lớp máu phụ tạm thời. Sát thương nhận vào sẽ trừ vào Shield trước khi trừ vào HP gốc. Shield thường sẽ biến mất khi kết thúc lượt.  
* **Heal (Hồi máu / Trị liệu):** Phục hồi lại lượng máu (HP) đã mất cho mục tiêu. Lượng máu được hồi phục không thể vượt quá giới hạn Máu tối đa (Max HP) của nhân vật, trừ khi có cơ chế đặc biệt cho phép chuyển hóa phần máu hồi dư thừa thành Khiên ảo (Shield).  
* **Cleanse / Dispel (Giải trừ / Thanh tẩy):** Loại bỏ ngay lập tức các trạng thái khống chế (Choáng, Câm lặng...) và các hiệu ứng bất lợi (Giảm chỉ số, DoT) đang bám trên đồng minh.  
* **CC Immunity (Miễn khống chế):** Chặn đứng và từ chối mọi trạng thái khống chế hoặc hiệu ứng bất lợi mới từ đối phương trong thời gian hiệu lực. Nhân vật vẫn nhận đủ sát thương vật lý gốc.

## **6\. CƠ CHẾ HIỆU ỨNG BẤT LỢI VÀ SÁT THƯƠNG THEO THỜI GIAN (DEBUFF / DOT)**

Các trạng thái làm suy yếu đối phương và hệ thống gây sát thương hao mòn.

* **DoT Trigger (Thời điểm kích hoạt DoT):** Sát thương thường kích hoạt ở đầu Giai đoạn Bắt đầu lượt (Start of Turn) của nhân vật dính hiệu ứng.  
* **Stack (Cộng dồn)**: Khả năng tích lũy nhiều lớp hiệu ứng:  
  * Intensity (Cộng dồn cường độ): Tăng lượng sát thương gây ra mỗi lượt dựa trên số stack.  
  * Duration (Cộng dồn thời lượng): Kéo dài số lượt tồn tại của hiệu ứng.  
* **Poison (Trúng độc)**: Hiệu ứng DoT tập trung vào việc cộng dồn cường độ. Sát thương nhỏ lúc đầu nhưng tăng tiến rất mạnh nếu không được giải trừ.  
* **Burn (Thiêu đốt)**: Hiệu ứng DoT có tỷ lệ lây lan (Spread) sang các nhân vật đứng ở các ô liền kề (Trên, Dưới, Trái, Phải) khi kết thúc lượt.  
* **Bleed (Chảy máu)**: Ép nhân vật nhận sát thương ngay lập tức mỗi khi nhân vật đó sử dụng một thẻ bài.  
* **Detonate / Catalyst (Kích nổ / Xúc tác):** Hiệu ứng lập tức tiêu thụ toàn bộ các stack DoT đang bám trên mục tiêu và gây ra tổng sát thương cùng một lúc.  
* **Decay (Hao mòn hiệu ứng):** Cơ chế tự động giảm đi 1 stack DoT vào cuối mỗi lượt của nhân vật bị dính hiệu ứng.  
* **Vulnerability / Frail (Dễ tổn thương):** Trạng thái Debuff khiến nhân vật nhận thêm phần trăm sát thương từ mọi nguồn hoặc từ đòn tấn công tiếp theo.

# **CHƯƠNG 2: Giai đoạn chuẩn bị trước trận đấu (Pre-match setup)**

Đây là giai đoạn chiến thuật nền tảng, nơi người chơi thu thập thông tin và tùy chỉnh đội hình trước khi chính thức bước vào vòng lặp chiến đấu. Do game đi theo hướng chiến thuật tất định (không có yếu tố bốc bài ngẫu nhiên \- RNG), mọi quyết định ở bước này sẽ ảnh hưởng trực tiếp đến kết quả trận đấu.

## **1\. NHẬN DIỆN MÀN CHƠI (STAGE PREVIEW)**

* **Nhận diện đối thủ:** Xem trước chủng loại quái, Boss và vị trí đứng của chúng trên sơ đồ lưới 2x3. Điều này giúp người chơi lên kế hoạch sắp xếp vị trí và chọn các nhân vật có bộ kỹ năng diện rộng (Pierce, Blast) nhắm đúng vào điểm yếu đội hình địch.  
* **Hiệu ứng môi trường (Stage/Weather Effects):** Xem trước các yếu tố ngoại cảnh hoặc luật chơi đặc biệt tác động chung lên trận đấu (Ví dụ: Màn chơi rực lửa tự động gắn hiệu ứng Thiêu đốt, hoặc sương mù làm tăng tỉ lệ đánh trượt).

## **2\. XÂY DỰNG ĐỘI HÌNH (TEAM FORMATION & POSITIONING)**

* **Chọn nhân vật:** Lựa chọn tối đa **4 nhân vật** xuất chiến từ kho nhân vật, ưu tiên sự phối hợp bù trừ vai trò (Sát thương, Đỡ đòn, Khống chế, Hồi phục).  
* **Sắp xếp Tọa độ (Grid Placement):** Kéo thả nhân vật vào lưới 2 Hàng x 3 Cột (2x3) của phe mình. Áp dụng chiến thuật vị trí: đặt nhân vật có DEF/HP cao ở Hàng trước (Frontline) để tận dụng luật Khóa Tiền tuyến, che chắn cho nhân vật yếu máu ở Hàng sau (Backline).  
* **Đo lường Quỹ Năng lượng (Cost Preview):** Giao diện tự động cộng dồn chỉ số *Năng lượng đóng góp (AP Contribution)* của các nhân vật được chọn, hiển thị mức **Tổng quỹ năng lượng (Total Turn Cost)** mà đội hình này sẽ tạo ra ở mỗi lượt. Điều này giúp người chơi cân đối lại đội hình nếu thiếu năng lượng vận hành chiêu thức.

## **3\. TÙY CHỈNH THẺ PHỤ TRỢ (TACTICAL DECK SETUP)**

* **Mở khóa Slot Thẻ phụ:** Cứ mỗi 1 nhân vật được đưa ra sân, đội hình sẽ được cấp phép mang theo 1 Thẻ phụ trợ (Utility Card). Với đội hình tối đa 4 người, người chơi được chọn đúng **4 lá bài phụ trợ** từ kho đồ chung mang vào trận.  
* **Tính chiến thuật:** Lựa chọn các thẻ bài đa dụng (như Tạo Khiên, Tăng ATK, Giải Khống chế) để lấp đầy điểm yếu của đội hình hiện tại. Các thẻ này tiêu tốn điểm Cost chung của đội và có thể sử dụng linh hoạt mà không phụ thuộc vào lượt đi của một nhân vật cụ thể.

## **4\. TỔNG QUAN BỘ BÀI TRONG TRẬN (IN-MATCH CARD SYSTEM)**

Khi bước vào trận đấu, toàn bộ bộ bài sẽ được hiển thị sẵn trên giao diện để người chơi tính toán chi tiêu Cost hợp lý, hoàn toàn không có cơ chế bốc bài ngẫu nhiên.

* **Thẻ Kỹ năng Nhân vật (Character Deck):** Tối đa 12 lá. Mỗi nhân vật mang theo đúng 3 thẻ chiêu thức chủ động (Đánh thường, Skill, Ultimate). Thẻ của nhân vật nào chỉ sáng lên khi đến lượt hành động của nhân vật đó.  
* **Thẻ Phụ trợ (Tactical Deck):** Tối đa 4 lá. Xếp tách biệt trên giao diện, đóng vai trò như các phép bổ trợ tức thời.  
* **Kỹ năng Bị động (Passive Traits):** Mỗi nhân vật có 1 kỹ năng bị động (tối đa 4 bị động trong đội). Các kỹ năng này không biến thành thẻ bài để bấm, mà hiển thị dưới dạng biểu tượng trên thanh máu, hoạt động ngầm và tự động kích hoạt khi thỏa mãn điều kiện.

# **CHƯƠNG 3: Vòng lặp chiến đấu trong trận(Ingame combat loop)**

Trận đấu diễn ra theo thể thức **Lượt đấu theo Phe (Team-based Turn Flow)**. Hai bên (Người chơi và Đối phương) sẽ luân phiên thực hiện toàn bộ hành động của phe mình, sau đó chuyển quyền cho bên còn lại. Một vòng lặp hoàn chỉnh bao gồm các giai đoạn và quy tắc sau:

## **1\. GIAI ĐOẠN BẮT ĐẦU LƯỢT (START OF TURN PHASE)**

* **Kiểm đếm và Làm mới Năng lượng:** Hệ thống quét số lượng nhân vật còn sống trên sân, cộng dồn chỉ số *AP Contribution* của họ để bơm đầy **Tổng quỹ năng lượng (Total Turn Cost)** cho lượt này.  
* **Kích hoạt Sát thương hao mòn (DoT):** Các hiệu ứng như Trúng Độc (Poison) sẽ trừ thẳng vào HP của nhân vật dính hiệu ứng ngay tại thời điểm này.  
* **Cập nhật Trạng thái (Status Update):** Giảm thời gian tồn tại (số lượt) của các Buff/Debuff. Giải trừ các trạng thái khống chế (CC) đã hết hạn.  
* **Giảm Thời gian hồi chiêu (Cooldown Reduction):** Lập tức giảm 1 lượt thời gian chờ (Cooldown) cho tất cả các thẻ kỹ năng đã sử dụng ở các lượt trước.

## **2\. GIAI ĐOẠN HÀNH ĐỘNG (ACTION PHASE)**

Đây là trọng tâm của trận đấu, nơi người chơi tiêu xài quỹ năng lượng cho việc tấn công, phòng thủ hoặc điều chỉnh đội hình.

* **Di chuyển chiến thuật (Tactical Movement):**  
  1. Người chơi có thể tiêu tốn **1 Cost** để thực hiện thao tác dịch chuyển đối với 1 nhân vật đang đứng trên sân.  
  2. Các thao tác hợp lệ: Đổi hàng (Tiến/Lùi), Đổi cột (Sang trái/phải), hoặc Hoán đổi (Swap) vị trí với đồng minh.  
  3. *Lưu vị trí:* Vị trí sau khi thay đổi sẽ được lưu lại làm vị trí cố định của nhân vật trong trận đấu cho đến lượt đi tiếp theo của phe ta, trừ khi bị kỹ năng Khống chế vị trí (Kéo/Đẩy) của địch tác động.  
* **Quy trình xuất chiêu (Dùng Thẻ Nhân vật):**  
  1. Người chơi chọn 1 thẻ bài đang sáng (đủ Cost và không bị Cooldown).  
  2. Hệ thống hiển thị tầm nhìn mục tiêu dựa trên luật *Khóa Tiền tuyến (Frontline Lock)*.  
  3. Người chơi xác nhận mục tiêu $\rightarrow$ Thẻ bài kích hoạt hiệu ứng.  
  4. Hệ thống trừ điểm Cost khỏi Tổng quỹ. Thẻ bài vừa dùng sẽ mờ đi và bắt đầu đếm ngược Cooldown.  
* **Sử dụng Thẻ Phụ trợ (Tactical Cards):** Người chơi có thể kích hoạt 4 thẻ phụ trợ (tiêu hao Cost chung) bất cứ lúc nào trong giai đoạn này để chữa cháy hoặc combo.

## **3\. CƠ CHẾ XỬ LÝ HẠ GỤC (DEATH / KNOCKOUT MECHANICS)**

Bất cứ khi nào HP của một nhân vật hoặc quái vật tụt xuống 0 (dù là trong lượt hành động hay do nhận sát thương DoT ở đầu lượt), hệ thống lập tức kích hoạt quy trình sau:

* **Giải phóng Vị trí:** Nhân vật/quái vật bị hạ gục sẽ lập tức bị đẩy ra khỏi bàn cờ. Ô vị trí mà họ đang đứng trên lưới 2x3 sẽ biến thành ô trống. (Người chơi có thể dùng Cost ở lượt sau để di chuyển nhân vật khác vào ô này, hoặc các kỹ năng Kéo/Đẩy có thể tận dụng khoảng trống mới này).  
* **Vô hiệu hóa Thẻ bài:** Toàn bộ 3 lá bài kỹ năng (Đánh thường, Skill, Ultimate) thuộc về nhân vật vừa bị hạ gục sẽ bị khóa vĩnh viễn và chuyển sang màu xám trên giao diện. Người chơi không thể sử dụng chúng nữa trong suốt phần còn lại của trận đấu.  
* **Suy giảm Năng lượng:** Từ lượt tiếp theo trở đi, quỹ Cost của đội hình sẽ bị hao hụt do mất đi lượng *AP Contribution* từ nhân vật đã tử trận.

## **4\. GIAI ĐOẠN KẾT THÚC LƯỢT (END OF TURN PHASE)**

* **Kích hoạt Hiệu ứng cuối lượt:** Một số hiệu ứng đặc biệt kích hoạt tại đây (Ví dụ: Thiêu đốt \- Burn lây lan; Khiên ảo \- Shield hết hạn).  
* **Xử lý Hao mòn (Decay):** Giảm số stack của các hiệu ứng DoT trên người mục tiêu.  
* **Chuyển quyền (Turn Switch):** Quyền hành động được chuyển sang cho phe Đối phương.

## 

## **5\. ĐIỀU KIỆN KẾT THÚC TRẬN ĐẤU (WIN / LOSS CONDITIONS)**

* **Chiến thắng (Victory):** Toàn bộ đối phương bị loại bỏ khỏi sân, hoặc hoàn thành mục tiêu màn chơi.  
* **Thất bại (Defeat):** Toàn bộ nhân vật bên phe Người chơi bị hạ gục và biến mất khỏi sơ đồ.

# **CHƯƠNG 4: Công thức tính toán sát thương (Damage formulas)**

Đối với dòng game chiến thuật thẻ bài tất định, có hai trường phái tính toán sát thương chính. Việc lựa chọn phụ thuộc vào quy mô chỉ số nhân vật và trải nghiệm nhẩm tính mà bạn muốn mang lại cho người chơi. **Hãy lựa chọn mục 1 HOẶC 2 để làm công thức tính giáp cho mục tiêu.**

## **1\. HỆ THỐNG CỘNG/TRỪ TRỰC TIẾP (FLAT REDUCTION \- KHUYÊN DÙNG CHO GAME THẺ BÀI)**

Hệ thống này phù hợp với các game có bộ chỉ số nhỏ, giúp người chơi dễ dàng cộng trừ kết quả ngay trong đầu trước khi quyết định tiêu hao điểm Cost.

* **Sát thương gốc (Raw Damage):** Tổng uy lực của đòn đánh trước khi chạm vào giáp địch:  
  *RawDamage \= (CardBaseDamage \+ CharacterATK)x(1 \+ Buff% \- Debuff%)*  
* **Sát thương thực nhận (Actual Damage):** Mức sát thương sau khi bị cản lại bởi chỉ số phòng thủ:  
  *$ActualDamage=\max\limits_{}(1,RawDamage-TargetDEF)$*


## **2\. HỆ THỐNG GIẢM TRỪ THEO PHẦN TRĂM (PERCENTAGE MITIGATION)**

* **Hệ số Phòng thủ (Defense Multiplier):** Quy đổi điểm DEF thành phần trăm giảm trừ sát thương. Điểm $C$ là hằng số cân bằng (ví dụ: $C=100$).  
  *$DEFMultiplier=\frac{C}{C+TargetDEF}$*  
* **Sát thương thực nhận (Actual Damage):**  
  *$ActualDamage=RawDamage\times DEFMultiplier$*


## **3\. QUY TRÌNH TRỪ MÁU THỰC TẾ (DAMAGE PIPELINE)**

Bất kể sử dụng công thức tính toán nào ở trên, khi con số **Actual Damage** được áp dụng lên mục tiêu, nó bắt buộc phải đi qua quy trình ưu tiên sau để tương tác đúng với các cơ chế đã định nghĩa ở các chương trước:

1. **Tương tác Shield (Khiên ảo):** Sát thương được trừ vào lớp Shield trước. Nếu Actual Damage vượt quá mức Shield hiện có, lớp Shield bị vỡ.  
2. **Tương tác HP (Sinh lực):** Phần sát thương dư thừa (Overkill Damage) sau khi phá vỡ Shield sẽ được trừ trực tiếp vào Base HP của nhân vật.  
3. **Khuếch đại nhược điểm (Vulnerability):** Nếu mục tiêu đang dính trạng thái Dễ tổn thương (Vulnerability), phần sát thương chạm vào HP sẽ được nhân thêm hệ số khuếch đại trước khi trừ máu (ví dụ: nhận thêm 50% sát thương):  
   *$FinalHPLoss=OverkillDamage\times 1.5$*  
4. **Sát thương lan/xuyên thấu (AoE Damage):** Sát thương của các mục tiêu phụ chịu đòn Pierce (cùng cột) hoặc Blast (cùng hàng) thường bị suy giảm một nửa so với mục tiêu chính:  
   *$SecondaryDamage=ActualDamage\times 0.5$*  
5. **Ngoại lệ \- Sát thương chuẩn (True Damage / DoT):** Sát thương từ các hiệu ứng hao mòn như Độc (Poison), Chảy máu (Bleed) bỏ qua toàn bộ DEF và Shield, trừ thẳng vào HP theo công thức:  
   *$DoTDamage=StackCount\times BaseDoTValue$*  
   

# **PHẦN 2: THIẾT KẾ NHÂN VẬT VÀ QUÁI**

# **CHƯƠNG 1: Bộ khung cân bằng**

1. **Sinh lực (HP):** Dao động từ 500 (nhân vật máu giấy) đến 2000 (nhân vật chống chịu tối đa). Khi HP tụt về 0, nhân vật bị hạ gục và loại bỏ khỏi bàn cờ.  
2. **Tấn công (ATK):** Dao động từ 20 (hỗ trợ) đến 150 (sát thương chủ lực).  
3. **Phòng thủ (DEF):** Dao động từ 0 đến 100 (tối đa 120). Áp dụng công thức Cộng/Trừ trực tiếp: Sát thương thực nhận \= Sát thương gốc \- DEF (nhưng không thấp hơn 1).  
4. **Năng lượng đóng góp (AP Contribution):** Dao động từ 1 đến 5 điểm mỗi nhân vật/lượt. Hệ thống cộng dồn chỉ số này của các nhân vật trên sân để tạo thành Tổng quỹ năng lượng (Total Turn Cost) chi trả cho kỹ năng và di chuyển. Mức trung bình của đội hình 4 người rơi vào khoảng 6-11 Cost/lượt.

# 

# **CHƯƠNG 2: Thiết kế nhân vật**

## **1\. Paladin Ánh Sáng / Vệ binh Năng lượng (Hỗ trợ / Đỡ đòn)**

Vai trò: Đứng tại Tiền tuyến (Frontline), chuyên hấp thụ sát thương và bơm năng lượng cho đội hình.

1. Chỉ số: HP 1800 \| ATK 20 \| DEF 80 \| AP Contribution 5\.  
2. Bộ Kỹ năng:  
* Đánh thường \- Sóng Xung Kích Ánh Sáng (Cost: 1 \| CD: 0): Đòn Blast (Lan rộng ngang hàng). Đánh trúng 1 mục tiêu ở Hàng trước theo luật Khóa Tiền tuyến và lan sang tối đa 2 ô kề bên. Gây sát thương bằng 100% ATK (20 sát thương) lên mục tiêu chính, mục tiêu phụ chịu 10 sát thương.  
* Skill \- Tường Ánh Sáng (Cost: 3 \| CD: 2): Cung cấp Shield (Khiên ảo) bằng 150% DEF (120 Shield) cho 1 đồng minh chỉ định trong 1 lượt. Đi kèm hiệu ứng Cleanse (Giải trừ) 1 trạng thái bất lợi ngẫu nhiên.  
* Ultimate \- Tiên Phong Bất Tử (Cost: 5 \| CD: 4): Kích hoạt trạng thái Taunt (Khiêu khích) và CC Immunity (Miễn khống chế) cho bản thân trong 2 lượt. Đồng thời Heal (Hồi máu) cho toàn đội bằng 10% HP tối đa của Paladin (180 HP).  
* Bị động \- Ánh Sáng Ban Mai: Ở đầu lượt, nếu HP của Paladin \> 80%, tự động tạo một lớp Shield mỏng (giá trị 30\) cho đồng minh có HP hiện tại thấp nhất.

## **2\. Xạ thủ Ám Ảnh / Bóng Ma Rừng Sâu (Sát thương chủ lực)**

Vai trò: Đứng tại Hậu tuyến (Backline), chuyên dồn sát thương và tiêu tốn nhiều tài nguyên.

1. Chỉ số: HP 700 \| ATK 140 \| DEF 10 \| AP Contribution 1\.  
2. Bộ Kỹ năng:  
* Đánh thường \- Mũi Tên Gió (Cost: 2 \| CD: 0): Gây sát thương bằng 100% ATK (140 sát thương) lên 1 mục tiêu ở Hàng trước.  
* Skill \- Mũi Tên Chấn Động (Cost: 3 \| CD: 3): Gây sát thương bằng 80% ATK (112 sát thương). Nếu đòn đánh trừ trực tiếp vào HP địch (vượt qua khiên), mục tiêu bị Stun (Choáng) trong 1 lượt.  
* Ultimate \- Xuyên Tâm Tiễn (Cost: 4 \| CD: 4): Đòn Pierce (Xuyên thấu dọc cột). Gây 150% ATK (210 sát thương) lên mục tiêu ở Hàng trước, đâm xuyên gây 100% ATK (140 sát thương) lên kẻ địch đứng ở Hàng sau trên cùng cột. Đòn đánh này cộng thêm 50 điểm Xuyên giáp.  
* Bị động \- Bước Chân Khinh Linh: Lần Di chuyển chiến thuật (đổi hàng, đổi cột, hoán đổi) đầu tiên trong mỗi lượt của phe ta không tiêu tốn Cost.

# **CHƯƠNG 3: Thiết kế quái**

## **1\. Goblin (Quái thường)**

1. Kích thước & Vị trí: 1x1 ô, thường xuất hiện ở Hàng trước.  
2. Chỉ số: HP 150 \| ATK 40 \| DEF 10\.  
   Nhắm Điểm Yếu (Nội tại): Đòn đánh thường phá vỡ luật Khóa Tiền tuyến, luôn tự động nhắm vào nhân vật có phần trăm HP thấp nhất trên sân. Luật này sẽ bị vô hiệu hóa, ép Goblin đánh vào mục tiêu khác nếu mục tiêu đó đang có trạng thái Taunt (Khiêu khích).  
   

## **2\. Vua Goblin (Boss Tân thủ)**

1. Kích thước & Vị trí: 1x1 ô, vị trí cố định ở Giữa \- Hàng sau (B2).  
2. Chỉ số: HP 800 x 2 Thanh máu (Tổng 1600 HP) \| ATK 90\.

Phase 1 (Thanh máu 1):

* DEF: 30\.  
* Hành vi: Lập tức gọi 3 Goblin lấp đầy Hàng trước khi bắt đầu trận. Mỗi lượt chỉ tung đòn Blast (Lan rộng) vào nhân vật đứng giữa để sát thương lan sang 2 bên.

Phase 2 (Thanh máu 2):

* DEF: 150 (Tăng vọt 120 điểm).  
* Hành vi: Kích hoạt ngay khi thanh máu 1 cạn. Xóa quái cũ, gọi 5 Goblin mới lấp kín bàn cờ. Nhận buff “Uy Quyền Nhà Vua”: Tăng lên 150 DEF và có CC Immunity (Miễn khống chế). Đòn đánh Blast ở Phase này có 30% tỷ lệ gây Stun (Choáng).  
* Điều kiện gỡ Buff: Người chơi phải tiêu diệt toàn bộ 5 Goblin trên sân để xóa Miễn khống chế và giảm DEF của Boss về lại mức 30\.

