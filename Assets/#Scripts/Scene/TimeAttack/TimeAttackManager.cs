using UnityEngine;

public class TimeAttackManager : MonoBehaviour
{
    public bool isPlay;

    public BindData<float>[] time = new BindData<float>[2];

    public LerpSlider timer = new();

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
        BoardCreator.Instance.boardSize[0] = 10;
        BoardCreator.Instance.boardSize[1] = 10;

        BoardCreator.Instance.CreateBoard();

        LevelManager.Instance.spawnHandle.SetDirection(Directions.Up);
        LevelManager.Instance.spawnHandle.SetPastelType(10);

        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[3, 3], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[3, 4], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[3, 5], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[4, 3], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[4, 4], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[4, 5], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[5, 3], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[5, 4], DistractionType.Stone);
        LevelManager.Instance.spawnHandle.SpawnPang(BoardCreator.Instance[5, 5], DistractionType.Stone);

        LevelManager.Instance.spawnHandle.SpawnAllPangs();

        SetTimer(600);
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
