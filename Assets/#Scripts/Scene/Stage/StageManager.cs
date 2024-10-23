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
        boardData = JsonUtility.FromJson<BoardData>(Resources.Load<TextAsset>("Json/Stage_" + _round).text);

        BoardCreator.Instance.CreateBoard(boardData);

        LevelManager.Instance.spawnHandle.SetDirection(boardData.dir);
        LevelManager.Instance.spawnHandle.SetPastelType(boardData.pangCount);

        for (int _y = 0; _y < boardData.blocks.Length; _y++)
        {
            for (int _x = 0; _x < boardData.blocks[^(_y + 1)].blockNums.Length; _x++)
            {
                if (boardData.blocks[^(_y + 1)].blockNums[_x] <= 0) continue;

                LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[_x, _y], (DistractionType)(boardData.blocks[^(_y + 1)].blockNums[_x] - 1));
            }
        }

        missions.Clear();

        for (int i = 0; i < boardData.missions.Length; i++)
        {
            if (boardData.missions[i].type == PangType.Pastel)
            {
                missions.Add(boardData.missions[i].type, new() { { LevelManager.Instance.spawnHandle.GetPangType(boardData.missions[i].typeNum), new int[] { 0, boardData.missions[i].count } } });
            }
            else missions.Add(boardData.missions[i].type, new() { { boardData.missions[i].typeNum, new int[] { 0, boardData.missions[i].count } } });

            missionClear[1]++;
        }

        LevelManager.Instance.spawnHandle.SpawnAllPangs();
    }

    private void DestroyAction(Pang _pang)
    {
        if (!missions.ContainsKey(_pang.PangType)) return;
        if (!missions[_pang.PangType].ContainsKey(_pang.PangTypeNum)) return;

        if (missions[_pang.PangType][_pang.PangTypeNum][0] == missions[_pang.PangType][_pang.PangTypeNum][1]) return;

        missions[_pang.PangType][_pang.PangTypeNum][0]++;

        if (missions[_pang.PangType][_pang.PangTypeNum][0] >= missions[_pang.PangType][_pang.PangTypeNum][1])
        {
            missions[_pang.PangType][_pang.PangTypeNum][0] = missions[_pang.PangType][_pang.PangTypeNum][1];

            missionClear[0]++;
        }

        if (missionClear[0] == missionClear[1]) isPlay = false;
    }
}
