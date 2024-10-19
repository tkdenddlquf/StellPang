public abstract class PangTypeBase
{
    protected Pang pang;

    protected int moveSpeed = 7;
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

    protected PangTypeBase(Pang _pang)
    {
        pang = _pang;
    }

    public abstract void OnMove();

    public abstract void OnDestroy();

    protected bool CheckSideBlock(int _x, int _y)
    {
        nextBlock = LevelManager.Instance[pang.TargetBlock.Pos, _x, _y];

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
