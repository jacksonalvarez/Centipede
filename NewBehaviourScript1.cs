using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator weaponAnimator; // Unified Animator for both weapons
    public GameObject gun; // Reference to the Gun GameObject
    public GameObject sword; // Reference to the Sword GameObject
    public GameObject knife; // Reference to the Knife GameObject

    private float lastClickTime = -Mathf.Infinity; // Store the last time a click was registered
    public float test = 0.1f; // Threshold to detect clicks within the last 0.1 seconds
    public bool isClicking = false; // Boolean to track if the player is clicking in the last 0.1 seconds

    public bool isUsingGun = true; // Start with the gun equipped
    public bool isShooting = false;
    public bool isReloading = false;

    private bool isSwordSwinging = false; // Track whether sword swing animation is active

    void Start()
    {
        weaponAnimator.SetBool("GunOut", isUsingGun);
        UpdateWeaponVisibility(); // Initialize the correct weapon visibility at the start
    }

    void Update()
    {
        HandleWeaponSwitch();

        if (isUsingGun)
        {
            HandleGunInput();
        }
        else
        {
            HandleSwordInput();
        }

        UpdateAnimationStates();
    }

    private void HandleWeaponSwitch()
    {
        // Press 'Q' to switch between gun and sword
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isUsingGun = !isUsingGun;
            weaponAnimator.SetBool("GunOut", isUsingGun);
            ResetStates();
            StartCoroutine(UpdateWeaponVisibilityWithDelay()); // Delay the visibility switch
            Debug.Log("Switched to " + (isUsingGun ? "Gun" : "Sword"));
        }
    }

    private void HandleGunInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReloading = true;
            isShooting = false;
            Debug.Log("Reloading...");
        }
        else if (Input.GetMouseButton(0))
        {
            isShooting = true;
            isReloading = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
    }

    private void HandleSwordInput()
    {
        // Detect if the player clicked the left mouse button (Mouse Button 0)
        if (Input.GetMouseButtonDown(0)) // Left Click to Swing Sword
        {
            weaponAnimator.SetBool("SwordSwing", true);
        }
        else if (Input.GetMouseButtonUp(0)) // When Left Mouse Button is released, stop swinging
        {
            weaponAnimator.SetBool("SwordSwing", false);
        }

        // Right Click to Parry
        if (Input.GetMouseButtonDown(1)) // Right Click to Parry
        {
            weaponAnimator.SetTrigger("Parry");
            Debug.Log("Parry triggered.");
        }

        // 'E' Key to Grab
        if (Input.GetKeyDown(KeyCode.E)) // 'E' Key to Grab
        {
            weaponAnimator.SetTrigger("Grab");
            Debug.Log("Grab triggered.");
        }
    }

    private IEnumerator UpdateWeaponVisibilityWithDelay()
    {
        yield return new WaitForSeconds(test); // Wait for 0.3 seconds before updating visibility
        gun.SetActive(isUsingGun);
        sword.SetActive(!isUsingGun); // If not using gun, enable sword
        knife.SetActive(!isUsingGun); // If not using gun, enable knife
        yield return new WaitForSeconds(test-.8f); // Wait for 0.3 seconds before updating visibility
        UpdateWeaponVisibility(); // Update the weapon visibility after the delay
    }

    private void UpdateWeaponVisibility()
    {
        // Enable/Disable the weapon GameObjects based on isUsingGun
        gun.SetActive(isUsingGun);
        sword.SetActive(!isUsingGun); // If not using gun, enable sword
        knife.SetActive(!isUsingGun); // If not using gun, enable knife
    }

    private void UpdateAnimationStates()
    {
        weaponAnimator.SetBool("isReloading", isReloading);
        weaponAnimator.SetBool("isFiring", isShooting);
        weaponAnimator.SetBool("Idle", !isReloading && !isShooting);
    }

    private void ResetStates()
    {
        isShooting = false;
        isReloading = false;
    }
}
