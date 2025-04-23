using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // Add this for scene management

public class Damageable : MonoBehaviour
{
    // Define DamageMessage struct only once
    public struct DamageMessage
    {
        public MonoBehaviour damager;
        public int amount;
        public Vector3 direction;
        public Vector3 damageSource;
        public bool throwing;
        public bool stopCamera;
    }

    [SerializeField] public int maxHitPoints;
    public int currentHitPoints;
    [SerializeField] private bool isInvulnerable;
    [SerializeField] private bool isHealing;
    [SerializeField] private bool isBoss;
    [SerializeField] private bool isFinalBoss;
    [SerializeField] private bool isDestructibleObject;
    [SerializeField] private bool isLava;
    [SerializeField] private bool isBoulder;
    [SerializeField] private bool isTree;
    [SerializeField] private bool isSpider;
    [SerializeField] private bool isPlant;

    public bool IsHealing => isHealing;
    public bool IsBoss => isBoss;
    public bool IsFinalBoss => isFinalBoss;
    public bool IsDestructibleObject => isDestructibleObject;
    public bool IsLava => isLava;
    public bool IsBoulder => isBoulder;
    public bool IsTree => isTree;
    public bool IsSpider => isSpider;
    public bool IsPlant => isPlant;

    public UnityEvent OnDeath, OnHitWhileInvulnerable, OnReceiveDamage, OnHeal, OnFullyHealed;

    private void Awake()
    {
        ResetDamage();
    }

    public void ApplyDamage(DamageMessage data)
    {
        Debug.Log($"Damage applied: {data.amount}, Current HP before: {currentHitPoints}");

        if (isInvulnerable)
        {
            OnHitWhileInvulnerable.Invoke();
            return;
        }

        currentHitPoints -= data.amount;

        if (currentHitPoints <= 0)
        {
            OnDeath.Invoke();
            return;
        }

        OnReceiveDamage.Invoke();
    }

    public void Heal(int amount)
    {
        if (currentHitPoints <= 0) return; // Prevent healing if already dead

        currentHitPoints += amount;
        OnHeal.Invoke();

        if (currentHitPoints >= maxHitPoints)
        {
            currentHitPoints = maxHitPoints;
            OnFullyHealed.Invoke();
        }

        Debug.Log($"Healed for {amount}. Current HP: {currentHitPoints}/{maxHitPoints}");
    }

    public void HealPlayerWithChance(int damageDealt)
    {
        XPAndUpgradeSystem xpSystem = FindObjectOfType<XPAndUpgradeSystem>();
        if (xpSystem != null && xpSystem.IsUpgradeActive("Health Siphon: 2% of the damage you do is turned into healing for you."))
        {
            // 2% chance to apply the heal effect
            if (Random.value <= 0.02f)
            {
                int healAmount = Mathf.FloorToInt(damageDealt * 0.02f); // 2% of the damage dealt

                // Find the player object by tag
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    Damageable playerDamageable = playerObject.GetComponent<Damageable>();
                    if (playerDamageable != null)
                    {
                        // Increase current health by the heal amount
                        playerDamageable.currentHitPoints += healAmount;

                        // Clamp current health to max health
                        if (playerDamageable.currentHitPoints > playerDamageable.maxHitPoints)
                        {
                            playerDamageable.currentHitPoints = playerDamageable.maxHitPoints;
                        }

                        // Trigger health-related events
                        playerDamageable.OnHeal.Invoke();
                        if (playerDamageable.currentHitPoints == playerDamageable.maxHitPoints)
                        {
                            playerDamageable.OnFullyHealed.Invoke();
                        }

                        Debug.Log($"Player healed for {healAmount} HP (2% of {damageDealt}). Current HP: {playerDamageable.currentHitPoints}/{playerDamageable.maxHitPoints}");
                    }
                    else
                    {
                        Debug.LogWarning("Player Damageable component not found.");
                    }
                }
                else
                {
                    Debug.LogWarning("Player object with tag 'Player' not found.");
                }
            }
        }
    }

    public void ResetDamage()
    {
        currentHitPoints = maxHitPoints;
    }

    public void SetIsInvulnerable(bool isInvulnerable)
    {
        this.isInvulnerable = isInvulnerable;
    }

    public void OnWormDeath()
    {
        Debug.Log("Worm Died...");

        // Play the particle effect from the "Poopie" object
        GameObject poopieObject = GameObject.Find("poopie");
        if (poopieObject != null)
        {
            ParticleSystem poopieEffect = poopieObject.GetComponent<ParticleSystem>();
            if (poopieEffect != null)
            {
                poopieEffect.Play();
                Debug.Log("Worm death particle effect played from 'poopie'.");
            }
            else
            {
                Debug.LogWarning("'poopie' object does not have a ParticleSystem component.");
            }
        }
        else
        {
            Debug.LogWarning("Object named 'poopie' not found in the scene.");
        }

        // Start the fade-to-black and scene loading process
        StartCoroutine(FadeToBlackAndLoadScene());
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player Died...");
        // Start the fade-to-black and scene loading process
        StartCoroutine(FadeToBlackAndLoadScenePlayer());
    }

    private IEnumerator FadeToBlackAndLoadScenePlayer()
    {
        // Create a full-screen black UI panel for the fade effect
        GameObject fadePanel = new GameObject("FadePanel");
        Canvas canvas = fadePanel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasGroup canvasGroup = fadePanel.AddComponent<CanvasGroup>();
        RectTransform rectTransform = fadePanel.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        // Add a black image to the panel
        UnityEngine.UI.Image blackImage = fadePanel.AddComponent<UnityEngine.UI.Image>();
        blackImage.color = Color.black;

        // Fade to black
        float fadeDuration = 2f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Load the next scene (replace "NextSceneName" with the actual scene name)
        SceneManager.LoadScene(3);
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        // Create a full-screen black UI panel for the fade effect
        GameObject fadePanel = new GameObject("FadePanel");
        Canvas canvas = fadePanel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasGroup canvasGroup = fadePanel.AddComponent<CanvasGroup>();
        RectTransform rectTransform = fadePanel.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        // Add a black image to the panel
        UnityEngine.UI.Image blackImage = fadePanel.AddComponent<UnityEngine.UI.Image>();
        blackImage.color = Color.black;

        // Fade to black
        float fadeDuration = 2f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Load the next scene (replace "NextSceneName" with the actual scene name)
        SceneManager.LoadScene(5);
    }
    public void OnSpiderDeath()
    {
        Debug.Log("Destroying spider...");

        SpiderNPCController spiderController = GetComponent<SpiderNPCController>();
        if (spiderController != null)
        {
            spiderController.Die();
        }
        else
        {
            Debug.LogError("SpiderNPCController not found on spider!");
            Destroy(gameObject);
        }
    }
}