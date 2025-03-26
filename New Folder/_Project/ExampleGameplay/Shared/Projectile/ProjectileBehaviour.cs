using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return; // Ignore everything except enemies

        if (other.TryGetComponent(out Damageable damageable))
        {
            Debug.Log("Hit Enemy: " + other.name);
            damageable.ApplyDamage(new Damageable.DamageMessage()
            {
                amount = 1,
                damageSource = transform.position
            });
        }

    }

    void RemoveProjectile()
    {
        Debug.Log("Projectile Removed");
        gameObject.SetActive(false);
    }
}
