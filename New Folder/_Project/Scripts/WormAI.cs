﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[System.Serializable] public class CameraEvent : UnityEvent<bool> { }
[System.Serializable] public class EffectEvent : UnityEvent<bool,bool> { }
[System.Serializable] public class ParticleEvent : UnityEvent<bool,int> { }

public class WormAI : MonoBehaviour, IMessageReceiver
{
    [HideInInspector] public CameraEvent OnBossReveal;
    [HideInInspector] public EffectEvent GroundContact;
    [HideInInspector] public ParticleEvent GroundDetection;

    [Header("Pathing")]
    [SerializeField] CinemachineSmoothPath path = default;
    [SerializeField] CinemachineDollyCart cart = default;
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] AudioClip surfaceSound = default;
    [SerializeField] AudioClip bossSound = default;
    [SerializeField] AudioClip submergeSound = default;
    [SerializeField] AudioClip laserChargeSound = default;  // Sound for charging the laser
    [SerializeField] AudioClip laserBeamSound = default;    // Sound for firing the laser

    [Header("Particle Effects")]
    [SerializeField] private GameObject breachEffectPrefab; // Particle effect for breaching
    [SerializeField] private GameObject submergeEffectPrefab; // Particle effect for submerging

    CharacterController playerShip;
    UIHooks ui;

    public Vector3 startPosition, endPosition;
    RaycastHit hitInfo;
    int totalHealth;
    int currentHealth;
    Damageable[] damageables;
    public Transform placeholdAttackPos;

    // New state variables
    private bool isTrackingPath = true;  // Whether we are following the path or attacking
    private bool isAttacking = false;    // Whether ana attack is in progress
    public updateAttacks test;
    public float lazer_anim = 9.6f;
    public float spider_summon_anim = 9.05f;
    public GameObject spiderPrefab;
    [SerializeField] AudioSource mainAudioSource = default;   // For surfacing, boss, and submerging sounds
    [SerializeField] AudioSource laserChargeAudioSource = default; // For laser charge sound
    [SerializeField] AudioSource laserBeamAudioSource = default;  // For laser beam sound

    [Header("Attack Settings")]
    [SerializeField] private float laserDamagePerTick = 5f;  // Damage per frame when laser hits player
    
        void Awake() {
        // Create audio sources if they don't exist
        if (mainAudioSource == null) {
            mainAudioSource = gameObject.AddComponent<AudioSource>();
            mainAudioSource.spatialBlend = 0.5f;
            mainAudioSource.playOnAwake = false;
        }
        
        if (laserChargeAudioSource == null) {
            laserChargeAudioSource = new GameObject("LaserChargeAudio").AddComponent<AudioSource>();
            laserChargeAudioSource.transform.parent = transform;
            laserChargeAudioSource.transform.localPosition = Vector3.forward * 2f;
            laserChargeAudioSource.spatialBlend = 0.3f;
            laserChargeAudioSource.minDistance = 15f;
            laserChargeAudioSource.maxDistance = 100f;
            laserChargeAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            laserChargeAudioSource.playOnAwake = false;
        }
        
        if (laserBeamAudioSource == null) {
            laserBeamAudioSource = new GameObject("LaserBeamAudio").AddComponent<AudioSource>();
            laserBeamAudioSource.transform.parent = transform;
            laserBeamAudioSource.transform.localPosition = Vector3.forward * 2f;
            laserBeamAudioSource.spatialBlend = 0.7f;
            laserBeamAudioSource.minDistance = 5f;
            laserBeamAudioSource.maxDistance = 200f;
            laserBeamAudioSource.rolloffMode = AudioRolloffMode.Linear;
            laserBeamAudioSource.volume = 0.8f;
            laserBeamAudioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        ui = GetComponent<UIHooks>();
        damageables = GetComponentsInChildren<Damageable>();

        foreach (Damageable damageable in damageables)
            totalHealth += damageable.currentHitPoints;

        currentHealth = totalHealth;
        ui.SetHealth(currentHealth, totalHealth);

        playerShip = Object.FindObjectOfType<CharacterController>();

        AI();
    }

    void AI()
    {
        UpdatePath();
        StartCoroutine(HandleAttacks());
    }

    // Handle different attacks based on attackType
    IEnumerator HandleAttacks()
    {
        while (true)
        {
            int attackType = Random.Range(0,3);  // 0: Path Follow, 1: Laser, 2: Spider Summon
            Debug.Log("at3: "+ attackType);
            // Based on attackType, perform the respective attack
            switch (attackType)
            {
                case 0:  // Path Follow attack a a
                    Debug.Log("Dig!");
                    cart.ResumeCartMovement();
                    cart.SetCartPosition(0);

                    yield return StartCoroutine(FollowPath());
                    break;
                case 1:  // Laser attack aa a
                    Debug.Log("Lazer!");
                    UpdatePath();

                    //test.StartPathUpdate();
                    yield return StartCoroutine(LaserAttack());
                    break; 
                case 2:  // Spider Summon attack a
                    Debug.Log("Spider!");
                    UpdatePath();

                    //test.StartPathUpdate();
                    yield return StartCoroutine(SpiderSummonAttack());
                    Debug.Log("cartf"+cart.m_Position);
                    break;
                default:
                    Debug.Log("Default!");
                    cart.ResumeCartMovement();
                    cart.SetCartPosition(0);

                    yield return StartCoroutine(FollowPath());
                    break;
            }

            yield return new WaitForSeconds(3f);
            // Wait for some time before the next attack
        }
    }

    IEnumerator LaserAttack()
    {
        Debug.Log("Performing Laser Attack");

        // Stop cart updates before the animation starts
        cart.SetCartPositionBeforeAnimation(0);

        // Trigger Laser attack animation using SetBool
        Animator animator = GetComponent<Animator>();
        animator.SetBool("lazer", true);

        // Play charge-up sound with dedicated audio source
        if (laserChargeAudioSource != null && laserChargeSound != null)
        {
            laserChargeAudioSource.clip = laserChargeSound;
            laserChargeAudioSource.Play();
        }

        // Wait for the worm to finish positioning and animating
        yield return new WaitForSeconds(2.5f); // Adjust this for worm positioning

        // Get references
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer found on the worm!");
            yield break;
        }

        // Initial laser setup
        Vector3 startLaserPos = transform.position + transform.forward * 2f; // Start slightly ahead
        lineRenderer.enabled = true;

        // Laser fire duration (same as animation, set here)
        float fireDuration = 3f;
        float elapsedTime = 0f;

        // Laser speed (controls how fast the laser moves towards the target)
        float laserSpeed = 4.5f; // Adjust the speed here for how fast the laser reaches the target

        // Get the player's Rigidbody for velocity
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Vector3 playerVelocity = playerRb != null ? playerRb.velocity : Vector3.zero;

        // Configure and play laser beam sound
        if (laserBeamAudioSource != null && laserBeamSound != null)
        {
            laserBeamAudioSource.clip = laserBeamSound;
            laserBeamAudioSource.loop = true;
            laserBeamAudioSource.Play();
        }

        // Fire the laser for 4 seconds while tracking the player
        while (elapsedTime < fireDuration)
        {
            elapsedTime += Time.deltaTime;

            // Update start position each frame to prevent desync
            startLaserPos = transform.position + transform.forward * 2f;

            // Dynamically calculate the predicted player position in the next 3 seconds based on velocity
            float timeStep = 3f; // Predict the player's position 3 seconds into the future
            Vector3 predictedPlayerPosition = player.position + playerVelocity * timeStep;

            // Smoothly move the laser's endpoint towards the predicted position
            Vector3 endLaserPos = Vector3.Lerp(lineRenderer.GetPosition(1), predictedPlayerPosition, Time.deltaTime * laserSpeed);

            // Update LineRenderer positions
            lineRenderer.SetPosition(0, startLaserPos);
            lineRenderer.SetPosition(1, endLaserPos);

            // Update laser beam sound position to follow the end of the laser
            if (laserBeamAudioSource != null)
            {
                laserBeamAudioSource.transform.position = endLaserPos;
            }

            // Check if the laser is hitting something
            RaycastHit laserHit;
            if (Physics.Raycast(startLaserPos, (endLaserPos - startLaserPos).normalized, out laserHit, 150f))
            {
                // Check specifically for the player tag
                if (laserHit.collider.CompareTag("Player"))
                {
                    Damageable playerDamageable = laserHit.collider.GetComponent<Damageable>();
                    if (playerDamageable != null)
                    {
                        // Apply damage using the Damageable system
                        playerDamageable.ApplyDamage(new Damageable.DamageMessage
                        {
                            amount = Mathf.FloorToInt(laserDamagePerTick),
                            damageSource = transform.position,
                            direction = (laserHit.point - startLaserPos).normalized,
                            damager = this,
                            throwing = false,
                            stopCamera = false
                        });

                        // Apply small knockback to simulate laser impact
                        if (playerRb != null)
                        {
                            Vector3 knockbackDirection = (laserHit.point - startLaserPos).normalized;
                            playerRb.AddForce(knockbackDirection * 1.5f, ForceMode.Impulse);
                        }
                    }
                }
            }

            yield return null;
        }

        // Stop the laser beam sound
        if (laserBeamAudioSource != null)
        {
            laserBeamAudioSource.Stop();
            laserBeamAudioSource.loop = false;
        }

        // Disable the laser after firing
        lineRenderer.enabled = false;

        // Wait for the remaining time for the animation to finish
        yield return new WaitForSeconds(fireDuration - elapsedTime); // Adjust this for worm positioning

        animator.SetBool("lazer", false);

        Debug.Log("Laser Attack Finished");
    }

    // Spider Summon attack (summon spiders around the boss and play animation)
    IEnumerator SpiderSummonAttack()
    {
        Debug.Log("Summoning Spiders");

        // Stop cart updates before the animation starts
        cart.SetCartPositionBeforeAnimation(0);

        // Start the spider summon animation
        Animator animator = GetComponent<Animator>();
        animator.SetBool("spider_summon", true);
        
        // Wait until the worm is well above ground (2 seconds into animation)
        yield return new WaitForSeconds(2f);
        
        // Get the worm's current position (now it should be above ground)
        Vector3 wormPosition = transform.position;
        
        // Spawn spiders around the worm rather than directly below
        for (int i = 0; i < 5; i++)
        {
            // Generate a random offset in a circle around the worm
            float angle = i * (360f / 5); // Evenly space spiders in a circle
            float radius = Random.Range(15f, 25f); // Random distance from worm
            
            // Calculate spawn position in a circle around the worm
            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0f, // Start at ground level
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            
            // Calculate the initial spawn location
            Vector3 spawnPosition = wormPosition + offset;
            
            // Perform a downward raycast to ensure the spider spawns on terrain
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 20f, Vector3.down, out hit, 40f, terrainLayer.value))
            {
                // Place spider 20 feet above the terrain surface
                spawnPosition = hit.point + Vector3.up * 20f;
            }
            else
            {
                Debug.LogWarning("No terrain found below the spawn position. Spider not spawned.");
                continue;
            }

            // Spawn the spider at the adjusted position
            GameObject spider = Instantiate(spiderPrefab, spawnPosition, Quaternion.identity);
            spider.tag = "Spider-Enemy";

            Debug.Log("Spider spawned at position: " + spawnPosition);
        }
        
        // Wait for the remainder of the animation
        yield return new WaitForSeconds(spider_summon_anim - 2f);

        // End the animation
        animator.SetBool("spider_summon", false);

        Debug.Log("Spider Summon Attack Finished");
    }

    // Path-following logic
