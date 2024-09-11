using UnityEngine;

public abstract class PangStateBase
{
    protected Pang pang;

    protected PangStateBase(Pang _pang)
    {
        pang = _pang;
    }
}
