using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
public class mainmenu : MonoBehaviour
{

    public Button startButton;
    public Button quitButton;
    public Image arrow;

    private Vector3 offset;
    private Vector3 startPos;
    private Transform arrowTrans;

    // input from rewired
    private int m_playerID = 0;
    private Player m_playerInput;

    void Start()
    {
        m_playerInput = ReInput.players.GetPlayer(m_playerID);

        Button startBtn = startButton.GetComponent<Button>();
        Button quitBtn  = quitButton.GetComponent<Button>();

        arrowTrans = arrow.GetComponent<Transform>();
        startPos   = arrowTrans.position;
        offset     = quitBtn.GetComponent<Transform>().position - startBtn.GetComponent<Transform>().position ;

		// startBtn.onClick.AddListener(startOnClick);
        // quitBtn.onClick.AddListener(quitOnClick);
    }

    void Update()
    {
        float vertical = m_playerInput.GetAxis("Vertical");
        
        if (m_playerInput.GetButtonDown("Submit"))
        {
            if (arrowTrans.position == startPos)
                startOnClick();
            else
                quitOnClick();
        }

        // move the arrow accordingly
        if (vertical < 0.0f && arrowTrans.position == startPos)
            arrowTrans.position = startPos + offset;
        else if (vertical > 0.0f && arrowTrans.position == startPos + offset)
            arrowTrans.position = startPos;  

    }

    void startOnClick()
    {
        SceneManager.LoadScene("Scenes/AlphaScene");
    }

    void quitOnClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
