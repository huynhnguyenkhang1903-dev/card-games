using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Antigravity.UI
{
    public class HeroLobbyUI : MonoBehaviour
    {
        [Header("Player Profile (Header)")]
        public Text PlayerNameText;
        public Text LevelText;
        public Text VipText;
        public Text EnergyText;
        public Text GoldText;
        public Text DiamondText;
        public Text StaminaText;

        [Header("Hero Showcase")]
        public Text HeroClassText;
        public Text HeroNameText;
        public Text HeroTierText;
        public Image HeroFullBodyImage;

        [Header("Hero Stats")]
        public Text HealthValueText;
        public Text AttackValueText;
        public Text DefenseValueText;
        public Text BaseAPValueText;
        public Text SecondaryStatsText;

        [Header("Summon Banner")]
        public Button SummonX1Button;
        public Button SummonX10Button;
        public Text SummonFeedbackText;

        [Header("Navigation Bar")]
        public Button HeroesNavButton;
        public Button CodexNavButton;
        public Button RecruitNavButton;
        public Button GuildNavButton;
        public Button VaultNavButton;

        [Header("Campaign Entry")]
        public Button EnterCampaignButton;

        [Header("Scene Navigation")]
        public string PrepSceneName = "PrepScene";
        public string CombatSceneName = "CombatScene";

        private int currentDiamonds = 4850;
        private int currentGold = 1248500;

        private void Start()
        {
            BindButtons();
            RefreshDisplay();
        }

        private void BindButtons()
        {
            if (EnterCampaignButton != null)
            {
                EnterCampaignButton.onClick.AddListener(OnEnterCampaignClicked);
            }

            if (SummonX1Button != null)
            {
                SummonX1Button.onClick.AddListener(() => PerformSummon(1, 150));
            }

            if (SummonX10Button != null)
            {
                SummonX10Button.onClick.AddListener(() => PerformSummon(10, 1500));
            }

            if (HeroesNavButton != null)
            {
                HeroesNavButton.onClick.AddListener(() => ShowFeedback("Chuyển đến danh sách Tướng"));
            }
            if (CodexNavButton != null)
            {
                CodexNavButton.onClick.AddListener(() => ShowFeedback("Mở Bách Khoa Toàn Thư (Codex)"));
            }
            if (RecruitNavButton != null)
            {
                RecruitNavButton.onClick.AddListener(() => ShowFeedback("Khu vực Chiêu Mộ Tướng (Recruit)"));
            }
            if (GuildNavButton != null)
            {
                GuildNavButton.onClick.AddListener(() => ShowFeedback("Tính năng Bang Hội đang chuẩn bị"));
            }
            if (VaultNavButton != null)
            {
                VaultNavButton.onClick.AddListener(() => ShowFeedback("Mở Kho Đồ (Vault)"));
            }
        }

        public void RefreshDisplay()
        {
            if (PlayerNameText != null) PlayerNameText.text = "Aethelgard";
            if (LevelText != null) LevelText.text = "Lv. 45";
            if (VipText != null) VipText.text = "VIP 6";
            if (EnergyText != null) EnergyText.text = "120 / 120";

            if (GoldText != null) GoldText.text = currentGold.ToString("N0");
            if (DiamondText != null) DiamondText.text = currentDiamonds.ToString("N0");
            if (StaminaText != null) StaminaText.text = "320";

            if (HeroClassText != null) HeroClassText.text = "🛡 VANGUARD PALADIN | LIGHT ELEMENT";
            if (HeroNameText != null) HeroNameText.text = "AURELIA THE SUNSWORN";
            if (HeroTierText != null) HeroTierText.text = "★★★★★  LEGENDARY TIER";

            if (HealthValueText != null) HealthValueText.text = "1,800";
            if (AttackValueText != null) AttackValueText.text = "20";
            if (DefenseValueText != null) DefenseValueText.text = "80";
            if (BaseAPValueText != null) BaseAPValueText.text = "+5";
            if (SecondaryStatsText != null) SecondaryStatsText.text = "CRIT RATE: 18.5%   |   RESIST: 35.0%   |   SPEED: 104";
        }

        private void PerformSummon(int count, int cost)
        {
            if (currentDiamonds < cost)
            {
                ShowFeedback("Không đủ Kim Cương để chiêu mộ!");
                return;
            }

            currentDiamonds -= cost;
            RefreshDisplay();
            ShowFeedback($"Chiêu mộ thành công x{count} tướng! Đã nhận được mảnh Aurelia & Tướng Tinh Anh.");
        }

        private void OnEnterCampaignClicked()
        {
            ShowFeedback("Đang tiến vào Chiến Dịch (Chapter IV)...");

            // Ưu tiên chuyển sang PrepScene để xếp đội hình hoặc CombatScene
            if (Application.CanStreamedLevelBeLoaded(PrepSceneName))
            {
                SceneManager.LoadScene(PrepSceneName);
            }
            else if (Application.CanStreamedLevelBeLoaded(CombatSceneName))
            {
                SceneManager.LoadScene(CombatSceneName);
            }
            else
            {
                Debug.Log($"[HeroLobby] Chuyển cảnh sang {PrepSceneName} hoặc {CombatSceneName}.");
            }
        }

        private void ShowFeedback(string msg)
        {
            if (SummonFeedbackText != null)
            {
                SummonFeedbackText.text = msg;
                CancelInvoke(nameof(ClearFeedback));
                Invoke(nameof(ClearFeedback), 3.0f);
            }
            Debug.Log($"[HeroLobby] {msg}");
        }

        private void ClearFeedback()
        {
            if (SummonFeedbackText != null)
            {
                SummonFeedbackText.text = "";
            }
        }
    }
}
