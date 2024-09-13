using UnityEngine;

public class CheckMatchSystem
{
    private BoardCreator BoardCreator => GameManager._instance.BoardCreator;

    private int matchCount;
    private Block checkBlock;

    private readonly LevelManager levelManager;

    public CheckMatchSystem(LevelManager _levelManager)
    {
        levelManager = _levelManager;
    }

    public int CheckLine(Directions _dir, Block _block, int _max)
    {
        matchCount = 1;
        _max--;

        for (int i = 0; i < _max; i++)
        {
            checkBlock = levelManager.NextBlock(_dir, _block.pos);

            if (checkBlock == null) break;

            if (checkBlock.TargetPang.PangType == _block.TargetPang.PangType)
            {
                matchCount++;

                _block = checkBlock;
            }
        }

        return matchCount;
    }
}
