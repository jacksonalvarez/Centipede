using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject explosionEffectPrefab; // Assign this in the inspector

    [Header("Grouping")]
    public float groupRadius = 5f; // Radius within which spiders form a group
    public float groupSpacing = 2f; // Minimum spacing between spiders in a group

    private static List<SpiderNPCController> allSpiders = new List<SpiderNPCController>();

    private float lastAttackTime = 0f; // Track the last time the spider attacked

    private void Awake()
    {
        // Load the projectile prefab from the Resources folder or assign it in the inspector
        if (projectilePrefab == null)
        {
            projectilePrefab = Resources.Load<GameObject>("Spit-Ball");
            if (projectilePrefab != null)
            {
                Debug.Log("Spit-Ball prefab loaded from Resources.");
            }
            else
            {
                Debug.LogError("Spit-Ball prefab not found in Resources! Ensure a prefab named 'Spit-Ball' exists in a Resources folder.");
            }
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogError("Player not found. Make sure the player is tagged with 'Player'.");

        prefersLeechAttack = Random.value < 0.5f;

        // Create and configure audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Full 3D sound
            audioSource.rolloffMode = AudioRolloffMode.Linear; // Linear falloff for spatial audio
            audioSource.minDistance = 5f; // Walking sounds can be heard within 5 meters
            audioSource.maxDistance = 8f; // Screaming sounds can be heard within 8 meters
        }

        // Add this spider to the global list
        allSpiders.Add(this);
    }

    private void OnDestroy()
    {
        // Remove this spider from the global list when destroyed
        allSpiders.Remove(this);
    }

    private void Start()
    {
        // Ensure the spider has a non-trigger collider and rigidbody for physics interactions
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"{gameObject.name} does not have a Collider. Add one to interact with the floor.");
        }
        else if (col.isTrigger)
        {
            Debug.LogWarning($"{gameObject.name} Collider is set as a trigger. Disabling trigger to interact with the floor.");
            col.isTrigger = false; // Ensure the main collider is not a trigger
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name} does not have a Rigidbody. Adding one for physics interactions.");
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false; // Ensure the Rigidbody is not kinematic
        rb.useGravity = true;   // Enable gravity for the spider

        // Ensure the child GameObject has a trigger collider for player detection
        Transform triggerChild = transform.Find("PlayerTrigger");
        if (triggerChild == null)
        {
            Debug.LogWarning("PlayerTrigger child not found. Creating one.");
            GameObject triggerObject = new GameObject("PlayerTrigger");
            triggerObject.transform.SetParent(transform);
            triggerObject.transform.localPosition = Vector3.zero;
            triggerObject.transform.localRotation = Quaternion.identity;

            SphereCollider triggerCollider = triggerObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = 1.5f; // Adjust radius as needed

            triggerObject.AddComponent<SpiderPlayerTrigger>().Initialize(this);
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        // Grouping logic
        FormGroup();

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
        // Rotate toward the player
        spider.turn(directionToPlayer);

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

            // Check if enough time has passed since the last attack
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Debug.Log("Performing projectile attack...");
                StartCoroutine(PerformProjectileAttack());
                lastAttackTime = Time.time; // Update the last attack time
            }
            else
            {
                Debug.Log("Attack on cooldown. Time remaining: " + (lastAttackTime + attackCooldown - Time.time));
            }
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

    public void OnPlayerHit(Damageable player)
    {
        Debug.Log("Leeching attack hit the player!");
        player.ApplyDamage(new Damageable.DamageMessage
        {
            amount = Mathf.FloorToInt(leechDamagePerSecond * damageInterval),
            damageSource = transform.position,
            direction = (player.transform.position - transform.position).normalized,
            damager = this,
            throwing = false,
            stopCamera = false
        });

        // Optional: Play leech attack sound
        PlaySound(leechAttackSound, 1.0f);
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

            yield return new WaitForSeconds(damageInterval);
            chaseTimer += damageInterval;
        }

        isLeeching = false;
        isAttacking = false;
    }

    private IEnumerator PerformProjectileAttack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned!");
            yield break;
        }

        if (projectileSpawnPoint == null)
        {
            Debug.LogError("Projectile spawn point is not assigned!");
            yield break;
        }

        Debug.Log("Spawning projectile...");
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Debug.Log($"Projectile spawned at {projectileSpawnPoint.position}");

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - projectileSpawnPoint.position).normalized;
            rb.velocity = direction * projectileSpeed;
            Debug.Log($"Projectile launched toward {player.position} with velocity {rb.velocity}");
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody!");
        }

        Destroy(projectile, 5f);
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

        Vector3 predictedPos = player.position + (playerRb.velocity * (Vector3.Distance(transform.position, player.position) / projectileSpeed));
        predictedPos += new Vector3(
            Random.Range(-predictionInaccuracy, predictionInaccuracy),
            Random.Range(-predictionInaccuracy, predictionInaccuracy),
            Random.Range(-predictionInaccuracy, predictionInaccuracy)
        );
        return predictedPos;
    }

    private void FormGroup()
    {
        // Find nearby spiders of the same kind
        var nearbySpiders = allSpiders
            .Where(spider => spider != this && Vector3.Distance(spider.transform.position, transform.position) <= groupRadius)
            .ToList();

        if (nearbySpiders.Count > 0)
        {
            // Calculate the group's center
            Vector3 groupCenter = nearbySpiders
                .Select(spider => spider.transform.position)
                .Aggregate(transform.position, (current, position) => current + position) / (nearbySpiders.Count + 1);

            // Adjust position to maintain spacing within the group
            Vector3 offset = (transform.position - groupCenter).normalized * groupSpacing;
            Vector3 targetPosition = groupCenter + offset;

            // Move toward the target position
            Vector3 direction = (targetPosition - transform.position).normalized;
            spider.walk(direction);
            spider.turn(direction);
            isWalking = true;
        }
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
        Debug.Log($"{gameObject.name} is dying.");

        // Grant XP to the player
        XPAndUpgradeSystem xpSystem = FindObjectOfType<XPAndUpgradeSystem>();
        if (xpSystem != null)
        {
            Debug.Log("Granting XP for killing spider.");
            xpSystem.AddXPFromSpiderKill(50); // Example: Grant 50 XP for killing a spider
        }
        else
        {
            Debug.LogWarning("XPAndUpgradeSystem not found.");
        }

        // Instantiate the explosion effect at the spider's position
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Explosion effect prefab is not assigned!");
        }

        // Destroy the spider GameObject
        Destroy(gameObject);
    }
}