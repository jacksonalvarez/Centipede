using System.Collections;
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

    // New state variables
    private bool isTrackingPath = true;  // Whether we are following the path or attacking
    private bool isAttacking = false;    // Whether an attack is in progress

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

        // Coroutine to handle alternating between normal attack and path-following
        IEnumerator HandleAttacks()
        {
            while (true)
            {
                if (isTrackingPath)
                {
                    // Follow Path if the state is set to true
                    yield return StartCoroutine(FollowPath());
                }
                else
                {
                    // Normal Attack if the state is set to false
                    yield return StartCoroutine(NormalAttack());
                }
                
                // Toggle between following the path and attacking
                isTrackingPath = !isTrackingPath;

                // Wait for a moment before toggling again (adjust this time as needed)
                yield return new WaitForSeconds(1f);
            }
        }
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

        Debug.Log("Finished following the path.");
    }

    // Normal attack logic
    IEnumerator NormalAttack()
    {
        Debug.Log("Performing Normal Attack");

        // Your normal attack logic here
        // This is a simple placeholder for normal attack, you can include animations, damage dealing, etc.
        yield return new WaitForSeconds(1f); // Simulating attack duration

        // After attack finishes
        Debug.Log("Normal Attack Finished");
    }

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
        cart.m_Speed = cart.m_Path.PathLength / 700;

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
