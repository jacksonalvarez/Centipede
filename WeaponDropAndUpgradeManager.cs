using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XtremeFPS.WeaponSystem.Pickup; // Add this to access the WeaponPickup class

public class WeaponDropAndUpgradeManager : MonoBehaviour
{
    [Header("Weapon Drop Settings")]
    public GameObject weaponDropPrefab; // Prefab for the weapon drop
    public Transform player; // Reference to the player
    public float spawnRadius = 10f; // Radius around the player to spawn the weapon drop
    public float spawnInterval = 60f; // Interval between spawns in seconds

    void Start()
    {
        // Start the periodic spawn coroutine
        StartCoroutine(SpawnWeaponDropsPeriodically());
    }

    private IEnumerator SpawnWeaponDropsPeriodically()
    {
        while (true)
        {
            SpawnWeaponDrop();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnWeaponDrop()
    {
        if (weaponDropPrefab == null || player == null)
        {
            Debug.LogError("WeaponDropPrefab or Player reference is not assigned.");
            return;
        }

        // Generate a random position around the player within the spawn radius
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0; // Keep the spawn position on the same horizontal plane as the player
        Vector3 spawnPosition = player.position + randomOffset;

        // Perform a downward raycast to ensure the drop spawns on the ground
        if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
        {
            spawnPosition = hit.point;
        }

        // Instantiate the weapon drop
        GameObject weaponDrop = Instantiate(weaponDropPrefab, spawnPosition, Quaternion.identity);

        // Ensure the weapon drop is initialized properly
        WeaponPickup weaponPickup = weaponDrop.GetComponent<WeaponPickup>();
        if (weaponPickup != null)
        {
            weaponPickup.hasBeenPickedUp = false; // Ensure the weapon is not marked as picked up
        }

        Debug.Log($"Weapon drop spawned at {spawnPosition}");
    }
}
