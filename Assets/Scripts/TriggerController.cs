
// #define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public AudioManagerMain audioManager;
    public ParticleSystem geyserBurst;

    private float triggerDistance;
    private AIController script;
    private GameObject monster;
    private AudioSource geyserSound;

    private bool isTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.Find("Monster").GetComponentInChildren<AIController>();
        //geyserBurst = GetComponent<ParticleSystem>();
        geyserSound = audioManager.Localize(gameObject, audioManager.geyser);

        // trigger distance is the radius = localScale.z / 2
        // Note: scale is messed up -- BetaLevelScale is doubled
        triggerDistance = this.transform.localScale.z - 0.2f;         // give some small offset for distance 
        // triggerDistance = this.transform.localScale.z / 2f - 0.2f;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Monster" && !isTriggered)
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
                float scale = transform.localScale.y;
                scale += 20f;
                if (script.hp > 0)
                {
                    audioManager.Play(audioManager.hurt, 0.8f);
                    script.TakeDamage(transform.position);
                    script.hp -= 1;
                    Debug.Log("monster lose health to " + script.hp);

                    isTriggered = true;
                }
                if (script.hp <= 0)
                {
                    Debug.Log("Monster dies!");
                    audioManager.Play(audioManager.defeat, 0.7f);
                }

                // geyser is one-time activated, therefore deactivate once used by change the color
                geyserBurst.Play();
                audioManager.Play(geyserSound, 0.85f);

            }
        }
    }

}
