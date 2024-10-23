using UnityEngine;

[System.Serializable]
public class BoardData
{
    public string title;

    public int pangCount;
    public int moveCount;

    public float time;
    public Directions dir;

    public BoardLineData[] blocks;
    public BoardMissionData[] missions;
}

[System.Serializable]
public class BoardLineData
{
    public int[] blockNums;
}

[System.Serializable]
public class BoardMissionData
{
    public PangType type;
    public int typeNum;
    public int count;
}
