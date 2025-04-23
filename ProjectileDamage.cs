using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public int damage = 10;
    public GameObject damageSource;
    private bool hasDealtDamage = false;

    private void Start()
    {
        // Ensure the projectile has a collider and rigidbody
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"{gameObject.name} does not have a Collider. Add one to interact with the environment.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name} Collider is not set as a trigger. Setting it to trigger.");
            col.isTrigger = true; // Projectiles should use triggers for collision detection
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name} does not have a Rigidbody. Adding one for physics interactions.");
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false; // Ensure the Rigidbody is not kinematic
        rb.useGravity = true;   // Enable gravity for the projectile
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDealtDamage)
            return;

        Debug.Log($"Projectile collided with: {other.gameObject.name}");

        // Check if the collision is with the player
        if (other.CompareTag("Player"))
        {
            Damageable playerDamageable = other.GetComponent<Damageable>();
            if (playerDamageable != null)
            {
                Debug.Log("Applying damage to the player.");
                playerDamageable.ApplyDamage(new Damageable.DamageMessage
                {
                    amount = damage,
                    damageSource = transform.position,
                    direction = (other.transform.position - transform.position).normalized,
                    damager = this,
                    throwing = false,
                    stopCamera = false
                });

                hasDealtDamage = true;
                Destroy(gameObject);
            }
        }
        else if (!other.CompareTag("Spider") && !other.CompareTag("Projectile"))
        {
            Debug.Log("Projectile hit a non-player object.");
            Destroy(gameObject, 3f);
        }
    }
}