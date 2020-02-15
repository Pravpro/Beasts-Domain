﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public int destroyDelay = 5;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
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
