using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Video;
using Rewired;

public class CutsceneController : MonoBehaviour
{
    [Tooltip("How long into the cutscene skipping is allowed.")]
    public float skipDelay = 5f;
    [Tooltip("The object that will show for skip option.")]
    public GameObject skipImage;

    private VideoPlayer cutscene;
    private bool started, finished, skipped = false;

    // input from rewired
    private int m_playerID = 0;
    private Player m_playerInput;

    private void Start()
    {
        skipImage.SetActive(false);
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
        cutscene = FindObjectOfType<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!started && cutscene.isPlaying) started = true;
        if (started && !cutscene.isPlaying) finished = true;
        if (skipImage.activeSelf && m_playerInput.GetButtonDown("Submit")) skipped = true;

        // Load title screen if skipped or if cutscene is done
        if (skipped || finished) StartCoroutine(LoadNextScene());
        // Wait till specified time to show skip image (which allows skip)
        else if (Time.time > skipDelay && !skipImage.activeSelf) skipImage.SetActive(true);
    }

    IEnumerator LoadNextScene()
    {
        skipImage.SetActive(false);
        if (cutscene.isPlaying) cutscene.Stop();

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("TitleScreen");
    }
}
