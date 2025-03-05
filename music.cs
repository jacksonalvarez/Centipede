using UnityEngine;

public class music : MonoBehaviour
{
    public AudioClip musicClip;
    private AudioSource audioSource;

    private void Awake()
    {
        // Add an AudioSource component if not already attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up the audio source
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }

    private void Start()
    {
        if (musicClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No music clip assigned to Music script.");
        }
    }
}
