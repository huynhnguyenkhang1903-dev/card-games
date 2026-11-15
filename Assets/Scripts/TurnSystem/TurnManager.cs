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
