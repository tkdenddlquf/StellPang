using UnityEngine;

public class PangState_Rin : PangStateBase
{
    public PangState_Rin(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(42, 48, 81, 255);
    }
}
