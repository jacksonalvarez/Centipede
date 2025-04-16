using TMPro;
using UnityEngine;
using XtremeFPS.Interfaces;

namespace XtremeFPS.WeaponSystem.Pickup
{
    [RequireComponent(typeof(UniversalWeaponSystem))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("Spoiled Unknown/XtremeFPS/Weapon Pickup")]
    public class WeaponPickup : MonoBehaviour, IPickup
    {
        // Flag to track whether the weapon has been picked up at least once.
        public bool hasBeenPickedUp = false;
        // This is assigned by WeaponDrop to set the initial spawn (drop) position.
        public Transform gunDropPosition;

        public CharacterController playerArmature;
        public Transform weaponHolder;       // Player’s gun position (hand)
        public Transform cameraRoot;
        public TextMeshProUGUI bulletText;

        public static bool IsWeaponEquipped { get; private set; }
        public int Priority;
        public float dropForwardForce;
        public float dropUpwardForce;
        public float dropTorqueMultiplier;

        // Variables for floating effect (active only when not yet picked up)
        private Vector3 initialPosition;
        public float floatAmplitude = 0.5f;
        public float floatFrequency = 1f;

        private UniversalWeaponSystem weaponSystem;
        private BoxCollider Collider;
        private Rigidbody rb;

        private void Start()
        {
            Collider = GetComponent<BoxCollider>();
            weaponSystem = GetComponent<UniversalWeaponSystem>();

            // When the weapon hasn't been picked up, position it at its drop location and enable floating.
            if (!hasBeenPickedUp)
            {
                transform.SetParent(gunDropPosition);
                transform.position = gunDropPosition.position;
                transform.rotation = gunDropPosition.rotation;
                initialPosition = transform.position; // Save the starting position for floating effect

                if (!TryGetComponent<Rigidbody>(out rb))
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                }
                rb.isKinematic = false;  // Keep kinematic while floating.
            }
            else
            {
                // If it has been picked up before, ensure rb exists.
                if (!TryGetComponent<Rigidbody>(out rb))
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                }
            }

            // Initialize state based on whether it's equipped.
            if (IsWeaponEquipped)
                Equip();
            else
                UnEquip();
        }

        private void Update()
        {
            // Apply floating effect only if the weapon hasn't been picked up yet.
            if (!hasBeenPickedUp && !IsWeaponEquipped)
            {
                float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                if (rb == null)
                {
                  rb = gameObject.AddComponent<Rigidbody>();
                }
                rb.isKinematic = true;

            }
        }

        private void UnEquip()
        {
            // Called when dropping the weapon.
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
            rb.isKinematic = false;  // Enable physics when dropped.
            weaponSystem.enabled = false;

            // Set collider to not trigger when dropped
            Collider.isTrigger = false;  
            IsWeaponEquipped = false;
        }

        private void Equip()
        {
            // Mark that the weapon has been picked up at least once.
            Destroy(rb);
            hasBeenPickedUp = true;
            weaponSystem.enabled = true;

            // Set collider to trigger when equipped to prevent collisions with player
            Collider.isTrigger = true;  
            IsWeaponEquipped = true;
        }

        public void PickUp()
        {
            if (!hasBeenPickedUp)
            {
                hasBeenPickedUp = true;
            }
            Equip();
            transform.SetParent(weaponHolder);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }

        public void Drop()
        {
            UnEquip();
            bulletText.SetText("00 / 00");
            transform.SetParent(null);

            // When dropped, enable physics and apply forces.
            rb.linearVelocity = playerArmature.velocity;
            rb.AddForce(cameraRoot.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(cameraRoot.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * dropTorqueMultiplier);
        }

        public bool IsEquiped()
        {
            return IsWeaponEquipped;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}
