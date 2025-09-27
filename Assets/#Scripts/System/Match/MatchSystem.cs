using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSystem
{
    private bool hint;
    private bool match;

    private float hintTime;

    public List<Block> AllBlcoks { get; } = new();

    public HashSet<Block> RemoveBlcoks { get; } = new();

    public readonly int[] checkVector = new int[2];

    private readonly HintHandle hintHandle;
    private readonly MatchHandle matchHandle;

    public MatchSystem()
    {
        hintHandle = new(this);
        matchHandle = new(this);
    }

    public IEnumerator CheckMatch()
    {
        LevelManager levelManager = LevelManager.Instance;
        BoardCreator boardCreator = BoardCreator.Instance;

        if (levelManager.Match) yield break;

        levelManager.Match = true;

        if (hintHandle.hint != null && hintHandle.hint.gameObject.activeSelf) hintHandle.hint.Animator.Play("Idle");

        AllBlcoks.Clear();
        RemoveBlcoks.Clear();

        checkVector[0] = (int)levelManager.spawnHandle.SpawnVector.x;
        checkVector[1] = (int)levelManager.spawnHandle.SpawnVector.y;

        for (int i = 0; i < boardCreator.boardSize[0]; i++)
        {
            for (int j = 0; j < boardCreator.boardSize[1]; j++)
            {
                if (IsCheckable(boardCreator[i, j])) AllBlcoks.Add(boardCreator[i, j]);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (LevelManager.Instance.blockHandle.selectBlocks[i] != null)
            {
                if (LevelManager.Instance.blockHandle.selectBlocks[i].TargetPang == null) continue;
                if (LevelManager.Instance.blockHandle.selectBlocks[i].TargetPang.PangType != PangType.Item) continue;

                RemoveBlcoks.Add(LevelManager.Instance.blockHandle.selectBlocks[i]);
            }
        }

        for (int i = AllBlcoks.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < 4; j++)
            {
                match = matchHandle.CheckT(AllBlcoks[i], checkVector[0], checkVector[1]);

                if (match)
                {
                    i -= 4;

                    matchHandle.GetSwapBlock(1).TargetPang.SetType(ItemType.BombLargeCross);

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1]);
            }

            if (match) continue;

            for (int j = 0; j < 4; j++)
            {
                match = matchHandle.CheckL(AllBlcoks[i], checkVector[0], checkVector[1]);

                if (match)
                {
                    i -= 4;

                    matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.Bomb5x5);

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1]);
            }

            if (match) continue;

            if (matchHandle.CheckBox(AllBlcoks[i]))
            {
                i -= 3;

                matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.BombSmallCross);
            }
        }

        for (int i = AllBlcoks.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < 2; j++)
            {
                match = matchHandle.CheckLine(AllBlcoks[i], checkVector[0], checkVector[1], 4);

                if (match)
                {
                    i -= 3;

                    if (j % 2 == 0) matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.BombVert);
                    else matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.BombHori);

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1], false);
            }

            if (match) continue;

            checkVector[0] = (int)LevelManager.Instance.spawnHandle.SpawnVector.x;
            checkVector[1] = (int)LevelManager.Instance.spawnHandle.SpawnVector.y;

            for (int j = 0; j < 2; j++)
            {
                if (matchHandle.CheckLine(AllBlcoks[i], checkVector[0], checkVector[1], 3))
                {
                    i -= 2;

                    break;
                }

                RotateDir(ref checkVector[0], ref checkVector[1], false);
            }
        }

        if (RemoveBlcoks.Count == 0)
        {
            levelManager.Combo = 0;
            levelManager.blockHandle.SwapSelect();

            if (hintHandle.CheckHint()) levelManager.StartCoroutine(NoticeHint());
            else if (levelManager.itemPangs.Count != 0) Debug.Log("아이템 사용");
            else
            {
                Debug.Log("리셋");

                yield return null;

                for (int i = 0; i < boardCreator.boardSize[0]; i++)
                {
                    for (int j = 0; j < boardCreator.boardSize[1]; j++)
                    {
                        if (boardCreator[i, j] == null) continue;
                        if (boardCreator[i, j].TargetPang == null) continue;
                        if (boardCreator[i, j].TargetPang.PangType == PangType.Distraction) continue;

                        boardCreator[i, j].TargetPang.Remove();
                    }
                }
            }
        }
        else
        {
            hint = false;

            levelManager.Combo++;

            foreach (Block _block in RemoveBlcoks) _block.TargetPang.StateBase.OnDestroy();
        }

        yield return new WaitUntil(() => levelManager.DestroyCount == 0);

        levelManager.Match = false;
        levelManager.blockHandle.ClearSelect();
    }

    public IEnumerator NoticeHint()
    {
        if (hint) yield break;

        hint = true;
        hintTime = Time.time;

        LevelManager levelManager = LevelManager.Instance;

        while (true)
        {
            if (Time.time - hintTime >= levelManager.hintTime)
            {
                hintHandle.hint.Animator.Play("Hint");

                yield break;
            }

            yield return null;
        }
    }

    public bool IsCheckable(Block _block)
    {
        if (_block == null) return false;
        if (_block.TargetPang == null) return false;
        if (_block.TargetPang.PangType != PangType.Pastel) return false;
        if (RemoveBlcoks.Contains(_block)) return false;

        return true;
    }

    public void RotateDir(ref int _x, ref int _y, bool _minus = true)
    {
        if (_minus)
        {
            if (_x == 0 && _y == 1)
            {
                _x = -1;
                _y = 0;
            }
            else if (_x == -1 && _y == 0)
            {
                _x = 0;
                _y = -1;
            }
            else if (_x == 0 && _y == -1)
            {
                _x = 1;
                _y = 0;
            }
            else if (_x == 1 && _y == 0)
            {
                _x = 0;
                _y = 1;
            }
        }
        else
        {
            if (_x == 0 && _y == 1)
            {
                _x = 1;
                _y = 0;
            }
            else if (_x == 1 && _y == 0)
            {
                _x = 0;
                _y = -1;
            }
            else if (_x == 0 && _y == -1)
            {
                _x = -1;
                _y = 0;
            }
            else if (_x == -1 && _y == 0)
            {
                _x = 0;
                _y = 1;
            }
        }
    }
}