IEnumerator FollowPath()
{
    Debug.Log("FollowPath: Starting path logic.");
    
    // Wait for cart to start moving
    yield return new WaitForSeconds(0.5f);
    
    // First breach point
    yield return new WaitUntil(() => cart.m_Position >= 0.06f);
    Debug.Log("FollowPath: Reached position 0.06f.");
    
    // Direct instantiation of breach effect
    if (breachEffectPrefab != null)
    {
        Instantiate(breachEffectPrefab, cart.transform.position, Quaternion.identity);
        Debug.Log("Directly instantiated breach effect");
    }
    else
    {
        Debug.LogError("breachEffectPrefab is null!");
    }
    
    // Play breach sound directly
    if (mainAudioSource != null && surfaceSound != null)
    {
        mainAudioSource.Stop();
        mainAudioSource.clip = surfaceSound;
        mainAudioSource.volume = 1.0f;
        mainAudioSource.Play();
    }
    
    GroundContact.Invoke(true, true);

    yield return new WaitUntil(() => cart.m_Position >= 0.23f);
    Debug.Log("FollowPath: Reached position 0.23f.");
    GroundContact.Invoke(false, true);

    yield return new WaitUntil(() => cart.m_Position >= 0.60f);
    Debug.Log("FollowPath: Reached position 0.60f.");
    
    // Direct instantiation of submerge effect
    if (submergeEffectPrefab != null)
    {
        Instantiate(submergeEffectPrefab, cart.transform.position, Quaternion.identity);
        Debug.Log("Directly instantiated submerge effect");
    }
    else
    {
        Debug.LogError("submergeEffectPrefab is null!");
    }
    
    // Play submerge sound directly
    if (mainAudioSource != null && submergeSound != null)
    {
        mainAudioSource.Stop();
        mainAudioSource.clip = submergeSound;
        mainAudioSource.volume = 1.0f;
        mainAudioSource.Play();
    }
    
    GroundContact.Invoke(true, false);

    yield return new WaitUntil(() => cart.m_Position >= 0.90f);
    Debug.Log("FollowPath: Reached position 0.90f.");
    GroundContact.Invoke(false, false);

    yield return new WaitUntil(() => cart.m_Position >= 0.99f);
    Debug.Log("FollowPath: Reached position 0.99f.");

    yield return new WaitForSeconds(Random.Range(1, 2));
    Debug.Log("FollowPath: Waiting before updating path.");

    UpdatePath();
    yield return new WaitUntil(() => cart.m_Position <= 0.05f);
    Debug.Log("FollowPath: Resetting to start position.");
    
    yield return new WaitForSeconds(3f);
    Debug.Log("FollowPath: Finished following the path.");
}

