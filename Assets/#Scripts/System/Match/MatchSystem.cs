using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSystem
{
    private bool hint;
    private bool match;

    public Vector2Int CheckDir { get; set; }

    private float hintTime;

    public List<Block> AllBlcoks { get; } = new();

    public HashSet<Block> RemoveBlcoks { get; } = new();

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

        CheckDir = levelManager.spawnHandle.SpawnDir;

        for (int i = 0; i < boardCreator.boardSize[0]; i++)
        {
            for (int j = 0; j < boardCreator.boardSize[1]; j++)
            {
                Vector2Int pos = new(i, j);
                Block block = boardCreator[pos];

                if (IsCheckable(block)) AllBlcoks.Add(block);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (levelManager.blockHandle.selectBlocks[i] != null)
            {
                if (levelManager.blockHandle.selectBlocks[i].TargetPang == null) continue;
                if (levelManager.blockHandle.selectBlocks[i].TargetPang.PangType != PangType.Item) continue;

                RemoveBlcoks.Add(levelManager.blockHandle.selectBlocks[i]);
            }
        }

        for (int i = AllBlcoks.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < 4; j++)
            {
                match = matchHandle.CheckT(AllBlcoks[i], CheckDir);

                if (match)
                {
                    i -= 4;

                    matchHandle.GetSwapBlock(1).TargetPang.SetType(ItemType.BombLargeCross);

                    break;
                }

                CheckDir = RotateDir(CheckDir);
            }

            if (match) continue;

            for (int j = 0; j < 4; j++)
            {
                match = matchHandle.CheckL(AllBlcoks[i], CheckDir);

                if (match)
                {
                    i -= 4;

                    matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.Bomb5x5);

                    break;
                }

                CheckDir = RotateDir(CheckDir);
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
                match = matchHandle.CheckLine(AllBlcoks[i], CheckDir, 4);

                if (match)
                {
                    i -= 3;

                    if (j % 2 == 0) matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.BombVert);
                    else matchHandle.GetSwapBlock(0).TargetPang.SetType(ItemType.BombHori);

                    break;
                }

                CheckDir = RotateDir(CheckDir, false);
            }

            if (match) continue;

            CheckDir = levelManager.spawnHandle.SpawnDir;

            for (int j = 0; j < 2; j++)
            {
                if (matchHandle.CheckLine(AllBlcoks[i], CheckDir, 3))
                {
                    i -= 2;

                    break;
                }

                CheckDir = RotateDir(CheckDir, false);
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

                Vector2Int boardSize = boardCreator.boardSize;

                for (int i = 0; i < boardSize.x; i++)
                {
                    for (int j = 0; j < boardSize.y; j++)
                    {
                        Vector2Int pos = new(i, j);
                        Block block = boardCreator[pos];

                        if (block == null) continue;
                        if (block.TargetPang == null) continue;
                        if (block.TargetPang.PangType == PangType.Distraction) continue;

                        block.TargetPang.Remove();
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

    public Vector2Int RotateDir(Vector2Int dir, bool counterClockwide = true)
    {
        if (counterClockwide)
        {
            if (dir.x == 0 && dir.y == 1)
            {
                dir.x = -1;
                dir.y = 0;
            }
            else if (dir.x == -1 && dir.y == 0)
            {
                dir.x = 0;
                dir.y = -1;
            }
            else if (dir.x == 0 && dir.y == -1)
            {
                dir.x = 1;
                dir.y = 0;
            }
            else if (dir.x == 1 && dir.y == 0)
            {
                dir.x = 0;
                dir.y = 1;
            }
        }
        else
        {
            if (dir.x == 0 && dir.y == 1)
            {
                dir.x = 1;
                dir.y = 0;
            }
            else if (dir.x == 1 && dir.y == 0)
            {
                dir.x = 0;
                dir.y = -1;
            }
            else if (dir.x == 0 && dir.y == -1)
            {
                dir.x = -1;
                dir.y = 0;
            }
            else if (dir.x == -1 && dir.y == 0)
            {
                dir.x = 0;
                dir.y = 1;
            }
        }

        return dir;
    }
}
