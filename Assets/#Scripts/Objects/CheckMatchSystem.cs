using System.Collections.Generic;

public class CheckMatchSystem
{
    private BoardCreator BoardCreator => GameManager._instance.BoardCreator;

    private Block checkBlock;

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
        CheckBlcoks.Clear();
        RemoveBlcoks.Clear();

        for (int i = 0; i < BoardCreator.boardSize[0]; i++)
        {
            for (int j = 0; j < BoardCreator.boardSize[1]; j++)
            {
                CheckBlcoks.Add(BoardCreator[i, j]);
            }
        }

        while (CheckBlcoks.Count > 0)
        {
            if (CheckT(Directions.Up, CheckBlcoks[0]))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }
            else if (CheckT(Directions.Right, CheckBlcoks[0]))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }
            else if (CheckT(Directions.Down, CheckBlcoks[0]))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }
            else if (CheckT(Directions.Left, CheckBlcoks[0]))
            {
                RecordBlcoks[1].TargetPang.SetType(ItemType.BombLargeCross);
                RemoveBlcoks.Remove(RecordBlcoks[1]);
            }

            else if (CheckL(Directions.Up, CheckBlcoks[0]))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckL(Directions.Right, CheckBlcoks[0]))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckL(Directions.Down, CheckBlcoks[0]))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckL(Directions.Left, CheckBlcoks[0]))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.Bomb5x5);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }

            else if (CheckLine(Directions.Right, CheckBlcoks[0], 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombHori);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }
            else if (CheckLine(Directions.Down, CheckBlcoks[0], 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombVert);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }

            else if (CheckBox(CheckBlcoks[0]))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombSmallCross);
                RemoveBlcoks.Remove(RecordBlcoks[0]);
            }

            else if (!CheckLine(Directions.Right, CheckBlcoks[0], 3))
            {
                CheckLine(Directions.Down, CheckBlcoks[0], 3);
            }

            if (RemoveBlcoks.Count == 0) CheckBlcoks.RemoveAt(0);
            else
            {
                while (RemoveBlcoks.Count > 0)
                {
                    levelManager.RemovePang(RemoveBlcoks[0].TargetPang);
                    RemoveBlcoks.RemoveAt(0);
                }
            }
        }

        levelManager.RefreshAllPangs();
    }

    private bool CheckT(Directions _dir, Block _block)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            checkBlock = levelManager[_dir, _block.Pos, i];

            if (!CheckSameType(_block)) break;
        }

        if (RecordBlcoks.Count == 3)
        {
            for (int i = 1; i < 3; i++)
            {
                checkBlock = levelManager[this[_dir], RecordBlcoks[1].Pos, i];

                if (!CheckSameType(_block)) break;
            }

            if (RecordBlcoks.Count == 5)
            {
                AddRemoveList();

                return true;
            }
        }

        return false;
    }

    private bool CheckL(Directions _dir, Block _block)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            checkBlock = levelManager[_dir, _block.Pos, i];

            if (!CheckSameType(_block)) break;
        }

        for (int i = 1; i < 3; i++)
        {
            checkBlock = levelManager[this[_dir], _block.Pos, i];

            if (!CheckSameType(_block)) break;
        }

        if (RecordBlcoks.Count == 5)
        {
            AddRemoveList();

            return true;
        }

        return false;
    }

    private bool CheckBox(Block _block)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        checkBlock = levelManager[Directions.Up, _block.Pos];

        if (!CheckSameType(_block)) return false;

        checkBlock = levelManager[Directions.Right, _block.Pos];

        if (!CheckSameType(_block)) return false;

        _block = checkBlock;

        checkBlock = levelManager[Directions.Up, _block.Pos];

        if (!CheckSameType(_block)) return false;

        if (RecordBlcoks.Count == 4)
        {
            AddRemoveList();

            return true;
        }

        return false;
    }

    private bool CheckLine(Directions _dir, Block _block, int _max)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < _max; i++)
        {
            checkBlock = levelManager[_dir, _block.Pos, i];

            if (!CheckSameType(_block)) break;
        }

        if (RecordBlcoks.Count == _max)
        {
            AddRemoveList();

            return true;
        }

        return false;
    }

    private bool CheckSameType(Block _block)
    {
        if (checkBlock == null) return false;
        if (checkBlock.TargetPang == null) return false;

        if (_block == null) return false;
        if (_block.TargetPang == null) return false;

        if (checkBlock.TargetPang.PangType != PangType.Pastel) return false;

        if (_block.CheckPangType(checkBlock)) RecordBlcoks.Add(checkBlock);
        else return false;

        return true;
    }

    private Directions this[Directions _dir, bool _minus = true]
    {
        get
        {
            if (_minus)
            {
                if (_dir == Directions.Up) return Directions.Left;
                else return _dir - 1;
            }
            else
            {
                if (_dir == Directions.Left) return Directions.Up;
                else return _dir + 1;
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
