using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionObj : MonoBehaviour
{
    private Image image;
    private TMP_Text count;

    public void Init()
    {
        if (image == null) image = GetComponentInChildren<Image>();
        if (count == null) count = GetComponentInChildren<TMP_Text>();

        gameObject.SetActive(true);
    }

    public void SetValue(int _value)
    {
        count.text = $"X {_value}";
    }

    public void SetType(BlockType _type)
    {
        image.sprite = GameManager._instance.blockSprites[(int)_type];
    }

    public void SetType(PangType _type)
    {
        image.sprite = GameManager._instance.pangSprites_Idle[(int)_type];
    }

    public void SetType(ItemType _type)
    {
        image.sprite = GameManager._instance.itemSprites[(int)_type];
    }
}
