using UnityEngine;

public class PangState_Hina : PangStateBase
{
    public PangState_Hina(Pang _pang, Sprite _sprite) : base(_pang)
    {
        _pang.pangImage.sprite = _sprite;
        _pang.pangImage.color = new Color32(123, 47, 64, 255);
    }
}
