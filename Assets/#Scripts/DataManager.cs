using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager _instance;
    private void Awake() { _instance = this; }

    public string fileName = "";
    public BoardData boardData;
    public List<PangType> pangTypes = new();
    public Queue<PangType> priorityPangTypes = new();

    public bool mission = false;
    public int[] mission_Clear = new int[] { 0, 0, 0 };
    public Dictionary<BlockType, MissionData> mission_BlockTypes = new();
    public Dictionary<PangType, MissionData> mission_PangTypes = new();
    public Dictionary<ItemType, MissionData> mission_ItemTypes = new();

    public void ResourceLoad(string _fileName)
    {
        fileName = _fileName;

        ResourceLoad();
    }

    public void ResourceLoad()
    {
        pangTypes.Clear();
        priorityPangTypes.Clear();
        ClearMissions();

        TextAsset _textAsset = (TextAsset)Resources.Load($"Json/{fileName}");
        boardData = JsonUtility.FromJson<BoardData>(_textAsset.text);

        if (pangTypes.Count < boardData.pangCount)
        {
            int _length = GameManager._instance.pangSprites_Idle.Length;
            PangType _type;

            while (true)
            {
                _type = (PangType)Random.Range(1, _length);

                if (!pangTypes.Contains(_type))
                {
                    if (boardData.pangCount < _length - 2)
                    {
                        switch (_type)
                        {
                            case PangType.Kanna:
                                if (!pangTypes.Contains(PangType.Hina))
                                {
                                    if (!pangTypes.Contains(PangType.Mashiro))
                                    {
                                        pangTypes.Add(_type);
                                    }
                                }
                                break;

                            case PangType.Hina:
                                if (!pangTypes.Contains(PangType.Kanna))
                                {
                                    if (!pangTypes.Contains(PangType.Mashiro))
                                    {
                                        pangTypes.Add(_type);
                                    }
                                }
                                break;

                            case PangType.Mashiro:
                                if (!pangTypes.Contains(PangType.Kanna))
                                {
                                    if (!pangTypes.Contains(PangType.Hina))
                                    {
                                        pangTypes.Add(_type);
                                    }
                                }
                                break;

                            default:
                                pangTypes.Add(_type);
                                break;
                        }
                    }
                    else pangTypes.Add(_type);

                    if (pangTypes.Count == boardData.pangCount) break;
                }
            }
        }

        if (boardData.priorityPangTypes != null)
        {
            for (int i = 0; i < boardData.priorityPangTypes.Length; i++)
            {
                if (boardData.priorityPangTypes[i] != -1)
                {
                    if (boardData.priorityPangTypes[i] <= pangTypes.Count)
                    {
                        priorityPangTypes.Enqueue(pangTypes[boardData.priorityPangTypes[i]]);

                        continue;
                    }
                }

                priorityPangTypes.Enqueue(PangType.None);
            }
        }

        if (boardData.missions != null)
        {
            for (int i = 0; i < boardData.missions.Length; i++)
            {
                switch (boardData.missions[i].datas[0])
                {
                    case 0:
                        SetMission((BlockType)boardData.missions[i].datas[1], boardData.missions[i].datas[2]);
                        break;

                    case 1:
                        SetMission(pangTypes[boardData.missions[i].datas[1]], boardData.missions[i].datas[2]);
                        break;

                    case 2:
                        SetMission((ItemType)boardData.missions[i].datas[1], boardData.missions[i].datas[2]);
                        break;
                }
            }

            mission = true;
        }
    }

    public void ClearMissions()
    {
        foreach (MissionData _obj in mission_BlockTypes.Values)
        {
            GameManager._instance.PushMissionObj(_obj.obj);
        }

        foreach (MissionData _obj in mission_PangTypes.Values)
        {
            GameManager._instance.PushMissionObj(_obj.obj);
        }

        foreach (MissionData _obj in mission_ItemTypes.Values)
        {
            GameManager._instance.PushMissionObj(_obj.obj);
        }

        mission_BlockTypes.Clear();
        mission_PangTypes.Clear();
        mission_ItemTypes.Clear();

        mission_Clear[0] = 0;
        mission_Clear[1] = 0;
        mission_Clear[2] = 0;

        mission = false;
    }

    public bool CheckMissionClear()
    {
        if (mission_Clear[0] != mission_BlockTypes.Count) return false;
        if (mission_Clear[1] != mission_PangTypes.Count) return false;
        if (mission_Clear[2] != mission_ItemTypes.Count) return false;

        return true;
    }

    private void SetMission(BlockType _type, int _value)
    {
        mission_BlockTypes.Add(_type, new MissionData(_value, GameManager._instance.PopMissionObj()));

        mission_BlockTypes[_type].obj.SetType(_type);
        mission_BlockTypes[_type].obj.SetValue(_value);
    }

    private void SetMission(PangType _type, int _value)
    {
        mission_PangTypes.Add(_type, new MissionData(_value, GameManager._instance.PopMissionObj()));

        mission_PangTypes[_type].obj.SetType(_type);
        mission_PangTypes[_type].obj.SetValue(_value);
    }

    private void SetMission(ItemType _type, int _value)
    {
        mission_ItemTypes.Add(_type, new MissionData(_value, GameManager._instance.PopMissionObj()));

        mission_ItemTypes[_type].obj.SetType(_type);
        mission_ItemTypes[_type].obj.SetValue(_value);
    }

    public void RecordRemoveType(BlockType _type)
    {
        if (mission_BlockTypes.ContainsKey(_type))
        {
            if (mission_BlockTypes[_type].AddValue()) mission_Clear[0]++;
        }
    }

    public void RecordRemoveType(PangType _type)
    {
        if (mission_PangTypes.ContainsKey(_type))
        {
            if (mission_PangTypes[_type].AddValue()) mission_Clear[1]++;
        }
    }

    public void RecordRemoveType(ItemType _type)
    {
        if (mission_ItemTypes.ContainsKey(_type))
        {
            if (mission_ItemTypes[_type].AddValue()) mission_Clear[2]++;
        }
    }
}

