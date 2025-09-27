using UnityEngine;

public class TitleHandle : MonoBehaviour
{
    public void TimeAttack()
    {
        LoadingManager.LoadScene("TimeAttack");
    }

    public void Stage()
    {
        LoadingManager.LoadScene("Stage");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
