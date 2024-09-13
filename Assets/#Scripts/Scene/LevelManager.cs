using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private BoardCreator BoardCreator => GameManager._instance.BoardCreator;
    private ObjectManager ObjectManager => GameManager._instance.ObjectManager;

    private Directions spawnDir;
    private Vector3 spawnVector;

    private int moveCount;
    private Pang spawnPang;

    private CheckMatchSystem checkMatchSystem;

    private readonly List<Block> spawnBlocks = new();

    public int MoveCount
    {
        get => moveCount;
        set
        {
            moveCount = value;

            if (value == 0) Debug.Log(checkMatchSystem.CheckLine(Directions.Right, BoardCreator[0, 0], 3));
        }
    }

    private void Start()
    {
        checkMatchSystem = new(this);
    }

    public void SetDirection(Directions _dir)
    {
        spawnBlocks.Clear();
        spawnDir = _dir;

        switch (spawnDir)
        {
            case Directions.Up:
                spawnVector = Vector3.up;

                for (int i = 0; i < BoardCreator.boardSize[0]; i++) spawnBlocks.Add(BoardCreator[i, BoardCreator.boardSize[1] - 1]);
                break;

            case Directions.Right:
                spawnVector = Vector3.right;

                for (int i = 0; i < BoardCreator.boardSize[1]; i++) spawnBlocks.Add(BoardCreator[BoardCreator.boardSize[0] - 1, i]);
                break;

            case Directions.Down:
                spawnVector = Vector3.down;

                for (int i = BoardCreator.boardSize[0] - 1; i >= 0; i--) spawnBlocks.Add(BoardCreator[i, 0]);
                break;

            case Directions.Left:
                spawnVector = Vector3.left;

                for (int i = BoardCreator.boardSize[1] - 1; i >= 0; i--) spawnBlocks.Add(BoardCreator[0, i]);
                break;
        }
    }

    public void SpawnAllPangs()
    {
        for (int i = 0; i < spawnBlocks.Count; i++) SpawnPang(spawnBlocks[i]);
    }

    public void SpawnPang(Block _block)
    {
        if (!IsFirst(_block.pos)) return;

        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.pangs.Dequeue();
            spawnPang.transform.position = _block.transform.position + spawnVector;
            spawnPang.TargetBlock = _block;

            spawnPang.SetType(PastelType.GangGi);
            spawnPang.StateBase.Move();
        }
    }

    public Block NextBlock(Directions _dir, int[] _pos)
    {
        return _dir switch
        {
            Directions.Down => BoardCreator[_pos[0] - (int)spawnVector.x, _pos[1] - (int)spawnVector.y],
            Directions.Left => BoardCreator[_pos[0] - (int)spawnVector.y, _pos[1] + (int)spawnVector.x],
            Directions.Up => BoardCreator[_pos[0] + (int)spawnVector.x, _pos[1] + (int)spawnVector.y],
            Directions.Right => BoardCreator[_pos[0] + (int)spawnVector.y, _pos[1] - (int)spawnVector.x],
            _ => null
        };
    }

    public bool IsFirst(int[] _pos)
    {
        return spawnDir switch
        {
            Directions.Up => _pos[1] == BoardCreator.boardSize[1] - 1,
            Directions.Right => _pos[0] == BoardCreator.boardSize[0] - 1,
            Directions.Down => _pos[1] == 0,
            Directions.Left => _pos[0] == 0,
            _ => false,
        };
    }
}

public enum Directions
{
    Up,
    Right,
    Down,
    Left
}
