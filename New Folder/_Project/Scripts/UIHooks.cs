using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIHooks : MonoBehaviour
{
    [SerializeField] Text healthText = default;
    [SerializeField] Image healthBar = default;
    [SerializeField] Image healthBarEffect = default;
    [SerializeField] Image fadeImage;

    // Update health and check for health under 3000
    public void SetHealth(int current, int total)
    {
        healthText.text = $"{current}/{total}";
        healthBar.fillAmount = current / (float)total;
        healthBarEffect.fillAmount = current / (float)total;

        // Check if health is below 3000
        if (current < 3000)
        {
            Debug.Log("Worm's health is below 3000!");
            // Optional: Add UI effects or notify other parts of the game here
        }
    }

    // Reload the scene with a fade effect
    public void ReloadScene()
    {
        fadeImage.DOFade(1, 3).OnComplete(() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name));
    }
}
