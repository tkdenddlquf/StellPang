using UnityEngine;

public class Pang : MonoBehaviour
{
    private PangStateBase stateBase;

    public SpriteRenderer pangImage;
    public SpriteRenderer pangGlow;

    public void SetType(PangType _type)
    {
        stateBase = new PangState_Pastel(this);

        pangImage.sprite = GameManager._instance.pastelSprite_Idle[(int)_type];
        pangGlow.color = GameManager._instance.pastelGlow_Color[(int)_type];
    }

    public void SetType(ItemType _type)
    {
        stateBase = new PangState_Item(this);

        pangImage.sprite = GameManager._instance.itemSprite[(int)_type];
        pangGlow.color = Color.black;
    }
}

public enum ItemType
{
    BombVert,
    BombHori,
    BombSmallCross,
    BombLargeCross,
    Bomb3x3,
    Bomb5x5,
    Bomb7x7,
}

public enum PangType
{
    GangGi,
    Kanna,
    Yuni,
    Hina,
    Shiro,
    Lize,
    Tabi,
    Buki,
    Rin,
    Nana,
    Riko
}
