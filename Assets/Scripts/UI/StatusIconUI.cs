using UnityEngine;
using UnityEngine.UI;

public class StatusIconUI : MonoBehaviour
{
    [Header("UI References")]
    public Image IconImage;
    public Text StackText;
    public Text DurationText;

    /// <summary>
    /// US-054: Thiết kế lưới Icon nhỏ giọt dưới thanh Máu
    /// US-055: Tự động cập nhật số Stack và Lượt duy trì lên Icon
    /// </summary>
    public void SetStatus(StatusEffectType type, int stack, int duration)
    {
        if (StackText != null)
        {
            StackText.text = stack > 1 ? $"x{stack}" : "";
        }

        if (DurationText != null)
        {
            DurationText.text = $"{duration}T";
        }

        Debug.Log($"[StatusIcon] Cập nhật Icon {type}: Stack={stack}, Duration={duration} lượt.");
    }
}
