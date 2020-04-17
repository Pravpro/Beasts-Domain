
// #define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public AudioManagerMain audioManager;
    public ParticleSystem geyserBurst, geyserSteam;
    public float cooldownTime = 30f;

    private float triggerDistance;
    private AIController script;
    private GameObject monster;
    private AudioSource geyserSound, geyserReady, geyserActive;

    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.Find("Monster").GetComponentInChildren<AIController>();
        geyserSound = audioManager.Localize(gameObject, audioManager.geyser);
        geyserReady = audioManager.Localize(gameObject, audioManager.geyserReady);
        geyserActive = audioManager.Localize(gameObject, audioManager.geyserActive);

        ActivateGeyser();

        // trigger distance is the radius = localScale.z / 2
        // Note: scale is messed up -- BetaLevelScale is doubled
        triggerDistance = this.transform.localScale.z - 0.2f;         // give some small offset for distance 
        // triggerDistance = this.transform.localScale.z / 2f - 0.2f;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Monster" && isActive)
        {
            // check the center of monster and geyser
            Vector3 monsterPos = col.gameObject.transform.position;
            Vector3 geyserPos  = this.transform.position;

            // ignore y axis
            monsterPos.y = 0;
            geyserPos.y  = 0;
#if DEBUG_LOG
            Debug.Log("geyser distance: " + Vector3.Distance(monsterPos, geyserPos) + "triggerDistance: " + triggerDistance);
#endif
            if (Vector3.Distance(monsterPos, geyserPos) < triggerDistance)
            {
                DamageMonster();
                isActive = false;

                // Particle Effects
                geyserBurst.Play();
                geyserSteam.Stop();
                // Sounds
                geyserActive.Stop();
                audioManager.Play(geyserSound, 0.85f);
                float totalWaitTime = geyserBurst.main.duration + 30f;
                Debug.Log(totalWaitTime);
                Coroutine WaitNextSpellCoroutine = StartCoroutine(Cooldown(totalWaitTime));
            }
        }
    }

    private IEnumerator Cooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        Debug.Log("Cooldown done");
        ActivateGeyser();
    }
    
    private void ActivateGeyser()
    {
        isActive = true;
        audioManager.Play(geyserReady, 0.95f, 3f, 10f);
        audioManager.Play(geyserActive, 1f, 2f, 15f, AudioRolloffMode.Linear);
        geyserSteam.Play();

    }

    private void DamageMonster()
    {
        if (script.hp > 0)
        {
            audioManager.Play(audioManager.hurt, 0.8f);
            script.TakeDamage(transform.position);
            script.hp -= 1;
        }
        if (script.hp <= 0)
            audioManager.Play(audioManager.defeat, 0.7f);
    }

}
