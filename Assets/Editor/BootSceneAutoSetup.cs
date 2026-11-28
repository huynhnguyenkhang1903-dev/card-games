using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Antigravity.UI;

namespace Antigravity.Editor
{
    public static class BootSceneAutoSetup
    {
        [MenuItem("Tools/Antigravity/Setup Boot Scene (Loading Figma)")]
        public static void CreateBootSceneUI()
        {
            EnsureEventSystem();

            // 1. Tạo Canvas chính
            GameObject canvasObj = GameObject.Find("BootSceneCanvas");
            if (canvasObj != null)
            {
                Undo.DestroyObjectImmediate(canvasObj);
            }

            canvasObj = new GameObject("BootSceneCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create BootSceneCanvas");

            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // 2. Main Background Panel
            GameObject mainPanel = CreateUIGameObject("BootMainPanel", canvasObj.transform);
            RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
            StretchFull(mainRect);
            Image mainBg = mainPanel.AddComponent<Image>();
            mainBg.color = new Color(0.04f, 0.05f, 0.08f, 1f);

            CanvasGroup canvasGroup = mainPanel.AddComponent<CanvasGroup>();
            BootSceneUI bootUI = mainPanel.AddComponent<BootSceneUI>();
            bootUI.FadeCanvasGroup = canvasGroup;

            // 3. Center Game Logo / Title
            GameObject titleGroup = CreateUIGameObject("TitleGroup", mainPanel.transform);
            RectTransform titleRect = titleGroup.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.45f);
            titleRect.anchorMax = new Vector2(0.8f, 0.85f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            Text emblem = CreateText("Emblem", titleGroup.transform, "🛡", 72, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform embRect = emblem.GetComponent<RectTransform>();
            embRect.anchorMin = new Vector2(0.35f, 0.65f);
            embRect.anchorMax = new Vector2(0.65f, 1f);
            embRect.offsetMin = Vector2.zero;
            embRect.offsetMax = Vector2.zero;
            emblem.color = new Color(0.96f, 0.77f, 0.26f); // Gold

            Text mainTitle = CreateText("MainTitle", titleGroup.transform, "AURELIA: SUNSWORN", 56, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform mtRect = mainTitle.GetComponent<RectTransform>();
            mtRect.anchorMin = new Vector2(0.05f, 0.35f);
            mtRect.anchorMax = new Vector2(0.95f, 0.70f);
            mtRect.offsetMin = Vector2.zero;
            mtRect.offsetMax = Vector2.zero;
            mainTitle.color = new Color(0.96f, 0.82f, 0.38f); // Lustrous Gold

            Text subTitle = CreateText("SubTitle", titleGroup.transform, "CHRONICLES OF AEGIS  •  TACTICAL BATTLE", 16, FontStyle.Bold, TextAnchor.MiddleCenter);
            RectTransform stRect = subTitle.GetComponent<RectTransform>();
            stRect.anchorMin = new Vector2(0.05f, 0.15f);
            stRect.anchorMax = new Vector2(0.95f, 0.35f);
            stRect.offsetMin = Vector2.zero;
            stRect.offsetMax = Vector2.zero;
            subTitle.color = new Color(0.92f, 0.90f, 0.85f, 0.8f);

            // 4. Bottom Loading Section
            GameObject loadingGroup = CreateUIGameObject("LoadingGroup", mainPanel.transform);
            RectTransform loadRect = loadingGroup.GetComponent<RectTransform>();
            loadRect.anchorMin = new Vector2(0.25f, 0.10f);
            loadRect.anchorMax = new Vector2(0.75f, 0.32f);
            loadRect.offsetMin = Vector2.zero;
            loadRect.offsetMax = Vector2.zero;

            // Status Text (Left) & Percent (Right)
            Text statusTxt = CreateText("StatusText", loadingGroup.transform, "Đang khởi tạo tài nguyên chiến đấu...", 15, FontStyle.Normal, TextAnchor.MiddleLeft);
            RectTransform statRect = statusTxt.GetComponent<RectTransform>();
            statRect.anchorMin = new Vector2(0.02f, 0.65f);
            statRect.anchorMax = new Vector2(0.80f, 0.95f);
            statRect.offsetMin = Vector2.zero;
            statRect.offsetMax = Vector2.zero;
            statusTxt.color = new Color(0.6f, 0.7f, 0.8f);
            bootUI.LoadingStatusText = statusTxt;

            Text percentTxt = CreateText("PercentText", loadingGroup.transform, "0%", 18, FontStyle.Bold, TextAnchor.MiddleRight);
            RectTransform pctRect = percentTxt.GetComponent<RectTransform>();
            pctRect.anchorMin = new Vector2(0.80f, 0.65f);
            pctRect.anchorMax = new Vector2(0.98f, 0.95f);
            pctRect.offsetMin = Vector2.zero;
            pctRect.offsetMax = Vector2.zero;
            percentTxt.color = new Color(0f, 0.9f, 1f); // Cyan neon
            bootUI.ProgressPercentText = percentTxt;

            // Progress Slider
            GameObject sliderObj = CreateUIGameObject("LoadingSlider", loadingGroup.transform);
            RectTransform slideRect = sliderObj.GetComponent<RectTransform>();
            slideRect.anchorMin = new Vector2(0.02f, 0.42f);
            slideRect.anchorMax = new Vector2(0.98f, 0.58f);
            slideRect.offsetMin = Vector2.zero;
            slideRect.offsetMax = Vector2.zero;

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;

            // Background
            Image bgImg = sliderObj.AddComponent<Image>();
            bgImg.color = new Color(0.09f, 0.12f, 0.18f, 0.95f);

            // Fill Area & Fill
            GameObject fillArea = CreateUIGameObject("FillArea", sliderObj.transform);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            StretchFull(fillAreaRect);

            GameObject fillObj = CreateUIGameObject("Fill", fillArea.transform);
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            StretchFull(fillRect);
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = new Color(0f, 0.85f, 1f, 1f); // Glowing cyan fill
            slider.fillRect = fillRect;
            bootUI.LoadingProgressBar = slider;

            // Lore / Gameplay Tip Text
            Text tipTxt = CreateText("TipText", loadingGroup.transform, "Mẹo: Tướng Paladin ở hàng Tiền Tuyến sẽ che chắn toàn bộ sát thương cho tướng hàng sau.", 13, FontStyle.Italic, TextAnchor.MiddleCenter);
            RectTransform tipRect = tipTxt.GetComponent<RectTransform>();
            tipRect.anchorMin = new Vector2(0.02f, 0.05f);
            tipRect.anchorMax = new Vector2(0.98f, 0.35f);
            tipRect.offsetMin = Vector2.zero;
            tipRect.offsetMax = Vector2.zero;
            tipTxt.color = new Color(0.85f, 0.78f, 0.65f, 0.85f);
            bootUI.LoreTipText = tipTxt;

            // Footer Version
            Text verTxt = CreateText("VersionText", mainPanel.transform, "v1.0.4-Alpha • Build 2026.11 • Dark Fantasy Edition", 12, FontStyle.Normal, TextAnchor.MiddleCenter);
            RectTransform verRect = verTxt.GetComponent<RectTransform>();
            verRect.anchorMin = new Vector2(0.3f, 0.02f);
            verRect.anchorMax = new Vector2(0.7f, 0.06f);
            verRect.offsetMin = Vector2.zero;
            verRect.offsetMax = Vector2.zero;
            verTxt.color = new Color(0.4f, 0.45f, 0.55f);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[BootSceneAutoSetup] ĐÃ TẠO THÀNH CÔNG BOOT SCENE (SPLASH & LOADING FIGMA)!");
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
