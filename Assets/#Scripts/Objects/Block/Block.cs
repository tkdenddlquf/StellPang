using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer background;

    public bool Blocked { get; set; }

    public Pang TargetPang { get; set; }
    public BlockState BlockState { get; set; }

    public int[] Pos { get; set; } = new int[2];

    public void OnMouseDown()
    {
        if (LevelManager.Instance.MoveCount != 0) return;
        if (LevelManager.Instance.DestroyCount != 0) return;

        LevelManager.Instance.blockHandle.SelectBlock(this);
    }

    public bool CheckPangType(Block _block)
    {
        if (TargetPang.PangTypeNum == _block.TargetPang.PangTypeNum) return true;

        return false;
    }
}