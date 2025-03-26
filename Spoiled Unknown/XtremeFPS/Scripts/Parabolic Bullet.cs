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
        private float bulletLifetime;

        private float startTime = -1;
        private Vector3 currentPoint;
        #endregion

        #region Initialization
        public void Initialize(Transform startPoint, float speed, float damage, float gravity, float bulletLifetime, GameObject particlePrefab)
        {
            this.startPosition = startPoint.position;
            this.startForward = startPoint.forward.normalized;
            this.speed = speed;
            this.damage = damage;
            this.gravity = gravity;
            this.particlesPrefab = particlePrefab;
            this.bulletLifetime = bulletLifetime;
        }
        #endregion

        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            StartCoroutine(DestroyBullets());
            startTime = -1f;
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
                if (CastRayBetweenPoints(prevPoint, currentPoint, out hit)) OnHit(hit);
            }

            Vector3 nextPoint = FindPointOnParabola(nextTime);
            if (CastRayBetweenPoints(currentPoint, nextPoint, out hit)) OnHit(hit);
        }

        private void Update()
        {
            if (startTime < 0) return;

            float currentTime = Time.time - startTime;
            currentPoint = FindPointOnParabola(currentTime);
            transform.position = currentPoint;
        }
        #endregion

        #region Private Methods
        private Vector3 FindPointOnParabola(float time)
        {
            Vector3 point = startPosition + (speed * time * startForward);
            Vector3 gravityVec = gravity * time * time * Vector3.down;
            return point + gravityVec;
        }

        private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
        {
            return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
        }

        private void OnHit(RaycastHit hit)
        {
            int finalDamage = Mathf.FloorToInt(damage); // Base damage

            // **Check for Enemy Type A**
            if (hit.transform.CompareTag("Enemy") && hit.transform.TryGetComponent(out Damageable damageableA))
            {
                damageableA.ApplyDamage(new Damageable.DamageMessage()
                {
                    amount = finalDamage,  // Full damage
                    damageSource = transform.position
                });

                Debug.Log($"Bullet hit Enemy A: {hit.transform.name}, Damage: {finalDamage}");
            }
            // **Check for Spider-Enemy (takes half damage and apply force)**
            else if (hit.transform.CompareTag("Spider-Enemy") && hit.transform.TryGetComponent(out Damageable damageableB))
            {
                int reducedDamage = Mathf.FloorToInt(finalDamage);  // Half damage
                damageableB.ApplyDamage(new Damageable.DamageMessage()
                {
                    amount = reducedDamage,
                    damageSource = transform.position
                });

                // Apply force to Spider-Enemy if it has a Rigidbody
Rigidbody spiderRigidbody = hit.transform.GetComponent<Rigidbody>();
if (spiderRigidbody != null)
{
    // Calculate impact direction (mostly forward with a slight upward lift)
    Vector3 forceDirection = (hit.transform.position - transform.position).normalized;
    forceDirection += Vector3.up * 0.2f; // Adds a slight lift effect

    // Apply a more realistic bullet force
    spiderRigidbody.AddForce(forceDirection * 2f, ForceMode.Impulse); // Reduced from 10f to 2f
}

                Debug.Log($"Bullet hit Spider-Enemy: {hit.transform.name}, Damage: {reducedDamage}");
            }

            // **Spawn impact particles**
            GameObject hitEffect = PoolManager.Instance.SpawnObject(particlesPrefab, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));
            hitEffect.transform.parent = hit.transform;

            OnBulletDestroy();
        }

        private IEnumerator DestroyBullets()
        {
            yield return new WaitForSeconds(bulletLifetime);
            OnBulletDestroy();
        }

        private void OnBulletDestroy()
        {
            PoolManager.Instance.DespawnObject(this.gameObject);
        }
        #endregion
    }
}
