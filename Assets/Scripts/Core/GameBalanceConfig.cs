using UnityEngine;

public static class GameBalanceConfig
{
    // US-057: Playtest nội bộ, tùy chỉnh lại sát thương quái cho Ải 1 và Boss
    public const float STAGE_1_MONSTER_DAMAGE_MULTIPLIER = 0.85f; // Giảm 15% để tránh sốc sát thương
    public const int GOBLIN_KING_PHASE1_HP = 300;
    public const int GOBLIN_KING_PHASE2_HP = 450;
    public const int GOBLIN_KING_PHASE2_BONUS_DEF = 120;
    public const int GOBLIN_KING_DEBUFFED_DEF = 30;

    // US-058: Cấu hình Player Settings và Build Android APK
    public const string APP_NAME = "Antigravity - Grid Tactics";
    public const string PACKAGE_NAME = "com.antigravity.gameunity";
    public const string VERSION = "1.0.0";
    public const int BUNDLE_VERSION_CODE = 1;
    public const string TARGET_ARCH = "ARM64";
}
