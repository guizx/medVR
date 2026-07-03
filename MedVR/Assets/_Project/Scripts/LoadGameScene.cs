using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    public string SceneName = "GameScene";
    public float Delay = 3f;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(Delay);
        SceneManager.LoadScene(SceneName);
    }
}
