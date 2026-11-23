using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StageMonsterSlot
{
    public int Row;
    public int Column;
    public EnemyStats MonsterData;
}

public class StagePreviewUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text StageTitleText;
    public Text StageDescriptionText;
    public Transform EnemyGridRoot; // 2x3 Grid preview
    public GameObject MonsterPreviewPrefab;
    public Button ConfirmStageButton;

    [Header("Current Stage Configuration")]
    public string CurrentStageId = "Stage-1-1";
    public List<StageMonsterSlot> MonsterLayout = new List<StageMonsterSlot>();

    private void Start()
    {
        LoadStagePreview(CurrentStageId);
    }

    /// <summary>
    /// US-010: Thiết kế và lắp ráp giao diện Stage Preview.
    /// US-011: Logic tải cấu hình quái vật lên lưới 2x3 của giao diện Stage Preview.
    /// </summary>
    public void LoadStagePreview(string stageId)
    {
        CurrentStageId = stageId;
        if (StageTitleText != null) StageTitleText.text = $"Ải Thử Thách: {stageId}";
        if (StageDescriptionText != null) StageDescriptionText.text = "Tiêu diệt toàn bộ kẻ địch trên lưới 2x3 để mở khóa ải tiếp theo.";

        // Xóa preview cũ
        if (EnemyGridRoot != null)
        {
            foreach (Transform child in EnemyGridRoot)
            {
                Destroy(child.gameObject);
            }

            // Tải cấu hình quái lên lưới 2x3
            foreach (var slot in MonsterLayout)
            {
                if (MonsterPreviewPrefab != null)
                {
                    GameObject item = Instantiate(MonsterPreviewPrefab, EnemyGridRoot);
                    var text = item.GetComponentInChildren<Text>();
                    if (text != null && slot.MonsterData != null)
                    {
                        text.text = $"{slot.MonsterData.EnemyName}\n(HP:{slot.MonsterData.BaseHP} DEF:{slot.MonsterData.BaseDEF})";
                    }
                }
            }
        }

        Debug.Log($"[StagePreview] Đã tải xong cấu hình ải '{stageId}' với {MonsterLayout.Count} quái vật.");
    }
}
