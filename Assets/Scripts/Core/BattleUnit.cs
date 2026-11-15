using UnityEngine;

public class BattleUnit : MonoBehaviour, IDamageable
{
    [Header("Unit Identity")]
    public string UnitName;
    public CharacterStats CharacterData;
    public EnemyStats EnemyData;

    [Header("Grid Coordinates")]
    public bool IsPlayerSide;
    public int Row;
    public int Column;

    [Header("Combat States")]
    public bool HasTaunt;
    public bool IsRooted;
    public bool IgnoreFrontlineLock;

    [Header("Stats & Resources")]
    public int MaxHP = 100;
    [SerializeField] private int currentHP = 100;
    [SerializeField] private int currentShield = 0;
    public int CurrentATK = 10;
    public int CurrentDEF = 10;
    public int APContribution = 0;

    public int CurrentHP
    {
        get => currentHP;
        set => currentHP = Mathf.Clamp(value, 0, MaxHP > 0 ? MaxHP : int.MaxValue);
    }

    public int CurrentShield
    {
        get => currentShield;
        set => currentShield = Mathf.Max(0, value);
    }

    public bool IsDead => CurrentHP <= 0;

    private void Awake()
    {
        if (CharacterData != null)
        {
            Initialize(CharacterData, true);
        }
        else if (EnemyData != null)
        {
            Initialize(EnemyData, false);
        }
    }

    public void Initialize(CharacterStats stats, bool isPlayerSide = true)
    {
        if (stats == null) return;

        CharacterData = stats;
        UnitName = stats.CharacterName;
        IsPlayerSide = isPlayerSide;
        MaxHP = stats.BaseHP;
        CurrentHP = stats.BaseHP;
        CurrentShield = 0;
        CurrentATK = stats.BaseATK;
        CurrentDEF = stats.BaseDEF;
        APContribution = stats.APContribution;
        IgnoreFrontlineLock = false;
    }

    public void Initialize(EnemyStats stats, bool isPlayerSide = false)
    {
        if (stats == null) return;

        EnemyData = stats;
        UnitName = stats.EnemyName;
        IsPlayerSide = isPlayerSide;
        MaxHP = stats.BaseHP;
        CurrentHP = stats.BaseHP;
        CurrentShield = 0;
        CurrentATK = stats.BaseATK;
        CurrentDEF = stats.BaseDEF;
        APContribution = 0;
        IgnoreFrontlineLock = stats.IgnoreFrontlineLock;
    }

    public void SetupUnit(string unitName, int hp, int atk = 20, int def = 10, int ap = 0, bool ignoreFrontline = false)
    {
        UnitName = unitName;
        MaxHP = hp;
        CurrentHP = hp;
        CurrentShield = 0;
        CurrentATK = atk;
        CurrentDEF = def;
        APContribution = ap;
        IgnoreFrontlineLock = ignoreFrontline;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead) return;

        int remainingDamage = amount;

        if (CurrentShield > 0)
        {
            int absorbed = Mathf.Min(CurrentShield, remainingDamage);
            CurrentShield -= absorbed;
            remainingDamage -= absorbed;
        }

        if (remainingDamage > 0)
        {
            CurrentHP -= remainingDamage;
        }

        Debug.Log($"[BattleUnit] '{UnitName}' nhận {amount} sát thương. HP còn: {CurrentHP}/{MaxHP}, Shield: {CurrentShield}.");

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void AddShield(int amount)
    {
        if (amount <= 0 || IsDead) return;
        CurrentShield += amount;
        Debug.Log($"[BattleUnit] '{UnitName}' nhận {amount} khiên. Tổng khiên: {CurrentShield}.");
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        Debug.Log($"[BattleUnit] '{UnitName}' hồi {amount} máu. HP hiện tại: {CurrentHP}/{MaxHP}.");
    }

    public void Die()
    {
        CurrentHP = 0;
        HasTaunt = false;
        Debug.Log($"[BattleUnit] '{UnitName}' tại ({(IsPlayerSide ? "Player" : "Enemy")}, Row {Row}, Col {Column}) đã bị hạ gục!");
    }
}

