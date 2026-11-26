using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public static class CharacterAnimationAutoSetup
{
    private class CharacterConfig
    {
        public string Key;
        public string DisplayName;
        public int BaseHP;
        public int BaseATK;
        public int BaseDEF;
        public int AP;
        public Vector2 ColliderSize;
        public Vector2 ColliderOffset;
    }

    private static readonly CharacterConfig[] Characters = new CharacterConfig[]
    {
        new CharacterConfig
        {
            Key = "Paladin",
            DisplayName = "Hiệp Sĩ Ánh Sáng (Paladin)",
            BaseHP = 1800,
            BaseATK = 20,
            BaseDEF = 80,
            AP = 5,
            ColliderSize = new Vector2(1.2f, 1.8f),
            ColliderOffset = new Vector2(0f, 0.9f)
        },
        new CharacterConfig
        {
            Key = "MagmaGolem",
            DisplayName = "Người Đá Nham Thạch (Magma Golem)",
            BaseHP = 2200,
            BaseATK = 35,
            BaseDEF = 100,
            AP = 4,
            ColliderSize = new Vector2(1.6f, 2.0f),
            ColliderOffset = new Vector2(0f, 1.0f)
        },
        new CharacterConfig
        {
            Key = "Mage",
            DisplayName = "Phù Thủy Tinh Tú (Celestial Mage)",
            BaseHP = 800,
            BaseATK = 95,
            BaseDEF = 10,
            AP = 2,
            ColliderSize = new Vector2(1.0f, 1.6f),
            ColliderOffset = new Vector2(0f, 0.8f)
        },
        new CharacterConfig
        {
            Key = "Assassin",
            DisplayName = "Sát Thủ Bóng Đêm (Shadow Assassin)",
            BaseHP = 1000,
            BaseATK = 110,
            BaseDEF = 20,
            AP = 3,
            ColliderSize = new Vector2(1.1f, 1.5f),
            ColliderOffset = new Vector2(0f, 0.75f)
        }
    };

    [MenuItem("Tools/Generate Character Animations & Prefabs")]
    public static void BuildAllCharacters()
    {
        Debug.Log("[AutoSetup] Bắt đầu tự động tạo Sprites, Animations, Animator và Prefabs...");

        AssetDatabase.Refresh();

        string prefabsFolder = "Assets/Prefabs";
        string animsBaseFolder = "Assets/Animations";

        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Animations");
        EnsureFolder("Assets", "ScriptableObjects");
        EnsureFolder(animsBaseFolder, "Heroes");

        foreach (var config in Characters)
        {
            BuildCharacter(config, prefabsFolder);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[AutoSetup] ĐÃ HOÀN TẤT toàn bộ 4 nhân vật, animations và prefabs!");
    }

    private static void BuildCharacter(CharacterConfig config, string prefabsFolder)
    {
        string spriteDir = $"Assets/Sprites/Heroes/{config.Key}";
        if (!Directory.Exists(spriteDir))
        {
            Debug.LogWarning($"[AutoSetup] Không tìm thấy thư mục: {spriteDir}");
            return;
        }

        string[] rawFiles = Directory.GetFiles(spriteDir, $"{config.Key}_*.png");
        List<string> frameFiles = new List<string>();
        foreach (var f in rawFiles)
        {
            string clean = f.Replace('\\', '/');
            if (!clean.EndsWith("_Sheet.png") && !clean.EndsWith("_FullBody.png") && !clean.EndsWith("_Portrait.png"))
            {
                frameFiles.Add(clean);
            }
        }
        frameFiles.Sort();

        // 1. Cấu hình TextureImporter thành Sprite 2D
        foreach (var path in frameFiles)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                bool changed = false;
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    changed = true;
                }
                if (importer.spriteImportMode != SpriteImportMode.Single)
                {
                    importer.spriteImportMode = SpriteImportMode.Single;
                    changed = true;
                }
                if (!importer.alphaIsTransparency)
                {
                    importer.alphaIsTransparency = true;
                    changed = true;
                }
                if (importer.mipmapEnabled)
                {
                    importer.mipmapEnabled = false;
                    changed = true;
                }
                if (importer.filterMode != FilterMode.Bilinear)
                {
                    importer.filterMode = FilterMode.Bilinear;
                    changed = true;
                }
                TextureImporterSettings texSettings = new TextureImporterSettings();
                importer.ReadTextureSettings(texSettings);
                if (texSettings.spriteAlignment != (int)SpriteAlignment.Custom || texSettings.spritePivot != new Vector2(0.5f, 0.1f))
                {
                    texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
                    texSettings.spritePivot = new Vector2(0.5f, 0.1f);
                    importer.SetTextureSettings(texSettings);
                    changed = true;
                }
                if (changed)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        // Cấu hình file SpriteSheet, FullBody và Portrait
        string[] extraTextures = new string[]
        {
            $"{spriteDir}/{config.Key}_Sheet.png",
            $"{spriteDir}/{config.Key}_FullBody.png",
            $"{spriteDir}/{config.Key}_Portrait.png"
        };
        foreach (string extraPath in extraTextures)
        {
            if (File.Exists(extraPath))
            {
                TextureImporter extraImporter = AssetImporter.GetAtPath(extraPath) as TextureImporter;
                if (extraImporter != null)
                {
                    extraImporter.textureType = TextureImporterType.Sprite;
                    extraImporter.alphaIsTransparency = true;
                    extraImporter.mipmapEnabled = false;
                    extraImporter.SaveAndReimport();
                }
            }
        }

        AssetDatabase.Refresh();

        // 2. Load các Sprite frames
        List<Sprite> sprites = new List<Sprite>();
        foreach (var path in frameFiles)
        {
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp != null)
            {
                sprites.Add(sp);
            }
        }

        if (sprites.Count == 0)
        {
            Debug.LogError($"[AutoSetup] Không load được sprite nào cho {config.Key} tại {spriteDir}");
            return;
        }

        // 3. Tạo thư mục Animations
        string charAnimFolder = $"Assets/Animations/Heroes/{config.Key}";
        EnsureFolder("Assets/Animations/Heroes", config.Key);

        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        // 3a. Idle Animation (frames 0 đến 11, loop = true)
        int idleFrames = Mathf.Min(12, sprites.Count);
        AnimationClip idleClip = new AnimationClip { frameRate = 12 };
        ObjectReferenceKeyframe[] idleKeys = new ObjectReferenceKeyframe[idleFrames];
        for (int i = 0; i < idleFrames; i++)
        {
            idleKeys[i] = new ObjectReferenceKeyframe
            {
                time = i / 12f,
                value = sprites[i]
            };
        }
        AnimationUtility.SetObjectReferenceCurve(idleClip, binding, idleKeys);
        var idleSettings = AnimationUtility.GetAnimationClipSettings(idleClip);
        idleSettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(idleClip, idleSettings);

        string idleClipPath = $"{charAnimFolder}/{config.Key}_Idle.anim";
        AssetDatabase.CreateAsset(idleClip, idleClipPath);

        // 3b. Attack Animation (toàn bộ frames, loop = false)
        AnimationClip attackClip = new AnimationClip { frameRate = 15 };
        ObjectReferenceKeyframe[] attackKeys = new ObjectReferenceKeyframe[sprites.Count];
        for (int i = 0; i < sprites.Count; i++)
        {
            attackKeys[i] = new ObjectReferenceKeyframe
            {
                time = i / 15f,
                value = sprites[i]
            };
        }
        AnimationUtility.SetObjectReferenceCurve(attackClip, binding, attackKeys);
        var attackSettings = AnimationUtility.GetAnimationClipSettings(attackClip);
        attackSettings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(attackClip, attackSettings);

        string attackClipPath = $"{charAnimFolder}/{config.Key}_Attack.anim";
        AssetDatabase.CreateAsset(attackClip, attackClipPath);

        // 4. Tạo Animator Controller
        string controllerPath = $"{charAnimFolder}/{config.Key}_Controller.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

        var stateMachine = controller.layers[0].stateMachine;
        var idleState = stateMachine.AddState("Idle");
        idleState.motion = idleClip;
        stateMachine.defaultState = idleState;

        var attackState = stateMachine.AddState("Attack");
        attackState.motion = attackClip;

        // Transition: Idle -> Attack khi trigger "Attack"
        var toAttack = idleState.AddTransition(attackState);
        toAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        toAttack.hasExitTime = false;
        toAttack.duration = 0.05f;

        // Transition: Attack -> Idle khi đòn đánh kết thúc
        var toIdle = attackState.AddTransition(idleState);
        toIdle.hasExitTime = true;
        toIdle.exitTime = 1.0f;
        toIdle.duration = 0.05f;

        // 5. Tạo GameObject & lưu thành Prefab
        GameObject go = new GameObject(config.Key);
        
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprites[0];
        sr.sortingOrder = 5;

        var anim = go.AddComponent<Animator>();
        anim.runtimeAnimatorController = controller;

        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = config.ColliderSize;
        col.offset = config.ColliderOffset;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        var unit = go.AddComponent<BattleUnit>();
        unit.UnitName = config.DisplayName;
        unit.MaxHP = config.BaseHP;
        unit.CurrentHP = config.BaseHP;
        unit.CurrentATK = config.BaseATK;
        unit.CurrentDEF = config.BaseDEF;
        unit.APContribution = config.AP;
        unit.IsPlayerSide = true;

        go.AddComponent<StatusEffectController>();

        if (config.Key == "Paladin")
        {
            go.AddComponent<HeroPaladin>();
        }

        // Cập nhật hoặc tạo CharacterStats ScriptableObject
        string statsPath = $"Assets/ScriptableObjects/{config.Key}_Stats.asset";
        CharacterStats statsAsset = AssetDatabase.LoadAssetAtPath<CharacterStats>(statsPath);
        if (statsAsset == null)
        {
            statsAsset = ScriptableObject.CreateInstance<CharacterStats>();
            AssetDatabase.CreateAsset(statsAsset, statsPath);
        }

        statsAsset.CharacterID = config.Key;
        statsAsset.CharacterName = config.DisplayName;
        statsAsset.BaseHP = config.BaseHP;
        statsAsset.BaseATK = config.BaseATK;
        statsAsset.BaseDEF = config.BaseDEF;
        statsAsset.APContribution = config.AP;
        statsAsset.Portrait = AssetDatabase.LoadAssetAtPath<Sprite>($"{spriteDir}/{config.Key}_Portrait.png");
        statsAsset.FullBody = AssetDatabase.LoadAssetAtPath<Sprite>($"{spriteDir}/{config.Key}_FullBody.png");
        EditorUtility.SetDirty(statsAsset);

        unit.CharacterData = statsAsset;

        string prefabPath = $"{prefabsFolder}/{config.Key}.prefab";
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        statsAsset.CharacterPrefab = savedPrefab;
        EditorUtility.SetDirty(statsAsset);
        GameObject.DestroyImmediate(go);

        Debug.Log($"[AutoSetup] Đã hoàn tất nhân vật: {config.Key} -> Prefab: {prefabPath}");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string full = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(full))
        {
            AssetDatabase.CreateFolder(parent, child);
        }
    }
}
