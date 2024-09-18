using System.Collections;
using UnityEngine;

public class PangType_Item : PangTypeBase
{
    public PangType_Item(Pang _pang) : base(_pang)
    {

    }

    public override void Move()
    {
        if (!IsMove)
        {
            IsMove = true;

            pang.StartCoroutine(MoveUpdate());
        }
    }

    private IEnumerator MoveUpdate()
    {
        while (true)
        {
            if (pang.TargetBlock == null) yield break;

            if (pang.transform.position == pang.TargetBlock.transform.position)
            {
                nextBlock = GameManager._instance.LevelManager[Directions.Down, pang.TargetBlock.Pos];

                if (nextBlock != null)
                {
                    if (nextBlock.BlockState == BlockState.Empty) pang.TargetBlock = nextBlock;
                    else if (nextBlock.BlockState == BlockState.Filled) break;
                }
                else break;
            }
            else pang.transform.position = Vector2.MoveTowards(pang.transform.position, pang.TargetBlock.transform.position, Time.deltaTime * moveSpeed);

            yield return null;
        }

        pang.TargetBlock.BlockState = BlockState.Filled;

        IsMove = false;
    }
}
