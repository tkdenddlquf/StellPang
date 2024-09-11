using UnityEngine;

public class PangState_Riko : PangStateBase
{
    public PangState_Riko(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(72, 209, 136, 255);
    }
}
