using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticalDeckUI : MonoBehaviour
{
    [Header("Tactical Deck Settings")]
    public const int MAX_TACTICAL_CARDS = 4;
    public List<CardData> AvailableTacticalCards = new List<CardData>();
    public List<CardData> SelectedTacticalCards = new List<CardData>();

    [Header("UI References")]
    public Transform AvailableCardsContainer;
    public Transform SelectedCardsContainer;
    public Text SelectedCountText;
    public Button ReadyBattleButton;

    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// US-015: Tạo màn hình chọn 4 thẻ Tactical Deck (Phép bổ trợ chung) trước trận.
    /// </summary>
    public bool SelectCard(CardData card)
    {
        if (card == null) return false;
        if (SelectedTacticalCards.Contains(card)) return false;

        if (SelectedTacticalCards.Count < MAX_TACTICAL_CARDS)
        {
            SelectedTacticalCards.Add(card);
            RefreshUI();
            Debug.Log($"[TacticalDeck] Đã chọn thẻ bổ trợ: {card.CardName} ({SelectedTacticalCards.Count}/{MAX_TACTICAL_CARDS})");
            return true;
        }

        Debug.LogWarning("[TacticalDeck] Đã đạt giới hạn tối đa 4 thẻ bổ trợ chung!");
        return false;
    }

    public void DeselectCard(CardData card)
    {
        if (SelectedTacticalCards.Remove(card))
        {
            RefreshUI();
            Debug.Log($"[TacticalDeck] Đã bỏ chọn thẻ: {card?.CardName}");
        }
    }

    private void RefreshUI()
    {
        if (SelectedCountText != null)
        {
            SelectedCountText.text = $"Đã chọn: {SelectedTacticalCards.Count}/{MAX_TACTICAL_CARDS}";
        }
        if (ReadyBattleButton != null)
        {
            ReadyBattleButton.interactable = SelectedTacticalCards.Count == MAX_TACTICAL_CARDS;
        }
    }
}
