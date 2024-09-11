using UnityEngine;

public class PangState_Kanna : PangStateBase
{
    public PangState_Kanna(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(50, 48, 74, 255);
    }
}
