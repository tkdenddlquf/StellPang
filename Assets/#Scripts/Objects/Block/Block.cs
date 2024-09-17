using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer background;

    public Pang TargetPang { get; set; }
    public BlockState BlockState { get; set; }

    public int[] pos = new int[2];

    public bool CheckPangType(Block _block)
    {
        if (TargetPang.PangTypeNum == _block.TargetPang.PangTypeNum) return true;

        return false;
    }
}

public enum BlockState
{
    Empty,
    Reserved,
    Filled
}