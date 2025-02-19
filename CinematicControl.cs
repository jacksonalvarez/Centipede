using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicControl : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip doucopy;
    public AudioClip yesICopy;
    public AudioClip samples;
    public AudioClip understoodCommander;
    public AudioClip clickOne;
    public AudioClip clickTwo;
    public AudioClip clickThree;
    public AudioClip spaceHum;
    public AudioClip bassHit;
    public AudioClip takeoff;

    void Start()
    {
        // Play the space hum sound once at the start
        if (spaceHum != null)
            audioSource.PlayOneShot(spaceHum);
        
        // Play the bass hit at the start
        if (bassHit != null)
            audioSource.PlayOneShot(bassHit);
        
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        for (int i = 0; i < 42; i++)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("time: " + i);

            if (i == 1 && clickOne != null)
                audioSource.PlayOneShot(clickOne);
            else if (i == 2 && clickTwo != null)
                audioSource.PlayOneShot(clickTwo);
            else if (i == 2) // Play clickThree at 2.5s
                StartCoroutine(PlayDelayed(clickThree, 0.5f));
            
            if (i == 5 && doucopy != null)
                audioSource.PlayOneShot(doucopy);
            else if (i == 10 && yesICopy != null)
                audioSource.PlayOneShot(yesICopy);
            else if (i == 14 && samples != null)
                audioSource.PlayOneShot(samples);
            else if (i == 27 && understoodCommander != null)
                audioSource.PlayOneShot(understoodCommander);
            else if (i == 39 && takeoff != null)
                audioSource.PlayOneShot(takeoff);
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator PlayDelayed(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
