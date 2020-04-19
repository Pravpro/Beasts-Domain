using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossWallController : MonoBehaviour
{
    [Tooltip("Add the tutorial text trigger Object here. If added it will be destroyed with the wall.")]
    public TextTriggerController trigger;

    // Start is called before the first frame update
    private GameObject babyMonster;
    void Start()
    {
        babyMonster = GameObject.Find("MonsterSmol");
    }

    // Update is called once per frame
    void Update()
    {
        if (!babyMonster.activeSelf)
        {
            if (trigger != null) Destroy(trigger);
            Destroy(gameObject);
        }


    }
}
