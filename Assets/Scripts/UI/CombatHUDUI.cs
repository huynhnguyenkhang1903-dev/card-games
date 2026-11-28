using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigravity.UI
{
    public class CombatHUDUI : MonoBehaviour
    {
        public static CombatHUDUI Instance { get; private set; }

        [Header("Top Header Bar")]
        public Button SpeedButton;
        public Text SpeedText;
        public Button AutoButton;
        public Text AutoText;
        public Text TurnBadgeText;
        public Text APValueText;
        public Transform APBlocksContainer;
        public List<Image> APBlocks = new List<Image>();
        public Text DeckCountText;
        public Button PauseButton;
        public Button SettingsButton;

        [Header("Arena Units HUD")]
        public Transform AlliesHUDContainer;
        public Transform EnemiesHUDContainer;
        public List<CombatUnitHUDUI> UnitHUDs = new List<CombatUnitHUDUI>();

        [Header("Bottom Hand Cards")]
        public Transform HandCardsContainer;
        public List<CombatCardUI> ActiveCards = new List<CombatCardUI>();

        [Header("Tactical Belt (Quick Actions)")]
        public Button ManaBeltButton;
        public Button PurgeBeltButton;
        public Button EvadeBeltButton;
        public Button RallyBeltButton;

        [Header("End Turn Control")]
        public Button EndTurnButton;
        public Text EndTurnStatusText;

        [Header("Combat Log / Notification")]
        public Text NotificationText;

        [Header("Runtime Settings")]
        public int MaxAP = 10;
        public int DeckCount = 18;
        private bool isAuto = false;
        private int speedMultiplier = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            BindControls();
            RefreshHUD();
        }

        private void Update()
        {
            UpdateAPDisplay();
            UpdateTurnDisplay();
        }

        private void BindControls()
        {
            if (SpeedButton != null)
            {
                SpeedButton.onClick.AddListener(ToggleSpeed);
            }

            if (AutoButton != null)
            {
                AutoButton.onClick.AddListener(ToggleAuto);
            }

            if (EndTurnButton != null)
            {
                EndTurnButton.onClick.AddListener(OnEndTurnClicked);
            }

            if (ManaBeltButton != null)
            {
                ManaBeltButton.onClick.AddListener(() => OnTacticalBeltAction("MANA", 2));
            }
            if (PurgeBeltButton != null)
            {
                PurgeBeltButton.onClick.AddListener(() => OnTacticalBeltAction("PURGE", 0));
            }
            if (EvadeBeltButton != null)
            {
                EvadeBeltButton.onClick.AddListener(() => OnTacticalBeltAction("EVADE", 0));
            }
            if (RallyBeltButton != null)
            {
                RallyBeltButton.onClick.AddListener(() => OnTacticalBeltAction("RALLY", 0));
            }
        }

        public void RefreshHUD()
        {
            UpdateTurnDisplay();
            UpdateAPDisplay();
            UpdateDeckCountDisplay();
            RefreshCardsAvailability();
        }

        public void UpdateTurnDisplay()
        {
            int turn = 1;
            if (TurnManager.Instance != null && TurnManager.Instance.TurnCount > 0)
            {
                turn = TurnManager.Instance.TurnCount;
            }

            if (TurnBadgeText != null)
            {
                TurnBadgeText.text = $"ENCOUNTER\nTURN {turn}";
            }
        }

        public void UpdateAPDisplay()
        {
            int currentAP = 8;
            if (TurnManager.Instance != null)
            {
                currentAP = TurnManager.Instance.CurrentAP;
            }

            if (APValueText != null)
            {
                APValueText.text = $"{currentAP}/{MaxAP}\nACTION POINTS";
            }

            // Cập nhật thanh block AP
            if (APBlocks != null && APBlocks.Count > 0)
            {
                for (int i = 0; i < APBlocks.Count; i++)
                {
                    if (APBlocks[i] != null)
                    {
                        bool active = i < currentAP;
                        APBlocks[i].color = active ? new Color(0.1f, 0.7f, 1f, 1f) : new Color(0.2f, 0.25f, 0.35f, 0.5f);
                    }
                }
            }
        }

        public void UpdateDeckCountDisplay()
        {
            if (DeckCountText != null)
            {
                DeckCountText.text = $"DECK: {DeckCount}";
            }
        }

        public void RefreshCardsAvailability()
        {
            foreach (var card in ActiveCards)
            {
                if (card != null)
                {
                    card.RefreshAvailability();
                }
            }
        }

        public void OnCardPlayAttempt(CombatCardUI cardUI)
        {
            if (cardUI == null || cardUI.CardData == null) return;

            int currentAP = TurnManager.Instance != null ? TurnManager.Instance.CurrentAP : 10;
            if (currentAP < cardUI.CurrentCost)
            {
                ShowNotification($"Không đủ Action Points! Cần {cardUI.CurrentCost} AP.");
                return;
            }

            if (cardUI.CurrentCD > 0)
            {
                ShowNotification($"Thẻ '{cardUI.CardData.CardName}' đang hồi chiêu ({cardUI.CurrentCD} lượt)!");
                return;
            }

            // Trừ AP
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.CurrentAP -= cardUI.CurrentCost;
            }

            // Đặt hồi chiêu
            cardUI.CardData.CurrentCooldown = cardUI.CardData.Cooldown;
            cardUI.UpdateCooldownDisplay();

            ShowNotification($"Đã thi triển '{cardUI.CardData.CardName}'! (-{cardUI.CurrentCost} AP)");

            // Kích hoạt effect
            cardUI.CardData.ActivateEffect(cardUI.OwnerUnit != null ? cardUI.OwnerUnit.gameObject : null, null);

            RefreshHUD();
        }

        public void OnEndTurnClicked()
        {
            ShowNotification("Kết thúc lượt người chơi! Chuyển giao lượt cho phe địch...");

            if (TurnManager.Instance != null)
            {
                // Reset khiên đầu hiệp nếu có logic
                if (TurnManager.Instance.EnemyState != null)
                {
                    TurnManager.Instance.ChangeState(TurnManager.Instance.EnemyState);
                }

                // Giả lập kết thúc lượt địch sau 1 giây
                CancelInvoke(nameof(StartNextPlayerTurn));
                Invoke(nameof(StartNextPlayerTurn), 1.2f);
            }
        }

        private void StartNextPlayerTurn()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.TurnCount++;
                TurnManager.Instance.CurrentAP = MaxAP;

                // Giảm hồi chiêu thẻ
                foreach (var card in ActiveCards)
                {
                    if (card != null && card.CardData != null && card.CardData.CurrentCooldown > 0)
                    {
                        card.CardData.CurrentCooldown--;
                        card.UpdateCooldownDisplay();
                    }
                }

                if (TurnManager.Instance.PlayerState != null)
                {
                    TurnManager.Instance.ChangeState(TurnManager.Instance.PlayerState);
                }
            }

            ShowNotification($"Bắt đầu Turn mới! Nhận đầy đủ {MaxAP} Action Points.");
            RefreshHUD();
        }

        private void OnTacticalBeltAction(string actionType, int apBonus)
        {
            switch (actionType)
            {
                case "MANA":
                    if (TurnManager.Instance != null)
                    {
                        TurnManager.Instance.CurrentAP = Mathf.Min(MaxAP, TurnManager.Instance.CurrentAP + apBonus);
                    }
                    ShowNotification($"Đã kích hoạt [MANA BELT]! Hồi phục +{apBonus} Action Points.");
                    break;
                case "PURGE":
                    ShowNotification("Đã kích hoạt [PURGE BELT]! Xóa sạch trạng thái bất lợi cho toàn đội.");
                    break;
                case "EVADE":
                    ShowNotification("Đã kích hoạt [EVADE BELT]! Toàn đội tăng 30% né tránh đòn đánh kế tiếp.");
                    break;
                case "RALLY":
                    ShowNotification("Đã kích hoạt [RALLY BELT]! Toàn đội tăng 25% sát thương tấn công lượt này.");
                    break;
            }

            RefreshHUD();
        }

        private void ToggleSpeed()
        {
            speedMultiplier = speedMultiplier == 1 ? 2 : 1;
            Time.timeScale = speedMultiplier;
            if (SpeedText != null)
            {
                SpeedText.text = $"{speedMultiplier}X SPEED";
            }
        }

        private void ToggleAuto()
        {
            isAuto = !isAuto;
            if (AutoText != null)
            {
                AutoText.text = isAuto ? "AUTO: ON" : "AUTO: OFF";
            }
        }

        public void ShowNotification(string message)
        {
            if (NotificationText != null)
            {
                NotificationText.text = message;
                CancelInvoke(nameof(ClearNotification));
                Invoke(nameof(ClearNotification), 3.0f);
            }
            Debug.Log($"[CombatHUD] {message}");
        }

        private void ClearNotification()
        {
            if (NotificationText != null)
            {
                NotificationText.text = "";
            }
        }
    }
}
