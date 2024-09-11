using UnityEngine;

public class PangState_Lize : PangStateBase
{
    public PangState_Lize(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(76, 17, 17, 255);
    }
}
