using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Sprite[] itemSprite;
    public Sprite[] distractionSprite;
    public Sprite[] pastelSprite_Idle;
    public Sprite[] pastelSprite_Match;
    public Color32[] pastelGlow_Color = {
        new(129, 130, 184, 255),
        new(50, 48, 74, 255), new(155, 132, 222, 255),
        new(123, 47, 64, 255), new(43, 32, 32, 255), new(76, 17, 17, 255), new(71, 164, 223, 255),
        new(123, 102, 160, 255), new(42, 48, 81, 255), new(238, 113, 135, 255), new(72, 209, 136, 255)
    };

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
