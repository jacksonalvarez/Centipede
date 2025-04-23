using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Enum to define different enemy types (Worm, Spider, etc.)
public class DamageVisual : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private GameObject test; // This will reference the GameObject to be hidden on death.
    [ColorUsage(true, true)]
    public Color hitColor;

    // To store the enemy type (Worm or Spider)
    [SerializeField] public GameObject spiderRagdollPrefab; // Assign in Inspector
    public Rigidbody spiderRootRigidbody;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Assign the test GameObject to the object named "Head"
        test = GameObject.Find("Head");  // Searching for the GameObject named "Head"
    }

    public void OnDamage()
    {
        // Change the material color briefly to show the hit effect
        if(meshRenderer != null){
        meshRenderer.material.color = Color.white;
        meshRenderer.material.DOColor(hitColor, 0.1f).OnComplete(() => meshRenderer.material.DOColor(Color.white, 0.1f));
    }}

    public void OnWormDeath()
    {
        Debug.Log("Worm is dead.");

        // Worm-specific death logic here
        if (test != null)
        {
            // Disable the test GameObject (e.g., the Head part)
            test.SetActive(false);
        }
        else
        {
            Debug.LogError("Test GameObject 'Head' is not assigned.");
        }
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Player is dead.");


        gameObject.SetActive(false);
    
    }

    private void OnSpiderDeath()
    {
        Debug.Log("Spider is dead.");

        Vector3 lastPosition = transform.position;
        Quaternion lastRotation = transform.rotation;
        Vector3 lastScale = transform.localScale;

        // Ensure the prefab is assigned
        if (spiderRagdollPrefab != null)
        {
            GameObject ragdollInstance = Instantiate(spiderRagdollPrefab, lastPosition, lastRotation);
            Destroy(ragdollInstance, 15);
            ragdollInstance.transform.localScale = lastScale;
        }
        else
        {
            Debug.LogError("Spider Ragdoll Prefab is not assigned. Please assign it in the inspector.");
        }

        // Apply upgrades for spider death
        XPAndUpgradeSystem xpSystem = FindObjectOfType<XPAndUpgradeSystem>();
        if (xpSystem != null)
        {
            Debug.Log("Applying upgrades for spider death.");
            xpSystem.AddXPFromSpiderKill(10); // Example: Grant 50 XP for killing a spider
        }
        else
        {
            Debug.LogWarning("XPAndUpgradeSystem not found.");
        }

        // Destroy the spider after the ragdoll is spawned
        Destroy(gameObject);
    }
}
