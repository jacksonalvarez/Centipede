using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CINEMATICLAND : MonoBehaviour
{
    public Rigidbody targetRigidbody; // Reference to the Rigidbody to launch
    public float launchForce = 1000f; // Impulse force to apply on the Y-axis
    public float randomForceMin = 0f; // Minimum random force for X and Z axes
    public float randomForceMax = 0.2f; // Maximum random force for X and Z axes
    public GameObject objectToEnable; // First GameObject to enable after launch
    public GameObject obj2; // Second GameObject to enable after 4 seconds
    public Transform radioTransform; // Reference to the radio transform
    public Transform playerTransform; // Reference to the player transform
    public GameObject delayedObject; // GameObject to activate after 15 seconds
    public AudioSource audioSource; // Audio source for playing the landing sound
    public AudioClip landingSound; // Audio clip to play when the object lands
    private bool isWithinRange = false; // Tracks if the player is within range

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform reference is not assigned.");
        }

        if (radioTransform == null)
        {
            Debug.LogError("Radio Transform reference is not assigned.");
        }

        StartCoroutine(LaunchAfterDelay());
    }

    void Update()
    {
        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            UpdateTransformPositions();
            CheckPlayerProximity();
        }
        else
        {
            Debug.LogWarning("Player is not active in the scene.");
        }
    }

    private IEnumerator LaunchAfterDelay()
    {
        yield return new WaitForSeconds(13f); // Wait for 13 seconds

        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = false; // Disable isKinematic

            // Generate random forces for X and Z axes
            float randomX = Random.Range(randomForceMin, randomForceMax);
            float randomZ = Random.Range(randomForceMin, randomForceMax);

            // Apply the forces
            Vector3 launchDirection = new Vector3(randomX, 1f, randomZ);
            targetRigidbody.AddForce(new Vector3(0, launchForce, 0), ForceMode.Impulse); // Apply Y-axis force
            targetRigidbody.AddForce(new Vector3(randomX, 0, randomZ), ForceMode.Impulse); // Apply random X and Z forces

            Debug.Log($"Rigidbody launched with Y force: {launchForce}, and random XZ forces: ({randomX}, {randomZ})");
        }
        else
        {
            Debug.LogWarning("No Rigidbody assigned to launch.");
        }

        // Enable the first GameObject
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
            Debug.Log("Enabled GameObject: " + objectToEnable.name); 
        }

        // Wait for 4 seconds
        yield return new WaitForSeconds(2.5f);
            objectToEnable.SetActive(false);

        // Enable the second GameObject
        if (obj2 != null)
        {
            obj2.SetActive(true);
            Debug.Log("Enabled GameObject: " + obj2.name);
        }
        else
        {
            Debug.LogWarning("Second GameObject (obj2) is not specified.");
        }
    }

    private void UpdateTransformPositions()
    {
        if (playerTransform != null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            Debug.Log($"Updated Player position: {playerTransform?.position}");
        }

        if (radioTransform != null)
        {
            radioTransform = GameObject.FindGameObjectWithTag("Radio")?.transform;
            Debug.Log($"Updated Radio position: {radioTransform?.position}");
        }
    }

    private void CheckPlayerProximity()
    {
        if (radioTransform == null || delayedObject == null || playerTransform == null || isWithinRange)
        {
            Debug.LogWarning("One or more required references are null or the proximity check has already been triggered.");
            return;
        }

        // Log the current positions of the player and the radio
        Debug.Log($"Player position: {playerTransform.position}");
        Debug.Log($"Radio position: {radioTransform.position}");

        float distanceToRadio = Vector3.Distance(playerTransform.position, radioTransform.position);
        Debug.Log($"Distance to radio: {distanceToRadio}");

        if (distanceToRadio <= 10f)
        {
            Debug.Log("Player is within 10f of the radio. Starting 15-second timer.");
            isWithinRange = true;
            StartCoroutine(ActivateDelayedObject());
        }
    }

    private IEnumerator ActivateDelayedObject()
    {
        yield return new WaitForSeconds(15f);
        delayedObject.SetActive(true);
        Debug.Log("Delayed object activated after 15 seconds.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Play the landing sound only when the object impacts the ground
        if (collision.gameObject.CompareTag("Terrain") && audioSource != null && landingSound != null)
        {
            audioSource.PlayOneShot(landingSound);
            Debug.Log("Landing sound played after impact with the ground.");
        }
    }
}
