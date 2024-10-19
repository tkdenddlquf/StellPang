using System.Collections.Generic;

public class CheckMatchSystem
{
    private bool match;

    private List<Block> CheckBlcoks { get; } = new();

    private List<Block> RecordBlcoks { get; } = new();
    private HashSet<Block> RemoveBlcoks { get; } = new();

    private readonly int[] checkVector = new int[2];
    private readonly LevelManager levelManager;

    public CheckMatchSystem(LevelManager _levelManager)
    {
        levelManager = _levelManager;
    }

    public void CheckMatch()
    {
        if (levelManager.match) return;

        levelManager.match = true;

        CheckBlcoks.Clear();
        RemoveBlcoks.Clear();

        checkVector[0] = (int)levelManager.SpawnVector.x;
        checkVector[1] = (int)levelManager.SpawnVector.y;

        for (int i = 0; i < BoardCreator.Instance.boardSize[0]; i++)
        {
            for (int j = 0; j < BoardCreator.Instance.boardSize[1]; j++)
            {
                if (IsCheckable(BoardCreator.Instance[i, j])) CheckBlcoks.Add(BoardCreator.Instance[i, j]);
            }
        }

        for (int i = CheckBlcoks.Count - 1; i >= 0; i--)
        {
            match = false;

            for (int j = 0; j < 4; j++)
            {
                if (CheckT(CheckBlcoks[i], checkVector[0], checkVector[1]))
                {
                    match = true;
                    i -= 4;

                    GetSwapBlock(1).TargetPang.SetType(ItemType.BombLargeCross);

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1]);
            }

            if (match) continue;

            for (int j = 0; j < 4; j++)
            {
                if (CheckL(CheckBlcoks[i], checkVector[0], checkVector[1]))
                {
                    match = true;
                    i -= 4;

                    GetSwapBlock(0).TargetPang.SetType(ItemType.Bomb5x5);

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1]);
            }

            if (match) continue;

            if (CheckBox(CheckBlcoks[i])) GetSwapBlock(0).TargetPang.SetType(ItemType.BombSmallCross);
        }

        for (int i = CheckBlcoks.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < 4; j++)
            {
                if (CheckLine(CheckBlcoks[i], checkVector[0], checkVector[1], 4))
                {
                    i -= 3;

                    if (j % 2 == 0) GetSwapBlock(0).TargetPang.SetType(ItemType.BombVert);
                    else GetSwapBlock(0).TargetPang.SetType(ItemType.BombHori);

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1]);
            }
        }

        for (int i = CheckBlcoks.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < 4; j++)
            {
                if (CheckLine(CheckBlcoks[i], checkVector[0], checkVector[1], 3))
                {
                    i -= 2;

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1]);
            }
        }

        foreach (var _block in RemoveBlcoks) _block.TargetPang.StateBase.OnDestroy();

        levelManager.match = false;
        levelManager.ClearSelect();
    }

    private bool CheckT(Block _block, int _x, int _y)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) return false;
        }

        RotateDir(ref _x, ref _y);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[RecordBlcoks[1].Pos, _x * i, _y * i])) return false;
        }

        AddRemoveList();

        return true;
    }

    private bool CheckL(Block _block, int _x, int _y)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) return false;
        }

        RotateDir(ref _x, ref _y);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) return false;
        }

        AddRemoveList();

        return true;
    }

    private bool CheckBox(Block _block)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        if (!CheckSameType(_block, levelManager[_block.Pos, 0, 1])) return false;
        if (!CheckSameType(_block, levelManager[_block.Pos, 1, 0])) return false;
        if (!CheckSameType(_block, levelManager[_block.Pos, 1, 1])) return false;

        AddRemoveList();

        return true;
    }

    private bool CheckLine(Block _block, int _x, int _y, int _max)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < _max; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) return false;
        }

        AddRemoveList();

        return true;
    }

    private bool IsCheckable(Block _block)
    {
        if (_block == null) return false;
        if (_block.TargetPang == null) return false;
        if (_block.TargetPang.PangType != PangType.Pastel) return false;
        if (RemoveBlcoks.Contains(_block)) return false;

        return true;
    }

    private bool CheckSameType(Block _block_1, Block _block_2)
    {
        if (!IsCheckable(_block_2)) return false;

        if (_block_1.CheckPangType(_block_2)) RecordBlcoks.Add(_block_2);
        else return false;

        return true;
    }

    private void RotateDir(ref int _x, ref int _y, bool _minus = true)
    {
        if (_minus)
        {
            if (_x == 0 && _y == 1)
            {
                _x = -1;
                _y = 0;
            }
            else if (_x == -1 && _y == 0)
            {
                _x = 0;
                _y = -1;
            }
            else if (_x == 0 && _y == -1)
            {
                _x = 1;
                _y = 0;
            }
            else if (_x == 1 && _y == 0)
            {
                _x = 0;
                _y = 1;
            }
        }
        else
        {
            if (_x == 0 && _y == 1)
            {
                _x = 1;
                _y = 0;
            }
            else if (_x == 1 && _y == 0)
            {
                _x = 0;
                _y = -1;
            }
            else if (_x == 0 && _y == -1)
            {
                _x = -1;
                _y = 0;
            }
            else if (_x == -1 && _y == 0)
            {
                _x = 0;
                _y = 1;
            }
        }
    }

    private void AddRemoveList()
    {
        for (int i = 0; i < RecordBlcoks.Count; i++)
        {
            RemoveBlcoks.Add(RecordBlcoks[i]);
            CheckBlcoks.Remove(RecordBlcoks[i]);
        }
    }

    private Block GetSwapBlock(int _index)
    {
        for (int i = 0; i < RecordBlcoks.Count; i++)
        {
            if (RecordBlcoks[i] == LevelManager.Instance.selectBlocks[0])
            {
                RemoveBlcoks.Remove(RecordBlcoks[i]);

                return LevelManager.Instance.selectBlocks[0];
            }
            if (RecordBlcoks[i] == LevelManager.Instance.selectBlocks[1])
            {
                RemoveBlcoks.Remove(RecordBlcoks[i]);

                return LevelManager.Instance.selectBlocks[1];
            }
        }

        RemoveBlcoks.Remove(RecordBlcoks[_index]);

        return RecordBlcoks[_index];
    }
}
