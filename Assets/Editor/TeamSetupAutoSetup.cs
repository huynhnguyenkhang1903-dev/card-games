using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Antigravity.UI;

namespace Antigravity.Editor
{
    public static class TeamSetupAutoSetup
    {
        private const string ScriptableObjectsFolder = "Assets/ScriptableObjects";
        private const string PaladinAssetPath = "Assets/ScriptableObjects/Paladin_Stats.asset";
        private const string SniperAssetPath = "Assets/ScriptableObjects/Sniper_Stats.asset";
        private const string MageAssetPath = "Assets/ScriptableObjects/Mage_Stats.asset";
        private const string MagmaGolemAssetPath = "Assets/ScriptableObjects/MagmaGolem_Stats.asset";
        private const string AssassinAssetPath = "Assets/ScriptableObjects/Assassin_Stats.asset";

        [MenuItem("Tools/Antigravity/Setup Team Setup UI (US-012)")]
        public static void CreateTeamSetupUI()
        {
            // 1. Tạo ScriptableObjects mẫu nếu chưa có
            EnsureScriptableObjects();

            // 2. Tạo EventSystem
            EnsureEventSystem();

            // Gỡ bỏ các giao diện nguyên mẫu cũ nếu tồn tại
            string[] oldNames = new string[] { "LevelPreps", "Levels", "MenuBackgroundButtons", "Shop", "BuyAsk" };
            foreach (var name in oldNames)
            {
                GameObject oldObj = GameObject.Find(name);
                if (oldObj != null) Undo.DestroyObjectImmediate(oldObj);
            }

            // 3. Tạo Canvas UI chính
            GameObject canvasObj = GameObject.Find("TeamSetupCanvas");
            if (canvasObj != null)
            {
                Undo.DestroyObjectImmediate(canvasObj);
            }

            canvasObj = new GameObject("TeamSetupCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create TeamSetupCanvas");

            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // 4. Panel chính (Main Background)
            GameObject mainPanel = CreateUIGameObject("MainPanel", canvasObj.transform);
            RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
            StretchFull(mainRect);
            Image mainBg = mainPanel.AddComponent<Image>();
            mainBg.color = new Color(0.08f, 0.10f, 0.14f, 0.98f);

            TeamSetupUI setupUI = mainPanel.AddComponent<TeamSetupUI>();

            // 5. Header Bar
            GameObject header = CreateUIGameObject("HeaderBar", mainPanel.transform);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.88f);
            headerRect.anchorMax = new Vector2(1, 1f);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;
            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.12f, 0.15f, 0.22f, 1f);

