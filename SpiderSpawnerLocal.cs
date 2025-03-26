using UnityEngine;
using System.Collections;

public class SpiderSpawnerLocal : MonoBehaviour
{
    [Tooltip("Spider prefab to be spawned")]
    public GameObject spiderPrefab;

    [Tooltip("Number of spiders to spawn per minute")]
    public int spidersPerMinute = 10;

    [Tooltip("Maximum number of spiders allowed in the scene")]
    public int maxSpidersInScene = 20;

    private float spawnInterval;
    private bool isSpawning = true;
    private Transform playerTransform;

    void Start()
    {
        spawnInterval = 60f / spidersPerMinute;

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Ensure the player GameObject has the 'Player' tag.");
            isSpawning = false;
            return;
        }

        StartCoroutine(SpawnSpidersPerInterval());
    }

    void OnDisable()
    {
        isSpawning = false;
    }

    IEnumerator SpawnSpidersPerInterval()
    {
        while (isSpawning)
        {
            int currentSpiderCount = GetCurrentSpiderCount();
            if (currentSpiderCount < maxSpidersInScene)
            {
                Debug.Log("Spawning spider. Current spider count: " + currentSpiderCount);
                SpawnSpider();
            }
            else
            {
                Debug.Log("Max spiders reached. Current spider count: " + currentSpiderCount);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSpider()
    {
        if (playerTransform == null) return;

        Vector3 spawnPosition;
        int attempts = 5; 

        do
        {
            // Random height offset between 1-2 feet above the player (0.3 to 0.6 meters)
            float offsetY = Random.Range(0.3f, 0.6f);

            // Spawning exactly 200 meters out on both Y and Z axes
            float offsetYZ = Random.Range(0f,1f);

            // Final spawn position
            spawnPosition = playerTransform.position + new Vector3(offsetYZ* 100, offsetY, offsetYZ*100);

            attempts--;
        } 
        while (Physics.CheckSphere(spawnPosition, 0.5f) && attempts > 0);

        if (attempts > 0)
        {
            GameObject newSpider = Instantiate(spiderPrefab, spawnPosition, Quaternion.identity);
            newSpider.tag = "Spider-Enemy"; 
            Debug.Log("Spider spawned at position: " + spawnPosition);
        }
        else
        {
            Debug.LogWarning("Failed to find a valid spawn position.");
        }
    }

    int GetCurrentSpiderCount()
    {
        return GameObject.FindGameObjectsWithTag("Spider-Enemy").Length;
    }
}
