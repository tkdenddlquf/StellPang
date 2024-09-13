using System.Collections;
using UnityEngine;

public abstract class PangTypeBase
{
    protected Pang pang;

    private bool isMove;

    protected Block nextBlock;

    protected bool IsMove
    {
        get => isMove;
        set
        {
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
