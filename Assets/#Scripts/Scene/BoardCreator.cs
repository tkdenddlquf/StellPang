using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    public SpriteMask mask;

    private Color32 blockColor = new(10, 3, 36, 150);

    public Block[,] Board { get; private set; }
    public ObjectManager ObjectManager { get; private set; }

    public readonly int[] boardSize = new int[2];

    private void Start()
    {
        ObjectManager = GetComponent<ObjectManager>();

        boardSize[0] = 5;
        boardSize[1] = 4;

        CreateBoard();
    }

    public void CreateBoard()
    {
        Board = new Block[boardSize[1], boardSize[0]];

        Vector2 _startPos = new(-(float)(boardSize[0] - 1) / 2, (float)(boardSize[1] - 1) / 2);

        for (int x = 0; x < boardSize[1]; x++)
        {
            for (int y = 0; y < boardSize[0]; y++)
            {
                Board[x, y] = ObjectManager.blocks.Dequeue();
                Board[x, y].SetType(BlockType.None);

                if ((x + y) % 2 == 0) blockColor.a = 150;
                else blockColor.a = 200;

                Board[x, y].background.color = blockColor;
                Board[x, y].transform.position = _startPos;

                _startPos.x++;
            }

            _startPos.x = -(float)(boardSize[0] - 1) / 2;
            _startPos.y--;
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