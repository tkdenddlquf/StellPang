using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private int moveCount;

    public readonly List<Pang> itemPangs = new();

    public readonly BlockHandle blockHandle = new();
    public readonly SpawnHandle spawnHandle = new();

    private readonly MatchSystem matchSystem = new();

    public bool Match { get; set; }
    public int Combo { get; set; }
    public int DestroyCount { get; set; }
    public int MoveCount
    {
        get => moveCount;
        set
        {
            moveCount = value;

            if (value == 0) StartCoroutine(matchSystem.CheckMatch());
        }
    }
}
