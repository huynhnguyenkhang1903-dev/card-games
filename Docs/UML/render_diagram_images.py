import matplotlib.pyplot as plt
import matplotlib.patches as patches
from PIL import Image

def render_us005_class_diagram():
    fig, ax = plt.subplots(figsize=(20, 14), dpi=200)
    ax.set_xlim(0, 2000)
    ax.set_ylim(0, 1400)
    ax.axis('off')

    # Background
    fig.patch.set_facecolor('#F8F9FA')
    ax.set_facecolor('#F8F9FA')

    # Title
    ax.text(1000, 1350, "US-005: SƠ ĐỒ LỚP (UML CLASS DIAGRAM) - THẺ BÀI & TƯỚNG", 
            fontsize=20, fontweight='bold', ha='center', va='center', color='#1A202C',
            bbox=dict(boxstyle='round,pad=0.5', facecolor='#EDF2F7', edgecolor='#CBD5E0', lw=1.5))
    ax.text(1000, 1310, "Antigravity Game Core Architecture • Data-Driven Design (Unity ScriptableObject & MonoBehaviour)", 
            fontsize=12, fontstyle='italic', ha='center', va='center', color='#4A5568')

    def draw_class_box(x, y, w, h, name, stereotype, attrs, ops, header_bg='#E2E8F0', border_color='#4A5568'):
        # Outer border
        rect = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0,rounding_size=6", 
                                      facecolor='white', edgecolor=border_color, linewidth=2)
        ax.add_patch(rect)

        # Header box
        header_h = 50
        header_rect = patches.FancyBboxPatch((x, y + h - header_h), w, header_h, 
                                             boxstyle="round,pad=0,rounding_size=6", 
                                             facecolor=header_bg, edgecolor=border_color, linewidth=1.5)
        ax.add_patch(header_rect)
        
        # Clip line between header and body
        ax.plot([x, x + w], [y + h - header_h, y + h - header_h], color=border_color, lw=1.5)

        # Stereotype & Name
        if stereotype:
            ax.text(x + w/2, y + h - 16, f"«{stereotype}»", fontsize=9, fontstyle='italic', ha='center', va='center', color='#4A5568')
            ax.text(x + w/2, y + h - 35, name, fontsize=12, fontweight='bold', ha='center', va='center', color='#1A202C')
        else:
            ax.text(x + w/2, y + h - 25, name, fontsize=13, fontweight='bold', ha='center', va='center', color='#1A202C')

        # Attributes
        curr_y = y + h - header_h - 18
        for attr in attrs:
            ax.text(x + 15, curr_y, attr, fontsize=9.5, fontfamily='monospace', va='center', color='#2D3748')
            curr_y -= 19

        # Divider if operations
        if ops:
            ax.plot([x, x + w], [curr_y - 5, curr_y - 5], color='#E2E8F0', lw=1)
            curr_y -= 18
            for op in ops:
                ax.text(x + 15, curr_y, op, fontsize=9.5, fontfamily='monospace', va='center', color='#1A365D')
                curr_y -= 19

    # 1. CardData
    card_attrs = [
        "+ CardID: string",
        "+ CardName: string",
        "+ CardType: CardType",
        "+ TargetRange: TargetRange",
        "+ CardCost: int",
        "+ Cooldown: int",
        "+ DamageMultiplier: float = 1.0f",
        "+ FlatDamage: int",
        "+ ShieldMultiplier: float",
        "+ Description: string",
        "+ ManaCost: int { get; set; }"
    ]
    card_ops = [
        "+ ActivateEffect(user, target): void"
    ]
    draw_class_box(60, 830, 430, 370, "CardData", "ScriptableObject", card_attrs, card_ops, '#EBF8FF', '#3182CE')

    # 2. CharacterStats
    char_attrs = [
        "+ CharacterID: string",
        "+ CharacterName: string",
        "+ BaseHP: int",
        "+ BaseATK: int",
        "+ BaseDEF: int",
        "+ APContribution: int",
        "+ Cards: List<CardData> (Size: 3)"
    ]
    draw_class_box(570, 940, 420, 260, "CharacterStats", "ScriptableObject", char_attrs, [], '#FEFCBF', '#D69E2E')

    # 3. EnemyStats
    enemy_attrs = [
        "+ EnemyID: string",
        "+ EnemyName: string",
        "+ IsBoss: bool",
        "+ BaseHP: int",
        "+ MaxHealthBars: int = 1",
        "+ BaseATK: int",
        "+ BaseDEF: int",
        "+ IgnoreFrontlineLock: bool"
    ]
    draw_class_box(1080, 940, 400, 260, "EnemyStats", "ScriptableObject", enemy_attrs, [], '#FED7D7', '#E53E3E')

    # 4. IDamageable (Interface)
    idam_attrs = [
        "+ CurrentHP: int",
        "+ CurrentShield: int"
    ]
    idam_ops = [
        "+ TakeDamage(amount: int): void",
        "+ AddShield(amount: int): void",
        "+ Heal(amount: int): void",
        "+ Die(): void"
    ]
    draw_class_box(1560, 870, 380, 240, "IDamageable", "interface", idam_attrs, idam_ops, '#E6FFFA', '#319795')

    # 5. BattleUnit
    unit_attrs = [
        "+ UnitName: string",
        "+ CharacterData: CharacterStats",
        "+ EnemyData: EnemyStats",
        "+ IsPlayerSide: bool",
        "+ Row: int (0=Frontline, 1=Backline)",
        "+ Column: int (0..2)",
        "+ HasTaunt: bool",
        "+ IsRooted: bool",
        "+ IgnoreFrontlineLock: bool",
        "+ MaxHP: int",
        "- currentHP: int",
        "- currentShield: int",
        "+ CurrentATK: int",
        "+ CurrentDEF: int",
        "+ APContribution: int",
        "+ CurrentHP: int { get; set; }",
        "+ CurrentShield: int { get; set; }",
        "+ IsDead: bool { get; }"
    ]
    unit_ops = [
        "+ Initialize(stats: CharacterStats, isPlayer: bool): void",
        "+ Initialize(stats: EnemyStats, isPlayer: bool): void",
        "+ SetupUnit(name, hp, atk, def, ap, ignoreFront): void",
        "+ TakeDamage(amount: int): void",
        "+ AddShield(amount: int): void",
        "+ Heal(amount: int): void",
        "+ Die(): void"
    ]
    draw_class_box(650, 240, 640, 570, "BattleUnit", "MonoBehaviour", unit_attrs, unit_ops, '#EDF2F7', '#2B6CB0')

    # 6. IStatusEffect (Interface)
    istat_attrs = [
        "+ EffectName: string",
        "+ Duration: int",
        "+ StackCount: int"
    ]
    istat_ops = [
        "+ ApplyEffect(target: GameObject): void",
        "+ RemoveEffect(target: GameObject): void",
        "+ OnTurnStart(): void",
        "+ OnTurnEnd(): void"
    ]
    draw_class_box(1560, 480, 380, 250, "IStatusEffect", "interface", istat_attrs, istat_ops, '#E6FFFA', '#319795')

    # 7. Enums
    draw_class_box(60, 560, 340, 190, "CardType", "enumeration", 
                   ["BasicAttack", "Skill", "Ultimate", "Tactical"], [], '#FAF5FF', '#805AD5')

    draw_class_box(60, 280, 340, 230, "TargetRange", "enumeration", 
                   ["SingleFrontline", "PierceColumn", "BlastRow", "AnyAlly", "AllAllies", "Self"], [], '#FAF5FF', '#805AD5')

    draw_class_box(60, 110, 340, 130, "GridRow", "enumeration", 
                   ["Frontline = 0", "Backline = 1"], [], '#FAF5FF', '#805AD5')

    # Relationships & Connectors
    # CharacterStats -> CardData (Aggregation 1 to 3)
    ax.annotate("", xy=(490, 1070), xytext=(570, 1070),
                arrowprops=dict(arrowstyle="<|-", lw=2, color='#D69E2E', mutation_scale=15))
    ax.text(530, 1085, "o-- 3 Cards", fontsize=10, fontweight='bold', color='#B7791F', ha='center')

    # BattleUnit -> CharacterStats
    ax.annotate("", xy=(780, 940), xytext=(850, 810),
                arrowprops=dict(arrowstyle="->", lw=2, color='#2B6CB0', mutation_scale=15))
    ax.text(820, 875, "references CharacterData", fontsize=9, fontweight='bold', color='#2B6CB0')

    # BattleUnit -> EnemyStats
    ax.annotate("", xy=(1200, 940), xytext=(1100, 810),
                arrowprops=dict(arrowstyle="->", lw=2, color='#E53E3E', mutation_scale=15))
    ax.text(1160, 875, "references EnemyData", fontsize=9, fontweight='bold', color='#E53E3E')

    # BattleUnit ..|> IDamageable (Interface Realization)
    ax.annotate("", xy=(1560, 920), xytext=(1290, 750),
                arrowprops=dict(arrowstyle="-|>", lw=2, color='#319795', linestyle='dashed', mutation_scale=18))
    ax.text(1430, 825, "«implements» IDamageable", fontsize=10, fontweight='bold', color='#234E52')

    # CardData -> CardType
    ax.annotate("", xy=(230, 750), xytext=(230, 830),
                arrowprops=dict(arrowstyle="->", lw=1.5, color='#805AD5', linestyle='dotted', mutation_scale=12))

    # Legend / Note
    ax.text(1600, 150, "GHI CHÚ QUAN HỆ UML:\n"
                       "──◇ Composition / Chứa 3 thẻ CardData\n"
                       "───> Association / Tham chiếu dữ liệu SO\n"
                       "- - -▷ Realization / Hiện thực Interface\n"
                       "···> Dependency / Sử dụng kiểu Enumeration", 
            fontsize=9.5, color='#2D3748', bbox=dict(boxstyle='round,pad=0.8', facecolor='white', edgecolor='#CBD5E0', lw=1.5))

    plt.tight_layout()
    plt.savefig(r'Docs\UML\US-005_Class_Diagram.png', dpi=200, bbox_inches='tight')
    plt.savefig(r'Docs\UML\US-005_Class_Diagram.svg', bbox_inches='tight')
    plt.close()
    print("Exported US-005 Class Diagram to PNG and SVG!")


