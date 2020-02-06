using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public bool triggered = false;
    public int resetTime = 500;
    private int resetTimer = -1;
    public float triggerCoeff = 3f;
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

    void FixedUpdate()
    {
        if (this.resetTimer >= 0)
        {
            this.resetTimer -= 1;
        }
        if (this.resetTimer == 0)
        {
            untrigger();
        }
    }

    void trigger() 
    {
        this.resetTimer = resetTime;
        this.triggered = true;
        this.modifySize(this.triggerCoeff);
    }

    void untrigger()
    {
        this.triggered = false;
        this.modifySize(1/this.triggerCoeff);
    }

    void modifySize(float coeff)
    {
        Vector3 pos = transform.position;
        pos.y *= coeff;
        transform.position = pos;
        Vector3 scale = transform.localScale;
        scale.y = pos.y * 2;
        transform.localScale = scale;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Throwable" && !this.triggered)
        {
            this.trigger();
        }
        if (col.tag == "Monster" && this.triggered)
        {
            if (script.hp > 0)
            {
                script.hp -= 1;
                Debug.Log("monster lose health to " + script.hp);
            }
            if (script.hp <= 0)
                Debug.Log("Monster dies!");
        }
    }
}
