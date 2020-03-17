using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ThrowableController : MonoBehaviour
{
    [HideInInspector] public SlingshotController slingshotScript;
    [HideInInspector] public AudioManagerMain audioManager;
    [HideInInspector] public GameObject monster;
    public GameObject hitEffect;

    private MonsterHearing hearingScript;
    Rigidbody rb;
    AudioSource hitSound;

    private void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        audioManager = FindObjectOfType<AudioManagerMain>();
        rb = GetComponent<Rigidbody>();
        hearingScript = monster.GetComponent<MonsterHearing>();
        hitSound = audioManager.Localize(gameObject, audioManager.rock);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
        {
            HandleHit(col);
            
        }
    }

    void HandleHit(Collision col)
    {
        Instantiate(hitEffect, col.contacts[0].point, new Quaternion());
        hearingScript.RockHit(transform.position);
        rb.isKinematic = true;
        audioManager.Play(hitSound, 0.9f, new float[] { 0.7f, 1.2f });

        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        Destroy(gameObject.transform.GetChild(0).gameObject);
        yield return new WaitWhile(() => hitSound.isPlaying);
        Destroy(gameObject);
    }

}
