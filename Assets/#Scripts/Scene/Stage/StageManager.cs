using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public bool isPlay;

    private BoardData boardData;

    private readonly int[] missionClear = new int[2];
    private readonly Dictionary<PangType, Dictionary<int, int[]>> missions = new();

    private void Start()
    {
        Stage(0);

        LevelManager.Instance.destroyAction = DestroyAction;
    }

    public void Stage(int _round)
    {
        BoardCreator boardCreator = BoardCreator.Instance;
        SpawnHandle spawnHandle = LevelManager.Instance.spawnHandle;

        boardData = JsonUtility.FromJson<BoardData>(Resources.Load<TextAsset>("Json/Stage_" + _round).text);

        boardCreator.CreateBoard(boardData);

        spawnHandle.SetDirection(boardData.dir);
        spawnHandle.SetPastelType(boardData.pangCount);

        BoardLineData[] boardLineDatas = boardData.blocks;

        for (int _y = 0; _y < boardLineDatas.Length; _y++)
        {
            int[] blockNums = boardLineDatas[^(_y + 1)].blockNums;

            for (int _x = 0; _x < blockNums.Length; _x++)
            {
                if (blockNums[_x] <= 0) continue;

                spawnHandle.SpawnPang(boardCreator[new(_x, _y)], (DistractionType)(blockNums[_x] - 1));
            }
        }

        missions.Clear();

        BoardMissionData[] missionDatas = boardData.missions;

        for (int i = 0; i < missionDatas.Length; i++)
        {
            PangType type = missionDatas[i].type;

            if (type == PangType.Pastel)
            {
                int key = spawnHandle.GetPangType(missionDatas[i].typeNum);
                int[] value = new int[] { 0, missionDatas[i].count };

                Dictionary<int, int[]> data = new() { { key, value } };

                missions.Add(type, data);
            }
            else
            {
                int key = missionDatas[i].typeNum;
                int[] value = new int[] { 0, missionDatas[i].count };

                Dictionary<int, int[]> data = new() { { key, value } };

                missions.Add(type, data);
            }

            missionClear[1]++;
        }

        spawnHandle.SpawnAllPangs();
    }

    private void DestroyAction(Pang _pang)
    {
        if (!missions.ContainsKey(_pang.PangType)) return;

        int typeNum = _pang.PangTypeNum;
        Dictionary<int, int[]> data = missions[_pang.PangType];

        if (!data.ContainsKey(typeNum)) return;

        int[] value = data[typeNum];

        if (value[0] == value[1]) return;

        data[typeNum][0]++;

        if (value[0] >= value[1])
        {
            data[typeNum][0] = data[typeNum][1];

            missionClear[0]++;
        }

        if (missionClear[0] == missionClear[1]) isPlay = false;
    }
}
