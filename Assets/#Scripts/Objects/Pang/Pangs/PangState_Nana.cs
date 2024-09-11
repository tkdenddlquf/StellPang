using UnityEngine;

public class PangState_Nana : PangStateBase
{
    public PangState_Nana(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(238, 113, 135, 255);
    }
}
