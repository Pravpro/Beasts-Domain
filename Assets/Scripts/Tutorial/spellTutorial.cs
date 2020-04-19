using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class spellTutorial : MonoBehaviour
{
    private ButtonPrompt buttonPrompt;
    private GameObject slingshotTrigger;
    private GameObject spellTrigger;

    private Player m_playerInput;

    // Start is called before the first frame update
    void Start()
    {
        buttonPrompt = GameObject.FindGameObjectWithTag("Player").GetComponent<ButtonPrompt>();
        slingshotTrigger = GameObject.Find("promptTriggers/slingshotPrompt");
        spellTrigger     = GameObject.Find("promptTriggers/spellPrompt");
        m_playerInput = ReInput.players.GetPlayer(0 /* m_playerID */);
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Monster")
        {
            buttonPrompt.disableActionPrompt("Slingshot");
            slingshotTrigger.SetActive(false);
            
            buttonPrompt.enableActionPrompt("Spell");
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Monster")
        {
           if ( m_playerInput.GetButtonUp("Spell") )
           {
               buttonPrompt.disableActionPrompt("Spell");
               spellTrigger.SetActive(false);

               this.enabled = false;
           }
        }
    }
}
