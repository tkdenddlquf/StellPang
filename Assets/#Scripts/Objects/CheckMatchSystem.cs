using System.Collections.Generic;
using UnityEngine;

public class CheckMatchSystem
{
    private Block checkBlock;

    public List<Block> CheckBlcoks { get; } = new();

    private List<Block> RecordBlcoks { get; } = new();
    private List<Block> RemoveBlcoks { get; } = new();

    private readonly LevelManager levelManager;

    public CheckMatchSystem(LevelManager _levelManager)
    {
        levelManager = _levelManager;
    }

    public void CheckMatch()
    {
        RemoveBlcoks.Clear();

        while (CheckBlcoks.Count > 0)
        {
            if (CheckLine(Directions.Right, CheckBlcoks[0], 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombHori);
            }
            else if (CheckLine(Directions.Down, CheckBlcoks[0], 4))
            {
                RecordBlcoks[0].TargetPang.SetType(ItemType.BombVert);
            }

            CheckBlcoks.Remove(RecordBlcoks[0]);
        }
    }

    private bool CheckLine(Directions _dir, Block _block, int _max)
    {
        RecordBlcoks.Clear();
        RecordBlcoks.Add(_block);

        for (int i = 0; i < _max; i++)
        {
            checkBlock = levelManager.NextBlock(_dir, _block.pos, i + 1);

            if (checkBlock == null) break;

            if (checkBlock.TargetPang.PangTypeNum == _block.TargetPang.PangTypeNum) RecordBlcoks.Add(checkBlock);
            else break;
        }

        if (RecordBlcoks.Count == _max)
        {
            Debug.Log(RecordBlcoks.Count);

            AddRemoveList();

            return true;
        }

        return false;
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
