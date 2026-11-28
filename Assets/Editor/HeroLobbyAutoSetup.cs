using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Antigravity.UI;

namespace Antigravity.Editor
{
    public static class HeroLobbyAutoSetup
    {
        private const string PaladinFullBodyPath = "Assets/Sprites/Heroes/Paladin/Paladin_FullBody.png";
        private const string PaladinPortraitPath = "Assets/Sprites/Heroes/Paladin/Paladin_Portrait.png";

        [MenuItem("Tools/Antigravity/Setup Hero Lobby UI (Sảnh Game Figma)")]
        public static void CreateHeroLobbyUI()
        {
            EnsureEventSystem();

            // Gỡ bỏ menu cũ nếu tồn tại
            GameObject oldMenu = GameObject.Find("MainMenu");
            if (oldMenu != null) Undo.DestroyObjectImmediate(oldMenu);
            GameObject oldSettings = GameObject.Find("Settings");
            if (oldSettings != null) Undo.DestroyObjectImmediate(oldSettings);

            // 1. Tạo Canvas chính
            GameObject canvasObj = GameObject.Find("HeroLobbyCanvas");
            if (canvasObj != null)
            {
                Undo.DestroyObjectImmediate(canvasObj);
            }

            canvasObj = new GameObject("HeroLobbyCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create HeroLobbyCanvas");

            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // 2. Main Background Panel
            GameObject mainPanel = CreateUIGameObject("LobbyMainPanel", canvasObj.transform);
            RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
            StretchFull(mainRect);
            Image mainBg = mainPanel.AddComponent<Image>();
            mainBg.color = new Color(0.06f, 0.08f, 0.12f, 0.98f);

            HeroLobbyUI lobbyUI = mainPanel.AddComponent<HeroLobbyUI>();

            // 3. Header Bar (Profile + Resources)
            BuildHeaderBar(mainPanel.transform, lobbyUI);

            // 4. Hero Showcase (Left & Center)
            BuildHeroShowcase(mainPanel.transform, lobbyUI);

            // 5. Hero Detail Card (Right Panel)
            BuildHeroDetailCard(mainPanel.transform, lobbyUI);

            // 6. Summon Banner (Bottom Left)
            BuildSummonBanner(mainPanel.transform, lobbyUI);

            // 7. Navigation Bar (Bottom Center)
            BuildNavigationBar(mainPanel.transform, lobbyUI);

            // 8. Enter Campaign Button (Bottom Right)
            BuildCampaignButton(mainPanel.transform, lobbyUI);

            // 9. Feedback Banner
            Text fbTxt = CreateText("SummonFeedbackText", mainPanel.transform, "", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform fbRect = fbTxt.GetComponent<RectTransform>();
            fbRect.anchorMin = new Vector2(0.2f, 0.15f);
            fbRect.anchorMax = new Vector2(0.8f, 0.20f);
            fbRect.offsetMin = Vector2.zero;
            fbRect.offsetMax = Vector2.zero;
            fbTxt.color = new Color(1f, 0.85f, 0.35f);
            lobbyUI.SummonFeedbackText = fbTxt;

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[HeroLobbyAutoSetup] ĐÃ TẠO THÀNH CÔNG SẢNH GAME (HERO LOBBY - FIGMA SCREEN 1)!");
        }

        private static void BuildHeaderBar(Transform parent, HeroLobbyUI lobbyUI)
        {
            GameObject header = CreateUIGameObject("HeaderBar", parent);
            RectTransform headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.91f);
            headerRect.anchorMax = new Vector2(1, 1f);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;
            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.08f, 0.10f, 0.15f, 0.9f);

            // Player Profile (Left)
            GameObject profileObj = CreateUIGameObject("PlayerProfile", header.transform);
            RectTransform profRect = profileObj.GetComponent<RectTransform>();
            profRect.anchorMin = new Vector2(0.02f, 0.1f);
            profRect.anchorMax = new Vector2(0.35f, 0.9f);
            profRect.offsetMin = Vector2.zero;
            profRect.offsetMax = Vector2.zero;