[System.Serializable]
public class BoardData
{
    public string title;
    public int pangCount;
    public int moveCount;
    public float time;

    public BoolArray[] board;
    public IntArray[] beforeBlockTypes;
    public IntArray[] afterBlockTypes;

    public IntArray[] missions;
    public int[] priorityPangTypes;
}

[System.Serializable]
public class IntArray
{
    public int[] datas;

    public IntArray(params int[] _datas)
    {
        datas = _datas;
    }
}

[System.Serializable]
public class BoolArray
{
    public bool[] datas;

    public BoolArray(params bool[] _datas)
    {
        datas = _datas;
    }
}

public class MissionData
{
    public MissionObj obj;

    private readonly int[] count = new int[2];

    public MissionData(int _value, MissionObj _obj)
    {
        count[1] = _value;
        obj = _obj;

        obj.Init();
    }

    public bool AddValue()
    {
        if (count[0] == count[1]) return false;

        count[0]++;

        if (obj != null)
        {
            obj.SetValue(count[1] - count[0]);
        }

        if (count[0] == count[1]) return true;

        return false;
    }
}

public class DirectionData
{
    public readonly int[] directionNumber = new int[4]; // ¿Þ, À§, ¿À, ¾Æ
    public readonly List<Block> spawnBlocks = new();
    public readonly List<Block> refreshBlocks = new();

    public void SetData(int _direction, int[] boardSize, ref Block[,] blocks)
    {
        refreshBlocks.Clear();
        spawnBlocks.Clear();

        switch (_direction)
        {
            case 0:
                directionNumber[0] = 0;
                directionNumber[1] = 1;
                directionNumber[2] = 2;
                directionNumber[3] = 3;
                break;

            case 1:
                directionNumber[0] = 3;
                directionNumber[1] = 0;
                directionNumber[2] = 1;
                directionNumber[3] = 2;
                break;

            case 2:
                directionNumber[0] = 2;
                directionNumber[1] = 3;
                directionNumber[2] = 0;
                directionNumber[3] = 1;
                break;

            default:
                directionNumber[0] = 1;
                directionNumber[1] = 2;
                directionNumber[2] = 3;
                directionNumber[3] = 0;
                break;
        }

        for (int i = 0; i < boardSize[0]; i++)
        {
            for (int j = 0; j < boardSize[1]; j++)
            {
                if (blocks[i, j] == null) continue;

                if (blocks[i, j].CheckNotExistBlock(directionNumber[3]))
                {
                    refreshBlocks.Add(blocks[i, j]);
                }

                if (blocks[i, j].CheckNotExistBlock(directionNumber[1]))
                {
                    spawnBlocks.Add(blocks[i, j]);
                }
            }
        }
    }

    public void RefreshBlocks()
    {
        for (int i = 0; i < refreshBlocks.Count; i++) refreshBlocks[i].CheckToss();
    }

    public void SpawnAllPangs()
    {
        for (int i = 0; i < spawnBlocks.Count; i++) spawnBlocks[i].SpwanPang();
    }
}

public enum BlockType
{
    None,
    DeadLock,
    Box,
    Ice
}

public enum BlockType_Common
{
    Move,
    DeadLock,
    Break
}

public enum PangType
{
    None,
    GangGi,
    Kanna,
    Yuni,
    Hina,
    Mashiro,
    Lize,
    Tabi,
    Shibuki,
    Rin,
    Nana,
    Riko,
}

public enum ItemType
{
    None,
    VertBomb,
    HoriBomb,
    TBomb_Small,
    TBomb_Long,
    Bomb_3x3,
    Bomb_5x5,
    Bomb_7x7,
}

public enum RemoveType
{
    Pang,
    Item,
    Clear
}