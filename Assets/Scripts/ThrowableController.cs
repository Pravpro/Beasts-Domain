using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public float force = 500.0f;
    public float up_velocity = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody  m_rid  = GetComponent<Rigidbody>();
            
        m_rid.useGravity = true;
        m_rid.AddForce((transform.forward + Vector3.up * up_velocity) * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player")
        {
            Debug.Log("collision detected, delete the object");
            Destroy(gameObject);
        }  
    }
}
