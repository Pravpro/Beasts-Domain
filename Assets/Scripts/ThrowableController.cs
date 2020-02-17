using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public float destroyDelay = 5;

    Rigidbody m_rb;

    private void Start()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
        m_rb.useGravity = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
        {
            Debug.Log("collision detected with: " + col.collider.name);
            m_rb.useGravity = true;
            Destroy(gameObject, destroyDelay);
        }
    }
}
