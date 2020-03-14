using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ThrowableController : MonoBehaviour
{
    public SlingshotController slingshotScript;
    public GameObject monster, hitEffect;

    private MonsterHearing hearingScript;

    private void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        hearingScript = monster.GetComponent<MonsterHearing>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
        {
            slingshotScript.playProjectileCollisionSound();
            Instantiate(hitEffect, col.contacts[0].point, new Quaternion());
            hearingScript.RockHit(transform.position);
            Destroy(gameObject);
        }
    }

}