private void PlayBreachEffect(Vector3 position)
{
    if (breachEffectPrefab != null)
    {
        Instantiate(breachEffectPrefab, position, Quaternion.identity);
    }
    
    // Check both the audio source and clip
    if (mainAudioSource != null && surfaceSound != null)
    {
        // Stop any currently playing audio
        mainAudioSource.Stop();
        
        Debug.Log("Playing breach sound: " + surfaceSound.name);
        mainAudioSource.clip = surfaceSound;
        mainAudioSource.volume = 1.0f;
        mainAudioSource.Play();
    }
    else
    {
        // Debug which component is missing
        if (mainAudioSource == null)
            Debug.LogError("Main audio source is null!");
        if (surfaceSound == null)
            Debug.LogError("Surface sound clip is null!");
    }
}

private void PlaySubmergeEffect(Vector3 position)
{
    if (submergeEffectPrefab != null)
    {
        Instantiate(submergeEffectPrefab, position, Quaternion.identity);
    }
    
    // Check both the audio source and clip
    if (mainAudioSource != null && submergeSound != null)
    {
        // Stop any currently playing audio
        mainAudioSource.Stop();
        
        Debug.Log("Playing submerge sound: " + submergeSound.name);
        mainAudioSource.clip = submergeSound;
        mainAudioSource.volume = 1.0f;
        mainAudioSource.Play();
    }
    else
    {
        // Debug which component is missing
        if (mainAudioSource == null)
            Debug.LogError("Main audio source is null!");
        if (submergeSound == null)
            Debug.LogError("Submerge sound clip is null!");
    }
}

    // Update the path based on the player's position
