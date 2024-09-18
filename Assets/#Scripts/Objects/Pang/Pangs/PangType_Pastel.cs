using System.Collections;
using UnityEngine;

public class PangType_Pastel : PangTypeBase
{
    public PangType_Pastel(Pang _pang) : base(_pang)
    {

    }

    public override void Move()
    {
        pang.StartCoroutine(MoveUpdate());
    }

    private IEnumerator MoveUpdate()
    {
        if (IsMove) yield break;

        IsMove = true;

        while (true)
        {
            if (pang.transform.position == pang.TargetBlock.transform.position)
            {
                nextBlock = GameManager._instance.LevelManager[Directions.Down, pang.TargetBlock.pos];

                if (nextBlock != null)
                {
                    if (nextBlock.BlockState == BlockState.Empty) pang.TargetBlock = nextBlock;
                    else if (nextBlock.BlockState == BlockState.Filled) break;
                }
                else break;
            }
            else pang.transform.position = Vector2.MoveTowards(pang.transform.position, pang.TargetBlock.transform.position, Time.deltaTime * 6f);

            yield return null;
        }

        pang.TargetBlock.BlockState = BlockState.Filled;

        IsMove = false;
    }
}
