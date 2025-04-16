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
    Debug.Log("Spider position: " + lastPosition + " (S22)");

    Quaternion lastRotation = transform.rotation;
    Debug.Log("Spider rotation: " + lastRotation + " (S22-1)");

    Vector3 lastVelocity = spiderRootRigidbody != null ? spiderRootRigidbody.linearVelocity : Vector3.zero;
    Debug.Log("Spider velocity: " + lastVelocity + " (S22-2)");

    Vector3 lastAngularVelocity = spiderRootRigidbody != null ? spiderRootRigidbody.angularVelocity : Vector3.zero;
    Debug.Log("Spider angular velocity: " + lastAngularVelocity + " (S22-3)");

    Vector3 lastScale = transform.localScale;
    Debug.Log("Spider scale: " + lastScale + " (S22-4)");

    // Ensure the prefab is assigned
    if (spiderRagdollPrefab != null)
    {
        Debug.Log("Ragdoll prefab is assigned.");

        // Instantiate the ragdoll first
            Debug.Log("Spider position: " + lastPosition + " (S22)");
        GameObject ragdollInstance = Instantiate(spiderRagdollPrefab, lastPosition, lastRotation);
            Destroy(ragdollInstance, 15);
            Debug.Log("Ragdoll spawned at position " + ragdollInstance.transform.position);
        ragdollInstance.transform.position = gameObject.transform.position;
        if (ragdollInstance != null)
        {
            ragdollInstance.transform.localScale = lastScale;
            Debug.Log("Ragdoll spawned at position " + ragdollInstance.transform.position);

            Rigidbody ragdollRootRb = ragdollInstance.GetComponent<Rigidbody>();
            if (ragdollRootRb != null)
            {
                // No velocity or angular velocity applied
                Debug.Log("Ragdoll Rigidbody found, but no velocities applied.");
            }
            else
            {
                Debug.LogError("Ragdoll's Rigidbody is missing or not correctly assigned.");
            }
        }
        else
        {
            Debug.LogError("Failed to instantiate ragdoll prefab. Make sure the prefab is valid.");
        }
    }
    else
    {
        Debug.LogError("Spider Ragdoll Prefab is not assigned. Please assign it in the inspector.");
    }

    // Now destroy the spider after the ragdoll is spawned
    Destroy(gameObject);
}

}
