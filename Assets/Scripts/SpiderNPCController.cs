﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SpiderNPCController : MonoBehaviour
{
    [Header("Debug")]
    public bool showDebug;

    [Header("Spider Reference")]
    public Spider spider;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 15f;
    public float hearingRange = 20f;

    [Header("Attacks")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 20f;
    public float predictionInaccuracy = 1.5f;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float circleSpeed = 2f;
    public float circleRadius = 5f;
    public float leechSpeedMultiplier = 10f;
    public float leechDamagePerSecond = 5f;

    [Header("Base Stats")]
    public float baseFireRate = 2f;
    public int baseDamage = 10;
    public float baseSpeed = 3f;
    private float moveSpeed;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpCooldown = 1f;
    private bool canJump = true;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip walkingSound;
    [SerializeField] private AudioClip leechAttackSound;
    [SerializeField] private AudioClip projectileAttackSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioSource audioSource;
    private float walkingSoundInterval = 0.3f;
    private float lastWalkSoundTime;

    private bool isLeeching = false;
    private bool isAttacking = false;
    private bool prefersLeechAttack = false;
    private int totalPlayerDamage = 0;
    private bool isWalking = false;

    [Header("Leech Attack Settings")]
    public float leechApproachDistance = 11f;
    public float rangedPreferredDistance = 15f;
    public float chaseDurationAfterJump = 5f;
    public float damageInterval = 1f;

    [Header("Death Settings")]
    public GameObject ragdollPrefab;

    private void Awake()
    {
        projectilePrefab = GameObject.Find("Spit-Ball");
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("Player not found. Make sure the player is tagged with 'Player'.");

        prefersLeechAttack = Random.value < 0.5f;
        
        // Create audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Full 3D sound
            audioSource.minDistance = 2f;
            audioSource.maxDistance = 20f;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (!isAttacking)
        {
            if (prefersLeechAttack)
                HandleLeechingBehavior(distanceToPlayer, directionToPlayer);
            else
                HandleRangedBehavior(distanceToPlayer, directionToPlayer);
        }
    }

    private void Update()
    {
        // Handle walking sound
        if (isWalking && Time.time > lastWalkSoundTime + walkingSoundInterval)
        {
            PlaySound(walkingSound, 0.5f);
            lastWalkSoundTime = Time.time;
        }
    }

    private void HandleLeechingBehavior(float distanceToPlayer, Vector3 directionToPlayer)
    {
        if (!isLeeching)
        {
            if (distanceToPlayer > leechApproachDistance && canJump)
            {
                JumpTowardsPlayer(player.position);
            }
            else
            {
                spider.walk(directionToPlayer);
                spider.turn(directionToPlayer);
                isWalking = true;
            }
        }
    }

    private void HandleRangedBehavior(float distanceToPlayer, Vector3 directionToPlayer)
    {
        if (!isAttacking)
        {
            if (distanceToPlayer > rangedPreferredDistance)
            {
                spider.walk(directionToPlayer);
                isWalking = true;
            }
            else if (distanceToPlayer < rangedPreferredDistance - 2f)
            {
                spider.walk(-directionToPlayer);
                isWalking = true;
            }
            else
            {
                isWalking = false;
                StartCoroutine(PerformProjectileAttack());
            }
            spider.turn(directionToPlayer);
        }
    }

    private void JumpTowardsPlayer(Vector3 targetPosition)
    {
        if (!canJump) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Play jump sound
            PlaySound(jumpSound, 0.7f);
            
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 jumpForceVector = direction * jumpForce;
            jumpForceVector.y = jumpForce;
            rb.AddForce(jumpForceVector, ForceMode.Impulse);
            canJump = false;
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void ApplyLeechDamage()
    {
        // Play leech attack sound when damage is applied
        PlaySound(leechAttackSound, 1.0f);
        
        totalPlayerDamage += (int)leechDamagePerSecond;
        Debug.Log($"Leeching Player! Total damage: {totalPlayerDamage}");
    }

    private IEnumerator PerformLeechAttack()
    {
        isLeeching = true;
        isAttacking = true;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > leechApproachDistance && canJump)
        {
            JumpTowardsPlayer(player.position);
            yield return new WaitForSeconds(jumpCooldown + 0.5f);
        }

        float chaseTimer = 0f;
        while (chaseTimer < chaseDurationAfterJump && Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            spider.walk(directionToPlayer);
            spider.turn(directionToPlayer);
            isWalking = true;

            if (Vector3.Distance(transform.position, player.position) < 1f)
            {
                ApplyLeechDamage();
                yield return new WaitForSeconds(damageInterval);
            }
            else
            {
                yield return null;
            }
            chaseTimer += Time.deltaTime;
        }

        isLeeching = false;
        isAttacking = false;
    }

    private IEnumerator PerformProjectileAttack()
    {
        isAttacking = true;
        isWalking = false;

        yield return new WaitForSeconds(attackCooldown * 0.5f);

        if (player == null)
        {
            isAttacking = false;
            yield break;
        }

        if (projectileSpawnPoint == null)
        {
            Debug.LogError("Projectile spawn point is not assigned!");
            isAttacking = false;
            yield break;
        }

        // Play projectile attack sound before launching
        PlaySound(projectileAttackSound, 0.8f);
        
        // Small delay after sound before actually launching projectile
        yield return new WaitForSeconds(0.2f);

        Vector3 predictedPos = PredictPlayerPosition();
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = (predictedPos - projectileSpawnPoint.position).normalized * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody!");
        }

        Destroy(projectile, 5f);

        yield return new WaitForSeconds(attackCooldown * 0.5f);

        PickNewRangedPosition();
        isAttacking = false;
    }

    private void PickNewRangedPosition()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-circleRadius, circleRadius),
            0,
            Random.Range(-circleRadius, circleRadius)
        );

        Vector3 targetPosition = player.position + randomOffset;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        spider.walk(moveDirection);
        spider.turn(moveDirection);
        isWalking = true;
    }

    private Vector3 PredictPlayerPosition()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null) return player.position;

        Vector3 predictedPos = player.position + (playerRb.linearVelocity * (Vector3.Distance(transform.position, player.position) / projectileSpeed));
        predictedPos += new Vector3(
            Random.Range(-predictionInaccuracy, predictionInaccuracy),
            Random.Range(-predictionInaccuracy, predictionInaccuracy),
            Random.Range(-predictionInaccuracy, predictionInaccuracy)
        );
        return predictedPos;
    }
    
    // Helper method to play sounds with volume control
    private void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null || audioSource == null) return;
        
        audioSource.pitch = Random.Range(0.9f, 1.1f); // Add slight pitch variation
        audioSource.PlayOneShot(clip, volume);
    }

    public void Die()
    {
        if (ragdollPrefab != null)
        {
            GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            ragdoll.transform.localScale = transform.localScale; // Match the scale of the spider

            Rigidbody spiderRb = GetComponent<Rigidbody>();
            if (spiderRb != null)
            {
                Rigidbody ragdollRb = ragdoll.GetComponent<Rigidbody>();
                if (ragdollRb != null)
                {
                    ragdollRb.linearVelocity = spiderRb.linearVelocity;
                    ragdollRb.angularVelocity = spiderRb.angularVelocity;
                }
            }
        }
        else
        {
            Debug.LogError("Ragdoll prefab is not assigned!");
        }

        Destroy(gameObject);
    }
}