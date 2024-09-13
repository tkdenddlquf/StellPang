using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private BoardCreator BoardCreator => GameManager._instance.BoardCreator;
    private ObjectManager ObjectManager => GameManager._instance.ObjectManager;

    private BoardDir boardDir;

    private Pang spawnPang;

    public void SpawnPang()
    {
        switch (boardDir)
        {
            case BoardDir.Up:
                for (int i = 0; i < BoardCreator.boardSize[0]; i++)
                {
                    SpawnPang(BoardCreator[i, 0], Vector3.up);
                }
                break;

            case BoardDir.Right:
                for (int i = 0; i < BoardCreator.boardSize[1]; i++)
                {
                    SpawnPang(BoardCreator[0, i], Vector3.right);
                }
                break;

            case BoardDir.Down:
                for (int i = BoardCreator.boardSize[0] - 1; i >= 0; i--)
                {
                    SpawnPang(BoardCreator[i, 0], Vector3.down);
                }
                break;

            case BoardDir.Left:
                for (int i = BoardCreator.boardSize[1] - 1; i >= 0; i--)
                {
                    SpawnPang(BoardCreator[0, i], Vector3.left);
                }
                break;
        }
    }

    private void SpawnPang(Block _block, Vector3 _pos)
    {
        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.pangs.Dequeue();
            spawnPang.transform.position = _block.transform.position + _pos;
            spawnPang.TargetBlock = _block;

            spawnPang.SetType(PastelType.GangGi);
            spawnPang.StateBase.Move();
        }
    }

    public Block NextBlock(int[] _pos)
    {
        return boardDir switch
        {
            BoardDir.Up => BoardCreator[_pos[0], _pos[1] + 1],
            BoardDir.Right => BoardCreator[_pos[0] - 1, _pos[1]],
            BoardDir.Down => BoardCreator[_pos[0], _pos[1] - 1],
            BoardDir.Left => BoardCreator[_pos[0] + 1, _pos[1]],
            _ => null,
        };
    }
}

public enum BoardDir
{
    Up,
    Right,
    Down,
    Left
}
