using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotation : MonoBehaviour
{
    public Transform cameraTransform;  // Drag your camera here
    public Transform bodyTransform;    // Drag your animated body here


    void Update()
    {
        // Synchronize the body's rotation with the camera's rotation
        bodyTransform.rotation = Quaternion.Euler(cameraTransform.eulerAngles.x-90, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        

    }

}
