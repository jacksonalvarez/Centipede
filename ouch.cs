using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageOnCollision : MonoBehaviour
{
    public int damageAmount = 10;

    private void Start()
    {
        // Ensure the object has a Rigidbody or is interacting with one
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name} does not have a Rigidbody. Ensure the player or this object has a Rigidbody.");
        }
        else
        {
            Debug.Log($"{gameObject.name} has a Rigidbody. IsKinematic: {rb.isKinematic}");
        }

        // Ensure the Collider is enabled and set as a trigger
        Collider col = GetComponent<Collider>();
        if (col == null || !col.enabled)
        {
            Debug.LogError($"{gameObject.name} does not have an enabled Collider. Trigger will not work.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name} Collider is not set as a trigger. Setting it to trigger.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detected with: {other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            Damageable player = other.GetComponent<Damageable>();
            if (player != null)
            {
                Debug.Log("Applying damage to the player.");
                player.ApplyDamage(new Damageable.DamageMessage
                {
                    amount = damageAmount,
                    damageSource = transform.position,
                    direction = (other.transform.position - transform.position).normalized,
                    damager = this,
                    throwing = false,
                    stopCamera = false
                });
            }
            else
            {
                Debug.LogWarning("Player does not have a Damageable component.");
            }
        }
    }
}