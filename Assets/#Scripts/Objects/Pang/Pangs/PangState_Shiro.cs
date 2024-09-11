using UnityEngine;

public class PangState_Shiro : PangStateBase
{
    public PangState_Shiro(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(43, 32, 32, 255);
    }
}
