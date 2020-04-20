using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Rewired;

public class WinScreenController : MonoBehaviour
{
    [Tooltip("How long into the cutscene skipping is allowed.")]
    public float skipDelay = 5f;
    [Tooltip("The object that will show for skip option.")]
    public GameObject skipImage;

    private bool skipped, loading = false;

    // input from rewired
    private int m_playerID = 0;
    private Player m_playerInput;

    private void Start()
    {
        skipDelay = Time.time + skipDelay;
        skipImage.SetActive(false);
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (skipImage.activeSelf && m_playerInput.GetButtonDown("Submit")) skipped = true;
        if (skipped && !loading) StartCoroutine(LoadNextScene());
        // Wait till specified time to show skip image (which allows skip)
        else if (Time.time > skipDelay && !skipImage.activeSelf) skipImage.SetActive(true);
    }

    IEnumerator LoadNextScene()
    {
        loading = true;
        skipImage.SetActive(false);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1.5f);

        SceneManager.LoadScene("TitleScreen");
    }
}
