using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    public void OnClickPlayButton()
    {
        StartCoroutine(LoadGameSceneRoutine());
    }

    private IEnumerator LoadGameSceneRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(gameSceneName);

    }
}
