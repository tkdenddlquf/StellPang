using System.Collections;
using UnityEngine;

public class PangType_Item : PangTypeBase
{
    public PangType_Item(Pang _pang) : base(_pang)
    {

    }

    public override void OnMove()
    {
        if (pang.TargetBlock == null) return;
        if (LevelManager.Instance.match) return;

        if (pang.transform.position == pang.TargetBlock.transform.position)
        {
            nextBlock = LevelManager.Instance[pang.TargetBlock.Pos, 0, -1];

            if (nextBlock != null)
            {
                if (nextBlock.BlockState == BlockState.Reserved) return;

                if (nextBlock.BlockState == BlockState.Empty)
                {
                    pang.TargetBlock = nextBlock;

                    return;
                }
            }

            if (CheckSideBlock(1, -1)) return;
            if (CheckSideBlock(-1, -1)) return;

            pang.TargetBlock.BlockState = BlockState.Filled;

            IsMove = false;
        }
        else
        {
            IsMove = true;

            pang.transform.position = Vector2.MoveTowards(pang.transform.position, pang.TargetBlock.transform.position, Time.deltaTime * moveSpeed);
        }
    }
}
