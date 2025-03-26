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
    }

    void Update()
    {
        // Apply downward movement
        rb.linearVelocity = new Vector3(0, -fallSpeed, 0);

        // Trigger atmosphere effect at a certain height
        if (!atmosphereEffectTriggered && transform.position.y <= atmosphereHeight)
        {
            atmosphereEffectTriggered = true;
            Instantiate(atmosphereEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Spawn impact effect when hitting the ground
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        // Optional: Destroy the object after a delay
        Destroy(gameObject, destroyDelay);
    }
}
