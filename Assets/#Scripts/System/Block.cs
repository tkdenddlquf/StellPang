using UnityEngine;

public class Block : MonoBehaviour
{
    private BlockType type = BlockType.None;
    private Pang pang;

    private int hp;
    private int[] directionNumber => GameManager._instance.DirectionNumber;
    private Sprite[] hpSprites;
    private SpriteRenderer sprite;
    private SpriteRenderer objSprite;

    private readonly Block[] blocks = new Block[4];
    private readonly Block[] blocks_Next = new Block[4];

    public BlockType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;

            SetObject();
        }
    }

    public Pang Pang
    {
        get
        {
            if (GameManager._instance.GetCommonType(Type, BlockType_Common.Move)) return pang;

            return null;
        }
        set
        {
            if (value != null)
            {
                value.Init();
                value.TargetBlock = this;
            }

            pang = value;
        }
    }

    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;

            if (hp < 0) hp = 0;

            if (hp == 0)
            {
                DataManager._instance.RecordRemoveType(Type);
                Type = BlockType.None;
            }
            else objSprite.sprite = hpSprites[hp - 1];
        }
    }

    public void Init()
    {
        if (sprite == null) TryGetComponent(out sprite);
        if (objSprite == null) objSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        objSprite.gameObject.SetActive(false);

        SetAlpha();
    }

    // SET
    public void SetBlock(int _index)
    {
        blocks[_index] = null;
    }

    public void SetBlock(int _index, ref Block _block)
    {
        blocks[_index] = _block;
    }

    public void SetBlock_Next(int _index)
    {
        blocks_Next[_index] = null;
    }

    public void SetBlock_Next(int _index, ref Block _block)
    {
        if (blocks[_index] == _block) return;

        blocks_Next[_index] = _block;
    }

    public void SetHP(Sprite[] _sprites)
    {
        hpSprites = _sprites;
        HP = _sprites.Length;
    }

    private void SetAlpha()
    {
        if ((int)transform.position.x != transform.position.x || (int)transform.position.y != transform.position.y)
        {
            if ((int)(transform.position.x + 0.5f + transform.position.y + 0.5f) % 2 == 0) sprite.color = GameManager._instance.GetBlockColor(false);
            else sprite.color = GameManager._instance.GetBlockColor(true);
        }
        else
        {
            if ((transform.position.x + transform.position.y) % 2 == 0) sprite.color = GameManager._instance.GetBlockColor(false);
            else sprite.color = GameManager._instance.GetBlockColor(true);
        }
    }

    private void SetObject()
    {
        switch (Type)
        {
            case BlockType.None:
                objSprite.gameObject.SetActive(false);
                return;

            default:
                objSprite.sprite = GameManager._instance.blockSprites[(int)Type];
                break;
        }

        objSprite.gameObject.SetActive(true);
    }

    // TOSS
    public bool CheckToss()
    {
        SpwanPang();

        if (Pang != null)
        {
            if (Pang.transform.position == transform.position)
            {
                if (GameManager._instance.CheckSelectBlock(this)) return false;

                if (TossPangs(this)) return true;

                if (CheckNullPang(ref blocks[directionNumber[0]], true)) // 좌측 확인
                {
                    if (TossPangs(blocks[directionNumber[0]])) return true;
                }

                if (CheckNullPang(ref blocks[directionNumber[2]], true)) // 우측 확인
                {
                    if (TossPangs(blocks[directionNumber[2]])) return true;
                }
            }
        }
        
        TossPangs();

        return false;
    }

    private bool TossPangs(Block _block)
    {
        if (!GameManager._instance.GetCommonType(Type, BlockType_Common.Move)) return false;
        
        if (CheckNullPang(ref _block.blocks[directionNumber[3]], true)) // 비어있다면
        {
            if (GameManager._instance.GetCommonType(_block.blocks[directionNumber[3]].Type, BlockType_Common.Move))
            {
                _block.blocks[directionNumber[3]].Pang = Pang;
                Pang = null;

                TossPangs();
                SpwanPang();

                return true;
            }
        }
        else if (CheckNullPang(ref _block.blocks_Next[directionNumber[3]], true)) // 비어있다면
        {
            if (GameManager._instance.GetCommonType(_block.blocks[directionNumber[3]].Type, BlockType_Common.Move))
            {
                _block.blocks_Next[directionNumber[3]].Pang = Pang;
                Pang = null;

                TossPangs();
                SpwanPang();

                return true;
            }
        }

        return false;
    }

    private void TossPangs() // 상단 또는 대각선 팡이 내려오도록 변경
    {
        if (blocks[directionNumber[1]] != null)
        {
            if (!blocks[directionNumber[1]].CheckToss())
            {
                if (blocks[directionNumber[1]].blocks[directionNumber[0]] != null) blocks[directionNumber[1]].blocks[directionNumber[0]].CheckToss();
                if (blocks[directionNumber[1]].blocks[directionNumber[2]] != null) blocks[directionNumber[1]].blocks[directionNumber[2]].CheckToss();
            }
        }
        else if (blocks_Next[directionNumber[1]] != null)
        {
            if (!blocks_Next[directionNumber[1]].CheckToss())
            {
                if (blocks_Next[directionNumber[1]].blocks[directionNumber[0]] != null) blocks_Next[directionNumber[1]].blocks[directionNumber[0]].CheckToss();
                if (blocks_Next[directionNumber[1]].blocks[directionNumber[2]] != null) blocks_Next[directionNumber[1]].blocks[directionNumber[2]].CheckToss();
            }
        }
    }

    // ATHER
    public void SpwanPang()
    {
        if (Pang != null) return;
        if (Type != BlockType.None) return;

        if (!GameManager._instance.GetSpawnBlock(this)) return;

        Pang = GameManager._instance.PopPang();

        if (Pang == null) return;

        Pang.transform.position = transform.position + GameManager._instance.GetSpawnVector();
        Pang.Type = GameManager._instance.GetRandomPangType(this);
    }

    public void RemovePang(RemoveType _type = RemoveType.Pang, float _delay = 0f)
    {
        if (Pang == null)
        {
            if (_type != RemoveType.Clear && GameManager._instance.GetCommonType(Type, BlockType_Common.Break)) Type = BlockType.None;

            return;
        }

        if (_type == RemoveType.Pang && Pang.ItemType != ItemType.None) return;

        switch (_type)
        {
            case RemoveType.Pang:
                BreakBlocks();

                DataManager._instance.RecordRemoveType(Pang.Type);
                break;

            case RemoveType.Item:
                BreakBlocks();

                if (Pang.ItemType == ItemType.None) DataManager._instance.RecordRemoveType(Pang.Type);
                else
                {
                    GameManager._instance.AddUseItemPang(Pang);
                    DataManager._instance.RecordRemoveType(Pang.ItemType);
                }
                break;

            case RemoveType.Clear:
                GameManager._instance.PushPang(Pang);

                Pang = null;
                return;
        }

        Pang.RemovePang(_delay);

        Pang = null;

        GameManager._instance.SetScore(60 * (GameManager._instance.Combo + 1));
    }

    private void BreakBlocks()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
                if (GameManager._instance.GetCommonType(blocks[i].Type, BlockType_Common.Break))
                {
                    blocks[i].HP--;
                }
            }
        }
    }

    // CHECK
    public bool CheckConnectBlock(ref Block _block)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] == _block) return true;
        }

        return false;
    }

    public bool CheckNotExistBlock(int _index)
    {
        if (blocks[_index] == null)
        {
            if (blocks_Next[_index] == null)
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckNullPang(ref Block _block, bool _null)
    {
        if (_block != null)
        {
            if (_null) return _block.Pang == null;
            else return _block.Pang != null;
        }

        return false;
    }

    public bool CheckMoveTrue(ref Block _block)
    {
        if (_block != null) return GameManager._instance.GetCommonType(_block.Type, BlockType_Common.Move);

        return false;
    }

    public bool CheckMatchPang(int _num)
    {
        return _num switch
        {
            0 => CheckMatchPang_T(),
            1 => CheckMatchPang_L(),
            2 => CheckMatchPang_Box(),
            3 => CheckMatchPang_Line(),
            4 => CheckMatchPang(),
            _ => false,
        };
    }

    public bool CheckMatchPang_T()
    {
        if (Pang == null) return false;

        if (CheckSamePang(ref blocks[0], ref blocks[2]))
        {
            if (CheckSamePang(ref blocks[1]))
            {
                if (CheckSamePang(ref blocks[1].blocks[1]))
                {
                    blocks[0].RemovePang();
                    blocks[2].RemovePang();
                    blocks[1].RemovePang();
                    blocks[1].blocks[1].RemovePang();

                    Pang.ItemType = ItemType.Bomb_5x5;
                    GameManager._instance.AddItemPang(Pang);

                    return true;
                }
            }

            if (CheckSamePang(ref blocks[3]))
            {
                if (CheckSamePang(ref blocks[3].blocks[3]))
                {
                    blocks[0].RemovePang();
                    blocks[2].RemovePang();
                    blocks[3].RemovePang();
                    blocks[3].blocks[3].RemovePang();

                    Pang.ItemType = ItemType.Bomb_5x5;
                    GameManager._instance.AddItemPang(Pang);

                    return true;
                }
            }
        }

        if (CheckSamePang(ref blocks[1], ref blocks[3]))
        {
            if (CheckSamePang(ref blocks[0]))
            {
                if (CheckSamePang(ref blocks[0].blocks[0]))
                {
                    blocks[1].RemovePang();
                    blocks[3].RemovePang();
                    blocks[0].RemovePang();
                    blocks[0].blocks[0].RemovePang();

                    Pang.ItemType = ItemType.Bomb_5x5;
                    GameManager._instance.AddItemPang(Pang);

                    return true;
                }
            }

            if (CheckSamePang(ref blocks[2]))
            {
                if (CheckSamePang(ref blocks[2].blocks[2]))
                {
                    blocks[1].RemovePang();
                    blocks[3].RemovePang();
                    blocks[2].RemovePang();
                    blocks[2].blocks[2].RemovePang();

                    Pang.ItemType = ItemType.Bomb_5x5;
                    GameManager._instance.AddItemPang(Pang);

                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckMatchPang_L()
    {
        if (Pang == null) return false;

        if (CheckSamePang(ref blocks[1]))
        {
            if (CheckSamePang(ref blocks[1].blocks[1]))
            {
                if (CheckSamePang(ref blocks[0]))
                {
                    if (CheckSamePang(ref blocks[0].blocks[0]))
                    {
                        blocks[1].RemovePang();
                        blocks[1].blocks[1].RemovePang();
                        blocks[0].RemovePang();
                        blocks[0].blocks[0].RemovePang();

                        Pang.ItemType = ItemType.TBomb_Long;
                        GameManager._instance.AddItemPang(Pang);

                        return true;
                    }
                }

                if (CheckSamePang(ref blocks[2]))
                {
                    if (CheckSamePang(ref blocks[2].blocks[2]))
                    {
                        blocks[1].RemovePang();
                        blocks[1].blocks[1].RemovePang();
                        blocks[2].RemovePang();
                        blocks[2].blocks[2].RemovePang();

                        Pang.ItemType = ItemType.TBomb_Long;
                        GameManager._instance.AddItemPang(Pang);

                        return true;
                    }
                }
            }
        }

        if (CheckSamePang(ref blocks[3]))
        {
            if (CheckSamePang(ref blocks[3].blocks[3]))
            {
                if (CheckSamePang(ref blocks[0]))
                {
                    if (CheckSamePang(ref blocks[0].blocks[0]))
                    {
                        blocks[3].RemovePang();
                        blocks[3].blocks[3].RemovePang();
                        blocks[0].RemovePang();
                        blocks[0].blocks[0].RemovePang();

                        Pang.ItemType = ItemType.TBomb_Long;
                        GameManager._instance.AddItemPang(Pang);

                        return true;
                    }
                }

                if (CheckSamePang(ref blocks[2]))
                {
                    if (CheckSamePang(ref blocks[2].blocks[2]))
                    {
                        blocks[3].RemovePang();
                        blocks[3].blocks[3].RemovePang();
                        blocks[2].RemovePang();
                        blocks[2].blocks[2].RemovePang();

                        Pang.ItemType = ItemType.TBomb_Long;
                        GameManager._instance.AddItemPang(Pang);

                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool CheckMatchPang_Box()
    {
        if (Pang == null) return false;

        if (CheckSamePang(ref blocks[2], ref blocks[3]))
        {
            if (CheckSamePang(ref blocks[3].blocks[2]))
            {
                if (GameManager._instance.CheckSelectBlock(this))
                {
                    Pang.ItemType = ItemType.TBomb_Small;
                    GameManager._instance.AddItemPang(Pang);

                    blocks[2].RemovePang();
                    blocks[3].RemovePang();
                    blocks[3].blocks[2].RemovePang();
                }
                else if (GameManager._instance.CheckSelectBlock(blocks[2]))
                {
                    blocks[2].Pang.ItemType = ItemType.TBomb_Small;
                    GameManager._instance.AddItemPang(blocks[2].Pang);

                    blocks[3].RemovePang();
                    blocks[3].blocks[2].RemovePang();
                    RemovePang();
                }
                else if (GameManager._instance.CheckSelectBlock(blocks[3]))
                {
                    blocks[3].Pang.ItemType = ItemType.TBomb_Small;
                    GameManager._instance.AddItemPang(blocks[3].Pang);

                    blocks[2].RemovePang();
                    blocks[3].blocks[2].RemovePang();
                    RemovePang();
                }
                else
                {
                    blocks[3].blocks[2].Pang.ItemType = ItemType.TBomb_Small;
                    GameManager._instance.AddItemPang(blocks[3].blocks[2].Pang);

                    blocks[2].RemovePang();
                    blocks[3].RemovePang();
                    RemovePang();
                }

                return true;
            }
        }

        return false;
    }

    public bool CheckMatchPang_Line()
    {
        if (Pang == null) return false;

        if (CheckSamePang(ref blocks[0], ref blocks[2]))
        {
            if (CheckSamePang(ref blocks[2].blocks[2]))
            {
                if (GameManager._instance.CheckSelectBlock(this))
                {
                    Pang.ItemType = ItemType.HoriBomb;
                    GameManager._instance.AddItemPang(Pang);

                    blocks[0].RemovePang();
                    blocks[2].RemovePang();
                    blocks[2].blocks[2].RemovePang();
                }
                else
                {
                    blocks[2].Pang.ItemType = ItemType.HoriBomb;
                    GameManager._instance.AddItemPang(blocks[2].Pang);

                    blocks[0].RemovePang();
                    blocks[2].blocks[2].RemovePang();
                    RemovePang();
                }
            }

            return true;
        }

        if (CheckSamePang(ref blocks[1], ref blocks[3]))
        {
            if (CheckSamePang(ref blocks[3].blocks[3]))
            {
                if (GameManager._instance.CheckSelectBlock(this))
                {
                    Pang.ItemType = ItemType.VertBomb;
                    GameManager._instance.AddItemPang(Pang);

                    blocks[1].RemovePang();
                    blocks[3].RemovePang();
                    blocks[3].blocks[3].RemovePang();
                }
                else
                {
                    blocks[3].Pang.ItemType = ItemType.VertBomb;
                    GameManager._instance.AddItemPang(blocks[3].Pang);

                    blocks[1].RemovePang();
                    blocks[3].blocks[3].RemovePang();
                    RemovePang();
                }
            }

            return true;
        }

        return false;
    }

    public bool CheckMatchPang()
    {
        if (Pang == null) return false;

        if (CheckSamePang(ref blocks[0], ref blocks[2]))
        {
            blocks[0].RemovePang();
            blocks[2].RemovePang();
            RemovePang();

            return true;
        }

        if (CheckSamePang(ref blocks[1], ref blocks[3]))
        {
            blocks[1].RemovePang();
            blocks[3].RemovePang();
            RemovePang();

            return true;
        }

        return false;
    }

    public bool CheckSamePang(ref Block _block)
    {
        if (Pang == null) return false;
        if (Pang.Type == PangType.None) return false;
        
        if (CheckNullPang(ref _block, false))
        {
            if (_block.Type != BlockType.None) return false;
            if (_block.Pang.Type == Pang.Type) return true;
        }

        return false;
    }

    public bool CheckSamePang(ref Block _block1, ref Block _block2)
    {
        if (Pang == null) return false;

        if (CheckNullPang(ref _block1, false) && CheckNullPang(ref _block2, false))
        {
            if (_block1.Type != BlockType.None || _block2.Type != BlockType.None) return false;
            if (_block1.Pang.Type == Pang.Type && _block2.Pang.Type == Pang.Type) return true;
        }

        return false;
    }

    public void CheckHintPang(ref Block _hint)
    {
        if (Pang == null) return;

        for (int i = 0; i < blocks.Length; i++) // 3매치 확인
        {
            if (CheckSamePang(ref blocks[i]))
            {
                if (CheckMoveTrue(ref blocks[i].blocks[i]))
                {
                    if (CheckSamePang(ref blocks[i].blocks[i].blocks[i]))
                    {
                        _hint = blocks[i].blocks[i].blocks[i];

                        return;
                    }

                    if (CheckSamePang(ref blocks[i].blocks[i].blocks[i % 2 == 0 ? 1 : 0]))
                    {
                        _hint = blocks[i].blocks[i].blocks[i % 2 == 0 ? 1 : 0];

                        return;
                    }

                    if (CheckSamePang(ref blocks[i].blocks[i].blocks[i % 2 == 0 ? 3 : 2]))
                    {
                        _hint = blocks[i].blocks[i].blocks[i % 2 == 0 ? 3 : 2];

                        return;
                    }
                }
            }
        }

        if (CheckMoveTrue(ref blocks[0]))
        {
            if (CheckSamePang(ref blocks[0].blocks[1], ref blocks[0].blocks[3]))
            {
                _hint = this;

                return;
            }
        }

        if (CheckMoveTrue(ref blocks[2]))
        {
            if (CheckSamePang(ref blocks[2].blocks[1], ref blocks[2].blocks[3]))
            {
                _hint = this;

                return;
            }
        }

        if (CheckSamePang(ref blocks[0], ref blocks[1])) // 박스 형태 확인
        {
            if (CheckMoveTrue(ref blocks[0].blocks[1]))
            {
                if (CheckSamePang(ref blocks[0].blocks[1].blocks[1]))
                {
                    _hint = blocks[0].blocks[1].blocks[1];
                }
                else if (CheckSamePang(ref blocks[0].blocks[1].blocks[0]))
                {
                    _hint = blocks[0].blocks[1].blocks[0];
                }
            }

            return;
        }
    }
}
