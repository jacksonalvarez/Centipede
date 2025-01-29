using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Called when the bullet collides with something
    private void OnCollisionEnter(Collision collision) {
        // Destroy the bullet on impact
        Destroy(gameObject);
    }
}
