using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer background;

    public Pang TargetPang { get; set; }
    public BlockState BlockState { get; set; }

    public int[] Pos { get; set; } = new int[2];

    public bool CheckPangType(Block _block)
    {
        if (TargetPang.PangTypeNum == _block.TargetPang.PangTypeNum) return true;

        return false;
    }

    public void OnMouseDown()
    {
        if (TargetPang == null) return;
        if (GameManager._instance.LevelManager.MoveCount != 0) return;

        TargetPang.selectImage.SetActive(!TargetPang.selectImage.activeSelf);
    }
}

public enum BlockState
{
    Empty,
    Reserved,
    Filled
}