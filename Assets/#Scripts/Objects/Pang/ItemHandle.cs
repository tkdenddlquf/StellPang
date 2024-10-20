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
                CheckLine(0, 1, 1);
                CheckLine(0, 1, -1);
                break;

            case 1: // BombHori
                CheckLine(1, 0, 1);
                CheckLine(1, 0, -1);
                break;

            case 2: // BombSmallCross
                CheckLine(0, 1, 1, 1);
                CheckLine(0, 1, -1, 1);
                CheckLine(1, 0, 1, 1);
                CheckLine(1, 0, -1, 1);
                break;

            case 3: // BombLargeCross
                CheckLine(0, 1, 1);
                CheckLine(0, 1, -1);
                CheckLine(1, 0, 1);
                CheckLine(1, 0, -1);
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

    private void CheckLine(int _x, int _y, int _dir, int _max = -1)
    {
        for (int i = _dir; ; i += _dir)
        {
            if (LevelManager.Instance.blockHandle.CheckOutBlockIndex(itemBlock.Pos, _x * i, _y * i)) break;

            removeBlcok = LevelManager.Instance.blockHandle[itemBlock.Pos, _x * i, _y * i];

            if (!Remove(Mathf.Abs(i), _max)) break;
        }
    }

    private void CheckBox(int _length)
    {
        _length += 1;

        for (int i = 1; i < _length; i++)
        {
            for (int j = -i; j < i + 1; j++)
            {
                if (!LevelManager.Instance.blockHandle.CheckOutBlockIndex(itemBlock.Pos, j, i))
                {
                    removeBlcok = LevelManager.Instance.blockHandle[itemBlock.Pos, j, i];

                    Remove(Mathf.Abs(i));
                }

                if (!LevelManager.Instance.blockHandle.CheckOutBlockIndex(itemBlock.Pos, j, -i))
                {
                    removeBlcok = LevelManager.Instance.blockHandle[itemBlock.Pos, j, -i];

                    Remove(Mathf.Abs(i));
                }
            }
        }

        for (int i = 1; i < _length; i++)
        {
            for (int j = -i + 1; j < i; j++)
            {
                if (!LevelManager.Instance.blockHandle.CheckOutBlockIndex(itemBlock.Pos, i, j))
                {
                    removeBlcok = LevelManager.Instance.blockHandle[itemBlock.Pos, i, j];

                    Remove(Mathf.Abs(i));
                }

                if (!LevelManager.Instance.blockHandle.CheckOutBlockIndex(itemBlock.Pos, -i, j))
                {
                    removeBlcok = LevelManager.Instance.blockHandle[itemBlock.Pos, -i, j];

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
