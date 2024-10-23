using UnityEngine;

public class TimeAttackManager : MonoBehaviour
{
    public bool isPlay;

    public BindData<float>[] time = new BindData<float>[2];

    public LerpSlider timer = new();

    private BoardData boardData;

    private readonly LerpAction lerpAction = new();

    private void Start()
    {
        TimeAttack();

        timer.action = lerpAction;

        time[0].SetBind(TimeBind);
    }

    private void FixedUpdate()
    {
        lerpAction.actions?.Invoke();

        if (!isPlay) return;

        time[0].Data -= Time.deltaTime;
    }

    public void TimeAttack()
    {
        boardData = JsonUtility.FromJson<BoardData>(Resources.Load<TextAsset>("Json/TimeAttack").text);

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

        LevelManager.Instance.spawnHandle.SpawnAllPangs();

        SetTimer(boardData.time);
    }

    public void SetTimer(float _time)
    {
        time[1].Data = _time;
        time[0].Data = _time;
    }

    // 바인드
    private void TimeBind(ref float _current, float _change)
    {
        if (_change < 0) _change = 0;
        else if (_change > time[1].Data) _change = time[1].Data;

        _current = _change;

        timer.SetData(_current / time[1].Data);
    }
}
