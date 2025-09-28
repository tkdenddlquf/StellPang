using UnityEngine;

public abstract class PangTypeBase
{
    protected Pang pang;

    protected int moveSpeed = 9;
    protected float removeDelay;

    protected Block nextBlock;

    protected bool IsMove
    {
        get => pang.isMove;
        set
        {
            if (pang.isMove == value) return;

            LevelManager levelManager = LevelManager.Instance;

            pang.isMove = value;

            if (value) levelManager.MoveCount++;
            else
            {
                levelManager.MoveCount--;

                pang.selectImage.SetActive(false);
            }
        }
    }

    protected bool IsDestroy
    {
        get => pang.isDestroy;
        set
        {
            if (pang.isDestroy == value) return;

            LevelManager levelManager = LevelManager.Instance;

            pang.isDestroy = value;

            levelManager.destroyAction?.Invoke(pang);

            if (value) levelManager.DestroyCount++;
            else levelManager.DestroyCount--;
        }
    }

    protected PangTypeBase(Pang _pang)
    {
        pang = _pang;
    }

    public abstract void OnMove();

    public abstract void OnDestroy();

    public void SetRemoveDelay(float _delay)
    {
        removeDelay = _delay;
    }

    protected bool CheckSideBlock(Vector2Int pos)
    {
        nextBlock = LevelManager.Instance.blockHandle[pang.TargetBlock.Pos, pos];

        if (nextBlock != null)
        {
            if (nextBlock.BlockState == BlockState.Empty && nextBlock.Blocked)
            {
                pang.TargetBlock = nextBlock;

                return true;
            }
        }

        return false;
    }
}
