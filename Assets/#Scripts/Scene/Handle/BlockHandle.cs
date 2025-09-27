using UnityEngine;

public class BlockHandle
{
    public Block[] selectBlocks = new Block[2];

    public Block this[int[] _pos, int _x, int _y]
    {
        get
        {
            return LevelManager.Instance.spawnHandle.SpawnDir switch
            {
                Directions.Up => BoardCreator.Instance[_pos[0] + _x, _pos[1] + _y],
                Directions.Right => BoardCreator.Instance[_pos[0] + _y, _pos[1] - _x],
                Directions.Down => BoardCreator.Instance[_pos[0] - _x, _pos[1] - _y],
                Directions.Left => BoardCreator.Instance[_pos[0] - _y, _pos[1] + _x],
                _ => null
            };
        }
    }

    public bool CheckOutBlockIndex(int[] _pos, int _x, int _y) => this[_pos, _x, _y] == null;

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
            if (this[selectBlocks[0].Pos, 0, 1] == _block ||
                this[selectBlocks[0].Pos, 1, 0] == _block ||
                this[selectBlocks[0].Pos, 0, -1] == _block ||
                this[selectBlocks[0].Pos, -1, 0] == _block)
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
