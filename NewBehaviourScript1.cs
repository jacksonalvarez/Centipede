using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour{
    public Animator gunAnimator; // Reference to the Animator component
    public bool isReloading = false;
    public bool gunUp = true; // Always true as per your requirement
    public bool isShooting = false;

    void Update()
    {
        HandleInput();
        UpdateAnimationStates();
    }

    private void HandleInput()
    {
        // Reload when pressing 'R'
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReloading = true;
            isShooting = false;
        }
        // Hold Mouse 1 to shoot continuously
        else if (Input.GetMouseButton(0))
        {
            isShooting = true;
            isReloading = false;
        }
        // Release Mouse 1 to stop shooting
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
        // If no input is detected, set to idle
        else if (!Input.anyKey)
        {
            isReloading = false;
            isShooting = false;
        }
    }

    private void UpdateAnimationStates()
    {
        // Update Animator parameters based on state
        gunAnimator.SetBool("isReloading", isReloading);
        gunAnimator.SetBool("isFiring", isShooting);

        // Always set GunUp to true
        gunAnimator.SetBool("GunUp", gunUp);

        // Idle is the fallback state when neither shooting nor reloading
        bool isIdle = !isReloading && !isShooting;
        gunAnimator.SetBool("Idle", isIdle);
    }
}
