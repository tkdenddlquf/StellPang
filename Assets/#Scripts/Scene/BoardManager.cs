using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public SpriteMask mask;

    private Block[,] board;
    private ObjectManager objectManager;

    private void Start()
    {
        TryGetComponent(out objectManager);

        CreateBoard(5, 4);
    }

    public void CreateBoard(int _sizeX, int _sizeY)
    {
        board = new Block[_sizeY, _sizeX];

        Vector2 _startPos = new(-(float)(_sizeX - 1) / 2, (float)(_sizeY - 1) / 2);
        Color32 _blockColor = new(10, 3, 36, 150);

        for (int x = 0; x < _sizeY; x++)
        {
            for (int y = 0; y < _sizeX; y++)
            {
                board[x, y] = objectManager.blocks.Dequeue();
                board[x, y].SetType(BlockType.None);

                if ((x + y) % 2 == 0) _blockColor.a = 150;
                else _blockColor.a = 200;

                board[x, y].background.color = _blockColor;
                board[x, y].transform.position = _startPos;

                _startPos.x++;
            }

            _startPos.x = -(float)(_sizeX - 1) / 2;
            _startPos.y--;
        }

        CreateMask(_sizeX, _sizeY);
    }

    private void CreateMask(int _sizeX, int _sizeY)
    {
        Texture2D _texture = new(_sizeX, _sizeY)
        {
            filterMode = FilterMode.Point
        };

        Color32[] _colors = new Color32[_sizeX * _sizeY];
        int _count = 0;

        for (int x = 0; x < _sizeY; x++)
        {
            for (int y = 0; y < _sizeX; y++)
            {
                if (board[x, y] != null) _colors[_count++] = Color.white;
                else _colors[_count++] = new(0, 0, 0, 0);
            }
        }

        _texture.SetPixels32(_colors);
        _texture.Apply();

        mask.sprite = Sprite.Create(_texture, new(0, 0, _sizeX, _sizeY), new(0.5f, 0.5f), 1);
    }
}