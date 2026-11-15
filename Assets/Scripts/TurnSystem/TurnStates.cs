using UnityEngine;

public class BattleInitState : ITurnState
{
    public void EnterState(TurnManager turnManager)
    {
        Debug.Log("[TurnSystem] Bắt đầu trạng thái khởi tạo trận đấu (BattleInitState).");
        turnManager.TurnCount = 1;

        // Chuyển sang lượt người chơi sau khi khởi tạo xong
        turnManager.ChangeState(turnManager.PlayerState);
    }

    public void UpdateState(TurnManager turnManager)
    {
    }

    public void ExitState(TurnManager turnManager)
    {
        Debug.Log("[TurnSystem] Kết thúc khởi tạo trận đấu.");
    }
}

public class PlayerTurnState : ITurnState
{
    public void EnterState(TurnManager turnManager)
    {
        Debug.Log($"[TurnSystem] Bắt đầu lượt Người Chơi (Turn {turnManager.TurnCount}).");
    }

    public void UpdateState(TurnManager turnManager)
    {
    }

    public void ExitState(TurnManager turnManager)
    {
        Debug.Log("[TurnSystem] Kết thúc lượt Người Chơi.");
    }
}

public class EnemyTurnState : ITurnState
{
    public void EnterState(TurnManager turnManager)
    {
        Debug.Log($"[TurnSystem] Bắt đầu lượt Kẻ Địch (Turn {turnManager.TurnCount}).");
    }

    public void UpdateState(TurnManager turnManager)
    {
    }

    public void ExitState(TurnManager turnManager)
    {
        Debug.Log("[TurnSystem] Kết thúc lượt Kẻ Địch.");
    }
}