            // Avatar Frame
            GameObject avatarObj = CreateUIGameObject("Avatar", profileObj.transform);
            RectTransform avRect = avatarObj.GetComponent<RectTransform>();
            avRect.anchorMin = new Vector2(0, 0);
            avRect.anchorMax = new Vector2(0.18f, 1);
            avRect.offsetMin = Vector2.zero;
            avRect.offsetMax = Vector2.zero;
            Image avImg = avatarObj.AddComponent<Image>();
            avImg.color = new Color(0.95f, 0.8f, 0.3f);
            Sprite port = AssetDatabase.LoadAssetAtPath<Sprite>(PaladinPortraitPath);
            if (port != null) avImg.sprite = port;

            // Info Texts
            Text nameTxt = CreateText("PlayerName", profileObj.transform, "Aethelgard  Lv. 45", 18, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform nRect = nameTxt.GetComponent<RectTransform>();
            nRect.anchorMin = new Vector2(0.22f, 0.5f);
            nRect.anchorMax = new Vector2(0.95f, 1f);
            nRect.offsetMin = Vector2.zero;
            nRect.offsetMax = Vector2.zero;
            lobbyUI.PlayerNameText = nameTxt;

            Text energyTxt = CreateText("EnergyText", profileObj.transform, "VIP 6   ⚡ 120 / 120", 14, FontStyle.Normal, TextAnchor.MiddleLeft);
            RectTransform eRect = energyTxt.GetComponent<RectTransform>();
            eRect.anchorMin = new Vector2(0.22f, 0f);
            eRect.anchorMax = new Vector2(0.95f, 0.5f);
            eRect.offsetMin = Vector2.zero;
            eRect.offsetMax = Vector2.zero;
            energyTxt.color = new Color(0.3f, 0.85f, 1f);
            lobbyUI.EnergyText = energyTxt;

            // Currencies & Utilities (Right)
            GameObject rightGroup = CreateUIGameObject("CurrenciesGroup", header.transform);
            RectTransform rRect = rightGroup.GetComponent<RectTransform>();
            rRect.anchorMin = new Vector2(0.50f, 0.15f);
            rRect.anchorMax = new Vector2(0.98f, 0.85f);
            rRect.offsetMin = Vector2.zero;
            rRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup rLayout = rightGroup.AddComponent<HorizontalLayoutGroup>();
            rLayout.spacing = 20;
            rLayout.childForceExpandWidth = false;
            rLayout.childAlignment = TextAnchor.MiddleRight;

            lobbyUI.GoldText = CreateResourceBadge(rightGroup.transform, "🟡 1,248,500 +", new Color(0.95f, 0.85f, 0.35f));
            lobbyUI.DiamondText = CreateResourceBadge(rightGroup.transform, "💎 4,850 +", new Color(0.25f, 0.75f, 1f));
            lobbyUI.StaminaText = CreateResourceBadge(rightGroup.transform, "🔴 320", new Color(0.95f, 0.35f, 0.35f));

            CreateIconButton(rightGroup.transform, "✉ 3");
            CreateIconButton(rightGroup.transform, "📢");
            CreateIconButton(rightGroup.transform, "🔊");
            CreateIconButton(rightGroup.transform, "⚙");
        }

