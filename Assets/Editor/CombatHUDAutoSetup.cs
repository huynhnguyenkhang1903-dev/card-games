using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Antigravity.UI;

namespace Antigravity.Editor
{
    public static class CombatHUDAutoSetup
    {
        [MenuItem("Tools/Antigravity/Setup Tactical Combat UI (Figma)")]
        public static void CreateTacticalCombatHUD()
        {
            EnsureEventSystem();
            EnsureGameManagers();

            // Gỡ bỏ giao diện cũ nếu tồn tại
            GameObject oldUI = GameObject.Find("CombatBackgroundButtons");
            if (oldUI != null) Undo.DestroyObjectImmediate(oldUI);
            GameObject oldLevel = GameObject.Find("Level1");
            if (oldLevel != null) Undo.DestroyObjectImmediate(oldLevel);

            // 1. Tạo Canvas chính
            GameObject canvasObj = GameObject.Find("CombatHUDCanvas");
            if (canvasObj != null)
            {
                Undo.DestroyObjectImmediate(canvasObj);
            }

            canvasObj = new GameObject("CombatHUDCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create CombatHUDCanvas");

            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // 2. Main Background Vignette Overlay
            GameObject mainPanel = CreateUIGameObject("CombatMainPanel", canvasObj.transform);
            RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
            StretchFull(mainRect);
            Image mainBg = mainPanel.AddComponent<Image>();
            mainBg.color = new Color(0.05f, 0.07f, 0.10f, 0.95f);

            CombatHUDUI hudUI = mainPanel.AddComponent<CombatHUDUI>();

            // 3. TOP HEADER BAR
            BuildTopHeader(mainPanel.transform, hudUI);

            // 4. ARENA OVERLAYS (Floating Units HUDs: Allies & Enemies)
            BuildArenaHUDs(mainPanel.transform, hudUI);

            // 5. BOTTOM CARDS & CONTROL DOCK (Hand Cards, Tactical Belt, End Turn Button)
            BuildBottomDock(mainPanel.transform, hudUI);

            // 6. Notification Text
            Text notifText = CreateText("NotificationText", mainPanel.transform, "Trận đấu sẵn sàng! Lựa chọn thẻ bài kỹ năng để tung chiêu.", 18, FontStyle.Normal, TextAnchor.MiddleLeft);
            RectTransform notifRect = notifText.GetComponent<RectTransform>();
            notifRect.anchorMin = new Vector2(0.04f, 0.32f);
            notifRect.anchorMax = new Vector2(0.50f, 0.36f);
            notifRect.offsetMin = Vector2.zero;
            notifRect.offsetMax = Vector2.zero;
            notifText.color = new Color(0.8f, 0.9f, 1f, 0.85f);
            hudUI.NotificationText = notifText;

            // Đánh dấu Scene đã thay đổi
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[CombatHUDAutoSetup] ĐÃ TẠO THÀNH CÔNG GIAO DIỆN TACTICAL COMBAT THEO FIGMA!");
        }

        private static void BuildTopHeader(Transform parent, CombatHUDUI hudUI)
        {
            GameObject header = CreateUIGameObject("HeaderBar", parent);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.91f);
            headerRect.anchorMax = new Vector2(1, 1f);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;

            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.09f, 0.11f, 0.16f, 0.9f);

            // Left: Speed & Auto Buttons
            GameObject leftControls = CreateUIGameObject("LeftControls", header.transform);
            RectTransform leftRect = leftControls.GetComponent<RectTransform>();
            leftRect.anchorMin = new Vector2(0.02f, 0.15f);
            leftRect.anchorMax = new Vector2(0.25f, 0.85f);
            leftRect.offsetMin = Vector2.zero;
            leftRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup leftLayout = leftControls.AddComponent<HorizontalLayoutGroup>();
            leftLayout.spacing = 15;
            leftLayout.childForceExpandWidth = false;
            leftLayout.childAlignment = TextAnchor.MiddleLeft;

            Button speedBtn = CreatePillButton("SpeedButton", leftControls.transform, "1X SPEED", new Vector2(110, 42), new Color(0.18f, 0.22f, 0.30f));
            hudUI.SpeedButton = speedBtn;
            hudUI.SpeedText = speedBtn.GetComponentInChildren<Text>();

