using System.Collections;
using UnityEngine;

public class Pang : MonoBehaviour
{
    private PangType type = PangType.None;
    private ItemType itemType = ItemType.None;

    private float distance;
    private bool move = false;
    private Block targetBlock;

    private Animator effect;
    private GameObject select;
    private GameObject particle;
    private SpriteRenderer glow;
    private SpriteRenderer sprite;

    public PangType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;

            if (type == PangType.None) glow.gameObject.SetActive(false);
            else
            {
                glow.gameObject.SetActive(true);
                glow.color = GameManager._instance.GetGlowColor(type);
            }

            SetSprite(false);
        }
    }

    public ItemType ItemType
    {
        get
        {
            return itemType;
        }
        set
        {
            if (value != ItemType.None)
            {
                sprite.sprite = GameManager._instance.itemSprites[(int)value];
                Type = PangType.None;
            }

            itemType = value;
        }
    }

    public bool Move
    {
        get
        {
            return move;
        }
        set
        {
            if (move == value) return;

            if (!value)
            {
                move = TargetBlock.CheckToss();

                if (move != value) return;
            }
            else
            {
                move = value;

                if (!GameManager._instance.CheckSelectBlock(TargetBlock)) select.SetActive(false);
            }

            if (!move) GameManager._instance.MoveLineCount--;
            else GameManager._instance.MoveLineCount++;
        }
    }

    public Block TargetBlock
    {
        get
        {
            return targetBlock;
        }
        set
        {
            targetBlock = value;

            StopAllCoroutines();
            StartCoroutine(FallingPang());
        }
    }

    public void Init()
    {
        if (select == null)
        {
            select = transform.GetChild(0).gameObject;
            particle = transform.GetChild(1).gameObject;

            particle.TryGetComponent(out effect);
            transform.GetChild(2).TryGetComponent(out glow);
            TryGetComponent(out sprite);
        }

        StopAllCoroutines();

        transform.localScale = Vector3.one;
        transform.eulerAngles = Vector3.zero;

        sprite.color = new(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        glow.color = new(glow.color.r, glow.color.g, glow.color.b, 1);
    }

    public void SelectPang(bool _select)
    {
        select.SetActive(_select);
    }

    private void SetSprite(bool _remove)
    {
        if (type == PangType.None) return;

        if (_remove) sprite.sprite = GameManager._instance.pangSprites_Pang[(int)type];
        else sprite.sprite = GameManager._instance.pangSprites_Idle[(int)type];
    }

    public void RemovePang(float _delay)
    {
        StartCoroutine(RemoveAnim(_delay));
    }

    private IEnumerator RemoveAnim(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        SetSprite(true);
        particle.SetActive(true);

        sprite.sortingOrder++;

        switch (ItemType)
        {
            case ItemType.VertBomb:
                effect.Play("VertBomb");
                break;

            case ItemType.HoriBomb:
                effect.Play("HoriBomb");
                break;

            case ItemType.TBomb_Small:
                effect.Play("TBomb_Small");
                break;

            case ItemType.TBomb_Long:
                effect.Play("TBomb_Long");
                break;

            case ItemType.Bomb_5x5:
                effect.Play("Bomb_5x5");
                break;
        }

        while (true)
        {
            if (sprite.color.a <= 0) break;

            transform.localScale += Vector3.one * Time.deltaTime;
            sprite.color -= Color.black * Time.deltaTime;
            glow.color -= Color.black * Time.deltaTime;

            yield return null;
        }

        GameManager._instance.PushPang(this);

        transform.localScale = Vector3.one;
        sprite.color = new(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        glow.color = new(glow.color.r, glow.color.g, glow.color.b, 1);

        sprite.sortingOrder--;
    }

    public void HintPang()
    {
        StartCoroutine(HintAnim());
    }

    private IEnumerator HintAnim()
    {
        bool _expand = true;

        while (true)
        {
            if (!GameManager._instance.NoticeHint) break;

            if (_expand)
            {
                if (transform.localScale.x < 1.15f) transform.localScale += 0.2f * Time.deltaTime * Vector3.one;
                else _expand = false;
            }
            else
            {
                if (transform.localScale.x > 1.0f) transform.localScale -= 0.2f * Time.deltaTime * Vector3.one;
                else _expand = true;
            }

            transform.eulerAngles += 30f * Time.deltaTime * Vector3.back;

            yield return null;
        }

        transform.localScale = Vector3.one;
        transform.eulerAngles = Vector3.zero;
    }

    private IEnumerator FallingPang()
    {
        while (true)
        {
            if (TargetBlock != null) // 대상이 있는 경우만 이동
            {
                if (transform.position != TargetBlock.transform.position)
                {
                    distance = Vector2.Distance(transform.position, TargetBlock.transform.position);

                    if (distance < 0.01f) transform.position = TargetBlock.transform.position;
                    else
                    {
                        Move = true;

                        if (distance > 1)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, TargetBlock.transform.position, Time.deltaTime * 10 * distance);
                        }
                        else
                        {
                            transform.position = Vector2.MoveTowards(transform.position, TargetBlock.transform.position, Time.deltaTime * 10);
                        }
                    }
                }
                else
                {
                    Move = false;

                    yield break;
                }
            }

            yield return null;
        }
    }
}
