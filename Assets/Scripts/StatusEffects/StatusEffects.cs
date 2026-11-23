using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Stun,           // US-028: Khóa không cho cast bất kỳ thẻ bài nào
    Silence,        // US-029: Chỉ cho phép đánh thường, khóa Skill và Ulti
    Taunt,          // US-030: Khiêu khích, ép mọi đòn đánh đơn phải chĩa vào mình
    Vulnerability,  // US-031: Dễ tổn thương - Nhân hệ số % khi mất HP
    Poison,         // US-032: Độc - Trừ HP chuẩn, bỏ qua Shield vào đầu lượt
    Bleed           // US-033: Chảy máu - Trừ HP chuẩn ngay khi vừa xuất chiêu
}

public class ActiveStatus
{
    public StatusEffectType Type;
    public int Duration;
    public int Stack;
    public float Value; // ví dụ 0.3f = tăng 30% sát thương cho Vulnerability
}

public class StatusEffectController : MonoBehaviour
{
    private BattleUnit unit;
    public List<ActiveStatus> ActiveStatuses { get; private set; } = new List<ActiveStatus>();

    private void Awake()
    {
        unit = GetComponent<BattleUnit>();
    }

    public void AddStatus(StatusEffectType type, int duration, int stack = 1, float value = 0f)
    {
        var existing = ActiveStatuses.Find(s => s.Type == type);
        if (existing != null)
        {
            existing.Duration = Mathf.Max(existing.Duration, duration);
            existing.Stack += stack;
        }
        else
        {
            ActiveStatuses.Add(new ActiveStatus { Type = type, Duration = duration, Stack = stack, Value = value });
        }

        if (type == StatusEffectType.Taunt && unit != null)
        {
            unit.HasTaunt = true;
        }

        Debug.Log($"[Status] {unit?.UnitName} nhận hiệu ứng {type} trong {duration} lượt (Stack: {stack}).");
    }

    /// <summary>
    /// US-028: Kiểm tra bị Stun (không thể dùng bất kỳ bài nào)
    /// </summary>
    public bool IsStunned() => ActiveStatuses.Exists(s => s.Type == StatusEffectType.Stun && s.Duration > 0);

    /// <summary>
    /// US-029: Kiểm tra bị Silence (chỉ cho phép đánh thường, cấm Skill & Ulti)
    /// </summary>
    public bool IsSilenced() => ActiveStatuses.Exists(s => s.Type == StatusEffectType.Silence && s.Duration > 0);

    /// <summary>
    /// US-031: Lấy hệ số tổn thương (Vulnerability)
    /// </summary>
    public float GetVulnerabilityMultiplier()
    {
        var vul = ActiveStatuses.Find(s => s.Type == StatusEffectType.Vulnerability && s.Duration > 0);
        return vul != null ? (1.0f + vul.Value * vul.Stack) : 1.0f;
    }

    /// <summary>
    /// US-032: Kích hoạt sát thương Poison đầu lượt (Bỏ qua Shield)
    /// </summary>
    public void TriggerPoisonTurnStart()
    {
        var poison = ActiveStatuses.Find(s => s.Type == StatusEffectType.Poison && s.Duration > 0);
        if (poison != null && unit != null && !unit.IsDead)
        {
            int damage = 10 * poison.Stack;
            Debug.Log($"[Status] {unit.UnitName} chịu {damage} sát thương Độc (True Damage) đầu lượt.");
            DamagePipeline.ApplyDamage(unit, damage, isTrueDamage: true);
        }
    }

    /// <summary>
    /// US-033: Kích hoạt sát thương Bleed khi xuất chiêu
    /// </summary>
    public void TriggerBleedOnAction()
    {
        var bleed = ActiveStatuses.Find(s => s.Type == StatusEffectType.Bleed && s.Duration > 0);
        if (bleed != null && unit != null && !unit.IsDead)
        {
            int damage = 8 * bleed.Stack;
            Debug.Log($"[Status] {unit.UnitName} chịu {damage} sát thương Chảy máu khi hành động.");
            DamagePipeline.ApplyDamage(unit, damage, isTrueDamage: true);
        }
    }

    /// <summary>
    /// US-034: Hồi phục sinh lực có chặn trần Max HP
    /// </summary>
    public void ApplyHeal(int amount)
    {
        if (unit != null && !unit.IsDead)
        {
            unit.Heal(amount);
        }
    }

    /// <summary>
    /// US-035: Quét và xóa toàn bộ hiệu ứng bất lợi (Cleanse)
    /// </summary>
    public void CleanseAllDebuffs()
    {
        ActiveStatuses.RemoveAll(s => s.Type == StatusEffectType.Stun ||
                                      s.Type == StatusEffectType.Silence ||
                                      s.Type == StatusEffectType.Vulnerability ||
                                      s.Type == StatusEffectType.Poison ||
                                      s.Type == StatusEffectType.Bleed);

        if (unit != null) unit.HasTaunt = false;
        Debug.Log($"[Status] {unit?.UnitName} đã được Thanh Tẩy (Cleanse) sạch mọi hiệu ứng Debuff/CC!");
    }

    public void OnTurnEnd()
    {
        for (int i = ActiveStatuses.Count - 1; i >= 0; i--)
        {
            ActiveStatuses[i].Duration--;
            if (ActiveStatuses[i].Duration <= 0)
            {
                if (ActiveStatuses[i].Type == StatusEffectType.Taunt && unit != null)
                {
                    unit.HasTaunt = false;
                }
                ActiveStatuses.RemoveAt(i);
            }
        }
    }
}
