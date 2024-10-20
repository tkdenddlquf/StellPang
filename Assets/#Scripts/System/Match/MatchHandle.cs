using System.Collections.Generic;

public class MatchHandle
{
    private List<Block> RecordBlcoks { get; } = new();

    private readonly MatchSystem matchSystem;

    public MatchHandle(MatchSystem _system)
    {
        matchSystem = _system;
    }

    public bool CheckT(Block _block, int _x, int _y)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, _x * i, _y * i])) return false;
        }

        matchSystem.RotateDir(ref _x, ref _y);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, LevelManager.Instance.blockHandle[RecordBlcoks[1].Pos, _x * i, _y * i])) return false;
        }

        AddRemoveList();

        return true;
    }

    public bool CheckL(Block _block, int _x, int _y)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, _x * i, _y * i])) return false;
        }

        matchSystem.RotateDir(ref _x, ref _y);

        for (int i = 1; i < 3; i++)
        {
            if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, _x * i, _y * i])) return false;
        }

        AddRemoveList();

        return true;
    }

    public bool CheckBox(Block _block)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, 0, 1])) return false;
        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, 1, 0])) return false;
        if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, 1, 1])) return false;

        AddRemoveList();

        return true;
    }

    public bool CheckLine(Block _block, int _x, int _y, int _max)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < _max; i++)
        {
            if (!CheckSameType(_block, LevelManager.Instance.blockHandle[_block.Pos, _x * i, _y * i])) return false;
        }

        AddRemoveList();

        return true;
    }

    private bool CheckSameType(Block _block_1, Block _block_2)
    {
        if (!matchSystem.IsCheckable(_block_2)) return false;

        if (_block_1.CheckPangType(_block_2)) RecordBlcoks.Add(_block_2);
        else return false;

        return true;
    }

    private void AddRemoveList()
    {
        for (int i = 0; i < RecordBlcoks.Count; i++)
        {
            matchSystem.RemoveBlcoks.Add(RecordBlcoks[i]);
            matchSystem.AllBlcoks.Remove(RecordBlcoks[i]);
        }
    }

    public Block GetSwapBlock(int _index)
    {
        for (int i = 0; i < RecordBlcoks.Count; i++)
        {
            if (RecordBlcoks[i] == LevelManager.Instance.blockHandle.selectBlocks[0])
            {
                matchSystem.RemoveBlcoks.Remove(RecordBlcoks[i]);

                return LevelManager.Instance.blockHandle.selectBlocks[0];
            }
            if (RecordBlcoks[i] == LevelManager.Instance.blockHandle.selectBlocks[1])
            {
                matchSystem.RemoveBlcoks.Remove(RecordBlcoks[i]);

                return LevelManager.Instance.blockHandle.selectBlocks[1];
            }
        }

        matchSystem.RemoveBlcoks.Remove(RecordBlcoks[_index]);

        return RecordBlcoks[_index];
    }
}
