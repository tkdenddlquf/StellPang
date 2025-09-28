using UnityEngine;

public class BlockHandle
{
    public Block[] selectBlocks = new Block[2];

    public Block this[Vector2Int currentPos, Vector2Int moveVector] => BoardCreator.Instance[LevelManager.Instance.spawnHandle.SpawnDir, currentPos, moveVector];

    public bool CheckOutBlockIndex(Vector2Int currentPos, Vector2Int moveVector) => BoardCreator.Instance.CheckInRange(LevelManager.Instance.spawnHandle.SpawnDir, currentPos, moveVector);

    public void SelectBlock(Block _block)
    {
        if (LevelManager.Instance.Match) return;

        if (selectBlocks[0] == null)
        {
            if (_block.TargetPang == null) return;
            if (_block.TargetPang.PangType == PangType.Distraction) return;

            _block.TargetPang.selectImage.SetActive(true);

            selectBlocks[0] = _block;
        }
        else if (selectBlocks[0] == _block)
        {
            _block.TargetPang.selectImage.SetActive(false);

            selectBlocks[0] = null;
        }
        else if (selectBlocks[1] == null)
        {
            if (this[selectBlocks[0].Pos, Vector2Int.up] == _block ||
                this[selectBlocks[0].Pos, Vector2Int.right] == _block ||
                this[selectBlocks[0].Pos, Vector2Int.down] == _block ||
                this[selectBlocks[0].Pos, Vector2Int.right] == _block)
            {
                if (_block.TargetPang != null)
                {
                    if (_block.TargetPang.PangType == PangType.Distraction) return;

                    _block.TargetPang.selectImage.SetActive(true);
                }

                selectBlocks[1] = _block;

                SwapSelect();
            }
        }
    }

    public void SwapSelect()
    {
        if (selectBlocks[0] == null || selectBlocks[1] == null) return;

        if (selectBlocks[1].TargetPang == null)
        {
            selectBlocks[0].TargetPang.Swap(selectBlocks[1]);
            selectBlocks[0].TargetPang = null;

            selectBlocks[0].BlockState = BlockState.Filled;
            selectBlocks[1].BlockState = BlockState.Filled;
        }
        else if (selectBlocks[0].TargetPang == null)
        {
            selectBlocks[1].TargetPang.Swap(selectBlocks[0]);
            selectBlocks[1].TargetPang = null;

            selectBlocks[0].BlockState = BlockState.Filled;
            selectBlocks[1].BlockState = BlockState.Filled;
        }
        else
        {
            Pang _pang = selectBlocks[1].TargetPang;

            selectBlocks[0].TargetPang.Swap(selectBlocks[1]);
            _pang.Swap(selectBlocks[0]);
        }
    }

    public void ClearSelect()
    {
        if (selectBlocks[0] == null || selectBlocks[1] == null) return;

        if (selectBlocks[0].TargetPang == null) selectBlocks[0].BlockState = BlockState.Empty;
        if (selectBlocks[1].TargetPang == null) selectBlocks[1].BlockState = BlockState.Empty;

        selectBlocks[0] = null;
        selectBlocks[1] = null;
    }
}
