using UnityEngine;

public class GunFiringScript : MonoBehaviour
{
    public bool isFiring = false; // Ensure this is false by default
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            Debug.Log($"Gun firing triggered on {gameObject.name}");
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    public void Fire()
    {
        Debug.Log($"Firing gun: {gameObject.name}");
        // Add firing logic here
    }

    public void StartFiring()
    {
        Debug.Log($"Start firing called on {gameObject.name}");
        isFiring = true;
    }

    public void StopFiring()
    {
        Debug.Log($"Stop firing called on {gameObject.name}");
        isFiring = false;
    }
}
