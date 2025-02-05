using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Player stats
    public PlayerAnimationController AnimationController;
    public (float meleeDamage, float gunDamage) PlayerData;

    // Camera and movement
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float movementSpeed = 5f;

    // Rotation limits for the camera (vertical look)
    private float verticalLookRotation = 0f;
    public float verticalRotationLimit = 90f;

    // Movement input
    private float moveForwardBackward;
    private float moveLeftRight;

    // Shooting properties
    public GameObject firePrefab;
    public GameObject bulletPrefab;
    public Transform muzzlePos;
    public float shootingForce = 200f;

    // Firing rate properties
    private float lastFiredTime = 0f;
    public float fireRate = 0.5f;
    public Rigidbody rb;

    // Audio
    public AudioSource gunAudioSource;
    public AudioClip gunShotClip;

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    // Other
    public bool drinksWater = false;

    private void Start() {
        // Initialize PlayerData
        PlayerData = (0f, 0f);

        // Lock cursor to the center of the screen for FPS view
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure AudioSource is attached
        if (gunAudioSource == null) {
            gunAudioSource = gameObject.AddComponent<AudioSource>();
        }

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update() {
        // Handle Camera Movement (Look)
        HandleCameraLook();

        // Handle Player Movement (WASD)
        HandleMovement();

        // Handle shooting when Mouse1 is pressed
        if (Input.GetMouseButton(0)) {
            if (AnimationController.isUsingGun && !AnimationController.isReloading && AnimationController.isShooting) {
                HandleFire();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    private void HandleCameraLook() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalRotationLimit, verticalRotationLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleMovement() {
        moveForwardBackward = Input.GetAxis("Vertical") * movementSpeed;
        moveLeftRight = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 moveDirection = (transform.forward * moveForwardBackward) + (transform.right * moveLeftRight);
        transform.Translate(moveDirection * Time.deltaTime, Space.World);
    }

    private void HandleFire() {
        if (Time.time - lastFiredTime >= fireRate) {
            Shoot();
            lastFiredTime = Time.time;
        }
    }

    private void Shoot() {
        if (bulletPrefab != null && muzzlePos != null) {
            GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, muzzlePos.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            
            if (bulletRb != null) {
                bulletRb.AddForce(muzzlePos.forward * shootingForce, ForceMode.VelocityChange);
            }

            Destroy(bullet, 5f);
        }
        
        // Play gunshot sound
        if (gunAudioSource != null && gunShotClip != null) {
            gunAudioSource.PlayOneShot(gunShotClip);
        }
    }

    public void UpdateDamage(float meleeDamage, float gunDamage) {
        PlayerData = (PlayerData.meleeDamage + meleeDamage, PlayerData.gunDamage + gunDamage);
    }

    public string HitType {
        get {
            if (PlayerData.meleeDamage > PlayerData.gunDamage) {
                return "Prefers being Close";
            } else {
                return "Prefers at a range";
            }
        }
    }

    public void StartIntroCutscene() {
        // Placeholder for intro cutscene logic
    }
}
