using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    
    void Start()
    {
        // Ensure we start in the correct state
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("Game started with TimeScale: " + Time.timeScale);
    }
    
    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Check current state of the menu to decide what to do
            bool menuIsActive = pauseMenuUI.activeSelf;
            
            if (menuIsActive)
            {
                // Menu is showing, so hide it and resume game
                pauseMenuUI.SetActive(false);
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Menu closed, game resumed. TimeScale: " + Time.timeScale);
            }
            else
            {
                // Menu is hidden, so show it and pause game
                pauseMenuUI.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Menu opened, game paused. TimeScale: " + Time.timeScale);
            }
        }
    }
    
    // Called by the Resume button
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Resume button clicked. TimeScale: " + Time.timeScale);
    }
    
    // Called by the Restart button
    public void RestartGame()
    {
        // Ensure time is running before scene reload
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Debug.Log("Restarting game. TimeScale reset to: " + Time.timeScale);
    }
    
    // Called by the Quit button
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}