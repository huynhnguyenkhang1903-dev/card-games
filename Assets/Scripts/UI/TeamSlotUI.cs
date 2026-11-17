using System;
using UnityEngine;
using UnityEngine.UI;

namespace Antigravity.UI
{
    /// <summary>
    /// Quản lý hiển thị và tương tác của 1 ô trên lưới phe ta 2x3 (Team Setup Grid Slot).
    /// Hàng 0: Tiền Tuyến (Frontline), Hàng 1: Hậu Phương (Backline).
    /// </summary>
    public class TeamSlotUI : MonoBehaviour
    {
        [Header("Tọa độ ô")]
        public int Row;    // 0 = Frontline, 1 = Backline
        public int Column; // 0, 1, 2

        [Header("Dữ liệu nhân vật hiện tại")]
        [SerializeField] private CharacterStats assignedCharacter;
        public CharacterStats AssignedCharacter => assignedCharacter;
        public bool IsOccupied => assignedCharacter != null;

        [Header("UI Components")]
        public Image BackgroundImage;
        public Image BorderImage;
        public Text SlotNameText;

        [Header("Trạng thái trống")]
        public GameObject EmptyStateObject;
        public Text EmptyPromptText;

        [Header("Trạng thái đã có tướng")]
        public GameObject OccupiedStateObject;
        public Text CharacterNameText;
        public Text StatsSummaryText;
        public Text APBadgeText;
        public Button RemoveButton;

        [Header("Tương tác chọn ô")]
        public Button SlotButton;

        [Header("Màu sắc trạng thái")]
        public Color NormalColor = new Color(0.18f, 0.22f, 0.28f, 0.95f);
        public Color SelectedColor = new Color(0.24f, 0.45f, 0.65f, 1f);
        public Color OccupiedColor = new Color(0.15f, 0.32f, 0.25f, 0.95f);
        public Color FrontlineBorderColor = new Color(0.9f, 0.65f, 0.2f, 1f);
        public Color BacklineBorderColor = new Color(0.35f, 0.7f, 0.95f, 1f);

        // Sự kiện thông báo
        public event Action<TeamSlotUI> OnSlotClicked;
        public event Action<TeamSlotUI> OnSlotRemoved;

        private bool isSelected = false;

        private void Awake()
        {
            if (SlotButton != null)
            {
                SlotButton.onClick.AddListener(HandleSlotClicked);
            }

            if (RemoveButton != null)
            {
                RemoveButton.onClick.AddListener(HandleRemoveClicked);
            }
        }

        public void SetupCoordinate(int row, int col)
        {
            Row = row;
            Column = col;

            string rowLabel = (row == 0) ? "Tiền Tuyến" : "Hậu Phương";
            string colLabel = $"Cột {col + 1}";
            if (SlotNameText != null)
            {
                SlotNameText.text = $"{rowLabel} ({colLabel})";
            }

            if (BorderImage != null)
            {
                BorderImage.color = (row == 0) ? FrontlineBorderColor : BacklineBorderColor;
            }

            RefreshUI();
        }

        public void AssignCharacter(CharacterStats stats)
        {
            assignedCharacter = stats;
            RefreshUI();
        }

        public void ClearSlot()
        {
            assignedCharacter = null;
            RefreshUI();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateBackgroundColor();
        }

        public void RefreshUI()
        {
            if (IsOccupied)
            {
                if (EmptyStateObject != null) EmptyStateObject.SetActive(false);
                if (OccupiedStateObject != null) OccupiedStateObject.SetActive(true);

                if (CharacterNameText != null)
                {
                    CharacterNameText.text = assignedCharacter.CharacterName;
                }

                if (StatsSummaryText != null)
                {
                    StatsSummaryText.text = $"HP: {assignedCharacter.BaseHP} | ATK: {assignedCharacter.BaseATK} | DEF: {assignedCharacter.BaseDEF}";
                }

                if (APBadgeText != null)
                {
                    APBadgeText.text = $"+{assignedCharacter.APContribution} AP";
                }
            }
            else
            {
                if (EmptyStateObject != null) EmptyStateObject.SetActive(true);
                if (OccupiedStateObject != null) OccupiedStateObject.SetActive(false);

                if (EmptyPromptText != null)
                {
                    EmptyPromptText.text = "+ Trống";
                }
            }

            UpdateBackgroundColor();
        }

        private void UpdateBackgroundColor()
        {
            if (BackgroundImage == null) return;

            if (isSelected)
            {
                BackgroundImage.color = SelectedColor;
            }
            else if (IsOccupied)
            {
                BackgroundImage.color = OccupiedColor;
            }
            else
            {
                BackgroundImage.color = NormalColor;
            }
        }

        private void HandleSlotClicked()
        {
            OnSlotClicked?.Invoke(this);
        }

        private void HandleRemoveClicked()
        {
            OnSlotRemoved?.Invoke(this);
        }
    }
}
