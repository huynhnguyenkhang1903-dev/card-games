using UnityEngine;

public class HeroSniper : MonoBehaviour
{
    [Header("Unit Component")]
    public BattleUnit Unit;
    public StatusEffectController StatusController;

    [Header("Passive State")]
    public bool HasUsedFreeMoveThisCombat = false;

    private void Awake()
    {
        Unit = GetComponent<BattleUnit>();
        StatusController = GetComponent<StatusEffectController>();
    }

    /// <summary>
    /// US-041: Sniper cơ bản với AP=1
    /// </summary>
    public void SetupSniper()
    {
        if (Unit != null)
        {
            Unit.UnitName = "Xạ Thủ Tinh Anh (Sniper)";
            Unit.MaxHP = 120;
            Unit.CurrentHP = 120;
            Unit.CurrentATK = 35;
            Unit.CurrentDEF = 8;
            Unit.APContribution = 1; // AP = 1
        }
        HasUsedFreeMoveThisCombat = false;
    }

    /// <summary>
    /// US-042: Đánh thường (Mũi Tên Gió) - Gây sát thương vật lý đơn mục tiêu
    /// </summary>
    public void CastWindArrow(BattleUnit target)
    {
        if (Unit == null || target == null) return;
        Debug.Log($"[Sniper] Mũi Tên Gió bắn trúng {target.UnitName}!");
        DamagePipeline.ApplyDamage(target, Unit.CurrentATK);
    }

    /// <summary>
    /// US-043: Kỹ năng (Mũi Tên Chấn Động) - Nếu trừ trực tiếp HP thì áp dụng Stun
    /// </summary>
    public void CastConcussiveArrow(BattleUnit target)
    {
        if (Unit == null || target == null) return;
        int hpBefore = target.CurrentHP;
        DamagePipeline.ApplyDamage(target, Mathf.RoundToInt(Unit.CurrentATK * 1.2f));

        if (target.CurrentHP < hpBefore && !target.IsDead)
        {
            var targetStatus = target.GetComponent<StatusEffectController>();
            targetStatus?.AddStatus(StatusEffectType.Stun, duration: 1);
            Debug.Log($"[Sniper] Mũi Tên Chấn Động xuyên giáp thành công! Gắn Stun lên {target.UnitName}.");
        }
    }

    /// <summary>
    /// US-044: Ultimate (Xuyên Tâm Tiễn) - Trigger hitbox Pierce dọc cột
    /// </summary>
    public void CastPiercingArrow(int column)
    {
        if (Unit == null) return;
        Debug.Log($"[Sniper] Xuyên Tâm Tiễn phóng dọc cột {column}!");
        GridManager.Instance?.ApplyPierceDamage(column, Mathf.RoundToInt(Unit.CurrentATK * 2.0f));
    }

    /// <summary>
    /// US-045: Bị động (Bước Chân Khinh Linh) - Ghi nhận Free Cost cho lần di chuyển đầu tiên
    /// </summary>
    public int GetTacticalMoveCost()
    {
        if (!HasUsedFreeMoveThisCombat)
        {
            HasUsedFreeMoveThisCombat = true;
            Debug.Log("[Sniper] Bước Chân Khinh Linh kích hoạt: Miễn phí lượt di chuyển đầu tiên!");
            return 0;
        }
        return 1;
    }
}
