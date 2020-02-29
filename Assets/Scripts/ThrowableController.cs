using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ThrowableController : MonoBehaviour
{
    public SlingshotController slingshotScript;

    Rigidbody m_rb;
    public GameObject monster;
    private AIController monsterScript;

    private void Start()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
        m_rb.useGravity = false;
        monster = GameObject.FindGameObjectWithTag("Monster");
        monsterScript = monster.GetComponent<AIController>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
        {
            slingshotScript.playProjectileCollisionSound();
            monsterScript.MakeSound(transform.position);
            Destroy(gameObject);
        }
    }
}
