using UnityEngine;

public class StageHandle : MonoBehaviour
{
    public void Back()
    {
        LoadSystem.LoadScene(SceneNames.Title);
    }
}
