using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ButtonPrompt : MonoBehaviour
{
    private GameObject m_buttonPromptCanvas;
    private Player m_playerInput;
    private GameObject m_player;


    // list of button prmpt UI
    private Dictionary<string, GameObject> buttonPromptList = new Dictionary<string, GameObject>();

    void Awake()
    {
        m_buttonPromptCanvas = GameObject.Find("ButtonPromptCanvas");
        
        // m_slingshotUI = m_buttonPromptCanvas.transform.Find("slingshot").gameObject;
        
        buttonPromptList.Add("Jump",  m_buttonPromptCanvas.transform.Find("jump").gameObject);
        buttonPromptList.Add("Push",  m_buttonPromptCanvas.transform.Find("push").gameObject);
        buttonPromptList.Add("Spell", m_buttonPromptCanvas.transform.Find("spell").gameObject);
        buttonPromptList.Add("Slingshot", m_buttonPromptCanvas.transform.Find("slingshot").gameObject);
        buttonPromptList.Add("Sprint", m_buttonPromptCanvas.transform.Find("sprint").gameObject);

        // disable all the buttom prompt
        foreach(var element in buttonPromptList)
        {
            element.Value.SetActive(false);
        }
        m_player = GameObject.FindGameObjectWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
        
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

        // set position for the enabled button
        foreach(var element in buttonPromptList)
        {
            GameObject prompt = element.Value;

            if (prompt.activeSelf)
                setButtonPromptFollow(prompt);
        }


        if (isActivated("Push") && getNumActivatedPrompt() > 1)
        {
            disableActionPrompt("Push");
        }

    }

    void setButtonPromptFollow(GameObject buttonPrompt)
    {
        Vector3 offsetPos = m_player.transform.position; 
        offsetPos.y += 1f;

        // find the left position relative to the player for UI      
        Vector3 playerLeftAxis = Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized;

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos + playerLeftAxis * 1.5f);

        buttonPrompt.GetComponent<RectTransform>().position = screenPoint;
    }  

    public void enableActionPrompt(string actionName) 
    {   
        GameObject prompt = buttonPromptList[actionName];
        prompt.SetActive(true); 
    }

    public void disableActionPrompt(string actionName) 
    { 
        GameObject prompt = buttonPromptList[actionName];
        prompt.SetActive(false); 
    }

    public int getNumActivatedPrompt()
    {
        int i = 0;
        foreach(var element in buttonPromptList)
        {
            if (element.Value.activeSelf) 
                i++;
        }

        return i;
    }

    public bool isActivated(string actionName)
    {
        return buttonPromptList[actionName].activeSelf;
    }

}
