using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Rewired;

public class CutsceneController : MonoBehaviour
{
    public float skipDelay = 5f;
    public GameObject skipImage;
    
    // input from rewired
    private int m_playerID = 0;
    private Player m_playerInput;

    private void Start()
    {
        skipImage.SetActive(false);
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (skipImage.activeSelf && m_playerInput.GetButtonDown("Submit")) SceneManager.LoadScene("TitleScreen");

        if (Time.time > skipDelay && !skipImage.activeSelf) skipImage.SetActive(true);
    }
}
