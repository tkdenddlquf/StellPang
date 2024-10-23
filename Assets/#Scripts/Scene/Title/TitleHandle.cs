using UnityEngine;

public class TitleHandle : MonoBehaviour
{
    public void TimeAttack()
    {
        LoadSystem.LoadScene(SceneNames.TimeAttack);
    }

    public void Stage()
    {
        LoadSystem.LoadScene(SceneNames.Stage);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
