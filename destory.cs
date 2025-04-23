using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Destroy this GameObject after 1.5 seconds
        Destroy(gameObject, 5.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
