using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossWallController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject babyMonster;
    void Start()
    {
        babyMonster = GameObject.Find("MonsterSmol");
    }

    // Update is called once per frame
    void Update()
    {
        if (!babyMonster.activeSelf || Input.GetKeyDown(KeyCode.K))
        {
            Destroy(gameObject);
        }


    }
}
