using System.Collections.Generic;
using UnityEngine;

public class GoblinKingBossAI : MonoBehaviour
{
    [Header("Unit & Health Bars")]
    public BattleUnit Unit;
    public int Phase = 1;
    public int Phase1MaxHP = 300;
    public int Phase2MaxHP = 450;
    public int CurrentBossHP;

    [Header("Minions Management")]
    public List<BattleUnit> ActiveMinions = new List<BattleUnit>();
    public GameObject GoblinMinionPrefab;

    private void Awake()
    {
        Unit = GetComponent<BattleUnit>();
    }

    /// <summary>
    /// US-048: Tạo Prefab cấu hình 2 thanh máu tách biệt
    /// </summary>
    public void SetupBoss()
    {
        if (Unit != null)
        {
            Unit.UnitName = "Vua Goblin (Boss)";
            Unit.MaxHP = Phase1MaxHP;
            Unit.CurrentHP = Phase1MaxHP;
            Unit.CurrentATK = 28;
            Unit.CurrentDEF = 30;
            Unit.IsPlayerSide = false;
        }
        Phase = 1;
        CurrentBossHP = Phase1MaxHP;
        Debug.Log("[Boss] Vua Goblin xuất hiện với Phase 1: 300 HP!");
    }

    /// <summary>
    /// US-049: Phase 1 - Đánh Blast vào ô giữa và gọi 3 đệ Goblin
    /// </summary>
    public void ExecutePhase1Action(BattleUnit centerTarget)
    {
        if (Unit == null || Unit.IsDead) return;

        Debug.Log("[Boss Phase 1] Vua Goblin đánh Blast ô giữa và triệu hồi 3 đệ Goblin!");
        if (centerTarget != null)
        {
            DamagePipeline.ApplyDamage(centerTarget, Unit.CurrentATK);
            GridManager.Instance?.ApplyBlastDamage(centerTarget.Row, centerTarget.Column, Mathf.RoundToInt(Unit.CurrentATK * 0.7f));
        }

        SummonMinions(3);
    }

    /// <summary>
    /// US-050: Phase 2 - Cộng 120 DEF, gỡ bỏ thanh máu 1, gọi 5 đệ Goblin
    /// </summary>
    public void TriggerPhase2()
    {
        Phase = 2;
        Unit.MaxHP = Phase2MaxHP;
        Unit.CurrentHP = Phase2MaxHP;
        Unit.CurrentDEF += 120; // Siêu Giáp Phase 2
        Debug.Log($"[Boss Phase 2] VUA GOBLIN THỨC TỈNH! Máu mới: {Phase2MaxHP}, Giáp tăng vọt lên: {Unit.CurrentDEF}! Triệu hồi 5 đệ bảo vệ!");

        SummonMinions(5);
    }

    /// <summary>
    /// US-051: Logic gỡ buff Phase 2 - Khi toàn bộ đệ tử bị tiêu diệt thì đưa DEF về 30
    /// </summary>
    public void OnMinionDied(BattleUnit minion)
    {
        ActiveMinions.Remove(minion);
        Debug.Log($"[Boss] Một đệ tử bị hạ gục! Còn lại: {ActiveMinions.Count} đệ.");

        if (Phase == 2 && ActiveMinions.Count == 0)
        {
            Unit.CurrentDEF = 30;
            Debug.Log("[Boss] TOÀN BỘ ĐỆ TỬ ĐÃ CHẾT! Lớp phòng ngự Siêu Giáp bị phá vỡ! DEF Boss giảm về 30!");
        }
    }

    private void SummonMinions(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject($"Goblin_Minion_{i + 1}");
            var minion = obj.AddComponent<BattleUnit>();
            minion.SetupUnit($"Đệ Goblin {i + 1}", hp: 40, atk: 12, def: 2, ap: 0);
            minion.IsPlayerSide = false;
            ActiveMinions.Add(minion);
        }
    }
}
