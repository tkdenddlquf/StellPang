using System.Collections.Generic;
using UnityEngine;

public class MatchHandle
{
    private List<Block> RecordBlcoks { get; } = new();

    private readonly MatchSystem matchSystem;

    public MatchHandle(MatchSystem _system)
    {
        matchSystem = _system;
    }

    public bool CheckT(Block _block, Vector2Int pos)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            Vector2Int checkPos = pos * i;

            if (!CheckSameType(_block, blockHandle[_block.Pos, checkPos])) return false;
        }

        pos = matchSystem.RotateDir(pos);

        for (int i = 1; i < 3; i++)
        {
            Vector2Int checkPos = pos * i;

            if (!CheckSameType(_block, blockHandle[RecordBlcoks[1].Pos, checkPos])) return false;
        }

        AddRemoveList();

        return true;
    }

    public bool CheckL(Block _block, Vector2Int pos)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < 3; i++)
        {
            Vector2Int checkPos = pos * i;

            if (!CheckSameType(_block, blockHandle[_block.Pos, checkPos])) return false;
        }

        pos = matchSystem.RotateDir(pos);

        for (int i = 1; i < 3; i++)
        {
            Vector2Int checkPos = pos * i;

            if (!CheckSameType(_block, blockHandle[_block.Pos, checkPos])) return false;
        }

        AddRemoveList();

        return true;
    }

    public bool CheckBox(Block _block)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        if (!CheckSameType(_block, blockHandle[_block.Pos, Vector2Int.up])) return false;
        if (!CheckSameType(_block, blockHandle[_block.Pos, Vector2Int.right])) return false;
        if (!CheckSameType(_block, blockHandle[_block.Pos, Vector2Int.one])) return false;

        AddRemoveList();

        return true;
    }

    public bool CheckLine(Block _block, Vector2Int pos, int _max)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 1; i < _max; i++)
        {
            Vector2Int checkPos = pos * i;

            if (!CheckSameType(_block, blockHandle[_block.Pos, checkPos])) return false;
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
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        for (int i = 0; i < RecordBlcoks.Count; i++)
        {
            if (RecordBlcoks[i] == blockHandle.selectBlocks[0])
            {
                matchSystem.RemoveBlcoks.Remove(RecordBlcoks[i]);

                return blockHandle.selectBlocks[0];
            }

            if (RecordBlcoks[i] == blockHandle.selectBlocks[1])
            {
                matchSystem.RemoveBlcoks.Remove(RecordBlcoks[i]);

                return blockHandle.selectBlocks[1];
            }
        }

        matchSystem.RemoveBlcoks.Remove(RecordBlcoks[_index]);

        return RecordBlcoks[_index];
    }
}
