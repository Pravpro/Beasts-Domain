using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ThrowableController : MonoBehaviour
{
    [HideInInspector] public SlingshotController slingshotScript;
    [HideInInspector] public AudioManagerMain audioManager;
    [HideInInspector] public GameObject[] monsters;
    public GameObject hitEffect;

    private MonsterHearing hearingScript;
    Rigidbody rb;
    AudioSource hitSound;

    private bool hit = false;

    private void Start()
    {
        monsters = GameObject.FindGameObjectsWithTag("Monster");
        audioManager = FindObjectOfType<AudioManagerMain>();
        rb = GetComponent<Rigidbody>();
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
        if (hit)
            return;

        Instantiate(hitEffect, col.contacts[0].point, new Quaternion());
        foreach (GameObject monster in monsters)
            monster.GetComponent<MonsterHearing>().RockHit(transform.position);
        rb.isKinematic = true;
        audioManager.Play(hitSound, 0.9f, new float[] { 0.7f, 1.2f });
        hit = true;
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        Destroy(gameObject.transform.GetChild(0).gameObject);
        yield return new WaitWhile(() => hitSound.isPlaying);
        Destroy(gameObject);
    }

}
