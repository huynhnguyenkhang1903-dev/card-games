using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Antigravity.UI
{
    public class BootSceneUI : MonoBehaviour
    {
        [Header("UI References")]
        public Slider LoadingProgressBar;
        public Text ProgressPercentText;
        public Text LoadingStatusText;
        public Text LoreTipText;
        public CanvasGroup FadeCanvasGroup;

        [Header("Settings")]
        public string NextSceneName = "MainMenuScene";
        public float LoadingDuration = 2.5f;

        private static readonly string[] LoreTips = new string[]
        {
            "Mẹo: Tướng Paladin ở hàng Tiền Tuyến sẽ che chắn toàn bộ sát thương cho tướng hàng sau.",
            "Mẹo: Thẻ chiêu cuối (Ultimate) tiêu tốn nhiều Action Points nhưng có thể lật ngược cục diện trận đấu.",
            "Mẹo: Giáp ảo (Aegis Shield) sẽ tự động reset về 0 khi bắt đầu lượt mới, hãy cân nhắc phòng ngự đúng lúc.",
            "Mẹo: Tiêu diệt toàn bộ đệ tử của Boss Vua Goblin sẽ phá vỡ lớp Siêu Giáp 120 DEF của hắn."
        };

        private void Start()
        {
            if (LoreTipText != null && LoreTips.Length > 0)
            {
                LoreTipText.text = LoreTips[Random.Range(0, LoreTips.Length)];
            }

            StartCoroutine(SimulateBootProcess());
        }

        private IEnumerator SimulateBootProcess()
        {
            float elapsed = 0f;
            if (LoadingProgressBar != null) LoadingProgressBar.value = 0f;

            while (elapsed < LoadingDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / LoadingDuration);

                if (LoadingProgressBar != null)
                {
                    LoadingProgressBar.value = progress;
                }

                if (ProgressPercentText != null)
                {
                    ProgressPercentText.text = $"{(int)(progress * 100)}%";
                }

                if (LoadingStatusText != null)
                {
                    if (progress < 0.35f) LoadingStatusText.text = "Khởi tạo hệ thống chiến thuật...";
                    else if (progress < 0.70f) LoadingStatusText.text = "Tải dữ liệu tướng và bàn cờ...";
                    else if (progress < 0.95f) LoadingStatusText.text = "Đồng bộ tài nguyên Dark Fantasy...";
                    else LoadingStatusText.text = "Hoàn tất! Đang tiến vào Sảnh Game...";
                }

                yield return null;
            }

            // Fade out nhẹ nhàng
            if (FadeCanvasGroup != null)
            {
                float fadeTime = 0.4f;
                float f = 0f;
                while (f < fadeTime)
                {
                    f += Time.deltaTime;
                    FadeCanvasGroup.alpha = 1f - (f / fadeTime);
                    yield return null;
                }
            }

            // Chuyển sang Sảnh game
            if (Application.CanStreamedLevelBeLoaded(NextSceneName))
            {
                SceneManager.LoadScene(NextSceneName);
            }
            else
            {
                Debug.Log($"[BootScene] Đã tải xong! Không tìm thấy Scene '{NextSceneName}' trong Build Settings, vui lòng add vào.");
            }
        }
    }
}
