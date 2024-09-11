using UnityEngine;

public class BlockState_Stone : BlockStateBase
{
    public BlockState_Stone(Block _block, Sprite _sprite) : base(_block)
    {
        block.blockImage.sprite = _sprite;
    }
}
