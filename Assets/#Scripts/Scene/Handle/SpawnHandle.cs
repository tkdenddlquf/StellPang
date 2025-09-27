using System.Collections.Generic;
using UnityEngine;

public class SpawnHandle
{
    private Pang spawnPang;

    private readonly List<Block> spawnBlocks = new();
    private readonly List<PastelType> pastelTypes = new();

    public Directions SpawnDir { get; private set; }
    public Vector3 SpawnVector { get; private set; }

    public void SetPastelType(int _count)
    {
        PastelType _randType;

        while (pastelTypes.Count < _count)
        {
            _randType = (PastelType)Random.Range(0, System.Enum.GetNames(typeof(PastelType)).Length);

            if (!pastelTypes.Contains(_randType)) pastelTypes.Add(_randType);
        }
    }

    public int GetPangType(int _type)
    {
        return (int)pastelTypes[_type];
    }

    public void SetDirection(Directions _dir)
    {
        BoardCreator boardCreator = BoardCreator.Instance;

        spawnBlocks.Clear();
        SpawnDir = _dir;

        switch (_dir)
        {
            case Directions.Up:
                SpawnVector = Vector3.up;

                for (int i = 0; i < boardCreator.boardSize[0]; i++) spawnBlocks.Add(boardCreator[i, boardCreator.boardSize[1] - 1]);
                break;

            case Directions.Right:
                SpawnVector = Vector3.right;

                for (int i = 0; i < boardCreator.boardSize[1]; i++) spawnBlocks.Add(boardCreator[boardCreator.boardSize[0] - 1, i]);
                break;

            case Directions.Down:
                SpawnVector = Vector3.down;

                for (int i = boardCreator.boardSize[0] - 1; i >= 0; i--) spawnBlocks.Add(boardCreator[i, 0]);
                break;

            case Directions.Left:
                SpawnVector = Vector3.left;

                for (int i = boardCreator.boardSize[1] - 1; i >= 0; i--) spawnBlocks.Add(boardCreator[0, i]);
                break;
        }
    }

    public void SpawnAllPangs()
    {
        for (int i = 0; i < spawnBlocks.Count; i++) SpawnPang(spawnBlocks[i]);
    }

    public void SpawnPang(Block _block)
    {
        if (LevelManager.Instance.blockHandle[_block.Pos, 0, 1] != null) return;

        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.Instance.PangPool.Get();
            spawnPang.transform.position = _block.transform.position + SpawnVector;
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
