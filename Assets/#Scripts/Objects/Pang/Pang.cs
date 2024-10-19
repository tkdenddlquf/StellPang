using UnityEngine;

public class Pang : MonoBehaviour
{
    public GameObject selectImage;
    public GameObject particle;

    public SpriteRenderer pangImage;
    public SpriteRenderer pangGlow;

    public bool isMove;

    private Block targetBlock;

    public int PangTypeNum { get; private set; }
    public Animator Animator { get; private set; }
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

                LevelManager.Instance.SpawnPang(targetBlock);
            }

            targetBlock = value;

            if (targetBlock != null)
            {
                targetBlock.TargetPang = this;
                targetBlock.BlockState = BlockState.Reserved;
            }
        }
    }

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        StateBase.OnMove();
    }

    public void SetType(PastelType _type)
    {
        StateBase = new PangType_Pastel(this);

        pangImage.sprite = GameManager.Instance.pastelSprite_Idle[(int)_type];
        pangGlow.color = GameManager.Instance.pastelGlow_Color[(int)_type];

        PangType = PangType.Pastel;
        PangTypeNum = (int)_type;
    }

    public void SetType(ItemType _type)
    {
        StateBase = new PangType_Item(this);

        pangImage.sprite = GameManager.Instance.itemSprite[(int)_type];
        pangGlow.color = Color.black;

        PangType = PangType.Item;
        PangTypeNum = (int)_type;

        LevelManager.Instance.itemPangs.Add(this);
    }

    public void SetType(DistractionType _type)
    {
        StateBase = new PangType_Distraction(this);

        pangImage.sprite = GameManager.Instance.distractionSprite[(int)_type];
        pangGlow.color = Color.black;

        PangType = PangType.Distraction;
        PangTypeNum = (int)_type;
    }

    public void Swap(Block _block)
    {
        targetBlock = _block;
        targetBlock.TargetPang = this;
    }
}