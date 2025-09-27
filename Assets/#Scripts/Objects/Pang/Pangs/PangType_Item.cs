using System.Collections;
using UnityEngine;

public class PangType_Item : PangTypeBase
{
    private readonly ItemHandle itemHandle = new();

    public PangType_Item(Pang _pang) : base(_pang)
    {

    }

    public override void OnMove()
    {
        LevelManager levelManager = LevelManager.Instance;

        if (pang.TargetBlock == null) return;
        if (levelManager.Match) return;
        if (levelManager.DestroyCount != 0) return;

        if (pang.transform.position == pang.TargetBlock.transform.position)
        {
            nextBlock = levelManager.blockHandle[pang.TargetBlock.Pos, 0, -1];

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
        if (IsDestroy) return;

        IsDestroy = true;

        pang.StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(removeDelay);

        pang.Animator.Play("Destroy");
        pang.particle.SetActive(true);

        LevelManager.Instance.itemPangs.Remove(pang);

        yield return new WaitUntil(() => pang.Animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"));

        itemHandle.UseItem(pang.TargetBlock);

        while (true)
        {
            if (pang.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                pang.TargetBlock = null;

                ObjectManager.Instance.PangPool.Release(pang);

                IsDestroy = false;

                break;
            }

            yield return null;
        }
    }
}
