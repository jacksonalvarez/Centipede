using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateAttacks : MonoBehaviour
{

        public CharacterController playerShip;

    // Start is called before the first frame update
    public void StartPathUpdate()
    {
        Vector3 playerPosition = playerShip.transform.position;

        // Get random offsets for x, y, and z positions
        float randomX = Random.Range(10f, 15f) * (Random.value > 0.5f ? 1 : -1);  // Random offset for X (10-15 units)
        float randomZ = Random.Range(10f, 15f) * (Random.value > 0.5f ? 1 : -1);  // Random offset for Z (10-15 units)

        // Set Y to a fixed value, assuming you want to always spawn the worm slightly below the player
        float randomY = -3f;

        // Set the worm's new position based on the random offset from the player's position
        Vector3 newPosition = new Vector3(playerPosition.x + randomX, playerPosition.y + randomY, playerPosition.z + randomZ);
        transform.position = newPosition;

        // Set the rotation
        transform.rotation = Quaternion.Euler(-56.68f, -80.33f, 0f);

        // You can also update the path points or other game elements as needed.
        Debug.Log("Worm moved to position: " + transform.position);
    }

}
