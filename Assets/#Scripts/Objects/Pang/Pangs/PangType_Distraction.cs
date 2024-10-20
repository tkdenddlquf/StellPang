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

        pang.TargetBlock.BlockState = BlockState.Filled;

        for (int i = 1; ; i++)
        {
            nextBlock = LevelManager.Instance.blockHandle[pang.TargetBlock.Pos, 0, -i];

            if (nextBlock != null) nextBlock.Blocked = true;
            else if (LevelManager.Instance.blockHandle.CheckOutBlockIndex(pang.TargetBlock.Pos, 0, -i)) break;
        }
    }

    public override void OnDestroy()
    {
        hp--;

        if (hp == 0)
        {
            IsDestroy = true;
            pang.TargetBlock = null;

            ObjectManager.Instance.pangs.Enqueue(pang);
            IsDestroy = false;
        }
    }
}
