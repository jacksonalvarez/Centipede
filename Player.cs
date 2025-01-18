using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour{
    //public List<Equippable> Equippables;
    //public PlayerUIController UIController;
    public PlayerAnimationController AnimationController;

    public (float meleeDamage, float gunDamage) PlayerData;

    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float movementSpeed = 5f;

    // Rotation limits for the camera (vertical look)
    private float verticalLookRotation = 0f;
    public float verticalRotationLimit = 90f;

    // Movement input
    private float moveForwardBackward;
    private float moveLeftRight;

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

    // Example method to update damage
    public void UpdateDamage(float meleeDamage, float gunDamage) {
        PlayerData = (PlayerData.meleeDamage + meleeDamage, PlayerData.gunDamage + gunDamage);
    }

    public string HitType
    {
        get
        {
            // Check if melee damage is greater than gun damage
            if (PlayerData.meleeDamage > PlayerData.gunDamage)
            {
                return "Prefers being Close";
            }
            else
            {
                return "Prefers at a range";
            }
        }
    }
    
    public void StartIntroCutscene(){
        
    }
}