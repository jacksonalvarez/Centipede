using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public Scene FirstScene;
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
