/*Copyright © Spoiled Unknown*/
/*2024*/

using System.Collections;
using UnityEngine;
using XtremeFPS.PoolingSystem;

namespace XtremeFPS.WeaponSystem
{
    public class ParabolicBullet : MonoBehaviour
    {
        #region Variables
        private float speed;
        private float damage;
        private float gravity;
        private Vector3 startPosition;
        private Vector3 startForward;
        private GameObject particlesPrefab;
        private GameObject explosionEffectPrefab; // Added for War Machine++
        private float bulletLifetime;
        private float startTime = -1;
        private Vector3 currentPoint;
        private float bulletDeathTime;

        // Pre-allocated vectors to reduce GC pressure
        private Vector3 tempPoint;
        private Vector3 gravityVec;
        #endregion

        #region Initialization
        public void Initialize(Transform startPoint, float speed, float damage, float gravity, float bulletLifetime, GameObject particlePrefab, GameObject explosionEffectPrefab)
        {
            this.startPosition = startPoint.position;
            this.startForward = startPoint.forward.normalized;
            this.speed = speed;
            this.damage = damage;
            this.gravity = gravity;
            this.particlesPrefab = particlePrefab;
            this.explosionEffectPrefab = explosionEffectPrefab; // Added for War Machine++
            this.bulletLifetime = bulletLifetime;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            startTime = -1f;
            bulletDeathTime = Time.time + bulletLifetime;
        }

        private void FixedUpdate()
        {
            if (startTime < 0) startTime = Time.time;

            float currentTime = Time.time - startTime;
            float prevTime = currentTime - Time.fixedDeltaTime;
            float nextTime = currentTime + Time.fixedDeltaTime;

            RaycastHit hit;
            Vector3 currentPoint = FindPointOnParabola(currentTime);

            if (prevTime > 0)
            {
                Vector3 prevPoint = FindPointOnParabola(prevTime);
                if (CastRayBetweenPoints(prevPoint, currentPoint, out hit)) OnHit(hit, currentPoint - prevPoint);
            }

            Vector3 nextPoint = FindPointOnParabola(nextTime);
            if (CastRayBetweenPoints(currentPoint, nextPoint, out hit)) OnHit(hit, nextPoint - currentPoint);
        }

        private void Update()
        {
            if (startTime < 0) return;

            float currentTime = Time.time - startTime;
            currentPoint = FindPointOnParabola(currentTime);
            transform.position = currentPoint;

            // Check for bullet lifetime expiration
            if (Time.time >= bulletDeathTime)
            {
                OnBulletDestroy();
            }
        }
        #endregion

        #region Private Methods
        private Vector3 FindPointOnParabola(float time)
        {
            tempPoint = startPosition + (speed * time * startForward);
            gravityVec = gravity * time * time * Vector3.down;
            tempPoint += gravityVec;
            return tempPoint;
        }

        private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
        {
            return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
        }

        private void OnHit(RaycastHit hit, Vector3 bulletDirection)
        {
            Debug.Log($"Bullet hit: {hit.transform.name} with tag: {hit.transform.tag}");

            XPAndUpgradeSystem xpSystem = FindObjectOfType<XPAndUpgradeSystem>();
            bool isWarMachineActive = xpSystem != null && xpSystem.IsUpgradeActive("War Machine: Armour piercing bullets. Increase Damage by 10%");
            bool isWarMachinePlusActive = xpSystem != null && xpSystem.IsUpgradeActive("War Machine++: After 15 seconds of not being hit, bullets begin to explode, Dealing double damage.");

            int finalDamage = Mathf.FloorToInt(damage);
            if (isWarMachineActive)
            {
                finalDamage = Mathf.FloorToInt(finalDamage * 1.1f); // Increase damage by 10%
                Debug.Log("War Machine active: Damage increased by 10%.");
            }

            if (isWarMachinePlusActive)
            {
                finalDamage *= 2; // Double damage for War Machine++
                Debug.Log("War Machine++ active: Damage doubled.");
            }

            // Replace impact effect with explosion effect for War Machine++
            GameObject impactEffect = isWarMachinePlusActive ? explosionEffectPrefab : particlesPrefab;

            Debug.Log($"Impact Effect Prefab: {impactEffect.name}");

            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-bulletDirection));
            }

            if (hit.transform.CompareTag("Spider-Enemy") && hit.transform.TryGetComponent(out Damageable spiderDamageable))
            {
                Debug.Log("Hit Spider-Enemy.");
                spiderDamageable.ApplyDamage(new Damageable.DamageMessage()
                {
                    amount = finalDamage,
                    damageSource = transform.position
                });

                // Heal the player with a chance based on the damage dealt
                spiderDamageable.HealPlayerWithChance(finalDamage);

                // Grant XP for hitting a spider
                if (xpSystem != null)
                {
                    xpSystem.AddXPFromSpiderKill(5); // Example: Grant 5 XP for hitting a spider
                }

                Rigidbody spiderRigidbody = hit.transform.GetComponent<Rigidbody>();
                if (spiderRigidbody != null)
                {
                    Vector3 forceDirection = (hit.transform.position - transform.position).normalized + Vector3.up * 0.2f;
                    spiderRigidbody.AddForce(forceDirection * 2f, ForceMode.Impulse);
                }
            }
            else if (hit.transform.CompareTag("Enemy") && hit.transform.TryGetComponent(out Damageable bossDamageable))
            {
                Debug.Log("Hit Boss.");
                bossDamageable.ApplyDamage(new Damageable.DamageMessage()
                {
                    amount = finalDamage,
                    damageSource = transform.position
                });

                // Heal the player with a chance based on the damage dealt
                bossDamageable.HealPlayerWithChance(finalDamage);

                // Grant XP for hitting the boss
                if (xpSystem != null)
                {
                    xpSystem.AddXPFromBossHit();
                }
            }

            OnBulletDestroy();
        }

        private void OnBulletDestroy()
        {
            PoolManager.Instance.DespawnObject(this.gameObject);
        }
        #endregion
    }
}