            Text titleText = CreateText("TitleText", header.transform, "HỆ THỐNG THIẾT LẬP ĐỘI HÌNH (TEAM SETUP)", 28, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.05f, 0.1f);
            titleRect.anchorMax = new Vector2(0.45f, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            setupUI.TitleText = titleText;

            Text deployedText = CreateText("DeployedCountText", header.transform, "Tướng Xuất Trận: 0 / 3", 20, FontStyle.Normal, TextAnchor.MiddleRight);
            RectTransform depRect = deployedText.GetComponent<RectTransform>();
            depRect.anchorMin = new Vector2(0.5f, 0.1f);
            depRect.anchorMax = new Vector2(0.7f, 0.9f);
            depRect.offsetMin = Vector2.zero;
            depRect.offsetMax = Vector2.zero;
            setupUI.DeployedCountText = deployedText;

            Text totalAPText = CreateText("TotalAPText", header.transform, "Tổng Năng Lượng Đóng Góp: 0 AP", 20, FontStyle.Bold, TextAnchor.MiddleRight);
            totalAPText.color = new Color(1f, 0.8f, 0.2f, 1f);
            RectTransform apRect = totalAPText.GetComponent<RectTransform>();
            apRect.anchorMin = new Vector2(0.72f, 0.1f);
            apRect.anchorMax = new Vector2(0.96f, 0.9f);
            apRect.offsetMin = Vector2.zero;
            apRect.offsetMax = Vector2.zero;
            setupUI.TotalAPText = totalAPText;

            // 6. Khu vực Lưới 2x3 Phe Ta (Player Grid Area)
            GameObject gridArea = CreateUIGameObject("PlayerGridArea", mainPanel.transform);
            RectTransform gridAreaRect = gridArea.GetComponent<RectTransform>();
            gridAreaRect.anchorMin = new Vector2(0.03f, 0.18f);
            gridAreaRect.anchorMax = new Vector2(0.58f, 0.85f);
            gridAreaRect.offsetMin = Vector2.zero;
            gridAreaRect.offsetMax = Vector2.zero;
            Image gridAreaBg = gridArea.AddComponent<Image>();
            gridAreaBg.color = new Color(0.12f, 0.14f, 0.18f, 0.9f);

            Text gridLabel = CreateText("GridTitle", gridArea.transform, "SƠ ĐỒ LƯỚI CHIẾN THUẬT PHE TA (2 HÀNG x 3 CỘT)", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform gridLabelRect = gridLabel.GetComponent<RectTransform>();
            gridLabelRect.anchorMin = new Vector2(0, 0.9f);
            gridLabelRect.anchorMax = new Vector2(1, 1f);
            gridLabelRect.offsetMin = Vector2.zero;
            gridLabelRect.offsetMax = Vector2.zero;

            // Container cho 6 ô
            GameObject slotsContainer = CreateUIGameObject("SlotsContainer", gridArea.transform);
            RectTransform slotsRect = slotsContainer.GetComponent<RectTransform>();
            slotsRect.anchorMin = new Vector2(0.04f, 0.05f);
            slotsRect.anchorMax = new Vector2(0.96f, 0.88f);
            slotsRect.offsetMin = Vector2.zero;
            slotsRect.offsetMax = Vector2.zero;

            GridLayoutGroup gridLayout = slotsContainer.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(290, 260);
            gridLayout.spacing = new Vector2(20, 20);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 3;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;

            setupUI.GridSlots.Clear();

            // Tạo 6 ô (Row 0: Frontline col 0,1,2; Row 1: Backline col 0,1,2)
            for (int i = 0; i < 6; i++)
            {
                int r = i / 3;
                int c = i % 3;
                TeamSlotUI slotUI = CreateSlotUI(slotsContainer.transform, r, c);
                setupUI.GridSlots.Add(slotUI);
            }

            // 7. Khu vực Kho Tướng (Roster Area)
            GameObject rosterArea = CreateUIGameObject("RosterArea", mainPanel.transform);
            RectTransform rosterRect = rosterArea.GetComponent<RectTransform>();
            rosterRect.anchorMin = new Vector2(0.61f, 0.18f);
            rosterRect.anchorMax = new Vector2(0.97f, 0.85f);
            rosterRect.offsetMin = Vector2.zero;
            rosterRect.offsetMax = Vector2.zero;
            Image rosterBg = rosterArea.AddComponent<Image>();
            rosterBg.color = new Color(0.12f, 0.14f, 0.18f, 0.9f);

            Text rosterLabel = CreateText("RosterTitle", rosterArea.transform, "KHO TƯỚNG KHẢ DỤNG", 22, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform rosterLabelRect = rosterLabel.GetComponent<RectTransform>();
            rosterLabelRect.anchorMin = new Vector2(0, 0.9f);
            rosterLabelRect.anchorMax = new Vector2(1, 1f);
            rosterLabelRect.offsetMin = Vector2.zero;
            rosterLabelRect.offsetMax = Vector2.zero;

            GameObject rosterList = CreateUIGameObject("RosterList", rosterArea.transform);
            RectTransform rosterListRect = rosterList.GetComponent<RectTransform>();
            rosterListRect.anchorMin = new Vector2(0.04f, 0.05f);
            rosterListRect.anchorMax = new Vector2(0.96f, 0.88f);
            rosterListRect.offsetMin = Vector2.zero;
            rosterListRect.offsetMax = Vector2.zero;

            VerticalLayoutGroup vLayout = rosterList.AddComponent<VerticalLayoutGroup>();
            vLayout.spacing = 15;
            vLayout.childControlHeight = false;
            vLayout.childControlWidth = true;
            vLayout.childForceExpandHeight = false;

            setupUI.RosterContainer = rosterList.transform;

            // 8. Bottom Bar (Control & Notifications)
            GameObject bottomBar = CreateUIGameObject("BottomBar", mainPanel.transform);
            RectTransform bottomRect = bottomBar.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0, 0);
            bottomRect.anchorMax = new Vector2(1, 0.15f);
            bottomRect.offsetMin = Vector2.zero;
            bottomRect.offsetMax = Vector2.zero;
            Image bottomBg = bottomBar.AddComponent<Image>();
            bottomBg.color = new Color(0.10f, 0.12f, 0.18f, 1f);

            Text notifText = CreateText("NotificationText", bottomBar.transform, "Sẵn sàng: Chọn ô trên lưới hoặc click tướng trong kho để xếp đội hình.", 18, FontStyle.Italic, TextAnchor.MiddleLeft);
            RectTransform notifRect = notifText.GetComponent<RectTransform>();
            notifRect.anchorMin = new Vector2(0.04f, 0.2f);
            notifRect.anchorMax = new Vector2(0.48f, 0.8f);
            notifRect.offsetMin = Vector2.zero;
            notifRect.offsetMax = Vector2.zero;
            setupUI.NotificationText = notifText;

            Button autoBtn = CreateButton("AutoDeployButton", bottomBar.transform, "TỰ ĐỘNG XẾP", new Color(0.2f, 0.5f, 0.7f, 1f));
            RectTransform autoRect = autoBtn.GetComponent<RectTransform>();
            autoRect.anchorMin = new Vector2(0.52f, 0.25f);
            autoRect.anchorMax = new Vector2(0.65f, 0.75f);
            autoRect.offsetMin = Vector2.zero;
            autoRect.offsetMax = Vector2.zero;
            setupUI.AutoDeployButton = autoBtn;

            Button clearBtn = CreateButton("ClearAllButton", bottomBar.transform, "XÓA ĐỘI HÌNH", new Color(0.65f, 0.25f, 0.25f, 1f));
            RectTransform clearRect = clearBtn.GetComponent<RectTransform>();
            clearRect.anchorMin = new Vector2(0.67f, 0.25f);
            clearRect.anchorMax = new Vector2(0.80f, 0.75f);
            clearRect.offsetMin = Vector2.zero;
            clearRect.offsetMax = Vector2.zero;
            setupUI.ClearAllButton = clearBtn;

            Button startBtn = CreateButton("StartBattleButton", bottomBar.transform, "VÀO TRẬN ĐẤU", new Color(0.2f, 0.7f, 0.35f, 1f));
            RectTransform startRect = startBtn.GetComponent<RectTransform>();
            startRect.anchorMin = new Vector2(0.82f, 0.25f);
            startRect.anchorMax = new Vector2(0.96f, 0.75f);
            startRect.offsetMin = Vector2.zero;
            startRect.offsetMax = Vector2.zero;
            setupUI.StartBattleButton = startBtn;

            // 9. Nạp danh sách Tướng mẫu vào UI
            setupUI.AvailableCharacters = new List<CharacterStats>();
            CharacterStats paladin = AssetDatabase.LoadAssetAtPath<CharacterStats>(PaladinAssetPath);
            if (paladin != null) setupUI.AvailableCharacters.Add(paladin);

            CharacterStats magmaGolem = AssetDatabase.LoadAssetAtPath<CharacterStats>(MagmaGolemAssetPath);
            if (magmaGolem != null) setupUI.AvailableCharacters.Add(magmaGolem);

            CharacterStats mage = AssetDatabase.LoadAssetAtPath<CharacterStats>(MageAssetPath);
            if (mage != null) setupUI.AvailableCharacters.Add(mage);

            CharacterStats assassin = AssetDatabase.LoadAssetAtPath<CharacterStats>(AssassinAssetPath);
            if (assassin != null) setupUI.AvailableCharacters.Add(assassin);

            CharacterStats sniper = AssetDatabase.LoadAssetAtPath<CharacterStats>(SniperAssetPath);
            if (sniper != null) setupUI.AvailableCharacters.Add(sniper);

            // Tạo Template Card Prefab cho Roster
            GameObject cardPrefab = CreateRosterCardTemplate(mainPanel.transform);
            cardPrefab.SetActive(false);
            setupUI.RosterCardPrefab = cardPrefab;

            Selection.activeGameObject = canvasObj;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[TeamSetupAutoSetup] Hoàn tất khởi tạo toàn bộ giao diện Team Setup (US-012) thành công!");
        }

        private static TeamSlotUI CreateSlotUI(Transform parent, int row, int col)
        {
            GameObject slotObj = CreateUIGameObject($"Slot_{row}_{col}", parent);
            RectTransform rect = slotObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(290, 260);

            Image bg = slotObj.AddComponent<Image>();
            bg.color = new Color(0.18f, 0.22f, 0.28f, 0.95f);

            Button slotBtn = slotObj.AddComponent<Button>();

            TeamSlotUI slotUI = slotObj.AddComponent<TeamSlotUI>();
            slotUI.BackgroundImage = bg;
            slotUI.SlotButton = slotBtn;

            // Border viền
            GameObject border = CreateUIGameObject("Border", slotObj.transform);
            RectTransform borderRect = border.GetComponent<RectTransform>();
            StretchFull(borderRect);
            Image borderImg = border.AddComponent<Image>();
            borderImg.color = (row == 0) ? new Color(0.9f, 0.65f, 0.2f, 0.8f) : new Color(0.35f, 0.7f, 0.95f, 0.8f);
            borderImg.raycastTarget = false;
            slotUI.BorderImage = borderImg;

            // Slot Name Text
            Text nameText = CreateText("SlotName", slotObj.transform, (row == 0 ? "Tiền Tuyến" : "Hậu Phương") + $" (Cột {col+1})", 16, FontStyle.Bold, TextAnchor.UpperCenter);
            RectTransform nameRect = nameText.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.82f);
            nameRect.anchorMax = new Vector2(1, 1f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            slotUI.SlotNameText = nameText;

            // Empty State
            GameObject emptyState = CreateUIGameObject("EmptyState", slotObj.transform);
            RectTransform emptyRect = emptyState.GetComponent<RectTransform>();
            StretchFull(emptyRect);
            Text emptyText = CreateText("EmptyText", emptyState.transform, "+ Ô Trống\n(Click để chọn)", 16, FontStyle.Italic, TextAnchor.MiddleCenter);
            RectTransform etRect = emptyText.GetComponent<RectTransform>();
            StretchFull(etRect);
            slotUI.EmptyStateObject = emptyState;
            slotUI.EmptyPromptText = emptyText;

            // Occupied State
            GameObject occState = CreateUIGameObject("OccupiedState", slotObj.transform);
            RectTransform occRect = occState.GetComponent<RectTransform>();
            StretchFull(occRect);

            Text charName = CreateText("CharName", occState.transform, "Tên Tướng", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform cnRect = charName.GetComponent<RectTransform>();
            cnRect.anchorMin = new Vector2(0, 0.55f);
            cnRect.anchorMax = new Vector2(1, 0.8f);
            cnRect.offsetMin = Vector2.zero;
            cnRect.offsetMax = Vector2.zero;
            slotUI.CharacterNameText = charName;

            Text statsText = CreateText("StatsSummary", occState.transform, "HP: 1000 | ATK: 50 | DEF: 30", 14, FontStyle.Normal, TextAnchor.MiddleCenter);
            RectTransform stRect = statsText.GetComponent<RectTransform>();
            stRect.anchorMin = new Vector2(0.05f, 0.3f);
            stRect.anchorMax = new Vector2(0.95f, 0.55f);
            stRect.offsetMin = Vector2.zero;
            stRect.offsetMax = Vector2.zero;
            slotUI.StatsSummaryText = statsText;

            Text apBadge = CreateText("APBadge", occState.transform, "+5 AP", 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            apBadge.color = new Color(1f, 0.85f, 0.3f, 1f);
            RectTransform apRect = apBadge.GetComponent<RectTransform>();
            apRect.anchorMin = new Vector2(0.3f, 0.1f);
            apRect.anchorMax = new Vector2(0.7f, 0.3f);
            apRect.offsetMin = Vector2.zero;
            apRect.offsetMax = Vector2.zero;
            slotUI.APBadgeText = apBadge;

            Button removeBtn = CreateButton("RemoveBtn", occState.transform, "X", new Color(0.8f, 0.2f, 0.2f, 1f));
            RectTransform rmRect = removeBtn.GetComponent<RectTransform>();
            rmRect.anchorMin = new Vector2(0.85f, 0.8f);
            rmRect.anchorMax = new Vector2(0.98f, 0.95f);
            rmRect.offsetMin = Vector2.zero;
            rmRect.offsetMax = Vector2.zero;
            slotUI.RemoveButton = removeBtn;

            slotUI.OccupiedStateObject = occState;
            occState.SetActive(false);

            slotUI.SetupCoordinate(row, col);
            return slotUI;
        }

        private static GameObject CreateRosterCardTemplate(Transform parent)
        {
            GameObject cardObj = CreateUIGameObject("RosterCardTemplate", parent);
            RectTransform cardRect = cardObj.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(620, 140);

            Image bg = cardObj.AddComponent<Image>();
            bg.color = new Color(0.20f, 0.24f, 0.32f, 1f);

            CharacterRosterCardUI cardUI = cardObj.AddComponent<CharacterRosterCardUI>();
            cardUI.CardBackground = bg;

            // Avatar Portrait
            GameObject avatarObj = CreateUIGameObject("AvatarImage", cardObj.transform);
            RectTransform avRect = avatarObj.GetComponent<RectTransform>();
            avRect.anchorMin = new Vector2(0.02f, 0.1f);
            avRect.anchorMax = new Vector2(0.18f, 0.9f);
            avRect.offsetMin = Vector2.zero;
            avRect.offsetMax = Vector2.zero;
            Image avatarImg = avatarObj.AddComponent<Image>();
            avatarImg.preserveAspect = true;
            cardUI.AvatarImage = avatarImg;

            Text nameText = CreateText("CharName", cardObj.transform, "Paladin Ánh Sáng", 20, FontStyle.Bold, TextAnchor.UpperLeft);
            RectTransform nRect = nameText.GetComponent<RectTransform>();
            nRect.anchorMin = new Vector2(0.20f, 0.65f);
            nRect.anchorMax = new Vector2(0.65f, 0.95f);
            nRect.offsetMin = Vector2.zero;
            nRect.offsetMax = Vector2.zero;
            cardUI.CharacterNameText = nameText;

            Text roleText = CreateText("RoleText", cardObj.transform, "Đỡ Đòn (Tiền Tuyến)", 15, FontStyle.Italic, TextAnchor.UpperLeft);
            roleText.color = new Color(0.8f, 0.85f, 0.9f, 1f);
            RectTransform rRect = roleText.GetComponent<RectTransform>();
            rRect.anchorMin = new Vector2(0.20f, 0.4f);
            rRect.anchorMax = new Vector2(0.65f, 0.65f);
            rRect.offsetMin = Vector2.zero;
            rRect.offsetMax = Vector2.zero;
            cardUI.RoleText = roleText;

            Text statsText = CreateText("StatsText", cardObj.transform, "HP: 1800   ATK: 20   DEF: 80", 15, FontStyle.Normal, TextAnchor.UpperLeft);
            RectTransform sRect = statsText.GetComponent<RectTransform>();
            sRect.anchorMin = new Vector2(0.20f, 0.1f);
            sRect.anchorMax = new Vector2(0.65f, 0.38f);
            sRect.offsetMin = Vector2.zero;
            sRect.offsetMax = Vector2.zero;
            cardUI.HPText = statsText;

            Text apText = CreateText("APText", cardObj.transform, "+5 AP", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
            apText.color = new Color(1f, 0.85f, 0.2f, 1f);
            RectTransform aRect = apText.GetComponent<RectTransform>();
            aRect.anchorMin = new Vector2(0.65f, 0.4f);
            aRect.anchorMax = new Vector2(0.78f, 0.8f);
            aRect.offsetMin = Vector2.zero;
            aRect.offsetMax = Vector2.zero;
            cardUI.APText = apText;

            Button deployBtn = CreateButton("DeployBtn", cardObj.transform, "CHỌN", new Color(0.2f, 0.6f, 0.8f, 1f));
            RectTransform dpRect = deployBtn.GetComponent<RectTransform>();
            dpRect.anchorMin = new Vector2(0.8f, 0.25f);
            dpRect.anchorMax = new Vector2(0.96f, 0.75f);
            dpRect.offsetMin = Vector2.zero;
            dpRect.offsetMax = Vector2.zero;
            cardUI.DeployButton = deployBtn;

            GameObject deployedOverlay = CreateUIGameObject("DeployedOverlay", cardObj.transform);
            RectTransform depRect = deployedOverlay.GetComponent<RectTransform>();
            StretchFull(depRect);
            Image depImg = deployedOverlay.AddComponent<Image>();
            depImg.color = new Color(0.1f, 0.1f, 0.1f, 0.75f);

            Text depBadge = CreateText("DeployedBadge", deployedOverlay.transform, "ĐÃ TRIỂN KHAI", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
            depBadge.color = new Color(0.5f, 0.9f, 0.5f, 1f);
            RectTransform dbRect = depBadge.GetComponent<RectTransform>();
            StretchFull(dbRect);

            cardUI.DeployedOverlay = deployedOverlay;
            cardUI.DeployedBadgeText = depBadge;
            deployedOverlay.SetActive(false);

            return cardObj;
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

        private static Button CreateButton(string name, Transform parent, string label, Color bgColor)
        {
            GameObject btnObj = CreateUIGameObject(name, parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.normalColor = bgColor;
            colors.highlightedColor = bgColor * 1.15f;
            colors.pressedColor = bgColor * 0.85f;
            btn.colors = colors;

            Text txt = CreateText("Label", btnObj.transform, label, 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform txtRect = txt.GetComponent<RectTransform>();
            StretchFull(txtRect);

            return btn;
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

        private static void EnsureScriptableObjects()
        {
            if (!AssetDatabase.IsValidFolder(ScriptableObjectsFolder))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            }

            // 1. Paladin
            CharacterStats paladin = AssetDatabase.LoadAssetAtPath<CharacterStats>(PaladinAssetPath);
            if (paladin == null)
            {
                paladin = ScriptableObject.CreateInstance<CharacterStats>();
                paladin.CharacterID = "HERO_PALADIN";
                paladin.CharacterName = "Paladin Ánh Sáng";
                paladin.BaseHP = 1800;
                paladin.BaseATK = 20;
                paladin.BaseDEF = 80;
                paladin.APContribution = 5;
                AssetDatabase.CreateAsset(paladin, PaladinAssetPath);
            }

            // 2. Sniper
            CharacterStats sniper = AssetDatabase.LoadAssetAtPath<CharacterStats>(SniperAssetPath);
            if (sniper == null)
            {
                sniper = ScriptableObject.CreateInstance<CharacterStats>();
                sniper.CharacterID = "HERO_SNIPER";
                sniper.CharacterName = "Sniper Tinh Anh";
                sniper.BaseHP = 950;
                sniper.BaseATK = 85;
                sniper.BaseDEF = 15;
                sniper.APContribution = 1;
                AssetDatabase.CreateAsset(sniper, SniperAssetPath);
            }

            // 3. Mage
            CharacterStats mage = AssetDatabase.LoadAssetAtPath<CharacterStats>(MageAssetPath);
            if (mage == null)
            {
                mage = ScriptableObject.CreateInstance<CharacterStats>();
                mage.CharacterID = "HERO_MAGE";
                mage.CharacterName = "Phù Thủy Hư Không";
                mage.BaseHP = 800;
                mage.BaseATK = 95;
                mage.BaseDEF = 10;
                mage.APContribution = 2;
                AssetDatabase.CreateAsset(mage, MageAssetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
