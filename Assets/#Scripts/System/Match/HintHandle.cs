using UnityEngine;

public class HintHandle
{
    public Block hint;
    public Block record;

    public readonly int[] dir = new int[2];

    private readonly MatchSystem matchSystem;

    public HintHandle(MatchSystem _system)
    {
        matchSystem = _system;
    }

    public bool CheckHint()
    {
        hint = null;

        matchSystem.checkVector[0] = (int)LevelManager.Instance.spawnHandle.SpawnVector.x;
        matchSystem.checkVector[1] = (int)LevelManager.Instance.spawnHandle.SpawnVector.y;

        for (int i = 0; i < BoardCreator.Instance.boardSize[0]; i++)
        {
            for (int j = 0; j < BoardCreator.Instance.boardSize[1]; j++)
            {
                if (!IsCheckable(BoardCreator.Instance[i, j])) continue;

                for (int k = 0; k < 4; k++)
                {
                    hint = CheckSideUp(BoardCreator.Instance[i, j], matchSystem.checkVector[0], matchSystem.checkVector[1], true);

                    if (hint != null) return true;

                    hint = CheckSideUp(BoardCreator.Instance[i, j], matchSystem.checkVector[0], matchSystem.checkVector[1], false);

                    if (hint != null) return true;

                    hint = CheckBothUp(BoardCreator.Instance[i, j], matchSystem.checkVector[0], matchSystem.checkVector[1]);

                    if (hint != null) return true;

                    hint = CheckOneWay(BoardCreator.Instance[i, j], matchSystem.checkVector[0], matchSystem.checkVector[1]);

                    if (hint != null) return true;

                    hint = CheckBox(BoardCreator.Instance[i, j], matchSystem.checkVector[0], matchSystem.checkVector[1]);

                    if (hint != null) return true;

                    matchSystem.RotateDir(ref matchSystem.checkVector[0], ref matchSystem.checkVector[1]);
                }
            }
        }

        return false;
    }

    private Block CheckSideUp(Block _block, int _x, int _y, bool _left)
    {
        if (LevelManager.Instance.blockHandle.CheckOutBlockIndex(_block.Pos, _x, _y)) return null;

        record = LevelManager.Instance.blockHandle[_block.Pos, _x, _y];

        matchSystem.RotateDir(ref _x, ref _y, _left);

        record = LevelManager.Instance.blockHandle[record.Pos, _x, _y];

        if (!CheckSameType(_block, record)) return null;

        matchSystem.RotateDir(ref _x, ref _y, _left);

        if (!IsMoveable(LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;

        matchSystem.RotateDir(ref _x, ref _y, _left);

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, _x, _y])) return null;

        return record;
    }

    private Block CheckBothUp(Block _block, int _x, int _y)
    {
        if (LevelManager.Instance.blockHandle.CheckOutBlockIndex(_block.Pos, _x, _y)) return null;

        record = LevelManager.Instance.blockHandle[_block.Pos, _x, _y];

        if (!IsMoveable(record)) return null;

        matchSystem.RotateDir(ref _x, ref _y);

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;

        matchSystem.RotateDir(ref _x, ref _y, false);
        matchSystem.RotateDir(ref _x, ref _y, false);

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;

        return _block;
    }

    private Block CheckOneWay(Block _block, int _x, int _y)
    {
        if (LevelManager.Instance.blockHandle.CheckOutBlockIndex(_block.Pos, _x, _y)) return null;

        record = LevelManager.Instance.blockHandle[_block.Pos, _x, _y];

        if (!IsMoveable(record)) return null;

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;
        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x * 2, _y * 2])) return null;

        return _block;
    }

    private Block CheckBox(Block _block, int _x, int _y)
    {
        if (LevelManager.Instance.blockHandle.CheckOutBlockIndex(_block.Pos, _x, _y)) return null;

        record = LevelManager.Instance.blockHandle[_block.Pos, _x, _y];

        if (!IsMoveable(record)) return null;

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;

        matchSystem.RotateDir(ref _x, ref _y);

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;

        matchSystem.RotateDir(ref _x, ref _y, false);

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[record.Pos, _x, _y])) return null;

        return _block;
    }

    private bool CheckSameType(Block _block_1, Block _block_2)
    {
        if (!IsCheckable(_block_2)) return false;
        if (!_block_1.CheckPangType(_block_2)) return false;

        return true;
    }

    private bool IsCheckable(Block _block)
    {
        if (_block == null) return false;
        if (_block.TargetPang == null) return false;
        if (_block.TargetPang.PangType != PangType.Pastel) return false;

        return true;
    }

    private bool IsMoveable(Block _block)
    {
        if (_block == null) return false;

        if (_block.TargetPang != null)
        {
            if (_block.TargetPang.PangType == PangType.Distraction) return false;
        }

        return true;
    }
}
