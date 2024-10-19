using UnityEngine;

public class PangType_Distraction : PangTypeBase
{
    public PangType_Distraction(Pang _pang) : base(_pang)
    {

    }

    public override void OnMove()
    {
        if (pang.TargetBlock.BlockState == BlockState.Filled) return;

        pang.TargetBlock.BlockState = BlockState.Filled;

        for (int i = 1; ; i++)
        {
            nextBlock = LevelManager.Instance[pang.TargetBlock.Pos, 0, -i];

            if (nextBlock != null) nextBlock.Blocked = true;
            else if (LevelManager.Instance.CheckOutBlockIndex(pang.TargetBlock.Pos, 0, -i)) break;
        }
    }

    public override void OnDestroy()
    {
        pang.TargetBlock = null;
        ObjectManager.Instance.pangs.Enqueue(pang);
    }
}
