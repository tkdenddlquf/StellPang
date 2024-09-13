using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer background;

    public Pang TargetPang { get; set; }
    public BlockState BlockState { get; set; }

    public int[] pos = new int[2];
}

public enum BlockState
{
    Empty,
    Reserved,
    Filled
}