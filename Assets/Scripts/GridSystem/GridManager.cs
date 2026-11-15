using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public const int ROWS = 2; // 0 = Frontline, 1 = Backline
    public const int COLS = 3; // Cột 0, 1, 2

    [Header("Grid Layout Settings")]
    public float CellSize = 2.0f;
    public Vector3 PlayerGridOrigin = new Vector3(-4f, 0f, 0f);
    public Vector3 EnemyGridOrigin = new Vector3(2f, 0f, 0f);

    public BattleUnit[,] PlayerGrid { get; private set; } = new BattleUnit[ROWS, COLS];
    public BattleUnit[,] EnemyGrid { get; private set; } = new BattleUnit[ROWS, COLS];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeGrids();
    }

    public void InitializeGrids()
    {
        PlayerGrid = new BattleUnit[ROWS, COLS];
        EnemyGrid = new BattleUnit[ROWS, COLS];
    }

    public BattleUnit[,] GetGrid(bool isPlayerSide)
    {
        if (PlayerGrid == null || EnemyGrid == null)
        {
            InitializeGrids();
        }
        return isPlayerSide ? PlayerGrid : EnemyGrid;
    }

    public bool IsValidCoordinate(int row, int col)
    {
        return row >= 0 && row < ROWS && col >= 0 && col < COLS;
    }

    /// <summary>
    /// Đặt nhân vật/quái vật vào ô (row, col) trên lưới của phe tương ứng.
    /// </summary>
    public bool PlaceUnit(BattleUnit unit, bool isPlayerSide, int row, int col)
    {
        if (unit == null)
        {
            Debug.LogWarning("[GridManager] Không thể đặt BattleUnit null lên lưới.");
            return false;
        }

        if (!IsValidCoordinate(row, col))
        {
            Debug.LogWarning($"[GridManager] Tọa độ ({row}, {col}) nằm ngoài phạm vi lưới 2x3!");
            return false;
        }

        BattleUnit[,] grid = GetGrid(isPlayerSide);
        if (grid[row, col] != null && grid[row, col] != unit && !grid[row, col].IsDead)
        {
            Debug.LogWarning($"[GridManager] Ô ({(isPlayerSide ? "Player" : "Enemy")}, Row {row}, Col {col}) đã có '{grid[row, col].UnitName}'.");
            return false;
        }

        grid[row, col] = unit;
        unit.IsPlayerSide = isPlayerSide;
        unit.Row = row;
        unit.Column = col;
        unit.transform.position = GetWorldPosition(isPlayerSide, row, col);

        string rowName = row == (int)GridRow.Frontline ? "Frontline(0)" : "Backline(1)";
        Debug.Log($"[GridManager] Đã đặt '{unit.UnitName}' vào phe {(isPlayerSide ? "Player" : "Enemy")} tại [{rowName}, Col {col}].");
        return true;
    }

    /// <summary>
    /// Kiểm tra Luật Khóa Tiền Tuyến (Frontline Lock) và Khiêu khích (Taunt) cho đòn đánh đơn mục tiêu.
    /// </summary>
    public bool IsValidSingleTarget(BattleUnit attacker, BattleUnit target, bool ignoreFrontline = false)
    {
        if (target == null || target.IsDead)
        {
            return false;
        }

        BattleUnit[,] targetGrid = GetGrid(target.IsPlayerSide);

        // 1. Kiểm tra Khiêu khích (Taunt): Nếu phe bị đánh có bất kỳ ai còn sống đang bật HasTaunt == true,
        // bắt buộc chỉ trả về true cho mục tiêu có HasTaunt == true.
        bool anyUnitHasTaunt = false;
        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                BattleUnit unitOnGrid = targetGrid[r, c];
                if (unitOnGrid != null && !unitOnGrid.IsDead && unitOnGrid.HasTaunt)
                {
                    anyUnitHasTaunt = true;
                    break;
                }
            }
            if (anyUnitHasTaunt) break;
        }

        if (anyUnitHasTaunt)
        {
            return target.HasTaunt;
        }

        // 2. Kiểm tra Khóa Tiền Tuyến (Frontline Lock):
        // Nếu không có Taunt và ignoreFrontline == false: Chỉ cho phép đánh mục tiêu ở Backline (row = 1)
        // khi ô Frontline (row = 0) cùng cột đó đang trống (null) hoặc đã chết.
        bool bypassFrontline = ignoreFrontline || (attacker != null && attacker.IgnoreFrontlineLock);
        if (!bypassFrontline && target.Row == (int)GridRow.Backline)
        {
            BattleUnit frontlineUnit = targetGrid[(int)GridRow.Frontline, target.Column];
            if (frontlineUnit != null && !frontlineUnit.IsDead)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Trả về danh sách mục tiêu trên cùng cột col (Hàng trước là mục tiêu chính 100% sát thương, Hàng sau là mục tiêu phụ 50% sát thương).
    /// </summary>
    public List<BattleUnit> GetPierceTargets(bool targetPlayerSide, int col)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        if (col < 0 || col >= COLS)
        {
            return targets;
        }

        BattleUnit[,] grid = GetGrid(targetPlayerSide);

        // Hàng trước (Row 0 - Frontline): Mục tiêu chính (100% sát thương)
        BattleUnit frontUnit = grid[(int)GridRow.Frontline, col];
        if (frontUnit != null && !frontUnit.IsDead)
        {
            targets.Add(frontUnit);
        }

        // Hàng sau (Row 1 - Backline): Mục tiêu phụ (50% sát thương)
        BattleUnit backUnit = grid[(int)GridRow.Backline, col];
        if (backUnit != null && !backUnit.IsDead)
        {
            targets.Add(backUnit);
        }

        return targets;
    }

    /// <summary>
    /// Lấy hệ số sát thương xuyên thấu theo vị trí hàng (Frontline = 100%, Backline = 50%).
    /// </summary>
    public float GetPierceDamageRatio(BattleUnit target)
    {
        if (target == null) return 0f;
        return target.Row == (int)GridRow.Frontline ? 1.0f : 0.5f;
    }

    /// <summary>
    /// Trả về mục tiêu ở ô (row, centerCol) và các ô liền kề trái/phải (col - 1, col + 1) trên cùng hàng ngang row.
    /// </summary>
    public List<BattleUnit> GetBlastTargets(bool targetPlayerSide, int row, int centerCol)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        if (!IsValidCoordinate(row, centerCol))
        {
            return targets;
        }

        BattleUnit[,] grid = GetGrid(targetPlayerSide);

        // Mục tiêu trung tâm (row, centerCol)
        BattleUnit centerUnit = grid[row, centerCol];
        if (centerUnit != null && !centerUnit.IsDead)
        {
            targets.Add(centerUnit);
        }

        // Ô liền kề bên trái (col - 1)
        int leftCol = centerCol - 1;
        if (leftCol >= 0)
        {
            BattleUnit leftUnit = grid[row, leftCol];
            if (leftUnit != null && !leftUnit.IsDead)
            {
                targets.Add(leftUnit);
            }
        }

        // Ô liền kề bên phải (col + 1)
        int rightCol = centerCol + 1;
        if (rightCol < COLS)
        {
            BattleUnit rightUnit = grid[row, rightCol];
            if (rightUnit != null && !rightUnit.IsDead)
            {
                targets.Add(rightUnit);
            }
        }

        return targets;
    }

    /// <summary>
    /// Di chuyển chiến thuật: Nếu unit.IsRooted == false và turnManager.TotalTurnCost >= 1 thì trừ 1 Cost,
    /// cho phép di chuyển sang ô trống hoặc hoán đổi vị trí (Swap) với đồng minh đang đứng ở ô (targetRow, targetCol).
    /// </summary>
    public bool TryTacticalMove(BattleUnit unit, int targetRow, int targetCol, TurnManager turnManager)
    {
        if (unit == null || unit.IsDead)
        {
            Debug.LogWarning("[GridManager] Thực thể không hợp lệ hoặc đã bị hạ gục.");
            return false;
        }

        if (!IsValidCoordinate(targetRow, targetCol))
        {
            Debug.LogWarning($"[GridManager] Tọa độ đích ({targetRow}, {targetCol}) không hợp lệ.");
            return false;
        }

        if (unit.IsRooted)
        {
            Debug.LogWarning($"[GridManager] '{unit.UnitName}' đang bị Trói chân (IsRooted = true), không thể di chuyển!");
            return false;
        }

        if (turnManager == null || turnManager.TotalTurnCost < 1)
        {
            Debug.LogWarning($"[GridManager] Không đủ Cost để di chuyển chiến thuật (TotalTurnCost hiện tại: {(turnManager != null ? turnManager.TotalTurnCost : 0)}).");
            return false;
        }

        int oldRow = unit.Row;
        int oldCol = unit.Column;
        if (oldRow == targetRow && oldCol == targetCol)
        {
            Debug.LogWarning($"[GridManager] '{unit.UnitName}' đã đứng sẵn tại ô ({targetRow}, {targetCol}).");
            return false;
        }

        BattleUnit[,] grid = GetGrid(unit.IsPlayerSide);
        BattleUnit occupant = grid[targetRow, targetCol];

        // Trừ 1 Cost cho hành động di chuyển chiến thuật
        turnManager.TotalTurnCost -= 1;

        if (occupant == null || occupant.IsDead)
        {
            // Di chuyển sang ô trống
            grid[oldRow, oldCol] = null;
            grid[targetRow, targetCol] = unit;

            unit.Row = targetRow;
            unit.Column = targetCol;
            unit.transform.position = GetWorldPosition(unit.IsPlayerSide, targetRow, targetCol);

            Debug.Log($"[GridManager] '{unit.UnitName}' di chuyển từ ({oldRow}, {oldCol}) sang ô trống ({targetRow}, {targetCol}). Cost còn lại: {turnManager.TotalTurnCost}.");
        }
        else
        {
            // Hoán đổi vị trí (Swap) với đồng minh đang đứng ở ô đích
            grid[oldRow, oldCol] = occupant;
            occupant.Row = oldRow;
            occupant.Column = oldCol;
            occupant.transform.position = GetWorldPosition(occupant.IsPlayerSide, oldRow, oldCol);

            grid[targetRow, targetCol] = unit;
            unit.Row = targetRow;
            unit.Column = targetCol;
            unit.transform.position = GetWorldPosition(unit.IsPlayerSide, targetRow, targetCol);

            Debug.Log($"[GridManager] Hoán đổi vị trí (Swap) thành công giữa '{unit.UnitName}' ({targetRow}, {targetCol}) và '{occupant.UnitName}' ({oldRow}, {oldCol}). Cost còn lại: {turnManager.TotalTurnCost}.");
        }

        return true;
    }

    public Vector3 GetWorldPosition(bool isPlayerSide, int row, int col)
    {
        Vector3 origin = isPlayerSide ? PlayerGridOrigin : EnemyGridOrigin;
        float rowDir = isPlayerSide ? -1f : 1f;
        return origin + new Vector3(row * rowDir * CellSize, col * CellSize, 0f);
    }

    [ContextMenu("Test Grid 2x3 Rules")]
    public void TestGrid2x3Rules()
    {
        Debug.Log("========== BẮT ĐẦU TEST LUẬT LƯỚI 2x3 ==========");
        InitializeGrids();

        // Tạo các GameObject tạm thời để giả lập
        GameObject paladinGO = new GameObject("Temp_Paladin");
        BattleUnit paladin = paladinGO.AddComponent<BattleUnit>();
        paladin.SetupUnit("Paladin Ánh Sáng", 1800, 20, 80, 5);

        GameObject sniperGO = new GameObject("Temp_Sniper");
        BattleUnit sniper = sniperGO.AddComponent<BattleUnit>();
        sniper.SetupUnit("Sniper Bắn Tỉa", 900, 65, 25, 4);

        GameObject goblin1GO = new GameObject("Temp_Goblin_Col0");
        BattleUnit goblin1 = goblin1GO.AddComponent<BattleUnit>();
        goblin1.SetupUnit("Goblin Trinh Sát", 500, 30, 10, 0, false);

        GameObject goblin2GO = new GameObject("Temp_Goblin_Col1");
        BattleUnit goblin2 = goblin2GO.AddComponent<BattleUnit>();
        goblin2.SetupUnit("Goblin Chiến Binh", 600, 35, 15, 0, false);

        GameObject goblin3GO = new GameObject("Temp_Goblin_Col2");
        BattleUnit goblin3 = goblin3GO.AddComponent<BattleUnit>();
        goblin3.SetupUnit("Goblin Cung Thủ", 450, 40, 10, 0, false);

        // 1. Đặt nhân vật lên lưới 2x3
        // Phe Player: Paladin ở Frontline (0, 0), Sniper ở Backline (1, 0)
        PlaceUnit(paladin, true, (int)GridRow.Frontline, 0);
        PlaceUnit(sniper, true, (int)GridRow.Backline, 0);

        // Phe Enemy: 3 Goblin dàn hàng ngang ở Frontline (Row 0, Col 0, 1, 2)
        PlaceUnit(goblin1, false, (int)GridRow.Frontline, 0);
        PlaceUnit(goblin2, false, (int)GridRow.Frontline, 1);
        PlaceUnit(goblin3, false, (int)GridRow.Frontline, 2);

        // 2. Kiểm tra Luật Khóa Tiền Tuyến (Frontline Lock) và Khiêu khích (Taunt)
        bool canHitSniperNormal = IsValidSingleTarget(goblin1, sniper, ignoreFrontline: false);
        bool canHitPaladinNormal = IsValidSingleTarget(goblin1, paladin, ignoreFrontline: false);
        bool canHitSniperBypass = IsValidSingleTarget(goblin1, sniper, ignoreFrontline: true);

        Debug.Log($"[Test Frontline Lock] Goblin đánh Sniper (Hàng sau, có Paladin chắn): {canHitSniperNormal} (Kỳ vọng: False)");
        Debug.Log($"[Test Frontline Lock] Goblin đánh Paladin (Hàng trước): {canHitPaladinNormal} (Kỳ vọng: True)");
        Debug.Log($"[Test Ignore Frontline] Goblin (IgnoreFrontline = true) đánh lén Sniper: {canHitSniperBypass} (Kỳ vọng: True)");

        // Bật Taunt cho Paladin
        paladin.HasTaunt = true;
        bool canHitSniperWhenPaladinTaunts = IsValidSingleTarget(goblin1, sniper, ignoreFrontline: true);
        bool canHitPaladinWhenTaunts = IsValidSingleTarget(goblin1, paladin, ignoreFrontline: true);
        Debug.Log($"[Test Taunt] Khi Paladin bật Taunt -> Goblin đánh Sniper: {canHitSniperWhenPaladinTaunts} (Kỳ vọng: False), đánh Paladin: {canHitPaladinWhenTaunts} (Kỳ vọng: True)");
        paladin.HasTaunt = false;

        // 3. Kiểm tra Pierce (Xuyên thấu dọc cột 0 của phe Player)
        List<BattleUnit> pierceTargets = GetPierceTargets(targetPlayerSide: true, col: 0);
        Debug.Log($"[Test Pierce] Đòn xuyên thấu cột 0 phe Player trúng {pierceTargets.Count} mục tiêu:");
        foreach (BattleUnit t in pierceTargets)
        {
            float ratio = GetPierceDamageRatio(t) * 100f;
            Debug.Log($"   -> Mục tiêu: '{t.UnitName}' tại (Row {t.Row}, Col {t.Column}) nhận {ratio}% sát thương.");
        }

        // 4. Kiểm tra Blast (Nổ lan hàng ngang Row 0, tâm Col 1 của phe Enemy)
        List<BattleUnit> blastTargets = GetBlastTargets(targetPlayerSide: false, row: (int)GridRow.Frontline, centerCol: 1);
        Debug.Log($"[Test Blast] Đòn nổ lan tại (Row 0, Tâm Col 1) phe Enemy trúng {blastTargets.Count} mục tiêu:");
        foreach (BattleUnit t in blastTargets)
        {
            Debug.Log($"   -> Mục tiêu: '{t.UnitName}' tại (Row {t.Row}, Col {t.Column}).");
        }

        // 5. Kiểm tra Tactical Move & Swap
        TurnManager turnMgr = GetComponent<TurnManager>();
        bool tempTurnMgrCreated = false;
        if (turnMgr == null)
        {
            turnMgr = gameObject.AddComponent<TurnManager>();
            tempTurnMgrCreated = true;
        }
        turnMgr.TotalTurnCost = 2;

        // Hoán đổi (Swap) giữa Paladin (0, 0) và Sniper (1, 0)
        bool swapResult = TryTacticalMove(paladin, (int)GridRow.Backline, 0, turnMgr);
        Debug.Log($"[Test Swap] Hoán đổi Paladin và Sniper: {swapResult} | Vị trí mới Paladin: ({paladin.Row}, {paladin.Column}), Sniper: ({sniper.Row}, {sniper.Column}) | Cost còn: {turnMgr.TotalTurnCost}");

        // Thử di chuyển khi bị Rooted
        sniper.IsRooted = true;
        bool rootedMoveResult = TryTacticalMove(sniper, (int)GridRow.Frontline, 1, turnMgr);
        Debug.Log($"[Test Rooted] Sniper bị Rooted thử di chuyển: {rootedMoveResult} (Kỳ vọng: False)");
        sniper.IsRooted = false;

        // Di chuyển Sniper sang ô trống (0, 1)
        bool emptyMoveResult = TryTacticalMove(sniper, (int)GridRow.Frontline, 1, turnMgr);
        Debug.Log($"[Test Move Empty] Sniper di chuyển sang ô trống (0, 1): {emptyMoveResult} | Vị trí mới Sniper: ({sniper.Row}, {sniper.Column}) | Cost còn: {turnMgr.TotalTurnCost}");

        // Dọn dẹp các GameObject tạm sau khi test
        DestroyTempObject(paladinGO);
        DestroyTempObject(sniperGO);
        DestroyTempObject(goblin1GO);
        DestroyTempObject(goblin2GO);
        DestroyTempObject(goblin3GO);
        if (tempTurnMgrCreated)
        {
            DestroyTempObject(turnMgr);
        }
        InitializeGrids();

        Debug.Log("========== HOÀN TẤT TEST LUẬT LƯỚI 2x3 ==========");
    }

    private void DestroyTempObject(Object obj)
    {
        if (obj == null) return;
        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
            DestroyImmediate(obj);
        }
    }
}
