using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public int destroyDelay = 5;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody m_rid = GetComponent<Rigidbody>();
        m_rid.useGravity = true;
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player" && col.collider.name != "MonsterViewArea")
        {
            Debug.Log("collision detected with: " + col.collider.name + "... will destroy rock in 5 seconds");
            StartCoroutine(DestroyThrowable());
        }
    }

    // Called to destroy rock
    IEnumerator DestroyThrowable()
    {
        //Makes function wait for 5 seconds before resuming
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