void UpdatePath()
{
    Vector3 playerPosition = playerShip.transform.position;
    playerPosition.y = Mathf.Max(30, playerPosition.y); // Increased minimum height for larger worm
    Vector3 randomRange = Random.insideUnitSphere * 200; // Increased range for larger movement area
    randomRange.y = 0;
    startPosition = playerPosition + randomRange;
    endPosition = playerPosition - randomRange;

    if (Physics.Raycast(startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
    {
        startPosition = hitInfo.point;
    }

    if (Physics.Raycast(endPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
    {
        endPosition = hitInfo.point;
        GroundDetection.Invoke(false, hitInfo.transform.CompareTag("Terrain") ? 0 : 1);
    }

    // Adjust waypoints for larger worm
    path.m_Waypoints[0].position = startPosition + (Vector3.down * 30); // Deeper starting point
    path.m_Waypoints[1].position = playerPosition + (Vector3.up * 21); // Higher apex point
    path.m_Waypoints[2].position = endPosition + (Vector3.down * 105); // Much deeper ending point

    path.InvalidateDistanceCache();
    cart.m_Position = 0;
    
    // Adjust speed to account for larger path
    float pathLength = path.PathLength;
    cart.m_Speed = pathLength / 1200; // Keep same time scale, speed will adjust automatically

    OnBossReveal.Invoke(true);
}

    // Sound effects for surface sound
    void PlaySurfaceSound(Vector3 position)
    {
        GameObject audioObj = new GameObject("TempAudioSource");
        audioObj.transform.position = position;
        AudioSource tempAudioSource = audioObj.AddComponent<AudioSource>();
        
        tempAudioSource.clip = surfaceSound;
        tempAudioSource.spatialBlend = .5f;
        tempAudioSource.Play();
        
        Destroy(audioObj, surfaceSound.length);
    }

    // Sound effects for exit sound
    void PlayExitSound(Vector3 position)
    {
        GameObject audioObj = new GameObject("TempAudioSource");
        audioObj.transform.position = position;
        AudioSource tempAudioSource = audioObj.AddComponent<AudioSource>();
        
        tempAudioSource.clip = submergeSound;
        tempAudioSource.spatialBlend = .5f;
        tempAudioSource.Play();
        
        Destroy(audioObj, surfaceSound.length);
    }

    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        if (type == MessageType.DAMAGED)
        {
            Damageable damageable = sender as Damageable;
            Damageable.DamageMessage message = (Damageable.DamageMessage)msg;
            currentHealth -= message.amount;

            if (currentHealth <= 0)
            {
                // Play the particle effect from the "poopie" object
                GameObject poopieObject = GameObject.Find("poopie");
                if (poopieObject != null)
                {
                    ParticleSystem poopieEffect = poopieObject.GetComponent<ParticleSystem>();
                    if (poopieEffect != null)
                    {
                        poopieEffect.Play();
                        Debug.Log("Worm death particle effect played from 'poopie'.");
                    }
                    else
                    {
                        Debug.LogWarning("'poopie' object does not have a ParticleSystem component.");
                    }
                }
                else
                {
                    Debug.LogWarning("Object named 'poopie' not found in the scene.");
                }

                ui.ReloadScene();
            }

            ui.SetHealth(currentHealth, totalHealth);
        }
    }
}