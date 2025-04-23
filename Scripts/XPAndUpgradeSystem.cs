using TMPro; // Import TextMeshPro namespace
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XtremeFPS.FPSController; // Add this to access the FirstPersonController class
using XtremeFPS.WeaponSystem;

using UnityEngine.Audio;

public class XPAndUpgradeSystem : MonoBehaviour
{
    [Header("XP and Leveling")]
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int currentLevel = 1;
    public int upgradePoints = 0;

    [Header("UI Elements")]
    public Slider xpSlider; // Slider for XP progress
    public TextMeshProUGUI levelText; // TextMeshPro field for Level

    [Header("Passive XP Gain")]
    public float passiveXPInterval = 2f; // Time interval for passive XP gain
    public int passiveXPAmount = 1; // Amount of XP gained passively

    [Header("Upgrade UI")]
    public GameObject upgradeUI; // UI panel for upgrades
    public TextMeshProUGUI card1Text; // TextMeshPro for card 1
    public TextMeshProUGUI card2Text; // TextMeshPro for card 2
    public Animator upgradeUIAnimator; // Animator for card appearance animation

    [Header("Active Upgrades")]
    private List<string> activeUpgrades = new List<string>(); // List to track active upgrades

    private List<string> upgrades = new List<string>
    {
        "Spider Hunter: Increase XP from killing spiders.",
        "Stamina +: Gain a dash. Get 50% more stamina.",
        "Stamina ++: Move Faster. 150% more stamina.",
        "Health Siphon: 2% of the damage you do is turned into healing for you.",
        "Overclocked: Your Weapons have been modified to shoot quicker.",
        "War Machine: Armour piercing bullets.",
        "War Machine++: EXPLODING BULLETS",
        "The World: Press and hold Q to slow down time. Only lasts for 2 seconds.",
        "The World+: Time is even slower, for up to 5 seconds.",
        "Extra Clips: 200% ammo for all guns.",
        "Chicken Dinner: Heal to full. 15% max health increase."
    };

    // Define dependencies for upgrades
    private Dictionary<string, string> upgradeDependencies = new Dictionary<string, string>
    {
        { "Stamina ++: Move Faster. 150% more stamina.", "Stamina +: Gain a dash. Get 50% more stamina." },
        { "War Machine++: EXPLODING BULLETS", "War Machine: Armour piercing bullets." },
        { "The World+: Time is even slower, for up to 5 seconds.", "The World: Press and hold Q to slow down time. Only lasts for 5 seconds." }
    };

    private string selectedUpgrade1;
    private string selectedUpgrade2;

    private bool isTimeSlowed = false;
    private float timeSlowDuration = 2f; // Default duration for "The World"
    private float timeSlowFactor = 0.5f; // Default time slow factor

    [Header("Time Stop Ability")]
    public AudioClip timeStopAudioClip; // Audio clip for time stop
    public float timeStopCooldown = 60f; // Cooldown duration for time stop ability
    private bool isTimeStopOnCooldown = false; // Tracks if the ability is on cooldown
    private bool isTimeStopActive = false; // Tracks if time stop is currently active
    private Coroutine timeStopCoroutine; // Tracks the active time stop coroutine

    private Color originalBackgroundColor;
    private Color blackAndWhiteColor = new Color(0.2f, 0.2f, 0.2f); // Black-and-white effect color

    private bool isWarMachinePlusActive = false;
    private float timeSinceLastHit = 0f;

    [Header("Health Overlay UI")]
    public Image healthOverlayImage; // The UI image to adjust based on health

    private void Start()
    {
        // Start passive XP gain
        StartCoroutine(PassiveXPGain());

        // Update the UI at the start
        UpdateUI();

        originalBackgroundColor = Camera.main.backgroundColor; // Store the original background color
    }

    void Update()
    {
        // Check for XP gain (example: killing enemies)
        if (Input.GetKeyDown(KeyCode.K)) // Example key to simulate XP gain
        {
            AddXP(200);
        }

        // Handle upgrade selection
        if (upgradeUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ApplyUpgrade(selectedUpgrade1);
                CloseUpgradeUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ApplyUpgrade(selectedUpgrade2);
                CloseUpgradeUI();
            }
        }

