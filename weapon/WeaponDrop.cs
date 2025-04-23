/*Copyright © Spoiled Unknown*/
/*2024*/

using UnityEngine;

namespace XtremeFPS.WeaponSystem.Pickup
{
    public class WeaponDrop : MonoBehaviour
    {
        public Transform spawnPointOne; // First spawn position (GunDrop position)
        public Transform spawnPointTwo; // Second spawn position (GunDrop position)
        public GameObject[] weaponPrefabs; // Array of weapon prefabs to choose from

        public int numberOfWeapons = 5; // Number of weapons in the dynamic array
        private GameObject[] selectedWeapons; // Array to store selected weapons

        private void Start()
        {
            // Ensure spawn points are assigned.
            if (spawnPointOne == null || spawnPointTwo == null)
            {
                Debug.LogError("Please assign both spawn points!");
                return;
            }

            // Ensure we have at least one weapon prefab.
            if (weaponPrefabs.Length == 0)
            {
                Debug.LogError("Weapon prefabs array is empty!");
                return;
            }

            // Spawn weapons at predefined positions.
            SpawnWeapons();
        }

        // Method to spawn random weapons at the predefined spawn points.
        private void SpawnWeapons()
        {
            // Dynamically allocate the weapons array.
            selectedWeapons = new GameObject[numberOfWeapons];

            // Fill the array with random weapon prefabs.
            for (int i = 0; i < numberOfWeapons; i++)
            {
                selectedWeapons[i] = weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
            }

            // Spawn the first weapon at spawn point one.
            int randomWeaponIndexOne = Random.Range(0, selectedWeapons.Length);
            GameObject weaponOne = Instantiate(selectedWeapons[randomWeaponIndexOne], spawnPointOne.position, Quaternion.identity);
            weaponOne.transform.SetParent(spawnPointOne);  // Make the weapon a child of spawnPointOne.

            // Set the gunDropPosition for this weapon.
            WeaponPickup weaponPickupOne = weaponOne.GetComponent<WeaponPickup>();
            if (weaponPickupOne != null)
            {
                weaponPickupOne.gunDropPosition = spawnPointOne; // It starts at spawn point one.
            }
            Debug.Log($"Spawned weapon at spawn point one: {selectedWeapons[randomWeaponIndexOne].name}");

            // Spawn the second weapon at spawn point two.
            int randomWeaponIndexTwo = Random.Range(0, selectedWeapons.Length);
            GameObject weaponTwo = Instantiate(selectedWeapons[randomWeaponIndexTwo], spawnPointTwo.position, Quaternion.identity);
            weaponTwo.transform.SetParent(spawnPointTwo);  // Make the weapon a child of spawnPointTwo.

            // Set the gunDropPosition for this weapon.
            WeaponPickup weaponPickupTwo = weaponTwo.GetComponent<WeaponPickup>();
            if (weaponPickupTwo != null)
            {
                weaponPickupTwo.gunDropPosition = spawnPointTwo; // It starts at spawn point two.
            }
            Debug.Log($"Spawned weapon at spawn point two: {selectedWeapons[randomWeaponIndexTwo].name}");
        }

        private void Update()
        {
            // Ensure weapons stay at their respective spawn points and update their local position/rotation
            if (spawnPointOne != null)
            {
                Transform weaponOne = spawnPointOne.GetChild(0);
                weaponOne.localPosition = Vector3.zero; // Reset local position to match the parent
                weaponOne.localRotation = Quaternion.identity; // Reset local rotation to match the parent
            }

            if (spawnPointTwo != null)
            {
                Transform weaponTwo = spawnPointTwo.GetChild(0);
                weaponTwo.localPosition = Vector3.zero; // Reset local position to match the parent
                weaponTwo.localRotation = Quaternion.identity; // Reset local rotation to match the parent
            }
        }
    }
}
