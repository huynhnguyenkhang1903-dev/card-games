using System;
using UnityEngine;

public static class DamagePipeline
{
    public static event Action<BattleUnit, int, DamageType> OnDamageCalculated;
    public static event Action<BattleUnit> OnUnitDeath;
    public static event Action<BattleUnit> OnCardGrayOutRequired;

    public enum DamageType
    {
        Physical,
        Magical,
        TrueDamage
    }

    /// <summary>
    /// US-023: Code thuật toán trừ DEF để tính Actual Damage từ Raw Damage.
    /// Actual Damage = Mathf.Max(1, RawDamage - DEF)
    /// Có tính đến Vulnerability (US-031).
    /// </summary>
    public static int CalculateActualDamage(int rawDamage, int targetDef, float vulnerabilityMultiplier = 1.0f)
    {
        int effectiveDamage = Mathf.Max(1, rawDamage - targetDef);
        int finalDamage = Mathf.RoundToInt(effectiveDamage * vulnerabilityMultiplier);
        return Mathf.Max(1, finalDamage);
    }

    /// <summary>
    /// US-024: Viết logic sát thương trừ vào Shield trước, Overkill mới trừ vào HP.
    /// </summary>
    public static void ApplyDamage(BattleUnit target, int rawDamage, bool isTrueDamage = false, float vulnerabilityMultiplier = 1.0f)
    {
        if (target == null || target.IsDead) return;

        int finalDamage = isTrueDamage ? rawDamage : CalculateActualDamage(rawDamage, target.CurrentDEF, vulnerabilityMultiplier);
        int remainingDamage = finalDamage;

        if (!isTrueDamage && target.CurrentShield > 0)
        {
            int absorbed = Mathf.Min(target.CurrentShield, remainingDamage);
            target.CurrentShield -= absorbed;
            remainingDamage -= absorbed;
            OnDamageCalculated?.Invoke(target, absorbed, DamageType.Physical);
        }

        if (remainingDamage > 0)
        {
            target.CurrentHP -= remainingDamage;
            OnDamageCalculated?.Invoke(target, remainingDamage, isTrueDamage ? DamageType.TrueDamage : DamageType.Physical);
        }

        Debug.Log($"[DamagePipeline] Target: {target.UnitName} | Raw: {rawDamage} | Final: {finalDamage} | HP: {target.CurrentHP}/{target.MaxHP} | Shield: {target.CurrentShield}");

        // US-025 & US-026: Kiểm tra tử trận
        if (target.IsDead)
        {
            HandleUnitDeath(target);
        }
    }

    /// <summary>
    /// US-025: Cài đặt sự kiện Tử trận (Xóa model nhân vật, giải phóng ô lưới).
    /// US-026: Cài đặt cơ chế khóa xám 3 thẻ bài khi chủ nhân tử trận.
    /// </summary>
    public static void HandleUnitDeath(BattleUnit unit)
    {
        if (unit == null) return;

        Debug.Log($"[DamagePipeline] Unit {unit.UnitName} tử trận! Giải phóng ô ({unit.Row}, {unit.Column}).");
        
        // Bắn event để GridManager xóa unit khỏi ô
        OnUnitDeath?.Invoke(unit);

        // Bắn event để UI khóa xám các thẻ bài liên quan
        OnCardGrayOutRequired?.Invoke(unit);

        unit.Die();
    }

    /// <summary>
    /// US-027: Tự động reset lớp Shield (Giáp ảo) về 0 khi kích hoạt pha End Turn.
    /// </summary>
    public static void ResetTurnShield(BattleUnit unit)
    {
        if (unit == null || unit.IsDead) return;
        if (unit.CurrentShield > 0)
        {
            Debug.Log($"[DamagePipeline] End Turn: Reset giáp ảo của {unit.UnitName} từ {unit.CurrentShield} về 0.");
            unit.CurrentShield = 0;
        }
    }
}
