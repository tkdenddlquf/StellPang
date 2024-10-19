using System.Collections;
using UnityEngine;

public class PangType_Pastel : PangTypeBase
{
    public PangType_Pastel(Pang _pang) : base(_pang)
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

    public override void OnDestroy()
    {
        pang.TargetBlock = null;

        pang.Animator.Play("Destroy");
        pang.particle.SetActive(true);

        pang.StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitUntil(() => pang.Animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"));

        while (true)
        {
            if (pang.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                ObjectManager.Instance.pangs.Enqueue(pang);

                break;
            }

            yield return null;
        }
    }
}
