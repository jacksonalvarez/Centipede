using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backsce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Start a coroutine to load the scene after 5 seconds
        StartCoroutine(LoadStartScreenAfterDelay());
    }

    private IEnumerator LoadStartScreenAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("StartScreen1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
