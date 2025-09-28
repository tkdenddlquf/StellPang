using UnityEngine;

public class BoardCreator : Singleton<BoardCreator>
{
    [SerializeField] private SpriteMask mask;

    public Vector2Int BoardSize { get; private set; }

    private Color32 blockColor = new(10, 3, 36, 150);

    public Block[,] Board { get; private set; }

    public Block this[Vector2Int pos]
    {
        get
        {
            if (!CheckInRange(pos)) return null;

            return Board[pos.y, pos.x];
        }
    }

    public Block this[Directions dir, Vector2Int currentPos, Vector2Int moveVector] => this[GetMovePos(dir, currentPos, moveVector)];

    public void CreateBoard(BoardData _boardData)
    {
        ObjectManager objectManager = ObjectManager.Instance;
        BoardLineData[] boardLineDatas = _boardData.blocks;

        BoardSize = new(boardLineDatas[0].blockNums.Length, boardLineDatas.Length);

        Board = new Block[BoardSize.y, BoardSize.x];

        Vector2 _startPos = new(-(BoardSize.x - 1f) / 2, -(BoardSize.y - 1f) / 2);

        for (int x = 0; x < BoardSize.y; x++)
        {
            for (int y = 0; y < BoardSize.x; y++)
            {
                if (boardLineDatas[^(y + 1)].blockNums[x] == -1) continue;

                Board[x, y] = objectManager.BlockPool.Get();
                Board[x, y].Pos = new (y, x);

                if ((x + y) % 2 == 0) blockColor.a = 150;
                else blockColor.a = 200;

                Board[x, y].background.color = blockColor;
                Board[x, y].transform.position = _startPos;

                _startPos.x++;
            }

            _startPos.x = -(BoardSize.x - 1f) / 2;
            _startPos.y++;
        }

        CreateMask();
    }

    private void CreateMask()
    {
        Texture2D _texture = new(BoardSize.x, BoardSize.y)
        {
            filterMode = FilterMode.Point
        };

        Color32[] _colors = new Color32[BoardSize.x * BoardSize.y];
        int _count = 0;

        for (int x = 0; x < BoardSize.y; x++)
        {
            for (int y = 0; y < BoardSize.x; y++)
            {
                if (Board[x, y] != null) _colors[_count++] = Color.white;
                else _colors[_count++] = new(0, 0, 0, 0);
            }
        }

        _texture.SetPixels32(_colors);
        _texture.Apply();

        mask.sprite = Sprite.Create(_texture, new(0, 0, BoardSize.x, BoardSize.y), new(0.5f, 0.5f), 1);
    }

    public bool CheckInRange(Vector2Int pos)
    {
        if (pos.x < 0) return false;
        if (pos.y < 0) return false;

        if (pos.x > BoardSize.x - 1) return false;
        if (pos.y > BoardSize.y - 1) return false;

        return true;
    }

    public bool CheckInRange(Directions dir, Vector2Int currentPos, Vector2Int moveVector)
    {
        Vector2Int pos = GetMovePos(dir, currentPos, moveVector);

        return CheckInRange(pos);
    }

    public Vector2Int GetMovePos(Directions dir, Vector2Int currentPos, Vector2Int moveVector)
    {
        return dir switch
        {
            Directions.Up => new(currentPos.x + moveVector.x, currentPos.y + moveVector.y),
            Directions.Right => new(currentPos.x + moveVector.y, currentPos.y - moveVector.x),
            Directions.Down => new(currentPos.x - moveVector.x, currentPos.y - moveVector.y),
            Directions.Left => new(currentPos.x - moveVector.y, currentPos.y + moveVector.x),
            _ => currentPos
        };
    }
}