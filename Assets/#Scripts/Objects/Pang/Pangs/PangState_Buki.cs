using UnityEngine;

public class PangState_Buki : PangStateBase
{
    public PangState_Buki(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(123, 102, 160, 255);
    }
}
