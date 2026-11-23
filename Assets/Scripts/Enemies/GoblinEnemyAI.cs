using UnityEngine;

public class GoblinEnemyAI : MonoBehaviour
{
    public BattleUnit Unit;

    private void Awake()
    {
        Unit = GetComponent<BattleUnit>();
    }

    /// <summary>
    /// US-046: Cấu hình chỉ số máu giấy cho Goblin Thường
    /// </summary>
    public void SetupGoblin()
    {
        if (Unit != null)
        {
            Unit.UnitName = "Goblin Binh Nhì";
            Unit.MaxHP = 45;
            Unit.CurrentHP = 45;
            Unit.CurrentATK = 14;
            Unit.CurrentDEF = 3;
            Unit.IsPlayerSide = false;
        }
    }

    /// <summary>
    /// US-047: Vòng lặp tự động dò tìm mục tiêu có %HP thấp nhất phe ta để đánh
    /// </summary>
    public void ExecuteAITurn(BattleUnit[] playerUnits)
    {
        if (Unit == null || Unit.IsDead || playerUnits == null || playerUnits.Length == 0) return;

        BattleUnit target = null;
        float minHPRatio = 2.0f;

        foreach (var p in playerUnits)
        {
            if (p != null && !p.IsDead)
            {
                // Ưu tiên Taunt nếu có
                if (p.HasTaunt)
                {
                    target = p;
                    break;
                }

                float ratio = (float)p.CurrentHP / p.MaxHP;
                if (ratio < minHPRatio)
                {
                    minHPRatio = ratio;
                    target = p;
                }
            }
        }

        if (target != null)
        {
            Debug.Log($"[Goblin AI] {Unit.UnitName} nhắm vào mục tiêu máu yếu nhất: {target.UnitName} (%HP: {minHPRatio:P0})");
            DamagePipeline.ApplyDamage(target, Unit.CurrentATK);
        }
    }
}
