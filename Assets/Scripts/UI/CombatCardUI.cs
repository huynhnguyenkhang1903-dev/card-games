using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Antigravity.UI
{
    public class CombatCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        public Text CardNameText;
        public Text CardTypeText;
        public Text CostText;
        public Text DescriptionText;
        public Text CooldownText;
        public Image CardBackground;
        public Image ArtworkImage;
        public Image CostDiamondImage;
        public GameObject CooldownBadge;
        public GameObject DisabledOverlay;
        public Button CardButton;

        [Header("Runtime State")]
        public CardData CardData;
        public BattleUnit OwnerUnit;
        public int CurrentCost;
        public int CurrentCD;
        public bool IsInteractable = true;

        public System.Action<CombatCardUI> OnClicked;

        private Vector3 originalScale;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalScale = transform.localScale;

            if (CardButton == null)
            {
                CardButton = GetComponent<Button>();
            }

            if (CardButton != null)
            {
                CardButton.onClick.AddListener(HandleClick);
            }
        }

        public void Setup(CardData data, BattleUnit owner)
        {
            CardData = data;
            OwnerUnit = owner;

            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            CurrentCost = data.CardCost;
            CurrentCD = data.CurrentCooldown;

            if (CardNameText != null) CardNameText.text = data.CardName;
            if (CardTypeText != null) CardTypeText.text = data.CardType.ToString().ToUpper();
            if (CostText != null) CostText.text = CurrentCost.ToString();
            if (DescriptionText != null) DescriptionText.text = data.Description;

            UpdateCooldownDisplay();
            RefreshAvailability();
        }

        public void UpdateCooldownDisplay()
        {
            if (CardData != null)
            {
                CurrentCD = CardData.CurrentCooldown;
            }

            if (CooldownBadge != null)
            {
                CooldownBadge.SetActive(CurrentCD > 0);
            }

            if (CooldownText != null)
            {
                CooldownText.text = CurrentCD > 0 ? $"CD: {CurrentCD}T" : "";
            }
        }

        public void RefreshAvailability()
        {
            int availableAP = TurnManager.Instance != null ? TurnManager.Instance.CurrentAP : 10;
            bool ownerAlive = OwnerUnit == null || !OwnerUnit.IsDead;
            bool canAfford = availableAP >= CurrentCost;
            bool notInCD = CurrentCD <= 0;

            IsInteractable = ownerAlive && canAfford && notInCD;

            if (DisabledOverlay != null)
            {
                DisabledOverlay.SetActive(!IsInteractable);
            }

            if (CardButton != null)
            {
                CardButton.interactable = IsInteractable;
            }
        }

        private void HandleClick()
        {
            if (!IsInteractable) return;
            OnClicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable)
            {
                transform.localScale = originalScale * 1.06f;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = originalScale;
        }
    }
}
