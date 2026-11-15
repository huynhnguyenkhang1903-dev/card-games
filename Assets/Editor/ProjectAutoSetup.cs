using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ProjectAutoSetup
{
    private const string ScriptableObjectsFolder = "Assets/ScriptableObjects";
    private const string PaladinAssetPath = "Assets/ScriptableObjects/Paladin_Stats.asset";
    private const string GameManagersObjectName = "GameManagers";

    [MenuItem("Tools/Auto Setup Step 3 & 4")]
    public static void RunAutoSetup()
    {
        // 1. Tạo thư mục Assets/ScriptableObjects nếu chưa tồn tại
        if (!AssetDatabase.IsValidFolder(ScriptableObjectsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            Debug.Log($"[AutoSetup] Đã tạo thư mục: {ScriptableObjectsFolder}");
        }
        else if (!Directory.Exists(ScriptableObjectsFolder))
        {
            Directory.CreateDirectory(ScriptableObjectsFolder);
        }

        // 2. Tạo hoặc cập nhật file Paladin_Stats.asset
        CharacterStats paladinStats = AssetDatabase.LoadAssetAtPath<CharacterStats>(PaladinAssetPath);
        if (paladinStats == null)
        {
            paladinStats = ScriptableObject.CreateInstance<CharacterStats>();
            AssetDatabase.CreateAsset(paladinStats, PaladinAssetPath);
            Debug.Log($"[AutoSetup] Đã tạo mới asset: {PaladinAssetPath}");
        }

        paladinStats.CharacterName = "Paladin Ánh Sáng";
        paladinStats.BaseHP = 1800;
        paladinStats.BaseATK = 20;
        paladinStats.BaseDEF = 80;
        paladinStats.APContribution = 5;

        EditorUtility.SetDirty(paladinStats);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 3. Tạo GameObject GameManagers gắn sẵn GridManager và TurnManager trong Scene hiện tại
        GameObject gameManagers = GameObject.Find(GameManagersObjectName);
        if (gameManagers == null)
        {
            gameManagers = new GameObject(GameManagersObjectName);
            Undo.RegisterCreatedObjectUndo(gameManagers, "Create GameManagers");
            Debug.Log($"[AutoSetup] Đã tạo GameObject '{GameManagersObjectName}' trong Scene hiện tại.");
        }

        if (gameManagers.GetComponent<GridManager>() == null)
        {
            Undo.AddComponent<GridManager>(gameManagers);
        }

        if (gameManagers.GetComponent<TurnManager>() == null)
        {
            Undo.AddComponent<TurnManager>(gameManagers);
        }

        Selection.activeGameObject = gameManagers;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("[AutoSetup] Hoàn tất Auto Setup Step 3 & 4 thành công!");
    }
}

