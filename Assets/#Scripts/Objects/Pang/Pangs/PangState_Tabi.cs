using UnityEngine;

public class PangState_Tabi : PangStateBase
{
    public PangState_Tabi(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(71, 164, 223, 255);
    }
}
