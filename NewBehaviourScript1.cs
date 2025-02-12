using System.Collections;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator weaponAnimator; // Unified Animator for both weapons
    public Animator gunAnimator; // Animator specifically for the gun
    public GameObject gun; // Reference to the Gun GameObject
    public GameObject sword; // Reference to the Sword GameObject
    public GameObject knife; // Reference to the Knife GameObject
    public GameObject gunMag; // Reference to the magazine GameObject

    private float lastClickTime = -Mathf.Infinity; // Store the last time a click was registered
    public float test = 0.1f; // Threshold to detect clicks within the last 0.1 seconds
    public bool isClicking = false; // Boolean to track if the player is clicking in the last 0.1 seconds
    public float testAnim = 0.1f; // Threshold to detect clicks within the last 0.1 seconds

    public bool isUsingGun = true; // Start with the gun equipped
    public bool isShooting = false;
    public bool isReloading = false;
    public Player playerController;


    void Start()
    {
        weaponAnimator.SetBool("GunOut", isUsingGun);
        gunAnimator.SetBool("GunOut", isUsingGun);

        UpdateWeaponVisibility(); // Initialize the correct weapon visibility at the start
    }

    void Update()
    {
        HandleWeaponSwitch();

        if (playerController.bullets <= 0) {
            isShooting = false;
        }

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
            gunAnimator.SetBool("GunOut", isUsingGun);

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
            StartCoroutine(ReloadGun());
            Debug.Log("Reloading...");
        }
        else if (Input.GetMouseButton(0) && playerController.bullets > 0)
        {
            isShooting = true;
            isReloading = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
        /* else if(Input.GetMouseButton(0) && playerController.bullets <= 0 && playerController.gunAudioSource.isPlaying == false){
            playerController.gunAudioSource.PlayOneShot(playerController.emptyGun);
        } */
        else if(Input.GetMouseButton(0) && playerController.bullets <= 0 && playerController.gunAudioSource.isPlaying == false)
        {
            PlayEmptyGunSound();
        } 
    }

    private void PlayEmptyGunSound()
    {
        
        if (Input.GetMouseButton(0) && playerController.bullets <= 0 && !playerController.gunAudioSource.isPlaying)
        {
            playerController.gunAudioSource.PlayOneShot(playerController.emptyGun);
        }
    }


    /* private IEnumerator PlayEmptyGunSound()
    {
        playerController.gunAudioSource.PlayOneShot(playerController.emptyGun);
        yield return new WaitForSeconds(1f);
    } */

    private IEnumerator ReloadGun()
    {
        isReloading = true;
        isShooting = false;
        weaponAnimator.SetBool("isReloading", true);
        gunAnimator.SetBool("isReloading", true);
        gunMag.SetActive(true); // Disable the magazine

        yield return new WaitForSeconds(1f); // Wait for 1 second


        isReloading = false;
        weaponAnimator.SetBool("isReloading", false);

        gunAnimator.SetBool("isReloading", false);
        yield return new WaitForSeconds(testAnim); // Wait for 1 second
        gunMag.SetActive(false); // Re-enable the magazine

    }

    private void HandleSwordInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click to Swing Sword
        {
            weaponAnimator.SetBool("SwordSwing", true);
        }
        else if (Input.GetMouseButtonUp(0)) // When Left Mouse Button is released, stop swinging
        {
            weaponAnimator.SetBool("SwordSwing", false);
        }

        if (Input.GetMouseButtonDown(1)) // Right Click to Parry
        {
            weaponAnimator.SetTrigger("Parry");
            Debug.Log("Parry triggered.");
        }

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
        sword.SetActive(!isUsingGun);
        knife.SetActive(!isUsingGun);
        yield return new WaitForSeconds(test - 0.8f);
        UpdateWeaponVisibility();
    }

    private void UpdateWeaponVisibility()
    {
        gun.SetActive(isUsingGun);
        sword.SetActive(!isUsingGun);
        knife.SetActive(!isUsingGun);
    }

    private void UpdateAnimationStates()
    {
        weaponAnimator.SetBool("isReloading", isReloading);
        weaponAnimator.SetBool("isFiring", isShooting);
        weaponAnimator.SetBool("Idle", !isReloading && !isShooting);

        gunAnimator.SetBool("isReloading", isReloading);
        gunAnimator.SetBool("isFiring", isShooting);
        gunAnimator.SetBool("Idle", !isReloading && !isShooting);
    }

    private void ResetStates()
    {
        isShooting = false;
        isReloading = false;
    }
}
