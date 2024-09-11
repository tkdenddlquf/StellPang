using UnityEngine;

public class PangState_GangGi : PangStateBase
{
    public PangState_GangGi(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(129, 130, 184, 255);
    }
}
