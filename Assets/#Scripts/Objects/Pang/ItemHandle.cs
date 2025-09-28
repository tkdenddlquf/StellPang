using UnityEngine;

public class ItemHandle
{
    private Block itemBlock;
    private Block removeBlcok;

    private readonly float delay = 0.07f;

    public void UseItem(Block _block)
    {
        itemBlock = _block;

        switch (itemBlock.TargetPang.PangTypeNum)
        {
            case 0: // BombVert
                CheckLine(Vector2Int.up, 1);
                CheckLine(Vector2Int.up, -1);
                break;

            case 1: // BombHori
                CheckLine(Vector2Int.right, 1);
                CheckLine(Vector2Int.right, -1);
                break;

            case 2: // BombSmallCross
                CheckLine(Vector2Int.up, 1, 1);
                CheckLine(Vector2Int.up, -1, 1);
                CheckLine(Vector2Int.right, 1, 1);
                CheckLine(Vector2Int.right, -1, 1);
                break;

            case 3: // BombLargeCross
                CheckLine(Vector2Int.up, 1);
                CheckLine(Vector2Int.up, -1);
                CheckLine(Vector2Int.right, 1);
                CheckLine(Vector2Int.right, -1);
                break;

            case 4: // Bomb3x3
                CheckBox(1);
                break;

            case 5: // Bomb5x5
                CheckBox(2);
                break;

            case 6: // Bomb7x7
                CheckBox(3);
                break;
        }
    }

    private void CheckLine(Vector2Int pos, int _dir, int _max = -1)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        for (int i = _dir; ; i += _dir)
        {
            Vector2Int checkPos = pos * i;

            if (blockHandle.CheckOutBlockIndex(itemBlock.Pos, checkPos)) break;

            removeBlcok = blockHandle[itemBlock.Pos, checkPos];

            if (!Remove(Mathf.Abs(i), _max)) break;
        }
    }

    private void CheckBox(int _length)
    {
        BlockHandle blockHandle = LevelManager.Instance.blockHandle;

        _length += 1;

        for (int i = 1; i < _length; i++)
        {
            for (int j = -i; j < i + 1; j++)
            {
                Vector2Int pos = new(j, i);

                if (!blockHandle.CheckOutBlockIndex(itemBlock.Pos, pos))
                {
                    removeBlcok = blockHandle[itemBlock.Pos, pos];

                    Remove(Mathf.Abs(i));
                }

                pos = new(j, -i);

                if (!blockHandle.CheckOutBlockIndex(itemBlock.Pos, pos))
                {
                    removeBlcok = blockHandle[itemBlock.Pos, pos];

                    Remove(Mathf.Abs(i));
                }
            }
        }

        for (int i = 1; i < _length; i++)
        {
            for (int j = -i + 1; j < i; j++)
            {
                Vector2Int pos = new(i, j);

                if (!blockHandle.CheckOutBlockIndex(itemBlock.Pos, pos))
                {
                    removeBlcok = blockHandle[itemBlock.Pos, pos];

                    Remove(Mathf.Abs(i));
                }

                pos = new(-i, j);

                if (!blockHandle.CheckOutBlockIndex(itemBlock.Pos, pos))
                {
                    removeBlcok = blockHandle[itemBlock.Pos, pos];

                    Remove(Mathf.Abs(i));
                }
            }
        }
    }

    private bool Remove(int _absNum, int _max = -1)
    {
        if (removeBlcok == null) return true;
        if (removeBlcok.TargetPang == null) return true;

        if (_max != -1 && _absNum == _max + 1) return false;

        removeBlcok.TargetPang.StateBase.SetRemoveDelay(delay * (_absNum - 1));
        removeBlcok.TargetPang.StateBase.OnDestroy();

        return true;
    }
}
