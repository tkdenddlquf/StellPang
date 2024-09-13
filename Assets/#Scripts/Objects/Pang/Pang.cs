using UnityEngine;

public class Pang : MonoBehaviour
{
    public SpriteRenderer pangImage;
    public SpriteRenderer pangGlow;

    private Block targetBlock;

    public PangState PangState { get; private set; }
    public PangType PangType { get; private set; }

    public PangTypeBase StateBase { get; private set; }

    public Block TargetBlock
    {
        get => targetBlock;
        set
        {
            if (targetBlock == value) return;

            if (targetBlock != null)
            {
                targetBlock.TargetPang = null;
                targetBlock.BlockState = BlockState.Empty;
            }

            targetBlock = value;

            if (targetBlock != null)
            {
                targetBlock.TargetPang = this;
                targetBlock.BlockState = BlockState.Reserved;
            }
        }
    }

    public void SetType(PastelType _type)
    {
        StateBase = new PangType_Pastel(this);

        pangImage.sprite = GameManager._instance.pastelSprite_Idle[(int)_type];
        pangGlow.color = GameManager._instance.pastelGlow_Color[(int)_type];

        PangType = PangType.Pastel;
    }

    public void SetType(ItemType _type)
    {
        StateBase = new PangType_Item(this);

        pangImage.sprite = GameManager._instance.itemSprite[(int)_type];
        pangGlow.color = Color.black;

        PangType = PangType.Item;
    }

    public void SetType(DistractionType _type)
    {
        StateBase = new PangType_Distraction(this);

        pangImage.sprite = GameManager._instance.distractionSprite[(int)_type];
        pangGlow.color = Color.black;

        PangType = PangType.Distraction;
    }
}

public enum PangState
{
    Move,
    Stop
}

public enum PangType
{
    Pastel,
    Item,
    Distraction
}

public enum DistractionType
{
    Stone
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

public enum PastelType
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
