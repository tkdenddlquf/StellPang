using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private static string nextScene;

    private void Start() => StartCoroutine(LoadSceneProcess());

    public static void LoadScene(string sceneName, bool loading = true)
    {
        nextScene = sceneName;

        if (loading) SceneManager.LoadScene("Loading");
        else SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        op.allowSceneActivation = false;

        float progress = 0;
        float speed = Time.unscaledDeltaTime * 0.07f;

        while (!op.isDone)
        {
            yield return null;

            progress = Mathf.MoveTowards(progress, op.progress, speed);

            progressBar.value = progress;

            if (progress == 0.9f)
            {
                op.allowSceneActivation = true;

                yield break;
            }
        }
    }
}
