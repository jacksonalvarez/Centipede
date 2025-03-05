using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class bosshealth : MonoBehaviour
{
    private int totalHealth = 0;
    private int currentHealth = 0;
    private List<Damageable> filteredDamageables;
    private UIHooks uiHooks;

    private void Start()
    {
        uiHooks = FindObjectOfType<UIHooks>(); // Find UIHooks in the scene
        FindRelevantDamageables();
        CalculateTotalHealth();
    }

    /// <summary>
    /// Finds all objects with a Damageable component that are NOT tagged as "Player" or "Spider-Enemy".
    /// </summary>
    private void FindRelevantDamageables()
    {
        filteredDamageables = FindObjectsOfType<Damageable>()
            .Where(d => d.gameObject.tag != "Player" && d.gameObject.tag != "Spider-Enemy")
            .ToList();

        Debug.Log($"Filtered Damageable Objects: {filteredDamageables.Count}");
    }

    /// <summary>
    /// Calculates the total health from all valid Damageable objects.
    /// </summary>
    private void CalculateTotalHealth()
    {
        totalHealth = filteredDamageables.Sum(d => d.maxHitPoints);
        currentHealth = filteredDamageables.Sum(d => d.currentHitPoints);
        UpdateUI();
    }

    /// <summary>
    /// Updates the boss's current health dynamically.
    /// </summary>
    private void Update()
    {
        currentHealth = filteredDamageables.Sum(d => d.currentHitPoints);
        UpdateUI();

        if (currentHealth <= 0)
        {
            OnWormDeath();
        }
    }

    /// <summary>
    /// Updates the UI with current health values.
    /// </summary>
    private void UpdateUI()
    {
        if (uiHooks != null)
        {
            uiHooks.SetHealth(currentHealth, totalHealth);
        }
        else
        {
            Debug.LogWarning("UIHooks not found in the scene!");
        }
    }

    /// <summary>
    /// Calls OnWormDeath on all Damageable segments.
    /// </summary>
    private void OnWormDeath()
    {
        Debug.Log("Worm is dead. Calling OnWormDeath for all segments.");
        foreach (var segment in filteredDamageables)
        {
            segment.SendMessage("OnWormDeath", SendMessageOptions.DontRequireReceiver);
        }
    }
}
