
// #define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public AudioManagerMain audioManager;
    public Material usedGeyserMaterial;

    private float triggerDistance;
    private AIController script;
    private GameObject monster;

    private bool isTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        script = monster.GetComponent<AIController>();

        // trigger distance is the radius = localScale.z / 2
        triggerDistance = this.transform.localScale.z / 2f - 0.2f; // give some small offset for distance 
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
            Debug.Log("geyser distance: " + Vector3.Distance(monsterPos, geyserPos));
#endif
            if (Vector3.Distance(monsterPos, geyserPos) < triggerDistance)
            {
                if (script.hp > 0)
                {
                    audioManager.Play(audioManager.hurt);
                    script.TakeDamage(transform.position);
                    script.hp -= 1;
                    Debug.Log("monster lose health to " + script.hp);

                    isTriggered = true;
                }
                if (script.hp <= 0)
                    Debug.Log("Monster dies!");
                
                // geyser is one-time activated, therefore deactivate once used by change the color
                this.GetComponentInChildren<Renderer>().material = usedGeyserMaterial;
            }
        }
    }

}