            Button autoBtn = CreatePillButton("AutoButton", leftControls.transform, "AUTO: OFF", new Vector2(120, 42), new Color(0.18f, 0.22f, 0.30f));
            hudUI.AutoButton = autoBtn;
            hudUI.AutoText = autoBtn.GetComponentInChildren<Text>();

            // Center: Encounter Turn Badge & Action Points Bar
            GameObject centerGroup = CreateUIGameObject("CenterGroup", header.transform);
            RectTransform centerRect = centerGroup.GetComponent<RectTransform>();
            centerRect.anchorMin = new Vector2(0.32f, 0.08f);
            centerRect.anchorMax = new Vector2(0.68f, 0.92f);
            centerRect.offsetMin = Vector2.zero;
            centerRect.offsetMax = Vector2.zero;

            // Turn Badge
            GameObject turnBadge = CreateUIGameObject("TurnBadge", centerGroup.transform);
            RectTransform turnRect = turnBadge.GetComponent<RectTransform>();
            turnRect.anchorMin = new Vector2(0.05f, 0.05f);
            turnRect.anchorMax = new Vector2(0.35f, 0.95f);
            turnRect.offsetMin = Vector2.zero;
            turnRect.offsetMax = Vector2.zero;
            Image turnBg = turnBadge.AddComponent<Image>();
            turnBg.color = new Color(0.14f, 0.16f, 0.22f);

