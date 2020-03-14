using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ButtonPrompt : MonoBehaviour
{
    private GameObject m_buttonPromptCanvas;
    private Player m_playerInput;
    // Start is called before the first frame update
    void Start()
    {
        m_buttonPromptCanvas = GameObject.Find("ButtonPromptCanvas");
        m_playerInput = ReInput.players.GetPlayer(0 /* player id */);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: check for pause as well
        
        if (m_playerInput.GetButton("Aim"))
            m_buttonPromptCanvas.SetActive(false);
        else if (m_playerInput.GetButtonUp("Aim"))
            m_buttonPromptCanvas.SetActive(true);
        
    }
}
