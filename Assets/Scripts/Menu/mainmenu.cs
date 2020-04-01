using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Rewired;

public class mainmenu : MonoBehaviour
{
    public Button startBtn;
    public Button quitBtn;
    public Image arrow;

    public AudioManagerTitle audioManager;

    private Vector3 offset;
    private Vector3 startPos;
    private Transform arrowTrans;

    // input from rewired
    private int m_playerID = 0;
    private Player m_playerInput;


    /**
     *  PLEASE CLEAN UP THIS DISGUSTING CODE!! 
     */

    void Start()
    {
        if (isLoaded("TitleScreen"))
        {
            audioManager.Play(audioManager.music);
            audioManager.Play(audioManager.narrator);
        }
        

        m_playerInput = ReInput.players.GetPlayer(m_playerID);

        arrowTrans = arrow.GetComponent<Transform>();
        startPos   = arrowTrans.position;
        offset     = quitBtn.GetComponent<Transform>().position - startBtn.GetComponent<Transform>().position ;

		startBtn.onClick.AddListener(startOnClick);
        quitBtn.onClick.AddListener(controllerOnClick);
    }

    void Update()
    {
        float vertical = m_playerInput.GetAxis("Vertical");
        
        // PLSSS look into event system!! STOP WRITING THIS SHITTY CODE!!
        if (m_playerInput.GetButtonDown("Submit"))
        {
            if (Vector3.Distance(arrowTrans.position, startPos) < 0.3f)
                triggerEvent(startBtn);
            else
                triggerEvent(quitBtn);
            audioManager.Play(audioManager.UISelection);
        }

        if (m_playerInput.GetButtonDown("Back"))
        {
            backToMainMenu();
        }

        // move the arrow accordingly
        if (vertical < 0.0f && Vector3.Distance(arrowTrans.position, startPos) < 0.3f)
        {
            arrowTrans.position = startPos + offset;
            audioManager.Play(audioManager.UIToggle);
        }
        else if (vertical > 0.0f && Vector3.Distance(arrowTrans.position, startPos + offset) < 0.3f)
        {
            
            arrowTrans.position = startPos;
            audioManager.Play(audioManager.UIToggle);
        }
            

    }

    void startOnClick()
    {
        SceneManager.LoadScene("Scenes/FinalScene");
    }

    void controllerOnClick()
    {
        // #if UNITY_EDITOR
        //     UnityEditor.EditorApplication.isPlaying = false;
        // #else
        //     Application.Quit();
        // #endif
        if (!isLoaded("ControlsDesc") )
            SceneManager.LoadScene("ControlsDesc", LoadSceneMode.Additive);
    }

    void backToMainMenu()
    {
        if (isLoaded("ControlsDesc") )
            SceneManager.UnloadSceneAsync("ControlsDesc");
    }

    void RestartOnClick()
    {
        SceneManager.LoadScene("Scenes/FinalScene", LoadSceneMode.Single);
    }

    void triggerEvent(Button btn)
    {
        if (btn.name.Contains("Start") || btn.name.Contains("start"))
            startOnClick();
        else if (btn.name.Contains("Control") || btn.name.Contains("control"))
            controllerOnClick();
        else if (btn.name.Contains("Restart") || btn.name.Contains("restart"))
            RestartOnClick();

    }
    private static bool isLoaded(string name)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == name)
            {
                return true;
            }
        }
        return false;
    }

}
