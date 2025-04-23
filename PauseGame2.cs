using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseMenuUI;
    
    [Header("Options Menu")]
    public GameObject optionsMenuUI;
    
    private bool isPaused = false;
    private bool inOptionsMenu = false;
    
    // Singleton instance
    public static PauseManager instance;
    
    private void Awake()
    {
        // Simple singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Make sure this object persists between scenes if needed
        // DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // Make sure all UI elements are hidden at start
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
    }
    
    private void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inOptionsMenu)
            {
                // If in options menu, go back to pause menu
                BackToPauseMenu();
            }
            else if (isPaused)
            {
                // If already paused, resume
                ResumeGame();
            }
            else
            {
                // If not paused, pause
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        // Activate pause menu UI
        pauseMenuUI.SetActive(true);
        
        // Freeze time
        Time.timeScale = 0f;
        
        // Update state
        isPaused = true;
        
        // Optional: Disable player controls
        // playerController.enabled = false;
        
        // Optional: Play pause sound
        // AudioManager.instance.PlaySound("PauseSound");
    }
    
    public void ResumeGame()
    {
        // Hide pause menu UI
        pauseMenuUI.SetActive(false);
        
        // Resume time
        Time.timeScale = 1f;
        
        // Update state
        isPaused = false;
        
        // Optional: Re-enable player controls
        // playerController.enabled = true;
        
        // Optional: Play resume sound
        // AudioManager.instance.PlaySound("ResumeSound");
    }
    
    public void OpenOptionsMenu()
    {
        // Hide pause menu and show options menu
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        
        // Update state
        inOptionsMenu = true;
    }
    
    public void BackToPauseMenu()
    {
        // Hide options menu and show pause menu
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        
        // Update state
        inOptionsMenu = false;
    }
    
    public void QuitGame()
    {
        // Optional: Save game data
        // SaveSystem.SaveGame();
        
        // Quit application (works in build, not in editor)
        Application.Quit();
        
        // This line helps for testing in the editor
        Debug.Log("Quitting game...");
    }
}