        private static void BuildHeroShowcase(Transform parent, HeroLobbyUI lobbyUI)
        {
            GameObject heroArea = CreateUIGameObject("HeroShowcaseArea", parent);
            RectTransform heroRect = heroArea.GetComponent<RectTransform>();
            heroRect.anchorMin = new Vector2(0.04f, 0.20f);
            heroRect.anchorMax = new Vector2(0.55f, 0.88f);
            heroRect.offsetMin = Vector2.zero;
            heroRect.offsetMax = Vector2.zero;

            // Element & Class
            Text classTxt = CreateText("HeroClass", heroArea.transform, "🛡 VANGUARD PALADIN | LIGHT ELEMENT", 15, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform cRect = classTxt.GetComponent<RectTransform>();
            cRect.anchorMin = new Vector2(0.05f, 0.92f);
            cRect.anchorMax = new Vector2(0.95f, 0.98f);
            cRect.offsetMin = Vector2.zero;
            cRect.offsetMax = Vector2.zero;
            classTxt.color = new Color(0.95f, 0.8f, 0.3f);
            lobbyUI.HeroClassText = classTxt;

            // Hero Full Title
            Text nameTxt = CreateText("HeroName", heroArea.transform, "AURELIA THE\nSUNSWORN", 34, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform nRect = nameTxt.GetComponent<RectTransform>();
            nRect.anchorMin = new Vector2(0.05f, 0.76f);
            nRect.anchorMax = new Vector2(0.95f, 0.92f);
            nRect.offsetMin = Vector2.zero;
            nRect.offsetMax = Vector2.zero;
            lobbyUI.HeroNameText = nameTxt;

            // Tier Stars
            Text starTxt = CreateText("HeroStars", heroArea.transform, "★★★★★  LEGENDARY TIER", 16, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform sRect = starTxt.GetComponent<RectTransform>();
            sRect.anchorMin = new Vector2(0.05f, 0.70f);
            sRect.anchorMax = new Vector2(0.95f, 0.76f);
            sRect.offsetMin = Vector2.zero;
            sRect.offsetMax = Vector2.zero;
            starTxt.color = new Color(0.98f, 0.82f, 0.25f);
            lobbyUI.HeroTierText = starTxt;

            // FullBody Hero Artwork Center
            GameObject bodyObj = CreateUIGameObject("HeroArtwork", heroArea.transform);
            RectTransform bRect = bodyObj.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(0.30f, 0f);
            bRect.anchorMax = new Vector2(0.98f, 0.88f);
            bRect.offsetMin = Vector2.zero;
            bRect.offsetMax = Vector2.zero;
            Image bodyImg = bodyObj.AddComponent<Image>();
            bodyImg.preserveAspect = true;

            Sprite fullBody = AssetDatabase.LoadAssetAtPath<Sprite>(PaladinFullBodyPath);
            if (fullBody != null)
            {
                bodyImg.sprite = fullBody;
            }
            else
            {
                bodyImg.color = new Color(0.9f, 0.75f, 0.3f, 0.8f);
            }
            lobbyUI.HeroFullBodyImage = bodyImg;
        }

        private static void BuildHeroDetailCard(Transform parent, HeroLobbyUI lobbyUI)
        {
            GameObject cardObj = CreateUIGameObject("HeroDetailCard", parent);
            RectTransform cardRect = cardObj.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.58f, 0.28f);
            cardRect.anchorMax = new Vector2(0.96f, 0.88f);
            cardRect.offsetMin = Vector2.zero;
            cardRect.offsetMax = Vector2.zero;

            Image bg = cardObj.AddComponent<Image>();
            bg.color = new Color(0.09f, 0.11f, 0.16f, 0.95f);

            // Tabs Header: OVERVIEW | SKILLS | LORE
            GameObject tabs = CreateUIGameObject("Tabs", cardObj.transform);
            RectTransform tabRect = tabs.GetComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0.05f, 0.90f);
            tabRect.anchorMax = new Vector2(0.95f, 0.98f);
            tabRect.offsetMin = Vector2.zero;
            tabRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup tLayout = tabs.AddComponent<HorizontalLayoutGroup>();
            tLayout.spacing = 20;
            tLayout.childForceExpandWidth = false;

            CreateTabButton(tabs.transform, "OVERVIEW", true);
            CreateTabButton(tabs.transform, "SKILLS", false);
            CreateTabButton(tabs.transform, "LORE", false);

            // Core Stats Grid 2x2
            GameObject statsGrid = CreateUIGameObject("StatsGrid", cardObj.transform);
            RectTransform gridRect = statsGrid.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.05f, 0.58f);
            gridRect.anchorMax = new Vector2(0.95f, 0.88f);
            gridRect.offsetMin = Vector2.zero;
            gridRect.offsetMax = Vector2.zero;

            GridLayoutGroup gLayout = statsGrid.AddComponent<GridLayoutGroup>();
            gLayout.cellSize = new Vector2(330, 70);
            gLayout.spacing = new Vector2(15, 12);
            gLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gLayout.constraintCount = 2;

            lobbyUI.HealthValueText = CreateStatBox(statsGrid.transform, "HEALTH", "1,800", "❤", new Color(0.95f, 0.3f, 0.3f));
            lobbyUI.AttackValueText = CreateStatBox(statsGrid.transform, "ATTACK", "20", "⚔", new Color(0.95f, 0.35f, 0.35f));
            lobbyUI.DefenseValueText = CreateStatBox(statsGrid.transform, "DEFENSE", "80", "🛡", new Color(0.2f, 0.7f, 1f));
            lobbyUI.BaseAPValueText = CreateStatBox(statsGrid.transform, "BASE AP", "+5", "⚡", new Color(0.15f, 0.85f, 1f));

            // Secondary Stats Bar
            Text secTxt = CreateText("SecondaryStats", cardObj.transform, "CRIT RATE: 18.5%   |   RESIST: 35.0%   |   SPEED: 104", 13, FontStyle.Normal, TextAnchor.MiddleCenter);
            RectTransform secRect = secTxt.GetComponent<RectTransform>();
            secRect.anchorMin = new Vector2(0.05f, 0.50f);
            secRect.anchorMax = new Vector2(0.95f, 0.56f);
            secRect.offsetMin = Vector2.zero;
            secRect.offsetMax = Vector2.zero;
            secTxt.color = new Color(0.7f, 0.8f, 0.9f);
            lobbyUI.SecondaryStatsText = secTxt;

            // Active Combat Arts Header
            Text artsHeader = CreateText("ArtsHeader", cardObj.transform, "ACTIVE COMBAT ARTS              3/3 Equipped", 14, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform ahRect = artsHeader.GetComponent<RectTransform>();
            ahRect.anchorMin = new Vector2(0.05f, 0.42f);
            ahRect.anchorMax = new Vector2(0.95f, 0.48f);
            ahRect.offsetMin = Vector2.zero;
            ahRect.offsetMax = Vector2.zero;
            artsHeader.color = new Color(0.95f, 0.8f, 0.35f);

            // 3 Combat Cards Container
            GameObject artsContainer = CreateUIGameObject("ArtsContainer", cardObj.transform);
            RectTransform artsRect = artsContainer.GetComponent<RectTransform>();
            artsRect.anchorMin = new Vector2(0.05f, 0.05f);
            artsRect.anchorMax = new Vector2(0.95f, 0.40f);
            artsRect.offsetMin = Vector2.zero;
            artsRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup artsLayout = artsContainer.AddComponent<HorizontalLayoutGroup>();
            artsLayout.spacing = 15;
            artsLayout.childForceExpandWidth = true;
            artsLayout.childForceExpandHeight = true;

            CreateArtCard(artsContainer.transform, "Shield Slam", 2, "Grant 1 Turn");
            CreateArtCard(artsContainer.transform, "Holy Bastion", 3, "Party Aegis");
            CreateArtCard(artsContainer.transform, "Solar Wrath", 5, "Radiant AOE");
        }

        private static void BuildSummonBanner(Transform parent, HeroLobbyUI lobbyUI)
        {
            GameObject summonBox = CreateUIGameObject("SummonBanner", parent);
            RectTransform boxRect = summonBox.GetComponent<RectTransform>();
            boxRect.anchorMin = new Vector2(0.04f, 0.04f);
            boxRect.anchorMax = new Vector2(0.36f, 0.18f);
            boxRect.offsetMin = Vector2.zero;
            boxRect.offsetMax = Vector2.zero;

            Image bg = summonBox.AddComponent<Image>();
            bg.color = new Color(0.10f, 0.12f, 0.18f, 0.9f);

            Text title = CreateText("Title", summonBox.transform, "✦ SOLAR CONVERGENCE    ⏱ 04d 18h 32m", 14, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform tRect = title.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.05f, 0.70f);
            tRect.anchorMax = new Vector2(0.95f, 0.95f);
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;
            title.color = new Color(1f, 0.82f, 0.3f);

            Text desc = CreateText("Desc", summonBox.transform, "Special recruitment rate for Sunsworn Relics. Guaranteed 4★ or higher hero every 10 summons.", 11, FontStyle.Normal, TextAnchor.MiddleLeft);
            RectTransform dRect = desc.GetComponent<RectTransform>();
            dRect.anchorMin = new Vector2(0.05f, 0.42f);
            dRect.anchorMax = new Vector2(0.95f, 0.68f);
            dRect.offsetMin = Vector2.zero;
            dRect.offsetMax = Vector2.zero;
            desc.color = new Color(0.7f, 0.75f, 0.85f);

            // Buttons: Summon x1 & Summon x10
            GameObject btnGroup = CreateUIGameObject("Buttons", summonBox.transform);
            RectTransform bRect = btnGroup.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(0.05f, 0.08f);
            bRect.anchorMax = new Vector2(0.95f, 0.38f);
            bRect.offsetMin = Vector2.zero;
            bRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup bLayout = btnGroup.AddComponent<HorizontalLayoutGroup>();
            bLayout.spacing = 15;
            bLayout.childForceExpandWidth = true;

            lobbyUI.SummonX1Button = CreateButton("SummonX1", btnGroup.transform, "SUMMON X1\n💎 150", new Color(0.20f, 0.25f, 0.35f));
            lobbyUI.SummonX10Button = CreateButton("SummonX10", btnGroup.transform, "SUMMON X10\n💎 1,500", new Color(0.85f, 0.65f, 0.18f));
        }

        private static void BuildNavigationBar(Transform parent, HeroLobbyUI lobbyUI)
        {
            GameObject navBar = CreateUIGameObject("NavigationBar", parent);
            RectTransform navRect = navBar.GetComponent<RectTransform>();
            navRect.anchorMin = new Vector2(0.38f, 0.04f);
            navRect.anchorMax = new Vector2(0.68f, 0.14f);
            navRect.offsetMin = Vector2.zero;
            navRect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup nLayout = navBar.AddComponent<HorizontalLayoutGroup>();
            nLayout.spacing = 12;
            nLayout.childForceExpandWidth = true;

            lobbyUI.HeroesNavButton = CreateNavButton(navBar.transform, "👤\nHEROES", false);
            lobbyUI.CodexNavButton = CreateNavButton(navBar.transform, "📖\nCODEX", false);
            lobbyUI.RecruitNavButton = CreateNavButton(navBar.transform, "✦\nRECRUIT", true);
            lobbyUI.GuildNavButton = CreateNavButton(navBar.transform, "🛡\nGUILD", false);
            lobbyUI.VaultNavButton = CreateNavButton(navBar.transform, "📦\nVAULT", false);
        }

        private static void BuildCampaignButton(Transform parent, HeroLobbyUI lobbyUI)
        {
            GameObject campObj = CreateUIGameObject("EnterCampaignButton", parent);
            RectTransform campRect = campObj.GetComponent<RectTransform>();
            campRect.anchorMin = new Vector2(0.72f, 0.04f);
            campRect.anchorMax = new Vector2(0.96f, 0.16f);
            campRect.offsetMin = Vector2.zero;
            campRect.offsetMax = Vector2.zero;

            Image bg = campObj.AddComponent<Image>();
            bg.color = new Color(0.92f, 0.72f, 0.22f); // Gold button

            Button btn = campObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1f, 0.95f, 0.8f);
            cb.pressedColor = new Color(0.8f, 0.6f, 0.1f);
            btn.colors = cb;
            lobbyUI.EnterCampaignButton = btn;

            Text txt = CreateText("Text", campObj.transform, "CHAPTER IV\n⚔ ENTER CAMPAIGN >", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
            txt.color = new Color(0.1f, 0.1f, 0.12f);
            StretchFull(txt.GetComponent<RectTransform>());
        }

        private static Text CreateStatBox(Transform parent, string label, string value, string icon, Color iconColor)
        {
            GameObject box = CreateUIGameObject($"Stat_{label}", parent);
            Image bg = box.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.17f, 0.24f, 0.9f);

            Text icTxt = CreateText("Icon", box.transform, icon, 24, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform icRect = icTxt.GetComponent<RectTransform>();
            icRect.anchorMin = new Vector2(0.05f, 0.1f);
            icRect.anchorMax = new Vector2(0.25f, 0.9f);
            icRect.offsetMin = Vector2.zero;
            icRect.offsetMax = Vector2.zero;
            icTxt.color = iconColor;

            Text valTxt = CreateText("Value", box.transform, value, 22, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform valRect = valTxt.GetComponent<RectTransform>();
            valRect.anchorMin = new Vector2(0.30f, 0.45f);
            valRect.anchorMax = new Vector2(0.95f, 0.95f);
            valRect.offsetMin = Vector2.zero;
            valRect.offsetMax = Vector2.zero;

            Text lblTxt = CreateText("Label", box.transform, label, 12, FontStyle.Normal, TextAnchor.MiddleLeft);
            RectTransform lblRect = lblTxt.GetComponent<RectTransform>();
            lblRect.anchorMin = new Vector2(0.30f, 0.08f);
            lblRect.anchorMax = new Vector2(0.95f, 0.45f);
            lblRect.offsetMin = Vector2.zero;
            lblRect.offsetMax = Vector2.zero;
            lblTxt.color = new Color(0.6f, 0.7f, 0.8f);

            return valTxt;
        }

        private static void CreateArtCard(Transform parent, string title, int ap, string desc)
        {
            GameObject card = CreateUIGameObject($"Art_{title}", parent);
            Image bg = card.AddComponent<Image>();
            bg.color = new Color(0.13f, 0.16f, 0.22f);

            GameObject apObj = CreateUIGameObject("APBadge", card.transform);
            RectTransform apRect = apObj.GetComponent<RectTransform>();
            apRect.anchorMin = new Vector2(0.08f, 0.65f);
            apRect.anchorMax = new Vector2(0.35f, 0.92f);
            apRect.offsetMin = Vector2.zero;
            apRect.offsetMax = Vector2.zero;
            Image apImg = apObj.AddComponent<Image>();
            apImg.color = new Color(0.1f, 0.65f, 1f);

            Text apTxt = CreateText("APVal", apObj.transform, ap.ToString(), 14, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(apTxt.GetComponent<RectTransform>());

            Text nameTxt = CreateText("Name", card.transform, title, 13, FontStyle.Bold, TextAnchor.MiddleLeft);
            RectTransform nRect = nameTxt.GetComponent<RectTransform>();
            nRect.anchorMin = new Vector2(0.08f, 0.35f);
            nRect.anchorMax = new Vector2(0.92f, 0.62f);
            nRect.offsetMin = Vector2.zero;
            nRect.offsetMax = Vector2.zero;

            Text descTxt = CreateText("Desc", card.transform, desc, 11, FontStyle.Normal, TextAnchor.MiddleLeft);
            RectTransform dRect = descTxt.GetComponent<RectTransform>();
            dRect.anchorMin = new Vector2(0.08f, 0.08f);
            dRect.anchorMax = new Vector2(0.92f, 0.35f);
            dRect.offsetMin = Vector2.zero;
            dRect.offsetMax = Vector2.zero;
            descTxt.color = new Color(0.7f, 0.75f, 0.85f);
        }

        private static Button CreateNavButton(Transform parent, string text, bool active)
        {
            GameObject btnObj = CreateUIGameObject("NavBtn", parent);
            Image bg = btnObj.AddComponent<Image>();
            bg.color = active ? new Color(0.95f, 0.75f, 0.25f, 0.35f) : new Color(0.12f, 0.15f, 0.22f, 0.6f);

            Button btn = btnObj.AddComponent<Button>();
            Text txt = CreateText("Label", btnObj.transform, text, 12, FontStyle.Bold, TextAnchor.MiddleCenter);
            txt.color = active ? new Color(1f, 0.85f, 0.3f) : new Color(0.7f, 0.75f, 0.85f);
            StretchFull(txt.GetComponent<RectTransform>());
            return btn;
        }

        private static void CreateTabButton(Transform parent, string text, bool active)
        {
            GameObject btnObj = CreateUIGameObject($"Tab_{text}", parent);
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 30);
            Text txt = CreateText("Label", btnObj.transform, text, 14, FontStyle.Bold, TextAnchor.MiddleCenter);
            txt.color = active ? new Color(1f, 0.82f, 0.25f) : new Color(0.6f, 0.65f, 0.75f);
            StretchFull(txt.GetComponent<RectTransform>());
        }

        private static Text CreateResourceBadge(Transform parent, string content, Color color)
        {
            GameObject obj = CreateUIGameObject("ResourceBadge", parent);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(130, 35);
            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.17f, 0.24f, 0.8f);

            Text txt = CreateText("Text", obj.transform, content, 14, FontStyle.Bold, TextAnchor.MiddleCenter);
            txt.color = color;
            StretchFull(txt.GetComponent<RectTransform>());
            return txt;
        }

        private static void CreateIconButton(Transform parent, string icon)
        {
            GameObject obj = CreateUIGameObject("IconBtn", parent);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(36, 36);
            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.14f, 0.17f, 0.24f, 0.8f);

            Text txt = CreateText("Text", obj.transform, icon, 14, FontStyle.Bold, TextAnchor.MiddleCenter);
            txt.color = Color.white;
            StretchFull(txt.GetComponent<RectTransform>());
        }

        private static Button CreateButton(string name, Transform parent, string label, Color bgColor)
        {
            GameObject btnObj = CreateUIGameObject(name, parent);
            Image img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            Button btn = btnObj.AddComponent<Button>();
            Text txt = CreateText("Label", btnObj.transform, label, 12, FontStyle.Bold, TextAnchor.MiddleCenter);
            StretchFull(txt.GetComponent<RectTransform>());
            txt.color = Color.white;
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
    }
}
