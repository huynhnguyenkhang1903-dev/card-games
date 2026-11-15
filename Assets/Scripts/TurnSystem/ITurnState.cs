public interface ITurnState
{
    void EnterState(TurnManager turnManager);
    void UpdateState(TurnManager turnManager);
    void ExitState(TurnManager turnManager);
}

