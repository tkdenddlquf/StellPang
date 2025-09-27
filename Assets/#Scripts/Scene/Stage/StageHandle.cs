using UnityEngine;

public class StageHandle : MonoBehaviour
{
    public void Back()
    {
        LoadingManager.LoadScene("Title");
    }
}
