using UnityEngine;

public class PangState_Yuni : PangStateBase
{
    public PangState_Yuni(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(155, 132, 222, 255);
    }
}
