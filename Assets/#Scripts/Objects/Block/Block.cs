using UnityEngine;

public class Block : MonoBehaviour
{
    private BlockStateBase stateBase;

    public SpriteRenderer background;
    public SpriteRenderer blockImage;

    public void SetType(BlockType _type)
    {
        switch (_type)
        {
            case BlockType.None:
                stateBase = new BlockState_None(this, blockImage.sprite);
                break;

            case BlockType.Stone:
                stateBase = new BlockState_Stone(this, blockImage.sprite);
                break;
        }
    }
}

public enum BlockType
{
    None,
    Stone
}