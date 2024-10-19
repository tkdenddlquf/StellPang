using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private Directions spawnDir;
    private Vector3 spawnVector;

    private int moveCount;
    private Pang spawnPang;

    private CheckMatchSystem checkMatchSystem;

    public bool match = false;
    public Block[] selectBlocks = new Block[2];

    public readonly List<Pang> itemPangs = new();

    private readonly List<Block> spawnBlocks = new();
    private readonly List<PastelType> pastelTypes = new();

    public int MoveCount
    {
        get => moveCount;
        set
        {
            moveCount = value;

            if (value == 0) checkMatchSystem.CheckMatch();
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

                for (int i = 0; i < BoardCreator.Instance.boardSize[0]; i++) spawnBlocks.Add(BoardCreator.Instance[i, BoardCreator.Instance.boardSize[1] - 1]);
                break;

            case Directions.Right:
                spawnVector = Vector3.right;

                for (int i = 0; i < BoardCreator.Instance.boardSize[1]; i++) spawnBlocks.Add(BoardCreator.Instance[BoardCreator.Instance.boardSize[0] - 1, i]);
                break;

            case Directions.Down:
                spawnVector = Vector3.down;

                for (int i = BoardCreator.Instance.boardSize[0] - 1; i >= 0; i--) spawnBlocks.Add(BoardCreator.Instance[i, 0]);
                break;

            case Directions.Left:
                spawnVector = Vector3.left;

                for (int i = BoardCreator.Instance.boardSize[1] - 1; i >= 0; i--) spawnBlocks.Add(BoardCreator.Instance[0, i]);
                break;
        }
    }

    // 팡 관련
    public void SetPastelType(int _count)
    {
        PastelType _randType;

        while (pastelTypes.Count < _count)
        {
            _randType = (PastelType)Random.Range(0, System.Enum.GetNames(typeof(PastelType)).Length);

            if (!pastelTypes.Contains(_randType)) pastelTypes.Add(_randType);
        }
    }

    public void SpawnAllPangs()
    {
        for (int i = 0; i < spawnBlocks.Count; i++) SpawnPang(spawnBlocks[i]);
    }

    public void SpawnPang(Block _block)
    {
        if (this[_block.Pos, 0, 1] != null) return;

        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.Instance.pangs.Dequeue();
            spawnPang.transform.position = _block.transform.position + spawnVector;
            spawnPang.TargetBlock = _block;

            spawnPang.SetType(pastelTypes[Random.Range(0, pastelTypes.Count)]);
            spawnPang.StateBase.OnMove();
        }
    }

    public void SpawnPang(Block _block, DistractionType _type)
    {
        if (_block.BlockState == BlockState.Empty)
        {
            spawnPang = ObjectManager.Instance.pangs.Dequeue();
            spawnPang.transform.position = _block.transform.position;
            spawnPang.TargetBlock = _block;

            spawnPang.SetType(_type);
            spawnPang.StateBase.OnMove();
        }
    }

    // 블록 관련
    public Block this[int[] _pos, int _x, int _y]
    {
        get
        {
            return spawnDir switch
            {
                Directions.Up => BoardCreator.Instance[_pos[0] + _x, _pos[1] + _y],
                Directions.Right => BoardCreator.Instance[_pos[0] + _y, _pos[1] - _x],
                Directions.Down => BoardCreator.Instance[_pos[0] - _x, _pos[1] - _y],
                Directions.Left => BoardCreator.Instance[_pos[0] - _y, _pos[1] + _x],
                _ => null
            };
        }
    }

    public bool CheckOutBlockIndex(int[] _pos, int _x, int _y)
    {
        switch (spawnDir)
        {
            case Directions.Up:
                _x = _pos[0] + _x;
                _y = _pos[1] + _y;
                break;

            case Directions.Right:
                _x = _pos[0] + _y;
                _y = _pos[1] - _x;
                break;

            case Directions.Down:
                _x = _pos[0] - _x;
                _y = _pos[1] - _y;
                break;

            case Directions.Left:
                _x = _pos[0] - _y;
                _y = _pos[1] + _x;
                break;
        }

        if (_x < 0) return true;
        if (_y < 0) return true;

        if (_x > BoardCreator.Instance.boardSize[0] - 1) return true;
        if (_y > BoardCreator.Instance.boardSize[1] - 1) return true;

        return false;
    }

    public void SelectBlock(Block _block)
    {
        if (match) return;

        if (selectBlocks[0] == null)
        {
            selectBlocks[0] = _block;
            selectBlocks[0].TargetPang.selectImage.SetActive(true);
        }
        else if (selectBlocks[0] == _block)
        {
            selectBlocks[0] = null;
            selectBlocks[0].TargetPang.selectImage.SetActive(false);
        }
        else if (selectBlocks[1] == null)
        {
            selectBlocks[1] = _block;
            selectBlocks[1].TargetPang.selectImage.SetActive(true);

            Pang _pang = selectBlocks[1].TargetPang;

            selectBlocks[0].TargetPang.Swap(selectBlocks[1]);
            _pang.Swap(selectBlocks[0]);

            selectBlocks[0] = null;
            selectBlocks[1] = null;
        }
    }
}
