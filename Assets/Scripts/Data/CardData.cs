using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Antigravity/Data/Card")]
public class CardData : ScriptableObject
{
    [Header("Card Identity")]
    public string CardID;
    public string CardName;
    public CardType CardType;
    public TargetRange TargetRange;

    [Header("Cost & Cooldown")]
    public int CardCost;
    public int Cooldown;

    [Header("Combat Values")]
    [Tooltip("Hệ số sát thương theo % ATK (ví dụ: 1.0 = 100% ATK)")]
    public float DamageMultiplier = 1.0f;
    public int FlatDamage;

    [Tooltip("Hệ số tạo khiên theo % DEF (ví dụ: 0.8 = 80% DEF)")]
    public float ShieldMultiplier;

    [Header("Description")]
    [TextArea(2, 4)]
    public string Description;

    // Giữ thuộc tính tương thích ngược nếu có code cũ gọi ManaCost
    public int ManaCost
    {
        get => CardCost;
        set => CardCost = value;
    }

    /// <summary>
    /// Kích hoạt hiệu ứng của thẻ bài.
    /// </summary>
    public virtual void ActivateEffect(GameObject user = null, GameObject target = null)
    {
        Debug.Log($"[CardData] Kích hoạt thẻ '{CardName}' (ID: {CardID}, Loại: {CardType}, Tầm: {TargetRange}) tốn {CardCost} Cost.");
    }
}
