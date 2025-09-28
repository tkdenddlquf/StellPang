using System.Collections.Generic;
using UnityEngine;

public class SpawnHandle
{
    private Pang spawnPang;

    private readonly List<Block> spawnBlocks = new();
    private readonly List<PastelType> pastelTypes = new();

    public Vector2Int SpawnDir { get; private set; }
    public Directions SpawnDirType { get; private set; }

    public void SetPastelType(int _count)
    {
        PastelType _randType;

        while (pastelTypes.Count < _count)
        {
            _randType = (PastelType)Random.Range(0, System.Enum.GetNames(typeof(PastelType)).Length);

            if (!pastelTypes.Contains(_randType)) pastelTypes.Add(_randType);
        }
    }

    public int GetPangType(int _type) => (int)pastelTypes[_type];

    public void SetDirection(Directions _dir)
    {
        BoardCreator boardCreator = BoardCreator.Instance;

        spawnBlocks.Clear();
        SpawnDirType = _dir;

        Vector2Int maxSize = boardCreator.boardSize - Vector2Int.one;

        switch (_dir)
        {
            case Directions.Up:
                SpawnDir = Vector2Int.up;

                for (int i = 0; i < boardCreator.boardSize[0]; i++) spawnBlocks.Add(boardCreator[new(i, maxSize.y)]);
                break;

            case Directions.Right:
                SpawnDir = Vector2Int.right;

                for (int i = 0; i < boardCreator.boardSize[1]; i++) spawnBlocks.Add(boardCreator[new(maxSize.x, i)]);
                break;

            case Directions.Down:
                SpawnDir = Vector2Int.down;

                for (int i = boardCreator.boardSize[0] - 1; i >= 0; i--) spawnBlocks.Add(boardCreator[new(i, 0)]);
                break;

            case Directions.Left:
                SpawnDir = Vector2Int.left;

                for (int i = boardCreator.boardSize[1] - 1; i >= 0; i--) spawnBlocks.Add(boardCreator[new(0, i)]);
                break;
        }
    }

    public void SpawnAllPangs()
    {
        for (int i = 0; i < spawnBlocks.Count; i++) SpawnPang(spawnBlocks[i]);
    }

    public void SpawnPang(Block _block)
    {
        if (LevelManager.Instance.blockHandle[_block.Pos, Vector2Int.up] != null) return;

        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.Instance.PangPool.Get();
            spawnPang.transform.position = (Vector2)_block.transform.position + (Vector2)SpawnDir;
            spawnPang.TargetBlock = _block;

            spawnPang.SetType(pastelTypes[Random.Range(0, pastelTypes.Count)]);
            spawnPang.StateBase.OnMove();
        }
    }

    public void SpawnPang(Block _block, DistractionType _type)
    {
        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.Instance.PangPool.Get();
            spawnPang.transform.position = _block.transform.position;
            spawnPang.TargetBlock = _block;

            spawnPang.SetType(_type);
            spawnPang.StateBase.OnMove();
        }
    }
}
