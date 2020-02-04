using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public bool triggered = false;
    private AIController script;
    private GameObject monster;
    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        script = monster.GetComponent<AIController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "flyingRock" && !this.triggered)
        {
            this.triggered = true;
            Vector3 scale = transform.localScale;
            scale.y = 6;
            transform.localScale = scale;
            Vector3 pos = transform.position;
            pos.y = 3;
            transform.position = pos;
        }
        if (col.tag == "Monster" && this.triggered)
        {
            if (script.hp > 0)
                script.hp -= 1;
            Debug.Log("monster lose health to " + script.hp);
            if (script.hp <= 0)
                Debug.Log("Monster dies!");
        }
    }
}
