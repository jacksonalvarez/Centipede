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
    [SerializeField] AudioSource audioSource = default;  // AudioSource for primary worm sound
    [SerializeField] AudioClip surfaceSound = default;
    [SerializeField] AudioClip bossSound = default;
    [SerializeField] AudioClip submergeSound = default;

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
        float fireDuration = 4f;
        float elapsedTime = 0f;

        // Laser speed (controls how fast the laser moves towards the target)
        float laserSpeed = 5f; // Adjust the speed here for how fast the laser reaches the target

        // Get the player's Rigidbody for velocity
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Vector3 playerVelocity = playerRb != null ? playerRb.linearVelocity : Vector3.zero;

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

            // Check if the laser is hitting something
            RaycastHit laserHit;
            if (Physics.Raycast(startLaserPos, (endLaserPos - startLaserPos).normalized, out laserHit, 50f))
            {
                Damageable target = laserHit.collider.GetComponent<Damageable>();
                if (target != null)
                {
                    //target.TakeDamage(5); // Apply small damage over time
                }
            }

            yield return null;
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

    // Get the player's position
    Animator animator = GetComponent<Animator>();
    animator.SetBool("spider_summon", true);
        yield return new WaitForSeconds(2f);
    Vector3 wormPosition = cart.transform.position;

    for (int i = 0; i < 5; i++)
    {
        // Generate a random offset for each spider
        Vector3 randomOffset = new Vector3(0, 15, 0);

        // Calculate the initial spawn location
        Vector3 startPosition = wormPosition + randomOffset;

        // Perform a downward raycast to ensure the spider spawns on terrain
        RaycastHit hitInfo;
        if (Physics.Raycast(startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            startPosition = hitInfo.point;
        }

        // Spawn the spider at the adjusted position
        GameObject spider = Instantiate(spiderPrefab, startPosition, Quaternion.identity);
        spider.tag = "Spider-Enemy";

        Debug.Log("Spider spawned at position: " + startPosition);
    }
    yield return new WaitForSeconds(spider_summon_anim-2f);

    animator.SetBool("spider_summon", false);

    // Get the worm's current position

    // Spawn 5 spiders at random positions around the worm

    Debug.Log("Spider Summon Attack Finished");
}





    // Path-following logic
    IEnumerator FollowPath()
    {
        while (cart.m_Position > 0.05f)
        {
            yield return new WaitUntil(() => cart.m_Position >= 0.06f);
            Debug.Log("TEST");

            GroundContact.Invoke(true, true);
            audioSource.PlayOneShot(bossSound); // Play main surface sound
            PlaySurfaceSound(cart.transform.position); // Temp method for additional sound

            yield return new WaitUntil(() => cart.m_Position >= 0.23f);
            Debug.Log("TEST2");

            GroundContact.Invoke(false, true);
            yield return new WaitUntil(() => cart.m_Position >= 0.60f);
            GroundContact.Invoke(true, false);
            PlayExitSound(cart.transform.position);

            yield return new WaitUntil(() => cart.m_Position >= 0.90f);
            GroundContact.Invoke(false, false);
            yield return new WaitUntil(() => cart.m_Position >= 0.99f);

            yield return new WaitForSeconds(Random.Range(1, 2));

            UpdatePath();
            yield return new WaitUntil(() => cart.m_Position <= 0.05f);
        }
            yield return new WaitForSeconds(3f);

        Debug.Log("Finished following the path.");
    }

    // Update the path based on the player's position
    // Update the path based on the player's position
  void UpdatePath()
    {
        Vector3 playerPosition = playerShip.transform.position;
        playerPosition.y = Mathf.Max(10, playerPosition.y);
        Vector3 randomRange = Random.insideUnitSphere * 100;
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

        path.m_Waypoints[0].position = startPosition + (Vector3.down * 10);
        path.m_Waypoints[1].position = playerPosition + (Vector3.up * 7);
        path.m_Waypoints[2].position = endPosition + (Vector3.down * 35);

        path.InvalidateDistanceCache();
        cart.m_Position = 0;
        cart.m_Speed = cart.m_Path.PathLength / 450;

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
                ui.ReloadScene();

            ui.SetHealth(currentHealth, totalHealth);
        }
    }
}