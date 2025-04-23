using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startskip : MonoBehaviour
{
    void Start()
    {
        // Start a coroutine to load scene 4 after 29 seconds
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(29f);
        Debug.Log("Loading scene 4...");
        SceneManager.LoadScene(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
