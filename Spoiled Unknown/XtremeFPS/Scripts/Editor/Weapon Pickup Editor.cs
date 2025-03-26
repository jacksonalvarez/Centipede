/*Copyright © Spoiled Unknown*/
/*2024*/

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
        // Once the gun is picked up for the first time, this flag remains true.
        public bool NoLongerGunDrop { get; private set; } = false;
        // This is assigned by WeaponDrop to set the initial spawn (drop) position.
        public Transform gunDropPosition; 

        #region Variables
        public static bool IsWeaponEquipped { get; private set; }
        public CharacterController playerArmature;
        public Transform weaponHolder;       // Player’s gun position (hand)
        public Transform cameraRoot;
        public TextMeshProUGUI bulletText;

        public bool equipped;
        public int Priority;
        public float dropForwardForce;
        public float dropUpwardForce;
        public float dropTorqueMultiplier;

        private UniversalWeaponSystem weaponSystem;
        private BoxCollider Collider;
        private Rigidbody rb;
        #endregion

        #region Monobehaviour Callbacks
        private void Start()
        {
            Collider = GetComponent<BoxCollider>();
            weaponSystem = GetComponent<UniversalWeaponSystem>();

            // If the weapon hasn't been picked up yet, position it at its drop location.
            if (!NoLongerGunDrop)
            {
                transform.SetParent(gunDropPosition);
                transform.position = gunDropPosition.position;
                transform.rotation = gunDropPosition.rotation;
                if (!gameObject.TryGetComponent<Rigidbody>(out rb))
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                }
                rb.isKinematic = true;  // Disable physics until pickup.
            }

            if (equipped)
                Equip();
            else
                UnEquip();
        }
        #endregion

        #region Private methods
        private void UnEquip()
        {
            // Called when dropping the weapon.
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;  // Enable physics so gravity and forces act normally.
            weaponSystem.enabled = false;
            Collider.isTrigger = false;
            equipped = false;
            IsWeaponEquipped = false;
        }

        private void Equip()
        {
            // Called when the weapon is picked up.
            if (rb != null)
            {
                // Removing the Rigidbody stops unwanted physics forces while held.
                Destroy(rb);
            }
            equipped = true;
            weaponSystem.enabled = true;
            Collider.isTrigger = true;
            IsWeaponEquipped = true;
        }

        public void PickUp()
        {
            // On the very first pickup, mark that it's no longer in the drop state.
            if (!NoLongerGunDrop)
            {
                NoLongerGunDrop = true;
            }

            // Reparent the weapon to the player's gun position (weaponHolder) with zero offset.
            transform.SetParent(weaponHolder);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Ensure a Rigidbody exists and disable physics while held.
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;

            Equip();
        }

        public void Drop()
        {
            // Drop the weapon using the unequip process.
            UnEquip();
            bulletText.SetText("00 / 00");
            transform.SetParent(null); // Detach from the player's gun position.

            // Ensure we have a Rigidbody and re-enable physics.
            if (!gameObject.TryGetComponent<Rigidbody>(out rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;  // Physics enabled so gravity and forces take effect.

            // Apply the player's current velocity and additional forces for a realistic throw.
            rb.velocity = playerArmature.velocity;
            rb.AddForce(cameraRoot.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(cameraRoot.up * dropUpwardForce, ForceMode.Impulse);

            float random = Random.Range(-1f, 1f);
            rb.AddTorque(new Vector3(random, random, random) * dropTorqueMultiplier);
        }

        public bool IsEquiped()
        {
            return equipped;
        }

        public Transform GetTransform()
        {
            return transform;
        }
        #endregion
    }
}
