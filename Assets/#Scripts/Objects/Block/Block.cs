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
        LevelManager levelManager = LevelManager.Instance;

        if (levelManager.MoveCount != 0) return;
        if (levelManager.DestroyCount != 0) return;

        levelManager.blockHandle.SelectBlock(this);
    }

    public bool CheckPangType(Block _block)
    {
        if (TargetPang.PangTypeNum == _block.TargetPang.PangTypeNum) return true;

        return false;
    }
}