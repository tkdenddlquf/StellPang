using System.Collections;
using UnityEngine;

public abstract class PangTypeBase
{
    protected Pang pang;

    protected bool move;
    protected Block nextBlock;

    protected PangTypeBase(Pang _pang)
    {
        pang = _pang;
    }

    public abstract void Move();
}
