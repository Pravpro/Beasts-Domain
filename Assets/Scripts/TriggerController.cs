using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public AudioManagerMain audioManager;
    public Material usedGeyserMaterial;
    public float triggerDistance = 4f;
    private AIController script;
    private GameObject monster;

    private bool isTriggered, changedColor = false;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        script = monster.GetComponent<AIController>();
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
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        // geyser is one-time activated therefore, if the geyser is triggered to give damage to monster,
        // we deactive the geyser
        if (isTriggered && !changedColor)
        {
            // change the material color
            this.GetComponent<Renderer>().material = usedGeyserMaterial;
            changedColor = true;
        }
    }

}
