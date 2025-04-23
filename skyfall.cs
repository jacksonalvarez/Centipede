using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skyfall : MonoBehaviour
{
    public float fallSpeed = 20f;  // Speed of descent
    public float atmosphereHeight = 150f;  // Height at which atmosphere effect triggers
    public GameObject atmosphereEffectPrefab;  // Particle effect for breaking the atmosphere
    public GameObject impactEffectPrefab;  // Particle effect for hitting the ground
    public float destroyDelay = 2f;  // Time before object is destroyed after impact
    public float hiddenPositionY = -50f;  // Position below ground to hide the object
    public float minWaitTime = 0f;  // Minimum time to wait before starting the fall
    public float maxWaitTime = 800f;  // Maximum time to wait before starting the fall
    public float spawnHeight = 150f;  // Height to teleport the object to before falling

    private Rigidbody rb;
    private bool atmosphereEffectTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;  // We'll control descent manually
        rb.isKinematic = true; // Disable physics while inactive or waiting

        // Hide the object below ground initially
        transform.position = new Vector3(transform.position.x, hiddenPositionY, transform.position.z);

        // Start the coroutine to handle the wait and fall logic
        StartCoroutine(HandleSkyfall());
    }

    private IEnumerator HandleSkyfall()
    {
        // Wait for a random time between minWaitTime and maxWaitTime
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // Enable Rigidbody physics before starting the fall
        rb.isKinematic = false;

        // Teleport the object to the correct height for falling
        transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z);

        // Start falling
        rb.linearVelocity = new Vector3(0, -fallSpeed, 0);
    }

    void Update()
    {
        // Trigger atmosphere effect at a certain height
        if (!atmosphereEffectTriggered && transform.position.y <= atmosphereHeight)
        {
            atmosphereEffectTriggered = true;
            if (atmosphereEffectPrefab != null)
            {
                Instantiate(atmosphereEffectPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop movement if the object hits the terrain
        if (collision.gameObject.CompareTag("Terrain"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;

            // Disable the Rigidbody completely to prevent further interactions
            rb.detectCollisions = false;

            // Disable the Collider to prevent further collisions
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Spawn impact effect
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }

            // Optional: Destroy the object after a delay
            Destroy(gameObject, destroyDelay);
        }
    }

    public static void SpawnNearPlayer(GameObject prefab, Transform player)
    {
        if (prefab == null || player == null) return;

        // Calculate spawn position 150 units above the player in y, and randomize x and z within 30 units
        Vector3 spawnPosition = player.position + new Vector3(
            Random.Range(-30f, 30f), 
            150f, 
            Random.Range(-30f, 30f)
        );

        // Instantiate the prefab at the calculated position
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}
