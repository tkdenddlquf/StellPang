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

            pang.isMove = value;

            if (value) LevelManager.Instance.MoveCount++;
            else
            {
                LevelManager.Instance.MoveCount--;

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

            pang.isDestroy = value;
            LevelManager.Instance.destroyAction?.Invoke(pang);

            if (value) LevelManager.Instance.DestroyCount++;
            else LevelManager.Instance.DestroyCount--;
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

    protected bool CheckSideBlock(int _x, int _y)
    {
        nextBlock = LevelManager.Instance.blockHandle[pang.TargetBlock.Pos, _x, _y];

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
