using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ThrowableController : MonoBehaviour
{
    public SlingshotController slingshotScript;

    Rigidbody m_rb;
    public GameObject monster;
    private MonsterHearing hearingScript;

    private void Start()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
        m_rb.useGravity = false;
        monster = GameObject.FindGameObjectWithTag("Monster");
        hearingScript = monster.GetComponent<MonsterHearing>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
        {
            slingshotScript.playProjectileCollisionSound();
            hearingScript.RockHit(transform.position);
            Destroy(gameObject);
        }
    }
}