        // Handle time-slowing ability toggle
        if (Input.GetKeyDown(KeyCode.CapsLock) && !isTimeStopOnCooldown)
        {
            if (IsUpgradeActive("The World: Press and hold Q to slow down time. Only lasts for 2 seconds.") ||
                IsUpgradeActive("The World+: Time is even slower, for up to 5 seconds."))
            {
                if (!isTimeStopActive)
                {
                    Debug.Log("Activating Time Slow.");
                    timeStopCoroutine = StartCoroutine(ActivateTimeSlow());
                }
                else
                {
                    Debug.Log("Deactivating Time Slow early.");
                    if (timeStopCoroutine != null) StopCoroutine(timeStopCoroutine);
                    StartCoroutine(DeactivateTimeSlow());
                }
            }
            else
            {
                Debug.Log("Time Slow ability is locked. Unlock it by acquiring 'The World' or 'The World+' upgrade.");
            }
        }

        // Check for "War Machine++" activation
        if (IsUpgradeActive("War Machine++: After 15 seconds of not being hit, bullets begin to explode."))
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                Damageable playerDamageable = playerObject.GetComponent<Damageable>();
                if (playerDamageable != null)
                {
                    // Check if the player has been hit
                    if (playerDamageable.currentHitPoints < playerDamageable.maxHitPoints)
                    {
                        timeSinceLastHit = 0f; // Reset timer if the player has been hit
                        isWarMachinePlusActive = false; // Deactivate War Machine++ if it was active
                    }
                    else
                    {
                        timeSinceLastHit += Time.deltaTime; // Increment timer if no damage is received
                        if (timeSinceLastHit >= 15f && !isWarMachinePlusActive)
                        {
                            isWarMachinePlusActive = true;
                            Debug.Log("War Machine++ activated: Bullets now explode and deal double damage.");
                        }
                    }
                }
            }
        }
        else
        {
            isWarMachinePlusActive = false; // Reset if the upgrade is not active
        }

        UpdateHealthOverlay();
    }

    private IEnumerator PassiveXPGain()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveXPInterval);
            AddXP(passiveXPAmount);
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        // Update the UI whenever XP changes
        UpdateUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        upgradePoints++;

        // Update XP required for the next level
        if (currentLevel == 2) xpToNextLevel = 200;
        else if (currentLevel == 3) xpToNextLevel = 400;
        else if (currentLevel == 4) xpToNextLevel = 800;
        else if (currentLevel == 5) xpToNextLevel = 1400;
        else xpToNextLevel += 600;

        ShowUpgradeUI();

        // Update the UI whenever the level changes
        UpdateUI();
    }

    private void ShowUpgradeUI()
    {
        upgradeUI.SetActive(true);

        // Play the card appearance animation
        if (upgradeUIAnimator != null)
        {
            upgradeUIAnimator.SetTrigger("ShowCards");
        }

        // Filter upgrades to exclude already selected ones
        List<string> availableUpgrades = upgrades.FindAll(upgrade =>
        {
            // Check if the upgrade is already active
            if (activeUpgrades.Contains(upgrade)) return false;

            // Check if the upgrade has a dependency and if the dependency is satisfied
            if (upgradeDependencies.TryGetValue(upgrade, out string dependency))
            {
                return activeUpgrades.Contains(dependency);
            }

            return true; // No dependency, so it's available
        });

        // Pick two random upgrades from the available list
        if (availableUpgrades.Count == 0)
        {
            Debug.LogWarning("No available upgrades to show.");
            upgradeUI.SetActive(false);
            return;
        }

        selectedUpgrade1 = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
        availableUpgrades.Remove(selectedUpgrade1);

        selectedUpgrade2 = availableUpgrades.Count > 0
            ? availableUpgrades[Random.Range(0, availableUpgrades.Count)]
            : null;

        // Display the upgrades on the UI
        card1Text.text = selectedUpgrade1;
        card2Text.text = selectedUpgrade2 ?? "No other upgrades available.";
    }

    private void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
    }

    public void ApplyUpgrade(string upgrade)
    {
        if (activeUpgrades.Contains(upgrade))
        {
            Debug.LogWarning($"Upgrade {upgrade} has already been applied.");
            return;
        }

        // Check if the upgrade has a dependency and if it's satisfied
        if (upgradeDependencies.TryGetValue(upgrade, out string dependency) && !activeUpgrades.Contains(dependency))
        {
            Debug.LogWarning($"Cannot apply {upgrade} because it depends on {dependency}.");
            return;
        }

        Debug.Log($"Applying Upgrade: {upgrade}");

        // Add the upgrade to the active upgrades list
        activeUpgrades.Add(upgrade);
        upgrades.Remove(upgrade); // Remove the upgrade from the list of available upgrades
        Debug.Log($"Upgrade added to activeUpgrades: {upgrade}");

        // Implement the effects of each upgrade
        switch (upgrade)
        {
            case "Spider Hunter: Increase XP from killing spiders.":
                // Example: Increase XP gain multiplier
                break;
            case "Stamina +: Gain a dash. Get 50% more stamina.":
                FirstPersonController playerController = FindObjectOfType<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.sprintDuration *= 1.5f; // Increase stamina by 50%
                    playerController.sprintCooldown /= 2f; // Double stamina regeneration speed
                    playerController.canDash = true; // Enable dash
                    Debug.Log("Stamina + applied: Sprint duration increased by 50%, dash enabled, and stamina regeneration doubled.");
                }
                break;
            case "Stamina ++: Move Faster. 150% more stamina.":
                playerController = FindObjectOfType<FirstPersonController>();
                if (playerController != null)
                {
                    playerController.sprintDuration *= 2.5f; // Increase stamina by 150%
                    playerController.sprintCooldown /= 4f; // Quadruple stamina regeneration speed
                    playerController.walkSpeed *= 1.2f; // Increase walk speed by 20%
                    playerController.sprintSpeed *= 1.2f; // Increase sprint speed by 20%
                    playerController.canDash = true; // Enable dash
                    Debug.Log("Stamina ++ applied: Sprint duration increased by 150%, movement speed increased, dash enabled, and stamina regeneration quadrupled.");
                }
                break;
            case "War Machine: Armour piercing bullets.":
                Debug.Log("War Machine applied: Bullets now deal 10% more damage.");
                break;
            case "War Machine++: EXPLODING BULLETS":
                Debug.Log("War Machine++ applied: Bullets now explode and deal double damage.");
                break;
            case "The World: Press and hold Q to slow down time. Only lasts for 5 seconds.":
                Debug.Log("The World upgrade applied.");
                break;
            case "The World+: Time is even slower, for up to 5 seconds.":
                Debug.Log("The World+ upgrade applied.");
                break;
            case "Extra Clips: 200% ammo for all guns.":
                Debug.Log("Extra Clips applied: Ammo capacity increased by 200%.");
                UniversalWeaponSystem[] weaponSystems = FindObjectsOfType<UniversalWeaponSystem>();
                foreach (var weapon in weaponSystems)
                {
                    weapon.totalBullets = Mathf.FloorToInt(weapon.totalBullets * 2f); // Increase total ammo by 200%
                    weapon.magazineSize = Mathf.FloorToInt(weapon.magazineSize * 2f); // Increase magazine size by 200%
                    Debug.Log($"Updated weapon: {weapon.name}, Total Bullets: {weapon.totalBullets}, Magazine Size: {weapon.magazineSize}");
                }
                break;
            case "Chicken Dinner: Heal to full. 15% max health increase.":
                Debug.Log("Chicken Dinner applied: Healing to full and increasing max health by 15%.");
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    Damageable playerDamageable = playerObject.GetComponent<Damageable>();
                    if (playerDamageable != null)
                    {
                        int newMaxHealth = Mathf.FloorToInt(playerDamageable.maxHitPoints * 1.15f);
                        playerDamageable.maxHitPoints = newMaxHealth;
                        playerDamageable.currentHitPoints = newMaxHealth;
                        playerDamageable.OnHeal.Invoke();
                        playerDamageable.OnFullyHealed.Invoke();
                        Debug.Log($"Player healed to full. New max health: {playerDamageable.maxHitPoints}");
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
                break;
        }
    }

    private IEnumerator ActivateTimeSlow()
    {
        isTimeStopActive = true;
        isTimeSlowed = true;

        // Save the original fixedDeltaTime
        float originalFixedDeltaTime = Time.fixedDeltaTime;

        // Determine duration and slow factor based on the active upgrade
        if (IsUpgradeActive("The World: Press and hold Q to slow down time. Only lasts for 5 seconds."))
        {
            timeSlowDuration = 2f;
            timeSlowFactor = 0.5f;
        }
        else if (IsUpgradeActive("The World+: Time is even slower, for up to 5 seconds."))
        {
            timeSlowDuration = 5f;
            timeSlowFactor = 0.2f;
        }

        // Adjust time scale and fixedDeltaTime
        Time.timeScale = timeSlowFactor;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        // Optionally adjust audio pitch
        AudioMixer audioMixer = FindObjectOfType<AudioMixer>();
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Pitch", timeSlowFactor);
        }

        // Optionally change background color
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            float lerpValue = Mathf.InverseLerp(0f, 1f, timeSlowFactor);
            mainCam.backgroundColor = Color.Lerp(new Color(0.14f, 0.19f, 0.19f), new Color(0.3f, 0.18f, 0f, 1f), lerpValue);
        }

        Debug.Log($"Time slowed: TimeScale = {Time.timeScale}, FixedDeltaTime = {Time.fixedDeltaTime}");

        // Wait for the duration in real-time
        yield return new WaitForSecondsRealtime(timeSlowDuration);

        // Start the deactivation process
        StartCoroutine(DeactivateTimeSlow());
    }

    private IEnumerator DeactivateTimeSlow()
    {
        isTimeStopActive = false;
        isTimeSlowed = false;

        // Restore time scale and fixedDeltaTime
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Reset audio pitch
        AudioMixer audioMixer = FindObjectOfType<AudioMixer>();
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Pitch", 1f);
        }

        // Smoothly transition back to the original background color
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            float transitionTime = 0.2f;
            float elapsedTime = 0f;
            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                mainCam.backgroundColor = Color.Lerp(mainCam.backgroundColor, originalBackgroundColor, elapsedTime / transitionTime);
                yield return null;
            }
            mainCam.backgroundColor = originalBackgroundColor;
        }

        Debug.Log("Time restored to normal.");

        // Start cooldown
        isTimeStopOnCooldown = true;
        yield return new WaitForSecondsRealtime(timeStopCooldown);
        isTimeStopOnCooldown = false;
    }

    public bool IsUpgradeActive(string upgradeName)
    {
        bool isActive = activeUpgrades.Contains(upgradeName);
        Debug.Log($"Checking if upgrade is active: {upgradeName} - {isActive}");
        Debug.Log($"Current active upgrades: {string.Join(", ", activeUpgrades)}");
        return isActive;
    }

    public bool IsWarMachinePlusActive()
    {
        return isWarMachinePlusActive;
    }

    public void AddXPFromSpiderKill(int baseXP)
    {
        int xpGained = baseXP;

        // Check if the "Spider Hunter" upgrade is active
        if (IsUpgradeActive("Spider Hunter: Increase XP from killing spiders."))
        {
            xpGained = Mathf.FloorToInt(baseXP * 2f); // Increase XP by 50%
        }

        AddXP(xpGained);
    }

    public void AddXPFromBossHit()
    {
        int xpGained = 1; // Base XP for hitting the boss

        // Scale XP based on the player's level
        if (currentLevel == 2) xpGained = 1;
        else if (currentLevel == 3) xpGained = 2;
        else if (currentLevel >= 4) xpGained = 2;

        AddXP(xpGained);
    }

    private void UpdateUI()
    {
        // Update XP slider
        if (xpSlider != null)
        {
            xpSlider.value = (float)currentXP / xpToNextLevel;
        }

        // Update level text
        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevel}";
        }
    }

    private void UpdateHealthOverlay()
    {
        if (healthOverlayImage == null) return;

        // Find the player's Damageable component
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;

        Damageable playerDamageable = playerObject.GetComponent<Damageable>();
        if (playerDamageable == null) return;

        // Calculate the player's health percentage
        float healthPercentage = (float)playerDamageable.currentHitPoints / playerDamageable.maxHitPoints;

        // Adjust the alpha of the image based on the health percentage
        Color overlayColor = healthOverlayImage.color;
        overlayColor.a = Mathf.Lerp(1f, 0f, healthPercentage); // Fully visible at 0% health, invisible at 100% health
        healthOverlayImage.color = overlayColor;
    }
}
