using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endgameafter25 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Start a coroutine to load scene 0 after 20 seconds
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(54f);
        Debug.Log("Loading scene 0...");
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
