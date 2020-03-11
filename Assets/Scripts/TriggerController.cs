using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public AudioManagerMain audioManager;
    public bool triggered = false;
    public int resetTime = 10;
    public float triggerCoeff = 3f;

    private AIController script;
    private GameObject monster;
    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindGameObjectWithTag("Monster");
        script = monster.GetComponent<AIController>();
    }

    IEnumerator TimedUntrigger()
    {
        yield return new WaitForSeconds(resetTime);
        Untrigger();
    }

    void Trigger() 
    {
        this.triggered = true;
        this.ModifySize(this.triggerCoeff);
    }

    void Untrigger()
    {
        this.triggered = false;
        this.ModifySize(1/this.triggerCoeff);
    }

    void ModifySize(float coeff)
    {
        // Vector3 pos = transform.position;
        // pos.y *= coeff;
        // transform.position = pos;
        Vector3 scale = transform.localScale;
        scale.y *= coeff;
        transform.localScale = scale;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Throwable" && !this.triggered)
        {
            this.Trigger();
            StartCoroutine(TimedUntrigger());
        }
        if (col.tag == "Monster" && this.triggered)
        {
            if (script.hp > 0)
            {
                audioManager.Play(audioManager.hurt);
                script.TakeDamage(transform.position);
                script.hp -= 1;
                Debug.Log("monster lose health to " + script.hp);
            }
            if (script.hp <= 0)
                Debug.Log("Monster dies!");
        }
    }
}
