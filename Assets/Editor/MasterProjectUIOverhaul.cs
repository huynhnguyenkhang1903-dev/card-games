using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Antigravity.Editor
{
    public static class MasterProjectUIOverhaul
    {
        private const string BootScenePath = "Assets/Scenes/BootScene.unity";
        private const string MainMenuScenePath = "Assets/Scenes/MainMenuScene.unity";
        private const string PrepScenePath = "Assets/Scenes/PrepScene.unity";
        private const string CombatScenePath = "Assets/Scenes/CombatScene.unity";

        [MenuItem("Tools/Antigravity/🚀 THAY THẾ TOÀN BỘ GIAO DIỆN (APPLY ALL FIGMA UI)")]
        public static void OverhaulAllScenes()
        {
            if (!EditorUtility.DisplayDialog("Xác nhận thiết kế lại toàn bộ Game", 
                "Hành động này sẽ tự động nâng cấp đồng bộ trọn vẹn 4 Scene của dự án theo chuẩn Dark Fantasy cao cấp:\n\n" +
                "1. BootScene: Màn hình khởi động, Logo thần thoại & thanh tải năng lượng Cyan.\n" +
                "2. MainMenuScene: Sảnh Game / Hero Lobby (Tướng Aurelia toàn thân, Gacha, Navigation).\n" +
                "3. PrepScene: Bàn cờ 2x3, Kho tướng, Tactical Deck Loadout & Nút Start Battle.\n" +
                "4. CombatScene: Trận đấu chiến thuật, 10 Action Points, 6 Thẻ bài kỹ năng & Nút End Turn.\n\n" +
                "Đồng thời đăng ký tự động cả 4 Scene vào Build Settings theo đúng thứ tự luồng game.\n\n" +
                "Bạn có muốn tiếp tục?", 
                "Tiến hành thay thế ngay", "Hủy"))
            {
                return;
            }

            Debug.Log("[MasterUIOverhaul] ================= BẮT ĐẦU NÂNG CẤP TOÀN BỘ 4 SCENE =================");

            // 1. Đăng ký Build Settings
            RegisterBuildSettings();

            // 2. Nâng cấp BootScene
            OverhaulBootScene();

            // 3. Nâng cấp MainMenuScene -> Hero Lobby
            OverhaulMainMenuScene();

            // 4. Nâng cấp PrepScene -> Team Setup
            OverhaulPrepScene();

            // 5. Nâng cấp CombatScene -> Combat HUD
            OverhaulCombatScene();

            // 6. Mở lại BootScene hoặc MainMenuScene cho người dùng
            EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);

            EditorUtility.DisplayDialog("Hoàn tất 100%!", 
                "ĐÃ THIẾT KẾ VÀ ĐỒNG BỘ TOÀN BỘ 4 SCENE THÀNH CÔNG!\n\n" +
                "• BootScene (Khởi động & Loading)\n" +
                "• MainMenuScene (Sảnh Hero Lobby)\n" +
                "• PrepScene (Chuẩn bị đội hình & Bàn cờ)\n" +
                "• CombatScene (Trận đấu chiến thuật)\n\n" +
                "Scene hiện tại đang mở là BootScene. Hãy bấm nút PLAY (▶) để trải nghiệm luồng game hoàn chỉnh từ đầu đến cuối!", 
                "Bắt đầu trải nghiệm");
        }

        private static void RegisterBuildSettings()
        {
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(BootScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(PrepScenePath, true),
                new EditorBuildSettingsScene(CombatScenePath, true)
            };
            EditorBuildSettings.scenes = scenes;
            Debug.Log("[MasterUIOverhaul] Đã đăng ký thành công cả 4 Scene vào EditorBuildSettings.");
        }

        private static void OverhaulBootScene()
        {
            Scene scene = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
            Debug.Log($"[MasterUIOverhaul] [1/4] Đang nâng cấp: {scene.name} (Loading & Splash)...");

            CleanOldGameObject("OldBootCanvas");
            BootSceneAutoSetup.CreateBootSceneUI();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MasterUIOverhaul] -> ĐÃ LƯU BootScene.");
        }

        private static void OverhaulMainMenuScene()
        {
            Scene scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            Debug.Log($"[MasterUIOverhaul] [2/4] Đang nâng cấp: {scene.name} (Hero Lobby Figma)...");

            CleanOldGameObject("MainMenu");
            CleanOldGameObject("Settings");
            HeroLobbyAutoSetup.CreateHeroLobbyUI();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MasterUIOverhaul] -> ĐÃ LƯU MainMenuScene.");
        }

        private static void OverhaulPrepScene()
        {
            Scene scene = EditorSceneManager.OpenScene(PrepScenePath, OpenSceneMode.Single);
            Debug.Log($"[MasterUIOverhaul] [3/4] Đang nâng cấp: {scene.name} (Team Prep Figma)...");

            CleanOldGameObject("LevelPreps");
            CleanOldGameObject("Levels");
            CleanOldGameObject("MenuBackgroundButtons");
            CleanOldGameObject("Shop");
            CleanOldGameObject("BuyAsk");
            TeamSetupAutoSetup.CreateTeamSetupUI();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MasterUIOverhaul] -> ĐÃ LƯU PrepScene.");
        }

        private static void OverhaulCombatScene()
        {
            Scene scene = EditorSceneManager.OpenScene(CombatScenePath, OpenSceneMode.Single);
            Debug.Log($"[MasterUIOverhaul] [4/4] Đang nâng cấp: {scene.name} (Tactical Combat Figma)...");

            CleanOldGameObject("CombatBackgroundButtons");
            CleanOldGameObject("Level1");
            CombatHUDAutoSetup.CreateTacticalCombatHUD();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MasterUIOverhaul] -> ĐÃ LƯU CombatScene.");
        }

        private static void CleanOldGameObject(string name)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                obj.SetActive(false);
                Undo.DestroyObjectImmediate(obj);
                Debug.Log($"[MasterUIOverhaul] Đã gỡ bỏ đối tượng cũ: '{name}'.");
            }
        }
    }
}
