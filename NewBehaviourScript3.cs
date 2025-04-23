using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewBehaviourScript3 : MonoBehaviour
{
    [Header("Credits Settings")]
    public float scrollSpeed = 50f; // Speed of the scrolling credits
    public float endDelay = 5f; // Delay before returning to the main menu

    [Header("Audio Settings")]
    public AudioClip victorySound; // Victory sound to play
    private AudioSource audioSource;

    private RectTransform creditsRect;

    void Start()
    {
        // Create the UI Canvas
        GameObject canvasObject = new GameObject("CreditsCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObject.AddComponent<GraphicRaycaster>();

        // Create the credits text
        GameObject textObject = new GameObject("CreditsText");
        textObject.transform.SetParent(canvasObject.transform, false);
        Text creditsText = textObject.AddComponent<Text>();
        creditsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        creditsText.fontSize = 40;
        creditsText.alignment = TextAnchor.MiddleCenter;
        creditsText.color = Color.white;
        creditsText.text = "CREDITS\n\n" +
                           "JACKSON ALVAREZ | GAME DESIGN\n\n" +
                           "JONATHAN WADELL | GAME DESIGN\n\n" +
                           "SONG | ADOBE FLASH BY dyron miller\n\n" +
                           "PROFESSOR | DR. Rahman Tashakkori";

        // Position the text
        RectTransform textRect = creditsText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(800, 2000);
        textRect.anchoredPosition = new Vector2(0, -Screen.height / 2);

        creditsRect = textRect;

        // Play the victory sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = victorySound;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.Play();

        // Start scrolling the credits
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {
        // Calculate the target position for the credits to scroll to
        float targetY = creditsRect.anchoredPosition.y + creditsRect.rect.height + Screen.height;

        while (creditsRect.anchoredPosition.y < targetY)
        {
            // Scroll the credits upward
            creditsRect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        // Wait for a delay before returning to the main menu
        yield return new WaitForSeconds(endDelay);

        // Load the main menu scene (replace "StartScreen1" with the actual scene name)
        SceneManager.LoadScene("StartScreen1");
    }
}
