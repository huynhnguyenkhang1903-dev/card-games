using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Antigravity.UI
{
    /// <summary>
    /// Điều phối chính cho giao diện Thiết Lập Đội Hình (Team Setup).
    /// Quản lý sơ đồ Lưới 2 Hàng x 3 Cột phe ta và Kho Tướng (Character Roster).
    /// </summary>
    public class TeamSetupUI : MonoBehaviour
    {
        public static TeamSetupUI Instance { get; private set; }

        public const int GRID_ROWS = 2; // 0 = Tiền tuyến (Frontline), 1 = Hậu phương (Backline)
        public const int GRID_COLS = 3; // 3 cột: 0, 1, 2

        [Header("Cấu hình Đội hình")]
        public int MaxDeployableUnits = 3; // Tối đa 3 tướng xuất trận theo GDD

        [Header("Danh sách Tướng khả dụng trong kho")]
        public List<CharacterStats> AvailableCharacters = new List<CharacterStats>();

        [Header("Tham chiếu 6 ô Lưới 2x3")]
        [Tooltip("Mảng 6 ô lưới phe ta theo thứ tự Row 0 (Col 0,1,2), Row 1 (Col 0,1,2)")]
        public List<TeamSlotUI> GridSlots = new List<TeamSlotUI>();

        [Header("Kho Tướng UI")]
        public Transform RosterContainer;
        public GameObject RosterCardPrefab;

        [Header("Thanh thông tin tổng quan (Top Bar)")]
        public Text TitleText;
        public Text DeployedCountText;
        public Text TotalAPText;

        [Header("Các nút điều khiển (Control Buttons)")]
        public Button AutoDeployButton;
        public Button ClearAllButton;
        public Button StartBattleButton;

        [Header("Thông báo trạng thái")]
        public Text NotificationText;

        // Lưu danh sách UI thẻ tướng trong kho
        private List<CharacterRosterCardUI> spawnedRosterCards = new List<CharacterRosterCardUI>();

        // Ô đang được người chơi click chọn
        private TeamSlotUI selectedSlot = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Khởi tạo sự kiện nút bấm
            if (AutoDeployButton != null)
                AutoDeployButton.onClick.AddListener(AutoDeployTeam);

            if (ClearAllButton != null)
                ClearAllButton.onClick.AddListener(ClearTeam);

            if (StartBattleButton != null)
                StartBattleButton.onClick.AddListener(StartBattle);
        }

        private void Start()
        {
            InitializeGridSlots();
            PopulateRoster();
            UpdateOverview();
        }

        /// <summary>
        /// Khởi tạo 6 ô lưới với tọa độ chuẩn 2x3.
        /// </summary>
        public void InitializeGridSlots()
        {
            if (GridSlots == null || GridSlots.Count == 0)
            {
                GridSlots = new List<TeamSlotUI>(GetComponentsInChildren<TeamSlotUI>(true));
            }

            for (int i = 0; i < GridSlots.Count; i++)
            {
                int row = i / GRID_COLS;
                int col = i % GRID_COLS;
                var slot = GridSlots[i];
                if (slot != null)
                {
                    slot.SetupCoordinate(row, col);
                    slot.OnSlotClicked += HandleSlotClicked;
                    slot.OnSlotRemoved += HandleSlotRemoved;
                }
            }
        }

        /// <summary>
        /// Tạo các thẻ tướng trong Kho Tướng (Roster).
        /// </summary>
        public void PopulateRoster()
        {
            if (RosterContainer == null) return;

            // Dọn dẹp thẻ cũ nếu có
            foreach (var card in spawnedRosterCards)
            {
                if (card != null) Destroy(card.gameObject);
            }
            spawnedRosterCards.Clear();

            // Sinh thẻ tướng mới từ danh sách dữ liệu AvailableCharacters
            foreach (var charData in AvailableCharacters)
            {
                if (charData == null) continue;

                GameObject cardObj = null;
                if (RosterCardPrefab != null)
                {
                    cardObj = Instantiate(RosterCardPrefab, RosterContainer);
                }
                else
                {
                    // Fallback tạo nhanh component nếu chưa kéo prefab
                    cardObj = new GameObject($"Card_{charData.CharacterName}", typeof(RectTransform));
                    cardObj.transform.SetParent(RosterContainer, false);
                }

                var cardUI = cardObj.GetComponent<CharacterRosterCardUI>();
                if (cardUI == null)
                {
                    cardUI = cardObj.AddComponent<CharacterRosterCardUI>();
                }

                cardUI.Initialize(charData);
                cardUI.OnCardClicked += HandleRosterCardClicked;
                spawnedRosterCards.Add(cardUI);
            }
        }

        /// <summary>
        /// Khi người chơi click vào 1 ô trên lưới.
        /// </summary>
        private void HandleSlotClicked(TeamSlotUI clickedSlot)
        {
            if (selectedSlot == clickedSlot)
            {
                // Bỏ chọn nếu bấm lại chính ô đó
                selectedSlot.SetSelected(false);
                selectedSlot = null;
                ShowNotification("Đã bỏ chọn ô lưới.");
            }
            else
            {
                if (selectedSlot != null) selectedSlot.SetSelected(false);
                selectedSlot = clickedSlot;
                selectedSlot.SetSelected(true);
                string rowName = selectedSlot.Row == 0 ? "Tiền Tuyến" : "Hậu Phương";
                ShowNotification($"Đã chọn ô [{rowName}, Cột {selectedSlot.Column + 1}]. Chọn tướng trong kho để xếp vào.");
            }
        }

        /// <summary>
        /// Khi người chơi nhấn nút gỡ tướng khỏi 1 ô.
        /// </summary>
        private void HandleSlotRemoved(TeamSlotUI removedSlot)
        {
            if (removedSlot == null || !removedSlot.IsOccupied) return;

            string name = removedSlot.AssignedCharacter.CharacterName;
            removedSlot.ClearSlot();
            ShowNotification($"Đã gỡ '{name}' khỏi đội hình.");
            UpdateOverview();
        }

        /// <summary>
        /// Khi người chơi click chọn 1 thẻ tướng từ Kho Tướng.
        /// </summary>
        private void HandleRosterCardClicked(CharacterRosterCardUI clickedCard)
        {
            if (clickedCard == null || clickedCard.CharacterStats == null) return;

            var character = clickedCard.CharacterStats;

            // Kiểm tra tướng này đã ở trên sân chưa
            if (IsCharacterAlreadyDeployed(character))
            {
                ShowNotification($"Tướng '{character.CharacterName}' đã có mặt trên lưới đội hình!");
                return;
            }

            // Kiểm tra giới hạn số lượng tướng xuất trận
            if (GetDeployedCount() >= MaxDeployableUnits && (selectedSlot == null || !selectedSlot.IsOccupied))
            {
                ShowNotification($"Đội hình đã đạt giới hạn tối đa ({MaxDeployableUnits} tướng)!");
                return;
            }

            TeamSlotUI targetSlot = null;

            // 1. Ưu tiên ô đang được người chơi click chọn
            if (selectedSlot != null)
            {
                targetSlot = selectedSlot;
            }
            else
            {
                // 2. Tự động tìm ô trống phù hợp theo vai trò:
                // AP cao (Tanker/Paladin) -> ưu tiên Tiền Tuyến (Row 0)
                // AP thấp (Sniper/Mage) -> ưu tiên Hậu Phương (Row 1)
                bool preferFrontline = character.APContribution >= 4;
                targetSlot = FindFirstEmptySlot(preferFrontline);
            }

            if (targetSlot != null)
            {
                targetSlot.AssignCharacter(character);
                if (selectedSlot != null)
                {
                    selectedSlot.SetSelected(false);
                    selectedSlot = null;
                }
                ShowNotification($"Đã xếp '{character.CharacterName}' vào ô [{ (targetSlot.Row == 0 ? "Tiền Tuyến" : "Hậu Phương") }, Cột {targetSlot.Column + 1}].");
                UpdateOverview();
            }
            else
            {
                ShowNotification("Không còn ô trống trên lưới để xếp tướng!");
            }
        }

        /// <summary>
        /// Tìm ô trống đầu tiên trên lưới.
        /// </summary>
        private TeamSlotUI FindFirstEmptySlot(bool preferFrontline)
        {
            int primaryRow = preferFrontline ? 0 : 1;
            int secondaryRow = preferFrontline ? 1 : 0;

            // Dò hàng ưu tiên trước
            for (int col = 0; col < GRID_COLS; col++)
            {
                var slot = GetSlotAt(primaryRow, col);
                if (slot != null && !slot.IsOccupied) return slot;
            }

            // Dò hàng còn lại
            for (int col = 0; col < GRID_COLS; col++)
            {
                var slot = GetSlotAt(secondaryRow, col);
                if (slot != null && !slot.IsOccupied) return slot;
            }

            return null;
        }

        public TeamSlotUI GetSlotAt(int row, int col)
        {
            int index = row * GRID_COLS + col;
            if (index >= 0 && index < GridSlots.Count)
            {
                return GridSlots[index];
            }
            return null;
        }

        public bool IsCharacterAlreadyDeployed(CharacterStats character)
        {
            if (character == null) return false;
            foreach (var slot in GridSlots)
            {
                if (slot != null && slot.AssignedCharacter == character)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetDeployedCount()
        {
            int count = 0;
            foreach (var slot in GridSlots)
            {
                if (slot != null && slot.IsOccupied) count++;
            }
            return count;
        }

        public int CalculateTotalAPContribution()
        {
            int total = 0;
            foreach (var slot in GridSlots)
            {
                if (slot != null && slot.IsOccupied && slot.AssignedCharacter != null)
                {
                    total += slot.AssignedCharacter.APContribution;
                }
            }
            return total;
        }

        /// <summary>
        /// Cập nhật hiển thị tổng quan: Tổng AP, số lượng tướng, trạng thái thẻ kho.
        /// </summary>
        public void UpdateOverview()
        {
            int count = GetDeployedCount();
            int totalAP = CalculateTotalAPContribution();

            if (DeployedCountText != null)
            {
                DeployedCountText.text = $"Tướng Xuất Trận: {count} / {MaxDeployableUnits}";
            }

            if (TotalAPText != null)
            {
                TotalAPText.text = $"Tổng Năng Lượng Đóng Góp (Turn Cost): {totalAP} AP";
            }

            // Cập nhật trạng thái 'Đã Triển Khai' cho các thẻ trong kho
            foreach (var card in spawnedRosterCards)
            {
                if (card != null && card.CharacterStats != null)
                {
                    bool isDeployed = IsCharacterAlreadyDeployed(card.CharacterStats);
                    card.SetDeployed(isDeployed);
                }
            }
        }

        /// <summary>
        /// Tự động xếp các tướng có trong kho vào lưới.
        /// </summary>
        public void AutoDeployTeam()
        {
            ClearTeam();

            int deployed = 0;
            foreach (var charData in AvailableCharacters)
            {
                if (charData == null || deployed >= MaxDeployableUnits) break;

                bool preferFront = charData.APContribution >= 4;
                var slot = FindFirstEmptySlot(preferFront);
                if (slot != null)
                {
                    slot.AssignCharacter(charData);
                    deployed++;
                }
            }

            ShowNotification($"Đã tự động xếp {deployed} tướng vào đội hình!");
            UpdateOverview();
        }

        /// <summary>
        /// Xóa sạch đội hình trên lưới.
        /// </summary>
        public void ClearTeam()
        {
            if (selectedSlot != null)
            {
                selectedSlot.SetSelected(false);
                selectedSlot = null;
            }

            foreach (var slot in GridSlots)
            {
                if (slot != null) slot.ClearSlot();
            }

            ShowNotification("Đã làm trống toàn bộ lưới đội hình.");
            UpdateOverview();
        }

        /// <summary>
        /// Xác nhận đội hình và bắt đầu trận đấu.
        /// </summary>
        public void StartBattle()
        {
            int deployedCount = GetDeployedCount();
            if (deployedCount == 0)
            {
                ShowNotification("Vui lòng xếp ít nhất 1 Tướng lên lưới trước khi bắt đầu!");
                return;
            }

            int totalAP = CalculateTotalAPContribution();
            Debug.Log($"[TeamSetup] Bắt đầu trận đấu với {deployedCount} tướng. Tổng Turn Cost khởi điểm: {totalAP} AP.");

            // Nếu GridManager tồn tại trong Scene, nạp dữ liệu vào GridManager
            if (GridManager.Instance != null)
            {
                for (int r = 0; r < GRID_ROWS; r++)
                {
                    for (int c = 0; c < GRID_COLS; c++)
                    {
                        var slot = GetSlotAt(r, c);
                        if (slot != null && slot.IsOccupied)
                        {
                            // Tìm hoặc tạo BattleUnit tại vị trí
                            GameObject unitObj = new GameObject($"Unit_{slot.AssignedCharacter.CharacterName}");
                            var unit = unitObj.AddComponent<BattleUnit>();
                            unit.Initialize(slot.AssignedCharacter, true);
                            GridManager.Instance.PlaceUnit(unit, true, r, c);
                        }
                    }
                }
            }

            // Nếu TurnManager tồn tại, cập nhật TotalTurnCost
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.TotalTurnCost = totalAP;
            }

            ShowNotification("Đội hình hợp lệ! Đang chuyển vào trận đấu...");

            if (UnityEngine.Application.CanStreamedLevelBeLoaded("CombatScene"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("CombatScene");
            }
        }

        private void ShowNotification(string msg)
        {
            if (NotificationText != null)
            {
                NotificationText.text = msg;
            }
            Debug.Log($"[TeamSetupUI] {msg}");
        }
    }
}