            Text turnTxt = CreateText("TurnText", turnBadge.transform, "ENCOUNTER\nTURN 1", 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            turnTxt.color = new Color(0.95f, 0.82f, 0.38f);
            StretchFull(turnTxt.GetComponent<RectTransform>());
            hudUI.TurnBadgeText = turnTxt;

            // Action Points Section
            GameObject apSection = CreateUIGameObject("APSection", centerGroup.transform);
            RectTransform apSecRect = apSection.GetComponent<RectTransform>();
            apSecRect.anchorMin = new Vector2(0.38f, 0.05f);
            apSecRect.anchorMax = new Vector2(0.95f, 0.95f);
            apSecRect.offsetMin = Vector2.zero;
            apSecRect.offsetMax = Vector2.zero;

            Text apValTxt = CreateText("APValueText", apSection.transform, "8 / 10\nACTION POINTS", 15, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform apTxtRect = apValTxt.GetComponent<RectTransform>();
            apTxtRect.anchorMin = new Vector2(0, 0);
            apTxtRect.anchorMax = new Vector2(0.42f, 1);
            apTxtRect.offsetMin = Vector2.zero;
            apTxtRect.offsetMax = Vector2.zero;
            apValTxt.color = new Color(0.1f, 0.8f, 1f);
            hudUI.APValueText = apValTxt;

            // 10 AP Blocks
            GameObject blocksContainer = CreateUIGameObject("APBlocksContainer", apSection.transform);
            RectTransform blocksRect = blocksContainer.GetComponent<RectTransform>();
            blocksRect.anchorMin = new Vector2(0.44f, 0.25f);
            blocksRect.anchorMax = new Vector2(1f, 0.75f);
            blocksRect.offsetMin = Vector2.zero;
            blocksRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup blocksLayout = blocksContainer.AddComponent<HorizontalLayoutGroup>();
            blocksLayout.spacing = 6;
            blocksLayout.childForceExpandWidth = true;
            blocksLayout.childForceExpandHeight = true;

            hudUI.APBlocks.Clear();
            for (int i = 0; i < 10; i++)
            {
                GameObject block = CreateUIGameObject($"APBlock_{i}", blocksContainer.transform);
                Image blockImg = block.AddComponent<Image>();
                blockImg.color = i < 8 ? new Color(0.05f, 0.75f, 1f, 1f) : new Color(0.2f, 0.25f, 0.35f, 0.4f);
                hudUI.APBlocks.Add(blockImg);
            }

            // Right: Deck Count, Pause & Settings
            GameObject rightControls = CreateUIGameObject("RightControls", header.transform);
            RectTransform rightRect = rightControls.GetComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(0.78f, 0.15f);
            rightRect.anchorMax = new Vector2(0.98f, 0.85f);
            rightRect.offsetMin = Vector2.zero;
            rightRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup rightLayout = rightControls.AddComponent<HorizontalLayoutGroup>();
            rightLayout.spacing = 15;
            rightLayout.childForceExpandWidth = false;
            rightLayout.childAlignment = TextAnchor.MiddleRight;

            GameObject deckObj = CreateUIGameObject("DeckCounter", rightControls.transform);
            RectTransform deckRect = deckObj.GetComponent<RectTransform>();
            deckRect.sizeDelta = new Vector2(100, 40);
            Text deckTxt = CreateText("DeckText", deckObj.transform, "DECK: 18", 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            deckTxt.color = new Color(0.95f, 0.85f, 0.45f);
            StretchFull(deckTxt.GetComponent<RectTransform>());
            hudUI.DeckCountText = deckTxt;

            Button pauseBtn = CreatePillButton("PauseButton", rightControls.transform, "||", new Vector2(45, 42), new Color(0.18f, 0.22f, 0.30f));
            hudUI.PauseButton = pauseBtn;

            Button settingsBtn = CreatePillButton("SettingsButton", rightControls.transform, "⚙", new Vector2(45, 42), new Color(0.18f, 0.22f, 0.30f));
            hudUI.SettingsButton = settingsBtn;
        }

        private static void BuildArenaHUDs(Transform parent, CombatHUDUI hudUI)
        {
            GameObject arenaArea = CreateUIGameObject("ArenaHUDContainer", parent);
            RectTransform arenaRect = arenaArea.GetComponent<RectTransform>();
            arenaRect.anchorMin = new Vector2(0.05f, 0.38f);
            arenaRect.anchorMax = new Vector2(0.95f, 0.88f);
            arenaRect.offsetMin = Vector2.zero;
            arenaRect.offsetMax = Vector2.zero;

            // Allied Side (Left)
            GameObject alliesContainer = CreateUIGameObject("AlliesContainer", arenaArea.transform);
            RectTransform alliesRect = alliesContainer.GetComponent<RectTransform>();
            alliesRect.anchorMin = new Vector2(0.02f, 0f);
            alliesRect.anchorMax = new Vector2(0.45f, 1f);
            alliesRect.offsetMin = Vector2.zero;
            alliesRect.offsetMax = Vector2.zero;

            hudUI.AlliesHUDContainer = alliesContainer.transform;

            // Unit 1: Lyra (Lv.40 Ranger)
            CreateAlliedUnitCard(alliesContainer.transform, new Vector2(0.1f, 0.45f), "Lyra (Lv.40)", "RANGER", 1420, 1420, 0, "FOCUS +20%");
            // Unit 2: Valerius (Lv.42 Tank)
            CreateAlliedUnitCard(alliesContainer.transform, new Vector2(0.55f, 0.45f), "Valerius (Lv.42)", "TANK", 2840, 2840, 650, null);

            // Empty Slot Markers
            CreateEmptySlotMarker(alliesContainer.transform, new Vector2(0.1f, 0.05f), "BACK - BTM");
            CreateEmptySlotMarker(alliesContainer.transform, new Vector2(0.55f, 0.05f), "FRONT - BTM");

            // Enemy Side (Right)
            GameObject enemiesContainer = CreateUIGameObject("EnemiesContainer", arenaArea.transform);
            RectTransform enemiesRect = enemiesContainer.GetComponent<RectTransform>();
            enemiesRect.anchorMin = new Vector2(0.52f, 0f);
            enemiesRect.anchorMax = new Vector2(0.98f, 1f);
            enemiesRect.offsetMin = Vector2.zero;
            enemiesRect.offsetMax = Vector2.zero;

            hudUI.EnemiesHUDContainer = enemiesContainer.transform;

            // Enemy 1: Goblin Raider
            CreateEnemyUnitCard(enemiesContainer.transform, new Vector2(0.15f, 0.68f), "Goblin Raider", 450, 450, "STRIKE", 120, false);
            // Enemy 2: Shieldbearer
            CreateEnemyUnitCard(enemiesContainer.transform, new Vector2(0.15f, 0.36f), "Shieldbearer", 850, 850, "PIERCE [TAUNT]", 0, false);
            // Enemy 3: Goblin Shaman
            CreateEnemyUnitCard(enemiesContainer.transform, new Vector2(0.15f, 0.04f), "Goblin Shaman", 600, 600, "POISON", 40, false);
            // Boss: WARLORD GORZOG
            CreateEnemyUnitCard(enemiesContainer.transform, new Vector2(0.55f, 0.36f), "WARLORD GORZOG", 5200, 6200, "INTENT: DREAD CLEAVE [AOE]", 180, true);
        }

        private static void CreateAlliedUnitCard(Transform parent, Vector2 anchorPos, string name, string role, int hp, int maxHp, int shield, string buff)
        {
            GameObject cardObj = CreateUIGameObject($"Ally_{name}", parent);
            RectTransform rect = cardObj.GetComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos + new Vector2(0.38f, 0.48f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image bg = cardObj.AddComponent<Image>();
            bg.color = new Color(0.11f, 0.14f, 0.20f, 0.85f);

            // Name & Role Header
            Text nameTxt = CreateText("NameText", cardObj.transform, $"{name} {role}", 15, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform nameRect = nameTxt.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.75f);
            nameRect.anchorMax = new Vector2(0.95f, 0.95f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            nameTxt.color = role == "TANK" ? new Color(0.95f, 0.8f, 0.3f) : new Color(0.2f, 0.8f, 1f);

            // HP / Shield Bar
            GameObject hpBar = CreateUIGameObject("HPBar", cardObj.transform);
            RectTransform hpRect = hpBar.GetComponent<RectTransform>();
            hpRect.anchorMin = new Vector2(0.05f, 0.55f);
            hpRect.anchorMax = new Vector2(0.95f, 0.70f);
            hpRect.offsetMin = Vector2.zero;
            hpRect.offsetMax = Vector2.zero;
            Image hpBg = hpBar.AddComponent<Image>();
            hpBg.color = new Color(0.2f, 0.25f, 0.35f);

            GameObject hpFill = CreateUIGameObject("HPFill", hpBar.transform);
            RectTransform fillRect = hpFill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2((float)hp / maxHp, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImg = hpFill.AddComponent<Image>();
            fillImg.color = new Color(0.18f, 0.80f, 0.44f);

            // Shield / Buff Tag
            string tagContent = shield > 0 ? $"+{shield} AEGIS" : buff;
            if (!string.IsNullOrEmpty(tagContent))
            {
                Text tagTxt = CreateText("TagText", cardObj.transform, tagContent, 13, FontStyle.Bold, TextAnchor.MiddleCenter);
                RectTransform tagRect = tagTxt.GetComponent<RectTransform>();
                tagRect.anchorMin = new Vector2(0.05f, 0.35f);
                tagRect.anchorMax = new Vector2(0.95f, 0.52f);
                tagRect.offsetMin = Vector2.zero;
                tagRect.offsetMax = Vector2.zero;
                tagTxt.color = shield > 0 ? new Color(0.2f, 0.85f, 1f) : new Color(0.95f, 0.85f, 0.3f);
            }

            Text hpValTxt = CreateText("HPValue", cardObj.transform, $"{hp:#,##0} / {maxHp:#,##0} HP", 13, FontStyle.Normal, TextAnchor.MiddleCenter);
            RectTransform hpValRect = hpValTxt.GetComponent<RectTransform>();
            hpValRect.anchorMin = new Vector2(0.05f, 0.1f);
            hpValRect.anchorMax = new Vector2(0.95f, 0.32f);
            hpValRect.offsetMin = Vector2.zero;
            hpValRect.offsetMax = Vector2.zero;
        }

        private static void CreateEmptySlotMarker(Transform parent, Vector2 anchorPos, string label)
        {
            GameObject slotObj = CreateUIGameObject($"Slot_{label}", parent);
            RectTransform rect = slotObj.GetComponent<RectTransform>();
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos + new Vector2(0.38f, 0.35f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image bg = slotObj.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.15f, 0.20f, 0.4f);

            Text txt = CreateText("Label", slotObj.transform, $"+ {label}", 14, FontStyle.Normal, TextAnchor.MiddleCenter);
            txt.color = new Color(0.5f, 0.6f, 0.7f, 0.6f);
            StretchFull(txt.GetComponent<RectTransform>());
        }

        private static void CreateEnemyUnitCard(Transform parent, Vector2 anchorPos, string name, int hp, int maxHp, string intent, int damage, bool isBoss)
        {
            GameObject cardObj = CreateUIGameObject($"Enemy_{name}", parent);
            RectTransform rect = cardObj.GetComponent<RectTransform>();
            float width = isBoss ? 0.42f : 0.36f;
            float height = isBoss ? 0.55f : 0.28f;
            rect.anchorMin = anchorPos;
            rect.anchorMax = anchorPos + new Vector2(width, height);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image bg = cardObj.AddComponent<Image>();
            bg.color = isBoss ? new Color(0.18f, 0.10f, 0.12f, 0.95f) : new Color(0.15f, 0.12f, 0.16f, 0.85f);

            // Intent Banner ở trên đầu
            GameObject intentBanner = CreateUIGameObject("IntentBanner", cardObj.transform);
            RectTransform inRect = intentBanner.GetComponent<RectTransform>();
            inRect.anchorMin = new Vector2(0.05f, 0.72f);
            inRect.anchorMax = new Vector2(0.95f, 0.95f);
            inRect.offsetMin = Vector2.zero;
            inRect.offsetMax = Vector2.zero;

            Text intentTxt = CreateText("IntentText", intentBanner.transform, damage > 0 ? $"{intent}  {damage} DMG" : intent, 13, FontStyle.Bold, TextAnchor.MiddleLeft);
            intentTxt.color = new Color(1f, 0.35f, 0.35f);
            StretchFull(intentTxt.GetComponent<RectTransform>());

            // Boss Tag
            if (isBoss)
            {
                Text bossTag = CreateText("BossTag", cardObj.transform, "STAGE BOSS", 12, FontStyle.Bold, TextAnchor.MiddleLeft);
                RectTransform bRect = bossTag.GetComponent<RectTransform>();
                bRect.anchorMin = new Vector2(0.05f, 0.55f);
                bRect.anchorMax = new Vector2(0.95f, 0.70f);
                bRect.offsetMin = Vector2.zero;
                bRect.offsetMax = Vector2.zero;
                bossTag.color = new Color(1f, 0.75f, 0.2f);
            }

            // Name
            Text nameTxt = CreateText("NameText", cardObj.transform, name, 15, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform nRect = nameTxt.GetComponent<RectTransform>();
            nRect.anchorMin = new Vector2(0.05f, isBoss ? 0.40f : 0.42f);
            nRect.anchorMax = new Vector2(0.95f, isBoss ? 0.55f : 0.70f);
            nRect.offsetMin = Vector2.zero;
            nRect.offsetMax = Vector2.zero;
            nameTxt.color = Color.white;

            // HP Bar
            GameObject hpBar = CreateUIGameObject("HPBar", cardObj.transform);
            RectTransform hpRect = hpBar.GetComponent<RectTransform>();
            hpRect.anchorMin = new Vector2(0.05f, isBoss ? 0.22f : 0.15f);
            hpRect.anchorMax = new Vector2(0.95f, isBoss ? 0.35f : 0.35f);
            hpRect.offsetMin = Vector2.zero;
            hpRect.offsetMax = Vector2.zero;
            Image hpBg = hpBar.AddComponent<Image>();
            hpBg.color = new Color(0.25f, 0.2f, 0.25f);

            GameObject hpFill = CreateUIGameObject("HPFill", hpBar.transform);
            RectTransform fillRect = hpFill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2((float)hp / maxHp, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImg = hpFill.AddComponent<Image>();
            fillImg.color = new Color(0.92f, 0.28f, 0.28f);

            Text hpValTxt = CreateText("HPValue", cardObj.transform, $"{hp:#,##0} / {maxHp:#,##0} HP", 12, FontStyle.Normal, TextAnchor.MiddleRight);
            RectTransform hpValRect = hpValTxt.GetComponent<RectTransform>();
            hpValRect.anchorMin = new Vector2(0.05f, 0.02f);
            hpValRect.anchorMax = new Vector2(0.95f, isBoss ? 0.20f : 0.18f);
            hpValRect.offsetMin = Vector2.zero;
            hpValRect.offsetMax = Vector2.zero;
        }

        private static void BuildBottomDock(Transform parent, CombatHUDUI hudUI)
        {
            GameObject bottomDock = CreateUIGameObject("BottomDock", parent);
            RectTransform dockRect = bottomDock.GetComponent<RectTransform>();
            dockRect.anchorMin = new Vector2(0.02f, 0.02f);
            dockRect.anchorMax = new Vector2(0.98f, 0.30f);
            dockRect.offsetMin = Vector2.zero;
            dockRect.offsetMax = Vector2.zero;

            // 1. Hero Tab Badges (VALERIUS • PALADIN & LYRA • RANGER)
            GameObject tabsObj = CreateUIGameObject("HeroTabs", bottomDock.transform);
            RectTransform tabRect = tabsObj.GetComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0.02f, 0.90f);
            tabRect.anchorMax = new Vector2(0.75f, 1f);
            tabRect.offsetMin = Vector2.zero;
            tabRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup tabLayout = tabsObj.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 15;
            tabLayout.childForceExpandWidth = false;

            CreateHeroTab(tabsObj.transform, "VALERIUS • PALADIN", new Color(0.95f, 0.75f, 0.25f));
            CreateHeroTab(tabsObj.transform, "LYRA • RANGER", new Color(0.15f, 0.65f, 0.95f));

            // 2. Hand Cards Row
            GameObject cardsContainer = CreateUIGameObject("HandCardsContainer", bottomDock.transform);
            RectTransform cardsRect = cardsContainer.GetComponent<RectTransform>();
            cardsRect.anchorMin = new Vector2(0.02f, 0.02f);
            cardsRect.anchorMax = new Vector2(0.80f, 0.88f);
            cardsRect.offsetMin = Vector2.zero;
            cardsRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup cardsLayout = cardsContainer.AddComponent<HorizontalLayoutGroup>();
            cardsLayout.spacing = 14;
            cardsLayout.childForceExpandWidth = true;
            cardsLayout.childForceExpandHeight = true;

            hudUI.HandCardsContainer = cardsContainer.transform;
            hudUI.ActiveCards.Clear();

            // Mẫu 6 lá bài kỹ năng theo Figma Screen 3
            var card1 = CreateCombatCard(cardsContainer.transform, "Shield Slam", 2, "SKILL", "Gây 220 ST vật lý. 60% làm CHOÁNG 1 lượt.", false, 0);
            var card2 = CreateCombatCard(cardsContainer.transform, "Holy Bastion", 3, "GUARD", "Tạo 450 Giáp Aegis toàn đội. Xóa 1 hiệu ứng xấu.", false, 0);
            var card3 = CreateCombatCard(cardsContainer.transform, "Solar Wrath", 5, "ULTIMATE", "Thiêu đốt toàn bộ kẻ địch, gây 680 ST thánh quang.", false, 0);
            var card4 = CreateCombatCard(cardsContainer.transform, "Quick Shot", 1, "PIERCE", "Bắn tỉa gây 160 ST xuyên giáp. Bốc thêm 1 lá bài.", false, 0);
            var card5 = CreateCombatCard(cardsContainer.transform, "Poison Volley", 2, "DEBUFF", "Gây 40 Độc/lượt trong 3 lượt lên toàn bộ hàng trước.", true, 1);
            var card6 = CreateCombatCard(cardsContainer.transform, "Rain of Arrows", 4, "ULTIMATE", "Mưa tên gây 520 ST, bỏ qua 50% phòng ngự mục tiêu.", false, 0);

            hudUI.ActiveCards.Add(card1);
            hudUI.ActiveCards.Add(card2);
            hudUI.ActiveCards.Add(card3);
            hudUI.ActiveCards.Add(card4);
            hudUI.ActiveCards.Add(card5);
            hudUI.ActiveCards.Add(card6);

            foreach (var c in hudUI.ActiveCards)
            {
                c.OnClicked = hudUI.OnCardPlayAttempt;
            }

            // 3. Tactical Belt (MANA, PURGE, EVADE, RALLY)
            GameObject beltObj = CreateUIGameObject("TacticalBelt", bottomDock.transform);
            RectTransform beltRect = beltObj.GetComponent<RectTransform>();
            beltRect.anchorMin = new Vector2(0.68f, 0.58f);
            beltRect.anchorMax = new Vector2(0.82f, 0.88f);
            beltRect.offsetMin = Vector2.zero;
            beltRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup beltLayout = beltObj.AddComponent<HorizontalLayoutGroup>();
            beltLayout.spacing = 8;
            beltLayout.childForceExpandWidth = true;
            beltLayout.childForceExpandHeight = true;

            hudUI.ManaBeltButton = CreateBeltButton(beltObj.transform, "MANA", new Color(0.1f, 0.6f, 0.9f));
            hudUI.PurgeBeltButton = CreateBeltButton(beltObj.transform, "PURGE", new Color(0.9f, 0.7f, 0.2f));
            hudUI.EvadeBeltButton = CreateBeltButton(beltObj.transform, "EVADE", new Color(0.3f, 0.8f, 0.5f));
            hudUI.RallyBeltButton = CreateBeltButton(beltObj.transform, "RALLY", new Color(0.8f, 0.3f, 0.3f));

            // 4. End Turn Button (Primary Action)
            GameObject endTurnObj = CreateUIGameObject("EndTurnButton", bottomDock.transform);
            RectTransform endRect = endTurnObj.GetComponent<RectTransform>();
            endRect.anchorMin = new Vector2(0.84f, 0.05f);
            endRect.anchorMax = new Vector2(0.98f, 0.88f);
            endRect.offsetMin = Vector2.zero;
            endRect.offsetMax = Vector2.zero;

            Image endBorder = endTurnObj.AddComponent<Image>();
            endBorder.color = new Color(0.95f, 0.77f, 0.25f); // Viền vàng kim loại

            Button endBtn = endTurnObj.AddComponent<Button>();
            ColorBlock colors = endBtn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.9f, 0.6f);
            colors.pressedColor = new Color(0.8f, 0.6f, 0.2f);
            endBtn.colors = colors;
            hudUI.EndTurnButton = endBtn;

            // Inner dark background
            GameObject innerBg = CreateUIGameObject("InnerBackground", endTurnObj.transform);
            RectTransform inRect = innerBg.GetComponent<RectTransform>();
            inRect.anchorMin = new Vector2(0.04f, 0.04f);
            inRect.anchorMax = new Vector2(0.96f, 0.96f);
            inRect.offsetMin = Vector2.zero;
            inRect.offsetMax = Vector2.zero;
            Image innerImg = innerBg.AddComponent<Image>();
            innerImg.color = new Color(0.09f, 0.11f, 0.16f);

            // Icon & Text
            Text iconTxt = CreateText("HourglassIcon", innerBg.transform, "⏳", 36, FontStyle.Normal, TextAnchor.MiddleCenter);
            RectTransform iconRect = iconTxt.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.2f, 0.55f);
            iconRect.anchorMax = new Vector2(0.8f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Text endTxt = CreateText("EndTurnText", innerBg.transform, "END TURN\nHOLD TO PASS", 17, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform endTxtRect = endTxt.GetComponent<RectTransform>();
            endTxtRect.anchorMin = new Vector2(0.05f, 0.05f);
            endTxtRect.anchorMax = new Vector2(0.95f, 0.55f);
            endTxtRect.offsetMin = Vector2.zero;
            endTxtRect.offsetMax = Vector2.zero;
            endTxt.color = new Color(0.95f, 0.77f, 0.25f);
        }

        private static CombatCardUI CreateCombatCard(Transform parent, string title, int cost, string type, string desc, bool inCD, int cdVal)
        {
            GameObject cardObj = CreateUIGameObject($"Card_{title}", parent);
            Image bg = cardObj.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.15f, 0.22f, 0.95f);

            Button btn = cardObj.AddComponent<Button>();
            CombatCardUI cardUI = cardObj.AddComponent<CombatCardUI>();
            cardUI.CardButton = btn;
            cardUI.CurrentCost = cost;
            cardUI.CurrentCD = cdVal;

            // Runtime CardData ảo để test click
            CardData data = ScriptableObject.CreateInstance<CardData>();
            data.CardName = title;
            data.CardCost = cost;
            data.Cooldown = cdVal;
            data.CurrentCooldown = cdVal;
            data.Description = desc;
            if (type == "ULTIMATE") data.CardType = CardType.Ultimate;
            else if (type == "GUARD" || type == "SKILL" || type == "DEBUFF" || type == "PIERCE") data.CardType = CardType.Skill;
            else data.CardType = CardType.BasicAttack;
            cardUI.CardData = data;

            // Cost Diamond Badge (Top Left)
            GameObject diamondObj = CreateUIGameObject("CostDiamond", cardObj.transform);
            RectTransform diaRect = diamondObj.GetComponent<RectTransform>();
            diaRect.anchorMin = new Vector2(0.05f, 0.78f);
            diaRect.anchorMax = new Vector2(0.32f, 0.96f);
            diaRect.offsetMin = Vector2.zero;
            diaRect.offsetMax = Vector2.zero;
            Image diaImg = diamondObj.AddComponent<Image>();
            diaImg.color = new Color(0.1f, 0.65f, 1f);

            Text costTxt = CreateText("CostValue", diamondObj.transform, cost.ToString(), 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            costTxt.color = Color.white;
            StretchFull(costTxt.GetComponent<RectTransform>());
            cardUI.CostText = costTxt;

            // Type Badge (Top Right)
            Text typeTxt = CreateText("TypeBadge", cardObj.transform, type, 12, FontStyle.Bold, TextAnchor.MiddleRight);
            RectTransform typeRect = typeTxt.GetComponent<RectTransform>();
            typeRect.anchorMin = new Vector2(0.35f, 0.80f);
            typeRect.anchorMax = new Vector2(0.92f, 0.96f);
            typeRect.offsetMin = Vector2.zero;
            typeRect.offsetMax = Vector2.zero;
            typeTxt.color = type == "ULTIMATE" ? new Color(0.95f, 0.75f, 0.25f) : new Color(0.3f, 0.85f, 1f);
            cardUI.CardTypeText = typeTxt;

            // Card Name
            Text nameTxt = CreateText("CardName", cardObj.transform, title, 15, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform nameRect = nameTxt.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.08f, 0.58f);
            nameRect.anchorMax = new Vector2(0.92f, 0.76f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            cardUI.CardNameText = nameTxt;

            // Description
            Text descTxt = CreateText("Description", cardObj.transform, desc, 11, FontStyle.Normal, TextAnchor.UpperLeft);
            RectTransform descRect = descTxt.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.08f, 0.08f);
            descRect.anchorMax = new Vector2(0.92f, 0.55f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            descTxt.color = new Color(0.8f, 0.85f, 0.92f);
            cardUI.DescriptionText = descTxt;

            // Cooldown Badge
            GameObject cdBadge = CreateUIGameObject("CDBadge", cardObj.transform);
            RectTransform cdRect = cdBadge.GetComponent<RectTransform>();
            cdRect.anchorMin = new Vector2(0.65f, 0.78f);
            cdRect.anchorMax = new Vector2(0.95f, 0.96f);
            cdRect.offsetMin = Vector2.zero;
            cdRect.offsetMax = Vector2.zero;
            Image cdBg = cdBadge.AddComponent<Image>();
            cdBg.color = new Color(0.85f, 0.25f, 0.25f);
            Text cdTxt = CreateText("CDText", cdBadge.transform, $"CD: {cdVal}T", 11, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(cdTxt.GetComponent<RectTransform>());
            cardUI.CooldownBadge = cdBadge;
            cardUI.CooldownText = cdTxt;
            cdBadge.SetActive(inCD);

            return cardUI;
        }

        private static Button CreateBeltButton(Transform parent, string label, Color color)
        {
            GameObject btnObj = CreateUIGameObject($"Belt_{label}", parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = color;
            Button btn = btnObj.AddComponent<Button>();

            Text txt = CreateText("Label", btnObj.transform, label, 11, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(txt.GetComponent<RectTransform>());
            txt.color = Color.white;
            return btn;
        }

        private static void CreateHeroTab(Transform parent, string label, Color color)
        {
            GameObject tab = CreateUIGameObject($"Tab_{label}", parent);
            RectTransform rect = tab.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 24);
            Image bg = tab.AddComponent<Image>();
            bg.color = new Color(color.r, color.g, color.b, 0.25f);

            Text txt = CreateText("Text", tab.transform, label, 12, FontStyle.Bold, TextAnchor.MiddleCenter);
            txt.color = color;
            StretchFull(txt.GetComponent<RectTransform>());
        }

        private static Button CreatePillButton(string name, Transform parent, string label, Vector2 size, Color bgColor)
        {
            GameObject btnObj = CreateUIGameObject(name, parent);
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            Button btn = btnObj.AddComponent<Button>();
            Text txt = CreateText("Label", btnObj.transform, label, 14, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(txt.GetComponent<RectTransform>());
            return btn;
        }

        private static GameObject CreateUIGameObject(string name, Transform parent)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static Text CreateText(string name, Transform parent, string content, int fontSize, FontStyle style, TextAnchor alignment)
        {
            GameObject obj = CreateUIGameObject(name, parent);
            Text text = obj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null) text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = Color.white;
            return text;
        }

        private static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            }
        }

        private static void EnsureGameManagers()
        {
            GameObject gm = GameObject.Find("GameManagers");
            if (gm == null)
            {
                gm = new GameObject("GameManagers");
                Undo.RegisterCreatedObjectUndo(gm, "Create GameManagers");
            }

            if (gm.GetComponent<GridManager>() == null)
            {
                Undo.AddComponent<GridManager>(gm);
            }

            if (gm.GetComponent<TurnManager>() == null)
            {
                Undo.AddComponent<TurnManager>(gm);
            }
        }
    }
}
