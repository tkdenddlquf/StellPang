using UnityEngine;

public class HintHandle
{
    public Pang hint;
    public Block record;

    private readonly MatchSystem matchSystem;

    public HintHandle(MatchSystem _system)
    {
        matchSystem = _system;
    }

    public bool CheckHint()
    {
        BoardCreator boardCreator = BoardCreator.Instance;
        SpawnHandle spawnHandle = LevelManager.Instance.spawnHandle;

        hint = null;

        matchSystem.CheckDir = spawnHandle.SpawnDir;

        for (int i = 0; i < boardCreator.BoardSize[0]; i++)
        {
            for (int j = 0; j < boardCreator.BoardSize[1]; j++)
            {
                Vector2Int pos = new(i, j);
                Block block = boardCreator[pos];

                if (!IsCheckable(block)) continue;

                for (int k = 0; k < 4; k++)
                {
                    hint = CheckSideUp(block, matchSystem.CheckDir, true);

                    if (hint != null) return true;

                    hint = CheckSideUp(block, matchSystem.CheckDir, false);

                    if (hint != null) return true;

                    hint = CheckBothUp(block, matchSystem.CheckDir);

                    if (hint != null) return true;

                    hint = CheckOneWay(block, matchSystem.CheckDir);

                    if (hint != null) return true;

                    hint = CheckBox(block, matchSystem.CheckDir);

                    if (hint != null) return true;

                    matchSystem.CheckDir = matchSystem.RotateDir(matchSystem.CheckDir);
                }
            }
        }

        return false;
    }

    private Pang CheckSideUp(Block _block, Vector2Int pos, bool _left)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        if (blockHandle.CheckOutBlockIndex(_block.Pos, pos)) return null;

        record = blockHandle[_block.Pos, pos];

        pos = matchSystem.RotateDir(pos, _left);

        if (record == null) return null;

        record = blockHandle[record.Pos, pos];

        if (!CheckSameType(_block, record)) return null;

        pos = matchSystem.RotateDir(pos, _left);

        if (!IsMoveable(blockHandle[record.Pos, pos])) return null;

        pos = matchSystem.RotateDir(pos, _left);

        if (!CheckSameType(_block, blockHandle[_block.Pos, pos])) return null;

        return record.TargetPang;
    }

    private Pang CheckBothUp(Block _block, Vector2Int pos)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        if (blockHandle.CheckOutBlockIndex(_block.Pos, pos)) return null;

        record = blockHandle[_block.Pos, pos];

        if (!IsMoveable(record)) return null;

        pos = matchSystem.RotateDir(pos);

        if (!CheckSameType(_block, blockHandle[record.Pos, pos])) return null;

        pos = matchSystem.RotateDir(pos, false);
        pos = matchSystem.RotateDir(pos, false);

        if (!CheckSameType(_block, blockHandle[record.Pos, pos])) return null;

        return _block.TargetPang;
    }

    private Pang CheckOneWay(Block _block, Vector2Int pos)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        if (blockHandle.CheckOutBlockIndex(_block.Pos, pos)) return null;

        record = blockHandle[_block.Pos, pos];

        if (!IsMoveable(record)) return null;

        if (!CheckSameType(_block, blockHandle[record.Pos, pos])) return null;
        if (!CheckSameType(_block, blockHandle[record.Pos, pos * 2])) return null;

        return _block.TargetPang;
    }

    private Pang CheckBox(Block _block, Vector2Int pos)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        if (blockHandle.CheckOutBlockIndex(_block.Pos, pos)) return null;

        record = blockHandle[_block.Pos, pos];

        if (!IsMoveable(record)) return null;

        record = blockHandle[record.Pos, pos];

        if (!CheckSameType(_block, record)) return null;

        pos = matchSystem.RotateDir(pos);

        record = blockHandle[record.Pos, pos];

        if (!CheckSameType(_block, record)) return null;

        pos = matchSystem.RotateDir(pos);

        record = blockHandle[record.Pos, pos];

        if (!CheckSameType(_block, record)) return null;

        return _block.TargetPang;
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
