using UnityEngine;

public class TimeAttackManager : MonoBehaviour
{
    public bool isPlay;

    public float currentTime;
    public float maxTime;

    private BoardData boardData;

    private void Start()
    {
        TimeAttack();
    }

    public void TimeAttack()
    {
        BoardCreator boardCreator = BoardCreator.Instance;
        SpawnHandle spawnHandle = LevelManager.Instance.spawnHandle;

        boardData = JsonUtility.FromJson<BoardData>(Resources.Load<TextAsset>("Json/TimeAttack").text);

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

        spawnHandle.SpawnAllPangs();

        SetTimer(boardData.time);
    }

    public void SetTimer(float _time)
    {
        currentTime = 0;
        maxTime = _time;
    }
}
