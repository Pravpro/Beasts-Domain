using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ThrowableController : MonoBehaviour
{
    public SlingshotController slingshotScript;

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
            slingshotScript.playProjectileCollisionSound();
            Destroy(gameObject, 5f);
        }
    }
}
