using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private void Awake() { _instance = this; }

    [Header("SCENE DATA")]
    public GameObject[] canvas;
    public GameObject[] popup;

    [Header("GAME CANVAS")]
    // Default
    public SpriteMask mask;
    public TMP_Text score_Text;
    public TMP_Text[] combo_Text;

    // TimeAttack
    public Slider timer;
    public TMP_Text timer_Text;

    // Stage
    public TMP_Text title_Text;
    public TMP_Text moveCount_Text;
    public Transform missionParent;

    [Header("RESULT POPUP")]
    public TMP_Text score_Text_Result;

    [Header("GAME DATA")]
    public Transform blocksParent;
    public Transform pangsParent;

    [Header("RESOURCES")]
    public Sprite[] pangSprites_Idle;
    public Sprite[] pangSprites_Pang;
    public Sprite[] itemSprites;
    public Sprite[] blockSprites;
    public Pang pangObj;
    public Block blockObj;
    public MissionObj missionObj;

    private Camera cam;
    private bool play = false;
    private bool checkMatch = false;
    private bool noticeHint = false;
    private float time = 0f;
    private float hintTime = 0f;
    private float nowScore = 0f;
    private int score = 0;
    private int combo = 0;
    private int moveCount = 0; // 내가 이동시킨 횟수
    private int moveLineCount = 0; // 이동중인 팡 갯수
    private int thisCanvas = -1;
    private int directionNumber = 0;
    private RaycastHit2D hitBlock;
    private Block hintBlock;
    private Block[,] blocks = new Block[0, 0];
    private PangType spawnType;

    private readonly Stack<Pang> leftOverPangs = new();
    private readonly Stack<Block> leftOverBlocks = new();
    private readonly Stack<MissionObj> leftOverMissionObjs = new();
    private readonly Dictionary<Block, PangType[]> spawnDict = new();

    private readonly List<Pang> itemPangs = new();
    private readonly Queue<Pang> useItemPangs = new();
    private readonly int[] boardSize = new int[2];
    private readonly Block[] hitBlocks = new Block[2];
    private readonly DirectionData[] directionData = new DirectionData[4];
    private readonly Color32[] blockColor =
    {
        new(10, 3, 36, 150), new(10, 3, 36, 200)
    };
    private readonly Color32[] glowColor =
    {
        new(129, 130, 184, 255),
        new(50, 46, 74, 255), new(155, 132, 222, 255),
        new(123, 47, 64, 255), new(43, 32, 32, 255), new(76, 17, 17, 255), new(71, 164, 223, 255),
        new(123, 102, 160, 255), new(42, 48, 81, 255), new(238, 113, 135, 255), new(72, 209, 136, 255)
    };
    
    public int Combo
    {
        get
        {
            return combo;
        }
        set
        {
            combo = value;

            if (combo > 1)
            {
                if (!combo_Text[0].gameObject.activeSelf) StartCoroutine(ComboAnim());
                else
                {
                    combo_Text[1].transform.localScale = Vector3.one;
                    combo_Text[1].color = new(combo_Text[1].color.r, combo_Text[1].color.g, combo_Text[1].color.b, 1);
                }

                combo_Text[0].text = $"{combo} 연속!";
                combo_Text[1].text = $"{combo} 연속!";
            }
        }
    }

    public int MoveCount
    {
        get
        {
            return moveCount;
        }
        set
        {
            moveCount = value;

            if (DataManager._instance.boardData.moveCount - moveCount < 0) moveCount_Text.text = 0.ToString();
            else moveCount_Text.text = (DataManager._instance.boardData.moveCount - moveCount).ToString();
        }
    }

    public int MoveLineCount
    {
        get
        {
            return moveLineCount;
        }
        set
        {
            moveLineCount = value;

            if (moveLineCount == 0)
            {
                if (!play)
                {
                    play = true;

                    SetBlockTypes(ref DataManager._instance.boardData.afterBlockTypes, true);

                    return;
                }

                HintTime = time;

                if (!checkMatch) StartCoroutine(CheckMatch());
            }
        }
    }

    public int[] DirectionNumber
    {
        get
        {
            return directionData[directionNumber].directionNumber;
        }
    }
    
    public bool NoticeHint
    {
        get
        {
            return noticeHint;
        }
        set
        {
            noticeHint = value;
        }
    }

    public float HintTime
    {
        get
        {
            return hintTime;
        }
        set
        {
            hintTime = value;
        }
    }

    public void Start()
    {
        cam = Camera.main;

        for (int i = 0; i < canvas.Length; i++) canvas[i].SetActive(false);

        SetCanvas(0);
    }

    private void Update()
    {
        if (play)
        {
            if (!checkMatch && Combo == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (NoticeHint)
                    {
                        NoticeHint = false;
                        HintTime = time;
                    }

                    if (MoveLineCount == 0)
                    {
                        hitBlock = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1);

                        if (hitBlock)
                        {
                            if (hitBlocks[0] == null)
                            {
                                hitBlock.transform.TryGetComponent(out hitBlocks[0]);

                                if (hitBlocks[0].Pang == null) ClearHitBlocks(0);
                                else if (hitBlocks[0].Type != BlockType.None) ClearHitBlocks(0);
                                else if (hitBlocks[0].Pang.ItemType != ItemType.None)
                                {
                                    AddUseItemPang(hitBlocks[0].Pang);

                                    if (!checkMatch) StartCoroutine(CheckMatch());

                                    ClearHitBlocks(0);
                                }
                                else hitBlocks[0].Pang.SelectPang(true);

                            }
                            else if (hitBlocks[1] == null)
                            {
                                hitBlock.transform.TryGetComponent(out hitBlocks[1]);

                                if (hitBlocks[0] == hitBlocks[1]) ClearHitBlocks();
                                else if (hitBlocks[1].Type != BlockType.None) ClearHitBlocks(1);
                                else if (hitBlocks[0].CheckConnectBlock(ref hitBlocks[1]))
                                {
                                    if (GetCommonType(hitBlocks[1].Type, BlockType_Common.Move))
                                    {
                                        if (hitBlocks[1].Pang != null)
                                        {
                                            if (hitBlocks[1].Pang.ItemType != ItemType.None) AddUseItemPang(hitBlocks[1].Pang);

                                            hitBlocks[1].Pang.SelectPang(true);
                                        }

                                        (hitBlocks[1].Pang, hitBlocks[0].Pang) = (hitBlocks[0].Pang, hitBlocks[1].Pang);
                                        MoveCount++;
                                    }
                                    else ClearHitBlocks(1);
                                }
                                else ClearHitBlocks();
                            }
                        }
                    }
                }
            }

            if (DataManager._instance.mission) // 미션이 있는 경우
            {
                SetTimer(Time.deltaTime);

                if (DataManager._instance.CheckMissionClear())
                {
                    StartCoroutine(EndGame());
                }
                else
                {
                    if (!NoticeHint)
                    {
                        if (time - HintTime > 3f)
                        {
                            NoticeHint = true;
                            HintTime = time;
                            hintBlock.Pang.HintPang();
                        }
                    }
                }
            }
            else
            {
                SetTimer(-Time.deltaTime);

                if (time == 0) StartCoroutine(EndGame());
                else
                {
                    if (!NoticeHint)
                    {
                        if (HintTime - time > 3f)
                        {
                            NoticeHint = true;
                            HintTime = 0;
                            hintBlock.Pang.HintPang();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) ESC();
        }

        if (nowScore < score) SetScore(Time.deltaTime * 5 * (score - nowScore));
    }

    // BUTTON
    public void PlayGame(string _name)
    {
        play = false;

        ClearHitBlocks();

        if (_name == "") // 재시작
        {
            ClearAllPangs(true);

            DataManager._instance.ResourceLoad();

            SetPopup(0, false);
        }
        else
        {
            DataManager._instance.ResourceLoad(_name);

            SetBoard(ref DataManager._instance.boardData.board);
        }

        title_Text.text = DataManager._instance.boardData.title;
        directionNumber = 0;
        MoveCount = 0;

        SetBlockTypes(ref DataManager._instance.boardData.beforeBlockTypes, false);

        SetScore(0, true);

        if (DataManager._instance.mission) SetTimer(0, true);
        else SetTimer(DataManager._instance.boardData.time, true);

        SetCanvas(1);

        directionData[directionNumber].SpawnAllPangs();
    }

    public void GoMain()
    {
        play = false;

        ClearBoard();

        SetScore(0, true);
        SetTimer(0, true);

        SetPopup(0, false);
        SetCanvas(0);
    }

    public void ESC()
    {
        if (!popup[1].activeSelf)
        {
            popup[1].SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            popup[1].SetActive(false);
            Time.timeScale = 1;
        }
    }

    // BOARD
    private void SetBoard(ref BoolArray[] _board)
    {
        boardSize[0] = _board.Length;
        boardSize[1] = _board[0].datas.Length;

        blocks = new Block[boardSize[0], boardSize[1]];

        for (int i = 0; i < boardSize[0]; i++)
        {
            for (int j = 0; j < boardSize[1]; j++)
            {
                if (!_board[i].datas[j]) continue;

                blocks[i, j] = PopBlock();
                blocks[i, j].transform.position = new(((float)(boardSize[1] - 1) / -2) + j, ((float)(boardSize[0] - 1) / 2) - i);
                blocks[i, j].gameObject.name = $"({i}, {j})";
                blocks[i, j].Init();
            }
        }

        for (int i = 0; i < boardSize[0]; i++)
        {
            for (int j = 0; j < boardSize[1]; j++)
            {
                if (blocks[i, j] == null) continue;

                if (i != 0)
                {
                    blocks[i, j].SetBlock(1, ref blocks[i - 1, j]);

                    for (int k = i - 1; k >= 0; k--)
                    {
                        if (blocks[k, j] == null) continue;

                        blocks[i, j].SetBlock_Next(1, ref blocks[k, j]);

                        break;
                    }
                }
                
                if (i != boardSize[0] - 1)
                {
                    blocks[i, j].SetBlock(3, ref blocks[i + 1, j]);

                    for (int k = i + 1; k < boardSize[0]; k++)
                    {
                        if (blocks[k, j] == null) continue;

                        blocks[i, j].SetBlock_Next(3, ref blocks[k, j]);

                        break;
                    }
                }
                
                if (j != 0)
                {
                    blocks[i, j].SetBlock(0, ref blocks[i, j - 1]);

                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (blocks[i, k] == null) continue;

                        blocks[i, j].SetBlock_Next(0, ref blocks[i, k]);

                        break;
                    }
                }
                
                if (j != boardSize[1] - 1)
                {
                    blocks[i, j].SetBlock(2, ref blocks[i, j + 1]);

                    for (int k = j + 1; k < boardSize[1]; k++)
                    {
                        if (blocks[i, k] == null) continue;

                        blocks[i, j].SetBlock_Next(2, ref blocks[i, k]);

                        break;
                    }
                }
            }
        }

        DrawBoardMask(ref _board);

        for (int i = 0; i < directionData.Length; i++)
        {
            if (directionData[i] == null) directionData[i] = new DirectionData();

            directionData[i].SetData(i, boardSize, ref blocks);
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < boardSize[0]; i++)
        {
            for (int j = 0; j < boardSize[1]; j++) PushBlock(blocks[i, j]);
        }
    }

    private void DrawBoardMask(ref BoolArray[] _board)
    {
        Texture2D _texture = new(boardSize[1], boardSize[0])
        {
            filterMode = FilterMode.Point
        };

        Color32[] _colors = new Color32[boardSize[0] * boardSize[1]];

        for (int x = 0; x < boardSize[0]; x++)
        {
            for (int y = 0; y < boardSize[1]; y++)
            {
                if (_board[boardSize[0] - x - 1].datas[y]) _colors[x * boardSize[1] + y] = Color.white;
                else _colors[x * boardSize[1] + y] = new(0, 0, 0, 0);
            }
        }

        _texture.SetPixels32(_colors);
        _texture.Apply();

        mask.sprite = Sprite.Create(_texture, new(0, 0, boardSize[1], boardSize[0]), new(0.5f, 0.5f), 1);
    }

    // STACK
    public Pang PopPang()
    {
        if (leftOverPangs.Count == 0) leftOverPangs.Push(Instantiate(pangObj, new(99, 99), Quaternion.identity, pangsParent));

        return leftOverPangs.Pop();
    }

    public void PushPang(Pang _pang)
    {
        if (_pang == null) return;

        _pang.Type = PangType.None;
        _pang.ItemType = ItemType.None;
        _pang.TargetBlock = null;
        _pang.transform.position = new(99, 99);

        leftOverPangs.Push(_pang);
    }

    public Block PopBlock()
    {
        if (leftOverBlocks.Count == 0) leftOverBlocks.Push(Instantiate(blockObj, new(99, 99), Quaternion.identity, blocksParent));

        return leftOverBlocks.Pop();
    }

    public void PushBlock(Block _block)
    {
        if (_block == null) return;

        _block.Type = BlockType.None;

        PushPang(_block.Pang);

        _block.Pang = null;
        _block.transform.position = new(99, 99);

        for (int i = 0; i < 4; i++) _block.SetBlock(i);

        leftOverBlocks.Push(_block);
    }

    public MissionObj PopMissionObj()
    {
        if (leftOverMissionObjs.Count == 0) leftOverMissionObjs.Push(Instantiate(missionObj, missionParent));

        return leftOverMissionObjs.Pop();
    }

    public void PushMissionObj(MissionObj _missionObj)
    {
        if (_missionObj == null) return;

        _missionObj.gameObject.SetActive(false);

        leftOverMissionObjs.Push(_missionObj);
    }

    // GET
    public Color32 GetBlockColor(bool _dark)
    {
        if (_dark) return blockColor[1];
        else return blockColor[0];
    }

    public Color32 GetGlowColor(PangType _type)
    {
        return glowColor[(int)_type - 1];
    }

    public Vector3 GetSpawnVector()
    {
        return directionData[directionNumber].directionNumber[1] switch
        {
            0 => Vector3.left,
            1 => Vector3.up,
            2 => Vector3.right,
            _ => Vector3.down,
        };
    }

    public bool GetSpawnBlock(Block _block)
    {
        return directionData[directionNumber].spawnBlocks.Contains(_block);
    }

    // SET
    public void SetDirection(int _num)
    {
        directionNumber = _num;
    }

    public void SetCanvas(int _num)
    {
        if (thisCanvas == _num) return;

        if (thisCanvas != -1) canvas[thisCanvas].SetActive(false);

        thisCanvas = _num;

        canvas[thisCanvas].SetActive(true);
    }

    public void SetScore(int _score, bool _clear = false)
    {
        if (_clear)
        {
            score = _score;
            nowScore = _score;

            score_Text.text = score.ToString();
        }
        else
        {
            score += _score;
        }
    }

    private void SetScore(float _score)
    {
        nowScore += _score;

        if (nowScore > score) nowScore = score;

        score_Text.text = $"{nowScore:N0}";
    }

    private void SetTimer(float _time, bool _clear = false)
    {
        if (_clear)
        {
            time = _time;
            timer.maxValue = _time;
        }
        else
        {
            time += _time;

            if (time < 0) time = 0;
        }

        timer.value = time;
        timer_Text.text = $"{time:N0}s";
    }

    // CHECK
    public IEnumerator CheckMatch()
    {
        checkMatch = true;

        bool _remove = false;

        for (int i = 0; i < 5; i++) // 아이템 생성 및 팡 실행
        {
            for (int j = 0; j < boardSize[0]; j++)
            {
                for (int k = 0; k < boardSize[1]; k++)
                {
                    if (blocks[j, k] == null) continue;

                    if (blocks[j, k].CheckMatchPang(i)) _remove = true;
                }
            }
        }

        while (true)
        {
            if (useItemPangs.Count == 0) break;

            if (UseItemPang(useItemPangs.Dequeue())) _remove = true;

            yield return null;
        }

        if (!_remove) // 파괴할 팡이 없는 경우
        {
            Combo = 0;

            if (hitBlocks[1] != null)
            {
                if  (hitBlocks[1].Pang != null) (hitBlocks[1].Pang, hitBlocks[0].Pang) = (hitBlocks[0].Pang, hitBlocks[1].Pang);
            }

            ClearHitBlocks();

            hintBlock = null;

            for (int i = 0; i < boardSize[0]; i++)
            {
                for (int j = 0; j < boardSize[1]; j++)
                {
                    if (blocks[i, j] == null) continue;

                    blocks[i, j].CheckHintPang(ref hintBlock);

                    if (hintBlock != null)
                    {
                        i = boardSize[0];

                        break;
                    }
                }
            }

            if (hintBlock == null) // 완성이 불가능한 경우
            {
                if (itemPangs.Count == 0)
                {
                    ClearAllPangs();
                    directionData[directionNumber].SpawnAllPangs();
                }
                else hintBlock = itemPangs[0].TargetBlock;
            }

            checkMatch = false;
        }
        else
        {
            ClearHitBlocks();

            Combo++;

            checkMatch = false;

            directionData[directionNumber].RefreshBlocks();
        }
    }

    public bool CheckSelectBlock(Block _block)
    {
        if (hitBlocks[0] == _block) return true;
        if (hitBlocks[1] == _block) return true;

        return false;
    }

    // TYPES
    public bool GetCommonType(BlockType _type, BlockType_Common _common)
    {
        switch (_common)
        {
            case BlockType_Common.Move:
                if (_type == BlockType.None) return true;
                else if (_type == BlockType.Ice) return true;
                break;

            case BlockType_Common.DeadLock:
                if (_type == BlockType.DeadLock) return true;
                break;

            case BlockType_Common.Break:
                if (_type == BlockType.Box) return true;
                else if (_type == BlockType.Ice) return true;
                break;
        }

        return false;
    }

    public PangType GetRandomPangType(Block _block)
    {
        PangType _type;

        if (DataManager._instance.priorityPangTypes.Count != 0)
        {
            _type = DataManager._instance.priorityPangTypes.Dequeue();

            if (_type != PangType.None) return _type;
        }

        if (play)
        {
            if (!DataManager._instance.mission) return DataManager._instance.pangTypes[Random.Range(0, DataManager._instance.pangTypes.Count)];
        }

        while (true)
        {
            _type = DataManager._instance.pangTypes[Random.Range(0, DataManager._instance.pangTypes.Count)];

            if (spawnType == _type) _type = PangType.None;
            else
            {
                if (spawnDict.ContainsKey(_block))
                {
                    if (spawnDict[_block][0] == _type) _type = PangType.None;
                }
            }

            if (_type != PangType.None) break;
        }

        RecordPangType(_block, _type);

        return _type;
    }

    private void RecordPangType(Block _block, PangType _type)
    {
        if (spawnDict.ContainsKey(_block))
        {
            spawnDict[_block][0] = spawnDict[_block][1];
            spawnDict[_block][1] = _type;
        }
        else spawnDict[_block] = new PangType[] { _type, _type };

        spawnType = _type;
    }

    private void SetBlockType(BlockType _type, int _x, int _y)
    {
        if (blocks[_y, _x] == null) return;

        blocks[_y, _x].Type = _type;
    }

    private void SetBlockTypes(ref IntArray[] _value, bool _start)
    {
        if (_value != null)
        {
            for (int i = 0; i < _value.Length; i++)
            {
                SetBlockType((BlockType)_value[i].datas[0], _value[i].datas[1], _value[i].datas[2]);
            }
        }

        if (_start)
        {
            HintTime = time;
            StartCoroutine(CheckMatch());
        }
    }

    // ITEM
    public void AddItemPang(Pang _pang)
    {
        itemPangs.Add(_pang);
    }

    public void AddUseItemPang(Pang _pang)
    {
        itemPangs.Remove(_pang);
        useItemPangs.Enqueue(_pang);
    }

    private bool UseItemPang(Pang _pang)
    {
        int _count = 1;

        for (int i = 0; i < boardSize[0]; i++)
        {
            for (int j = 0; j < boardSize[1]; j++)
            {
                if (blocks[i, j] == null) continue;

                if (blocks[i, j] == _pang.TargetBlock)
                {
                    switch (_pang.ItemType)
                    {
                        case ItemType.VertBomb:
                            while (true)
                            {
                                UseVertBomb(_count, i, j);

                                _count++;

                                if (i - _count < 0 && i + _count >= boardSize[0]) break;
                            }
                            break;

                        case ItemType.HoriBomb:
                            while (true)
                            {
                                UseHoriBomb(_count, i, j);

                                _count++;

                                if (j - _count < 0 && j + _count >= boardSize[1]) break;
                            }
                            break;

                        case ItemType.TBomb_Small:
                            UseVertBomb(_count, i, j);
                            UseHoriBomb(_count, i, j);
                            break;

                        case ItemType.TBomb_Long:
                            while (true)
                            {
                                UseVertBomb(_count, i, j);
                                UseHoriBomb(_count, i, j);

                                _count++;

                                if (i - _count < 0 && i + _count >= boardSize[0])
                                {
                                    if (j - _count < 0 && j + _count >= boardSize[1]) break;
                                }
                            }
                            break;

                        case ItemType.Bomb_3x3:
                            UseRangeBomb(_count, i, j);
                            break;

                        case ItemType.Bomb_5x5:
                            UseRangeBomb(_count++, i, j);
                            UseRangeBomb(_count, i, j);
                            break;

                        case ItemType.Bomb_7x7:
                            UseRangeBomb(_count++, i, j);
                            UseRangeBomb(_count++, i, j);
                            UseRangeBomb(_count, i, j);
                            break;
                    }

                    _pang.TargetBlock.RemovePang(RemoveType.Item);

                    return true;
                }
            }
        }

        return false;
    }

    private void UseVertBomb(int _count, int _x, int _y)
    {
        if (_x - _count >= 0)
        {
            if (blocks[_x - _count, _y] != null) blocks[_x - _count, _y].RemovePang(RemoveType.Item, (_count - 1) * 0.1f);
        }

        if (_x + _count < boardSize[0])
        {
            if (blocks[_x + _count, _y] != null) blocks[_x + _count, _y].RemovePang(RemoveType.Item, (_count - 1) * 0.1f);
        }
    }

    private void UseHoriBomb(int _count, int _x, int _y)
    {
        if (_y - _count >= 0)
        {
            if (blocks[_x, _y - _count] != null) blocks[_x, _y - _count].RemovePang(RemoveType.Item, (_count - 1) * 0.1f);
        }

        if (_y + _count < boardSize[1])
        {
            if (blocks[_x, _y + _count] != null) blocks[_x, _y + _count].RemovePang(RemoveType.Item, (_count - 1) * 0.1f);
        }
    }

    private void UseRangeBomb(int _range, int _x, int _y)
    {
        int _length = _y - _range + 2 * _range + 1;

        if (_x - _range >= 0)
        {
            for (int k = _y - _range; k < _length; k++)
            {
                if (k >= 0 && k < boardSize[1])
                {
                    if (blocks[_x - _range, k] == null) continue;

                    blocks[_x - _range, k].RemovePang(RemoveType.Item);
                }
            }
        }

        if (_x + _range < boardSize[0])
        {
            for (int k = _y - _range; k < _length; k++)
            {
                if (k >= 0 && k < boardSize[1])
                {
                    if (blocks[_x + _range, k] == null) continue;

                    blocks[_x + _range, k].RemovePang(RemoveType.Item);
                }
            }
        }

        _length = _x - (_range - 1) + 2 * (_range - 1) + 1;

        if (_y - _range >= 0)
        {
            for (int k = _x - (_range - 1); k < _length; k++)
            {
                if (k >= 0 && k < boardSize[0])
                {
                    if (blocks[k, _y - _range] == null) continue;

                    blocks[k, _y - _range].RemovePang(RemoveType.Item);
                }
            }
        }

        if (_y + _range < boardSize[1])
        {
            for (int k = _x - (_range - 1); k < _length; k++)
            {
                if (k >= 0 && k < boardSize[0])
                {
                    if (blocks[k, _y + _range] == null) continue;

                    blocks[k, _y + _range].RemovePang(RemoveType.Item);
                }
            }
        }
    }

    // ATEHR
    public void ClearAllPangs(bool _end = false)
    {
        for (int i = 0; i < boardSize[0]; i++)
        {
            for (int j = 0; j < boardSize[1]; j++)
            {
                if (blocks[i, j] == null) continue;

                if (_end) blocks[i, j].Type = BlockType.None;

                blocks[i, j].RemovePang(RemoveType.Clear);
            }
        }
    }

    private void ClearHitBlocks(int _index = -1)
    {
        if (_index == -1)
        {
            ClearHitBlocks(0);
            ClearHitBlocks(1);
        }
        else
        {
            if (hitBlocks[_index] != null)
            {
                if (hitBlocks[_index].Pang != null) hitBlocks[_index].Pang.SelectPang(false);
            }

            hitBlocks[_index] = null;
        }
    }

    private void SetPopup(int _index, bool _active = true)
    {
        if (_active) StartCoroutine(PopupAnim(popup[_index]));
        else popup[_index].SetActive(false);
    }

    private IEnumerator PopupAnim(GameObject _popup)
    {
        if (_popup.activeSelf) yield break;

        _popup.SetActive(true);
        _popup.transform.localScale = Vector3.one * 0.4f;

        while (true)
        {
            if (_popup.transform.localScale.x >= 1.2f) break;

            _popup.transform.localScale += new Vector3(4, 4) * Time.deltaTime;

            yield return null;
        }

        while (true)
        {
            if (_popup.transform.localScale.x < 1) break;

            _popup.transform.localScale -= new Vector3(4, 4) * Time.deltaTime;

            yield return null;
        }

        _popup.transform.localScale = Vector3.one;
    }

    private IEnumerator ComboAnim()
    {
        combo_Text[0].gameObject.SetActive(true);
        combo_Text[1].gameObject.SetActive(true);

        while (true)
        {
            if (combo_Text[1].color.a <= 0) break;

            combo_Text[1].transform.localScale += Vector3.one * Time.deltaTime;
            combo_Text[1].color -= Color.black * Time.deltaTime;

            yield return null;
        }

        combo_Text[0].gameObject.SetActive(false);
        combo_Text[1].gameObject.SetActive(false);

        combo_Text[1].transform.localScale = Vector3.one;
        combo_Text[1].color = new(combo_Text[1].color.r, combo_Text[1].color.g, combo_Text[1].color.b, 1);
    }

    private IEnumerator EndGame()
    {
        play = false;

        yield return new WaitUntil(() => MoveLineCount == 0);

        while (true)
        {
            if (itemPangs.Count == 0)
            {
                yield return new WaitForSeconds(1f);
                yield return new WaitUntil(() => MoveLineCount == 0);

                if (itemPangs.Count == 0) break;
            }

            AddUseItemPang(itemPangs[0]);

            StartCoroutine(CheckMatch());

            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => MoveLineCount == 0);
        }

        yield return new WaitForSeconds(1f);

        ClearAllPangs(true);

        score_Text_Result.text = $"점수 : {score:#,###} 점";
        SetPopup(0);

        yield return null;
    }
}
