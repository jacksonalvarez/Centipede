using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TMP_Text;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerUIHooks : MonoBehaviour
{
    [SerializeField] Text healthText = default;
    [SerializeField] Image healthBar = default;
    [SerializeField] Image healthBarEffect = default;
    [SerializeField] Image fadeImage = default;
    [SerializeField] GameObject lowHealthWarning = default;
    [SerializeField] float lowHealthThreshold = 0.3f; // Warning at 30% health
    [SerializeField] float healthBarEffectDelay = 0.5f; // Delay for the health bar effect

    private float currentHealth;
    private float maxHealth;
    private Coroutine healthBarEffectCoroutine;
    private Damageable playerDamageable;

    void Awake()
    {
        // Get the Damageable component from the player
        playerDamageable = GetComponent<Damageable>();
        if (playerDamageable == null)
        {
            Debug.LogError("Player does not have a Damageable component!");
        }
    }

    void Start()
    {
        // Hide low health warning by default
        if (lowHealthWarning != null)
            lowHealthWarning.SetActive(false);
    }

    void Update()
    {
        // Update the health UI based on the player's current health
        if (playerDamageable != null)
        {
            SetHealth(playerDamageable.currentHitPoints, playerDamageable.maxHitPoints);

            // Update the health bar effect in real-time
            if (healthBarEffect != null)
            {
                healthBarEffect.fillAmount = playerDamageable.currentHitPoints / (float)playerDamageable.maxHitPoints;
            }
        }
    }

    // Update health UI and check for low health condition
    public void SetHealth(int current, int total)
    {
        currentHealth = current;
        maxHealth = total;

        // Update health text
        if (healthText != null)
            healthText.text = $"{current}/{total}";

        // Update main health bar immediately
        if (healthBar != null)
            healthBar.fillAmount = current / (float)total;

        // Handle delayed health bar effect
        if (healthBarEffect != null)
        {
            if (healthBarEffectCoroutine != null)
                StopCoroutine(healthBarEffectCoroutine);

            healthBarEffectCoroutine = StartCoroutine(UpdateHealthBarEffect());
        }

        // Check if health is low
        if (lowHealthWarning != null)
        {
            bool isLowHealth = (current / (float)total) <= lowHealthThreshold;
            lowHealthWarning.SetActive(isLowHealth);
            
            if (isLowHealth && !lowHealthWarning.activeInHierarchy)
            {
                // Could add pulsing effect or sound here
                StartCoroutine(PulseHealthBar());
            }
        }
    }

    // Delayed health bar effect for smoother visual transition
    private IEnumerator UpdateHealthBarEffect()
    {
        yield return new WaitForSeconds(healthBarEffectDelay);
        healthBarEffect.DOFillAmount(currentHealth / maxHealth, 0.3f);
    }

    // Visual feedback when health is low
    private IEnumerator PulseHealthBar()
    {
        while ((currentHealth / maxHealth) <= lowHealthThreshold && lowHealthWarning.activeInHierarchy)
        {
            healthBar.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            healthBar.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Show damage flash
    public void ShowDamageFlash()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(1, 0, 0, 0.3f);
            fadeImage.DOFade(0, 0.5f);
        }
    }

    // Player death sequence
    public void PlayerDeath()
    {
        if (fadeImage != null)
        {
            // Fade to black
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(true);
            fadeImage.DOFade(1, 1.5f).OnComplete(() => 
            {
                // Add delay before reload
                StartCoroutine(DelayedReload(1.5f));
            });
        }
        else
        {
            // Immediate reload if no fade image
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator DelayedReload(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    // Handle checkpoint respawn (optional functionality)
    public void RespawnAtCheckpoint(Transform checkpointPosition)
    {
        if (fadeImage != null)
        {
            // Fade out
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.DOFade(1, 0.5f).OnComplete(() => 
            {
                // Move player to checkpoint (would need reference to player)
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && checkpointPosition != null)
                {
                    player.transform.position = checkpointPosition.position;
                }
                
                // Fade back in
                fadeImage.DOFade(0, 0.5f);
            });
        }
    }

    
}