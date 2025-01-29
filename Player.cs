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

    public GameObject bulletPrefab;  // Bullet prefab to instantiate
    public Transform muzzlePos;      // Position from which the bullet is fired
    public float shootingForce = 200f;  // Force applied to the bullet

    // Firing rate properties
    private float lastFiredTime = 0f;  // Time when the last shot was fired
    public float fireRate = 0.5f;      // Fire rate: 2 bullets per second (0.5s per bullet)

    // Other
    public bool drinksWater = false;

    private void Start() {
        // Initialize PlayerData
        PlayerData = (0f, 0f);

        // Lock cursor to the center of the screen for FPS view
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        // Handle Camera Movement (Look)
        HandleCameraLook();

        // Handle Player Movement (WASD)
        HandleMovement();

        // Handle shooting when Mouse1 is pressed
        if (Input.GetMouseButton(0)) {  // Check if the player is holding down Mouse1
            if (AnimationController.isUsingGun && !AnimationController.isReloading && AnimationController.isShooting) {
                HandleFire();
            }
        }
    }

    private void HandleCameraLook() {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player horizontally (Y-axis)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (X-axis), with clamping for vertical rotation limits
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalRotationLimit, verticalRotationLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleMovement() {
        // Get movement input
        moveForwardBackward = Input.GetAxis("Vertical") * movementSpeed;
        moveLeftRight = Input.GetAxis("Horizontal") * movementSpeed;

        // Calculate movement direction relative to the camera's forward vector
        Vector3 moveDirection = (transform.forward * moveForwardBackward) + (transform.right * moveLeftRight);

        // Move the player
        transform.Translate(moveDirection * Time.deltaTime, Space.World);
    }

    // Handle firing bullets based on time interval
    private void HandleFire() {
        if (Time.time - lastFiredTime >= fireRate) {
            Shoot();  // Fire the weapon
            lastFiredTime = Time.time;  // Record the time of the shot
        }
    }

    // Shooting function
// Shooting function
private void Shoot() {
    if (bulletPrefab != null && muzzlePos != null) {
        // Instantiate the bullet at the muzzle position with rotation
        GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, muzzlePos.rotation);

        // Get the Rigidbody component of the bullet
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        
        if (bulletRb != null) {
            // Apply force to the bullet in the direction the muzzle is facing
            bulletRb.AddForce(muzzlePos.forward * shootingForce, ForceMode.VelocityChange);
        }

        // Destroy the bullet after a few seconds to clean up
        Destroy(bullet, 5f);  // Bullet lifespan (5 seconds)

    }
}


    // Example method to update damage
    public void UpdateDamage(float meleeDamage, float gunDamage) {
        PlayerData = (PlayerData.meleeDamage + meleeDamage, PlayerData.gunDamage + gunDamage);
    }

    public string HitType {
        get {
            // Check if melee damage is greater than gun damage
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
