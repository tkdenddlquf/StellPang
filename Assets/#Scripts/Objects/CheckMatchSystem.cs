using System.Collections.Generic;

public class CheckMatchSystem
{
    private List<Block> CheckBlcoks { get; } = new();

    private List<Block> RecordBlcoks { get; } = new();
    private List<Block> RemoveBlcoks { get; } = new();

    private readonly LevelManager levelManager;

    public CheckMatchSystem(LevelManager _levelManager)
    {
        levelManager = _levelManager;
    }

    public void CheckMatch()
    {
        levelManager.match = true;

        CheckBlcoks.Clear();
        RemoveBlcoks.Clear();

        for (int i = 0; i < BoardCreator.Instance.boardSize[0]; i++)
        {
            for (int j = 0; j < BoardCreator.Instance.boardSize[1]; j++)
            {
                CheckBlcoks.Add(BoardCreator.Instance[i, j]);
            }
        }

        while (CheckBlcoks.Count > 0)
        {
            if (!IsCheckable(CheckBlcoks[0]))
            {
                CheckBlcoks.RemoveAt(0);

                continue;
            }

            if (CheckT(CheckBlcoks[0], 0, 1))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }
            else if (CheckT(CheckBlcoks[0], 1, 0))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }
            else if (CheckT(CheckBlcoks[0], 0, -1))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }
            else if (CheckT(CheckBlcoks[0], -1, 0))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }

            else if (CheckL(CheckBlcoks[0], 0, 1))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckL(CheckBlcoks[0], 1, 0))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckL(CheckBlcoks[0], 0, -1))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckL(CheckBlcoks[0], -1, 0))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }

            else if (CheckLine(CheckBlcoks[0], 0, 1, 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombVert);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckLine(CheckBlcoks[0], 1, 0, 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombHori);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckLine(CheckBlcoks[0], 0, -1, 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombVert);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckLine(CheckBlcoks[0], -1, 0, 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombHori);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }

            else if (CheckBox(CheckBlcoks[0]))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombSmallCross);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }

            else if (!CheckLine(CheckBlcoks[0], 1, 0, 3))
            {
                CheckLine(CheckBlcoks[0], 0, -1, 3);
            }

            if (RemoveBlcoks.Count == 0) CheckBlcoks.RemoveAt(0);
            else
            {
                while (RemoveBlcoks.Count > 0)
                {
                    ObjectManager.Instance.pangs.Enqueue(RemoveBlcoks[0].TargetPang);
                    RemoveBlcoks.RemoveAt(0);
                }
            }
        }

        levelManager.match = false;
    }

    private bool CheckT(Block _block, int _x, int _y)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) break;
        }

        if (RecordBlcoks.Count != 3) return false;

        RotateDir(ref _x, ref _y);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[RecordBlcoks[1].Pos, _x * i, _y * i])) break;
        }

        if (RecordBlcoks.Count != 5) return false;

        AddRemoveList();

        return true;
    }

    private bool CheckL(Block _block, int _x, int _y)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) break;
        }

        RotateDir(ref _x, ref _y);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) break;
        }

        if (RecordBlcoks.Count != 5) return false;

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

        if (RecordBlcoks.Count != 4) return false;

        AddRemoveList();

        return true;
    }

    private bool CheckLine(Block _block, int _x, int _y, int _max)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < _max; i++)
        {
            if (!CheckSameType(_block, levelManager[_block.Pos, _x * i, _y * i])) break;
        }

        if (RecordBlcoks.Count != _max) return false;

        AddRemoveList();

        return true;
    }

    private bool IsCheckable(Block _block)
    {
        if (_block == null) return false;
        if (_block.TargetPang == null) return false;
        if (_block.TargetPang.PangType != PangType.Pastel) return false;

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
}
