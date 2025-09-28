using UnityEngine;

public class PangType_Distraction : PangTypeBase
{
    private int hp;

    public PangType_Distraction(Pang _pang) : base(_pang)
    {
        switch (pang.PangTypeNum)
        {
            case 0: // Stone
                hp = -1;
                break;

            case 1: // Box
                hp = 2;
                break;

            case 2: // Ice
                hp = 3;
                break;
        }
    }

    public override void OnMove()
    {
        if (pang.TargetBlock.BlockState == BlockState.Filled) return;

        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        pang.TargetBlock.BlockState = BlockState.Filled;

        for (int i = 1; ; i++)
        {
            Vector2Int currentPos = pang.TargetBlock.Pos;
            Vector2Int moveVector = new(0, -i);

            nextBlock = blockHandle[currentPos, moveVector];

            if (nextBlock != null) nextBlock.Blocked = true;
            else if (blockHandle.CheckOutBlockIndex(currentPos, moveVector)) break;
        }
    }

    public override void OnDestroy()
    {
        hp--;

        if (hp == 0)
        {
            IsDestroy = true;
            pang.TargetBlock = null;

            ObjectManager.Instance.PangPool.Release(pang);
            IsDestroy = false;
        }
    }
}
