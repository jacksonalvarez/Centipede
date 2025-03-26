using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spitball : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out Damageable playerDamageable))
        {
            int leechDamage = Mathf.FloorToInt(2);
            playerDamageable.ApplyDamage(new Damageable.DamageMessage()
            {
                amount = leechDamage,
                damageSource = transform.position
            });

            Debug.Log($"Projectile Damage : {leechDamage}");
        }
    }
}
