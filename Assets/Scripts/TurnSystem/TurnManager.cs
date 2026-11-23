using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public ITurnState CurrentState { get; private set; }

    public BattleInitState InitState { get; private set; }
    public PlayerTurnState PlayerState { get; private set; }
    public EnemyTurnState EnemyState { get; private set; }

    [Header("Turn Settings")]
    public int TurnCount = 0;
    public int TotalTurnCost = 0;

    public int CurrentAP
    {
        get => TotalTurnCost;
        set => TotalTurnCost = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitState = new BattleInitState();
        PlayerState = new PlayerTurnState();
        EnemyState = new EnemyTurnState();
    }

    private void Start()
    {
        ChangeState(InitState);
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void ChangeState(ITurnState newState)
    {
        if (newState == null)
            return;

        CurrentState?.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    /// <summary>
    /// US-021: Logic Đầu Lượt - Tự động cập nhật lại Total Turn Cost dựa vào số tướng đang sống sót.
    /// </summary>
    public void RecalculateTurnCost(IEnumerable<BattleUnit> livingUnits)
    {
        int sumAP = 0;
        if (livingUnits != null)
        {
            foreach (var u in livingUnits)
            {
                if (u != null && !u.IsDead)
                {
                    sumAP += u.APContribution;
                }
            }
        }
        TotalTurnCost = sumAP;
        Debug.Log($"[TurnManager] Cập nhật Total Turn Cost đầu lượt: {TotalTurnCost} AP.");
    }

    /// <summary>
    /// US-022: Logic Đầu Lượt - Viết hàm tự động giảm thời gian Cooldown của các thẻ bài kỹ năng.
    /// </summary>
    public void ReduceSkillCooldowns(IEnumerable<CardData> handCards)
    {
        if (handCards != null)
        {
            foreach (var card in handCards)
            {
                if (card != null && card.CurrentCooldown > 0)
                {
                    card.CurrentCooldown--;
                    Debug.Log($"[TurnManager] Giảm Cooldown thẻ '{card.CardName}', còn: {card.CurrentCooldown}.");
                }
            }
        }
    }

    /// <summary>
    /// US-027: Tự động reset lớp Shield (Giáp ảo) về 0 khi kích hoạt pha End Turn.
    /// </summary>
    public void ResetShieldsOnEndTurn(IEnumerable<BattleUnit> allUnits)
    {
        if (allUnits != null)
        {
            foreach (var u in allUnits)
            {
                DamagePipeline.ResetTurnShield(u);
            }
        }
    }

    public void EndPlayerTurn()
    {
        if (CurrentState is PlayerTurnState)
        {
            ChangeState(EnemyState);
        }
    }

    public void EndEnemyTurn()
    {
        if (CurrentState is EnemyTurnState)
        {
            TurnCount++;
            ChangeState(PlayerState);
        }
    }
}
