using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    // float smooth = 5.0f;
    // float tiltAngle = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }

    void activate()
    {
        transform.Translate(new Vector3(0, 1, 0));
    }
}
