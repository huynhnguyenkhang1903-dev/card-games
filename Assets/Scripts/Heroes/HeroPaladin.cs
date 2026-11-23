using UnityEngine;

public class HeroPaladin : MonoBehaviour
{
    [Header("Unit Component")]
    public BattleUnit Unit;
    public StatusEffectController StatusController;

    private void Awake()
    {
        Unit = GetComponent<BattleUnit>();
        StatusController = GetComponent<StatusEffectController>();
    }

    /// <summary>
    /// US-036: Paladin cơ bản với AP=5
    /// </summary>
    public void SetupPaladin()
    {
        if (Unit != null)
        {
            Unit.UnitName = "Hiệp Sĩ Ánh Sáng (Paladin)";
            Unit.MaxHP = 220;
            Unit.CurrentHP = 220;
            Unit.CurrentATK = 18;
            Unit.CurrentDEF = 25;
            Unit.APContribution = 5; // AP = 5
        }
    }

    /// <summary>
    /// US-037: Đánh thường (Sóng Xung Kích) - Trigger hitbox Blast (nổ lan ngang)
    /// </summary>
    public void CastNormalAttack(BattleUnit targetCenter)
    {
        if (Unit == null || targetCenter == null) return;
        Debug.Log($"[Paladin] Sóng Xung Kích kích hoạt! Gây sát thương Blast vào ô ({targetCenter.Row}, {targetCenter.Column}) và 2 ô lân cận.");
        DamagePipeline.ApplyDamage(targetCenter, Unit.CurrentATK);
        // Sát thương lan sang 2 bên
        GridManager.Instance?.ApplyBlastDamage(targetCenter.Row, targetCenter.Column, Mathf.RoundToInt(Unit.CurrentATK * 0.6f));
    }

    /// <summary>
    /// US-038: Kỹ năng (Tường Ánh Sáng) - Tạo Shield 150% DEF và gọi Cleanse
    /// </summary>
    public void CastLightWall()
    {
        if (Unit == null) return;
        int shieldAmount = Mathf.RoundToInt(Unit.CurrentDEF * 1.5f);
        Unit.AddShield(shieldAmount);
        StatusController?.CleanseAllDebuffs();
        Debug.Log($"[Paladin] Tường Ánh Sáng tạo {shieldAmount} khiên và thanh tẩy debuff!");
    }

    /// <summary>
    /// US-039: Ultimate (Tiên Phong Bất Tử) - Gắn Taunt và Heal toàn đội
    /// </summary>
    public void CastImmortalVanguard(BattleUnit[] allies)
    {
        if (Unit == null) return;
        StatusController?.AddStatus(StatusEffectType.Taunt, duration: 2);
        Unit.AddShield(Unit.CurrentDEF * 2);

        if (allies != null)
        {
            foreach (var ally in allies)
            {
                if (ally != null && !ally.IsDead)
                {
                    ally.Heal(40);
                }
            }
        }
        Debug.Log("[Paladin] Tiên Phong Bất Tử: Kích hoạt Khiêu khích 2 lượt và hồi 40 HP toàn đội!");
    }

    /// <summary>
    /// US-040: Bị động (Ánh Sáng Ban Mai) - Tìm đồng minh HP thấp nhất và buff Shield đầu lượt
    /// </summary>
    public void TriggerPassiveMorningLight(BattleUnit[] allies)
    {
        if (Unit == null || Unit.IsDead || allies == null) return;

        BattleUnit lowestAlly = null;
        float minRatio = 1.0f;

        foreach (var ally in allies)
        {
            if (ally != null && !ally.IsDead)
            {
                float ratio = (float)ally.CurrentHP / ally.MaxHP;
                if (ratio < minRatio)
                {
                    minRatio = ratio;
                    lowestAlly = ally;
                }
            }
        }

        if (lowestAlly != null)
        {
            int buffShield = Mathf.RoundToInt(Unit.CurrentDEF * 0.8f);
            lowestAlly.AddShield(buffShield);
            Debug.Log($"[Paladin] Bị động Ánh Sáng Ban Mai buff {buffShield} khiên cho {lowestAlly.UnitName}!");
        }
    }
}