def render_us006_turn_flow_diagram():
    fig, ax = plt.subplots(figsize=(22, 14), dpi=200)
    ax.set_xlim(0, 2200)
    ax.set_ylim(0, 1400)
    ax.axis('off')

    # Background
    fig.patch.set_facecolor('#F8F9FA')
    ax.set_facecolor('#F8F9FA')

    # Title
    ax.text(1100, 1350, "US-006: SƠ ĐỒ TRẠNG THÁI VÒNG LẶP TRẬN ĐẤU (TURN FLOW STATE MACHINE)", 
            fontsize=20, fontweight='bold', ha='center', va='center', color='#1A202C',
            bbox=dict(boxstyle='round,pad=0.5', facecolor='#EDF2F7', edgecolor='#CBD5E0', lw=1.5))
    ax.text(1100, 1310, "Quản lý vòng lặp theo State Pattern (BattleInitState ➔ PlayerTurnState ➔ EnemyTurnState ➔ BattleEnd)", 
            fontsize=12, fontstyle='italic', ha='center', va='center', color='#4A5568')

    def draw_state_box(x, y, w, h, name, entries=[], exits=[], do_acts=[], bg_color='#EDF2F7', border_color='#4A5568'):
        rect = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0,rounding_size=12", 
                                      facecolor=bg_color, edgecolor=border_color, linewidth=2.5)
        ax.add_patch(rect)
        
        # Header title
        header_h = 45
        ax.plot([x, x + w], [y + h - header_h, y + h - header_h], color=border_color, lw=1.5)
        ax.text(x + w/2, y + h - 23, name, fontsize=13, fontweight='bold', ha='center', va='center', color='#1A202C')

        curr_y = y + h - header_h - 22
        for e in entries:
            ax.text(x + 20, curr_y, f"entry / {e}", fontsize=10, fontfamily='monospace', color='#2B6CB0')
            curr_y -= 22
        for d in do_acts:
            ax.text(x + 20, curr_y, f"do / {d}", fontsize=10, fontfamily='monospace', color='#2F855A')
            curr_y -= 22
        for ex in exits:
            ax.text(x + 20, curr_y, f"exit / {ex}", fontsize=10, fontfamily='monospace', color='#C53030')
            curr_y -= 22

    # 1. Initial State (Black circle)
    init_circle = patches.Circle((100, 1050), 16, facecolor='#1A202C', edgecolor='none')
    ax.add_patch(init_circle)
    ax.text(100, 1080, "Start Battle", fontsize=11, fontweight='bold', ha='center', color='#1A202C')

    # Arrow Init -> BattleInitState
    ax.annotate("", xy=(220, 1050), xytext=(120, 1050),
                arrowprops=dict(arrowstyle="-|>", lw=2, color='#1A202C', mutation_scale=15))

    # 2. BattleInitState
    draw_state_box(220, 940, 480, 210, "BattleInitState", 
                   entries=["GridManager.InitializeGrids(2, 3)", "Deploy Units onto PlayerGrid & EnemyGrid"],
                   do_acts=["Calculate initial team AP Contribution"],
                   exits=["turnManager.TurnCount = 1", "turnManager.ChangeState(PlayerState)"],
                   bg_color='#EBF8FF', border_color='#3182CE')

    # Arrow BattleInitState -> PlayerTurnState
    ax.annotate("", xy=(820, 1050), xytext=(700, 1050),
                arrowprops=dict(arrowstyle="-|>", lw=2.5, color='#3182CE', mutation_scale=18))
    ax.text(760, 1065, "Init Done", fontsize=10, fontweight='bold', color='#2B6CB0', ha='center')

    # 3. PlayerTurnState (Composite Big State)
    outer_player = patches.FancyBboxPatch((820, 560), 720, 680, boxstyle="round,pad=0,rounding_size=16",
                                          facecolor='#FEFCBF', edgecolor='#D69E2E', linewidth=3, alpha=0.5)
    ax.add_patch(outer_player)
    ax.text(1180, 1215, "PLAYER TURN STATE (LƯỢT NGƯỜI CHƠI)", fontsize=14, fontweight='bold', ha='center', color='#744210')

    # Sub-phase 1: Start Turn Phase
    draw_state_box(860, 1010, 640, 160, "1. StartTurnPhase (Pha Đầu Lượt)",
                   entries=["TotalTurnCost = Sum(Living Units APContribution)", 
                            "Giảm Cooldown tất cả thẻ kỹ năng (Cooldown--)",
                            "Trigger hiệu ứng đầu lượt (DoT Poison, Passive Paladin Shield)"],
                   bg_color='white', border_color='#B7791F')

    # Sub-phase 2: Action Phase
    draw_state_box(860, 720, 640, 240, "2. PlayerActionPhase (Pha Hành Động)",
                   do_acts=[
                       "[DÙNG THẺ BÀI] Kiểm tra AP >= CardCost & Frontline Lock/Taunt",
                       "-> Thi triển hiệu ứng thẻ & Trừ máu/khiên qua Damage Pipeline",
                       "-> Khấu trừ CurrentAP -= CardCost",
                       "[ĐỔI Ô / SWAP] Kiểm tra CurrentAP >= 1",
                       "-> Hoán đổi vị trí 2 ô trên lưới & Trừ 1 AP"
                   ],
                   bg_color='white', border_color='#B7791F')

    # Sub-phase 3: End Turn Phase
    draw_state_box(860, 590, 640, 95, "3. PlayerEndTurnPhase (Pha Cuối Lượt)",
                   exits=["Reset toàn bộ Giáp ảo (Shield) tạm thời của phe ta về 0"],
                   bg_color='white', border_color='#B7791F')

    # Internal sub-phase arrows
    ax.annotate("", xy=(1180, 960), xytext=(1180, 1010),
                arrowprops=dict(arrowstyle="-|>", lw=2, color='#B7791F', mutation_scale=14))
    ax.annotate("", xy=(1180, 685), xytext=(1180, 720),
                arrowprops=dict(arrowstyle="-|>", lw=2, color='#B7791F', mutation_scale=14))

    # 4. Victory State (Win)
    draw_state_box(1700, 1040, 420, 160, "VictoryState (Chiến Thắng)",
                   entries=["Tất cả quái vật và Boss đã tử trận!", "Mở Victory Screen & Trao thưởng EXP/Vàng"],
                   bg_color='#C6F6D5', border_color='#38A169')
    # Final circle inside Victory
    v_circle = patches.Circle((1910, 1080), 12, facecolor='#22543D', edgecolor='white', lw=2)
    ax.add_patch(v_circle)

    # Arrow PlayerTurn -> VictoryState
    ax.annotate("", xy=(1700, 1120), xytext=(1540, 1120),
                arrowprops=dict(arrowstyle="-|>", lw=2.5, color='#38A169', mutation_scale=18))
    ax.text(1620, 1135, "[AllEnemiesDead]", fontsize=11, fontweight='bold', color='#22543D', ha='center')

    # 5. EnemyTurnState (Lượt Kẻ Địch)
    draw_state_box(820, 150, 720, 290, "ENEMY TURN STATE (LƯỢT KẺ ĐỊCH)",
                   entries=["Kích hoạt hiệu ứng / DoT đầu lượt quái"],
                   do_acts=[
                       "Lần lượt từng quái/Boss chạy AI Pattern:",
                       "-> Goblin: Dò tìm mục tiêu %HP thấp nhất phe ta để tấn công",
                       "-> Boss Vua Goblin: Phase 1 (Nổ Blast + gọi đệ), Phase 2 (+120 DEF)",
                       "-> Áp dụng luật Khóa Tiền Tuyến (Frontline Lock) hoặc Bỏ qua nếu có cờ",
                       "-> Gây sát thương lên Tướng phe ta qua Damage Pipeline"
                   ],
                   exits=["Reset Giáp ảo quái về 0", "TurnCount++ (Tăng biến đếm lượt)"],
                   bg_color='#FED7D7', border_color='#E53E3E')

    # Arrow PlayerTurn -> EnemyTurn
    ax.annotate("", xy=(1120, 440), xytext=(1120, 560),
                arrowprops=dict(arrowstyle="-|>", lw=3, color='#C53030', mutation_scale=18))
    ax.text(1135, 500, "Bấm 'End Turn' / Hết AP", fontsize=11, fontweight='bold', color='#C53030')

    # 6. Defeat State (Lose)
    draw_state_box(1700, 200, 420, 160, "DefeatState (Thất Bại)",
                   entries=["Toàn bộ Tướng phe ta đã tử trận!", "Mở Defeat Screen & Nút Thử lại"],
                   bg_color='#FED7D7', border_color='#9B2C2C')
    # Final circle inside Defeat
    d_circle = patches.Circle((1910, 240), 12, facecolor='#742A2A', edgecolor='white', lw=2)
    ax.add_patch(d_circle)

    # Arrow EnemyTurn -> DefeatState
    ax.annotate("", xy=(1700, 280), xytext=(1540, 280),
                arrowprops=dict(arrowstyle="-|>", lw=2.5, color='#9B2C2C', mutation_scale=18))
    ax.text(1620, 295, "[AllPlayerDead]", fontsize=11, fontweight='bold', color='#742A2A', ha='center')

    # Arrow EnemyTurn -> PlayerTurn (Loop back)
    # Draw nice curved arrow back to PlayerTurn
    ax.annotate("", xy=(820, 850), xytext=(820, 300),
                arrowprops=dict(arrowstyle="-|>", lw=2.5, color='#3182CE', 
                                connectionstyle="arc3,rad=-0.35", mutation_scale=18))
    ax.text(670, 570, "Quái xong lượt\n[Còn Tướng sống]\n➔ Vòng lại Turn mới", 
            fontsize=11, fontweight='bold', color='#2B6CB0', ha='center',
            bbox=dict(boxstyle='round,pad=0.5', facecolor='#EBF8FF', edgecolor='#3182CE', lw=1.5))

    # Notes
    ax.text(100, 350, "CƠ CHẾ QUY TẮC ĐẶC THÙ:\n"
                      "• Total Turn Cost: Đầu mỗi lượt người chơi, hệ thống cộng dồn APContribution\n"
                      "  từ tất cả tướng còn sống trên lưới 2x3 để nạp vào Total Turn Cost.\n"
                      "• Giáp ảo (Shield): Không tồn tại vĩnh viễn, tự động reset về 0 ở cuối mỗi lượt.\n"
                      "• Đổi ô (Swap): Tốn 1 Cost, cho phép di chuyển sang ô trống hoặc đổi chỗ 2 tướng.\n"
                      "• Khóa Tiền Tuyến (Frontline Lock): Cấm đánh vào Hậu Phương (Row 1) nếu Tiền Tuyến\n"
                      "  (Row 0) cùng cột còn sống (trừ sát thương Pierce xuyên thấu hoặc quái có cờ bỏ qua).",
            fontsize=10, color='#2D3748', bbox=dict(boxstyle='round,pad=0.8', facecolor='white', edgecolor='#CBD5E0', lw=1.5))

    plt.tight_layout()
    plt.savefig(r'Docs\UML\US-006_Turn_Flow_State_Machine.png', dpi=200, bbox_inches='tight')
    plt.savefig(r'Docs\UML\US-006_Turn_Flow_State_Machine.svg', bbox_inches='tight')
    plt.close()
    print("Exported US-006 Turn Flow Diagram to PNG and SVG!")

if __name__ == '__main__':
    render_us005_class_diagram()
    render_us006_turn_flow_diagram()
