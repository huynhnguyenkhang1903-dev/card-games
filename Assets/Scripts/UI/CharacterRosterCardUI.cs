using System;
using UnityEngine;
using UnityEngine.UI;

namespace Antigravity.UI
{
    /// <summary>
    /// Quản lý hiển thị của 1 thẻ Tướng trong Kho Tướng (Character Inventory / Roster).
    /// </summary>
    public class CharacterRosterCardUI : MonoBehaviour
    {
        [Header("Dữ liệu Tướng")]
        [SerializeField] private CharacterStats characterStats;
        public CharacterStats CharacterStats => characterStats;

        [Header("UI Components")]
        public Image CardBackground;
        public Image AvatarImage;
        public Text CharacterNameText;
        public Text RoleText;
        public Text HPText;
        public Text ATKText;
        public Text DEFText;
        public Text APText;
        public Text SkillsListText;

        [Header("Trạng thái Triển khai")]
        public GameObject DeployedOverlay;
        public Text DeployedBadgeText;
        public Button DeployButton;

        [Header("Màu sắc")]
        public Color NormalColor = new Color(0.2f, 0.24f, 0.32f, 1f);
        public Color SelectedColor = new Color(0.3f, 0.5f, 0.75f, 1f);

        public event Action<CharacterRosterCardUI> OnCardClicked;

        public bool IsDeployed { get; private set; }

        private void Awake()
        {
            if (DeployButton != null)
            {
                DeployButton.onClick.AddListener(HandleDeployClicked);
            }
        }

        public void Initialize(CharacterStats stats)
        {
            characterStats = stats;
            if (stats == null) return;

            if (AvatarImage != null && stats.Portrait != null)
            {
                AvatarImage.sprite = stats.Portrait;
                AvatarImage.gameObject.SetActive(true);
            }

            if (CharacterNameText != null)
            {
                CharacterNameText.text = stats.CharacterName;
            }

            if (HPText != null)
            {
                HPText.text = $"HP: {stats.BaseHP}";
            }

            if (ATKText != null)
            {
                ATKText.text = $"ATK: {stats.BaseATK}";
            }

            if (DEFText != null)
            {
                DEFText.text = $"DEF: {stats.BaseDEF}";
            }

            if (APText != null)
            {
                APText.text = $"+{stats.APContribution} AP";
            }

            if (RoleText != null)
            {
                RoleText.text = stats.APContribution >= 4 ? "Đỡ Đòn (Tanker)" : "Xạ Thủ / Sát Thương";
            }

            if (SkillsListText != null)
            {
                if (stats.Cards != null && stats.Cards.Count > 0)
                {
                    string cardNames = "";
                    for (int i = 0; i < stats.Cards.Count; i++)
                    {
                        if (stats.Cards[i] != null)
                        {
                            cardNames += (i > 0 ? ", " : "") + stats.Cards[i].CardName;
                        }
                    }
                    SkillsListText.text = string.IsNullOrEmpty(cardNames) ? "Chưa nạp thẻ" : $"Kỹ năng: {cardNames}";
                }
                else
                {
                    SkillsListText.text = "Kỹ năng: Đánh thường, Chiêu thức, Tối thượng";
                }
            }

            SetDeployed(false);
        }

        public void SetDeployed(bool deployed)
        {
            IsDeployed = deployed;

            if (DeployedOverlay != null)
            {
                DeployedOverlay.SetActive(deployed);
            }

            if (DeployedBadgeText != null)
            {
                DeployedBadgeText.text = deployed ? "ĐÃ TRIỂN KHAI" : "CHỌN TƯỚNG";
            }

            if (DeployButton != null)
            {
                DeployButton.interactable = !deployed;
            }
        }

        private void HandleDeployClicked()
        {
            if (!IsDeployed)
            {
                OnCardClicked?.Invoke(this);
            }
        }
    }
}
