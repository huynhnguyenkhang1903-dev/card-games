using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Antigravity/Data/Enemy")]
public class EnemyStats : ScriptableObject
{
    [Header("Enemy Info")]
    public string EnemyID;
    public string EnemyName;
    public bool IsBoss;

    [Header("Base Attributes")]
    public int BaseHP;
    public int MaxHealthBars = 1;
    public int BaseATK;
    public int BaseDEF;

    [Header("Special Rules")]
    [Tooltip("Bỏ qua luật Khóa Tiền Tuyến (dành cho Goblin thường)")]
    public bool IgnoreFrontlineLock;
}

