using UnityEngine;

public class BlockState_None : BlockStateBase
{
    public BlockState_None(Block _block, Sprite _sprite) : base(_block)
    {
        block.blockImage.sprite = _sprite;
    }
}
