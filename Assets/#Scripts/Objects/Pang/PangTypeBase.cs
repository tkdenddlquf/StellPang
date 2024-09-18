public abstract class PangTypeBase
{
    protected Pang pang;

    private bool isMove;

    protected int moveSpeed = 7;
    protected Block nextBlock;

    protected bool IsMove
    {
        get => isMove;
        set
        {
            if (isMove == value) return;

            isMove = value;

            if (value) GameManager._instance.LevelManager.MoveCount++;
            else GameManager._instance.LevelManager.MoveCount--;
        }
    }

    protected PangTypeBase(Pang _pang)
    {
        pang = _pang;
    }

    public abstract void Move();
}
