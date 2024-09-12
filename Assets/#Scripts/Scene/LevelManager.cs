using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private BoardCreator boardCreator;

    private BoardDir boardDir;

    private void Start()
    {
        TryGetComponent(out boardCreator);
    }


}

public enum BoardDir
{
    Up,
    Right,
    Down,
    Left
}
