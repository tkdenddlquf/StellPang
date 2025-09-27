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

        for (int _y = 0; _y < boardData.blocks.Length; _y++)
        {
            for (int _x = 0; _x < boardData.blocks[^(_y + 1)].blockNums.Length; _x++)
            {
                if (boardData.blocks[^(_y + 1)].blockNums[_x] <= 0) continue;

                spawnHandle.SpawnPang(boardCreator[_x, _y], (DistractionType)(boardData.blocks[^(_y + 1)].blockNums[_x] - 1));
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
