using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCheat : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            AIController monsterScript = GetComponent<AIController>();
            monsterScript.hp = 0;
        }
    }
}
