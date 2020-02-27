using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public SlingshotController slingshot;

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
            slingshot.playProjectileCollisionSound();
            Destroy(gameObject);
        }
    }
}
