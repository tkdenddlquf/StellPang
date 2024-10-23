using UnityEngine;

public class BoardCreator : Singleton<BoardCreator>
{
    public SpriteMask mask;

    private Color32 blockColor = new(10, 3, 36, 150);

    public Block[,] Board { get; private set; }

    public Block this[int _x, int _y]
    {
        get
        {
            if (_x < 0) return null;
            if (_y < 0) return null;

            if (_x > boardSize[0] - 1) return null;
            if (_y > boardSize[1] - 1) return null;

            return Board[_y, _x];
        }
    }

    public readonly int[] boardSize = new int[2];

    public void CreateBoard(BoardData _boardData)
    {
        boardSize[0] = _boardData.blocks[0].blockNums.Length;
        boardSize[1] = _boardData.blocks.Length;

        Board = new Block[boardSize[1], boardSize[0]];

        Vector2 _startPos = new(-(boardSize[0] - 1f) / 2, -(boardSize[1] - 1f) / 2);

        for (int x = 0; x < boardSize[1]; x++)
        {
            for (int y = 0; y < boardSize[0]; y++)
            {
                if (_boardData.blocks[^(y + 1)].blockNums[x] == -1) continue;

                Board[x, y] = ObjectManager.Instance.blocks.Dequeue();

                Board[x, y].Pos[0] = y;
                Board[x, y].Pos[1] = x;

                if ((x + y) % 2 == 0) blockColor.a = 150;
                else blockColor.a = 200;

                Board[x, y].background.color = blockColor;
                Board[x, y].transform.position = _startPos;

                _startPos.x++;
            }

            _startPos.x = -(boardSize[0] - 1f) / 2;
            _startPos.y++;
        }

        CreateMask();
    }

    private void CreateMask()
    {
        Texture2D _texture = new(boardSize[0], boardSize[1])
        {
            filterMode = FilterMode.Point
        };

        Color32[] _colors = new Color32[boardSize[0] * boardSize[1]];
        int _count = 0;

        for (int x = 0; x < boardSize[1]; x++)
        {
            for (int y = 0; y < boardSize[0]; y++)
            {
                if (Board[x, y] != null) _colors[_count++] = Color.white;
                else _colors[_count++] = new(0, 0, 0, 0);
            }
        }

        _texture.SetPixels32(_colors);
        _texture.Apply();

        mask.sprite = Sprite.Create(_texture, new(0, 0, boardSize[0], boardSize[1]), new(0.5f, 0.5f), 1);
    }
}