using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private GameObject particleEffectPrefab; // Reference to particle effect prefab

    // Called when the bullet collides with something
    private void OnCollisionEnter(Collision collision) {
        // Get the contact point of the collision
        ContactPoint contact = collision.contacts[0];
        Vector3 position = contact.point;
        
        // Get the normal of the surface that was hit
        Quaternion rotation = Quaternion.LookRotation(contact.normal);
        
        // Instantiate and play the particle system at the collision point
        if (particleEffectPrefab != null) {
            GameObject effectObject = Instantiate(particleEffectPrefab, position, rotation);
            ParticleSystem particleSystem = effectObject.GetComponent<ParticleSystem>();
            
            if (particleSystem != null) {
                // Make sure it's playing
                particleSystem.Play();
                
                // Destroy the particle effect after it finishes playing
                float duration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
                Destroy(effectObject, duration);
            } else {
                // If there's no particle system, destroy after a default time
                Destroy(effectObject, 2f);
            }
        }
        
        // Destroy the bullet on impact
        Destroy(gameObject);
    }
}