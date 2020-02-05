using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody m_rid = GetComponent<Rigidbody>();
        m_rid.useGravity = true;
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player")
        {
            Debug.Log("collision detected, delete the object: " + other.name);
            Destroy(gameObject);
        }
    }
}
