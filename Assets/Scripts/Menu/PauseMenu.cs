using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
public class PauseMenu : MonoBehaviour
{
    private Player m_playerInput;
    private bool isPaused, buttonPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        m_playerInput = ReInput.players.GetPlayer(0 /* playerID */);
    }

    // Update is called once per frame
    void Update()
    {

        /** General Note:
         *      - using TimeScale requires to be called in Update(), otherwise it will not work.
         *      - For some reason, this GetButtonDown is triggered twice, when pauseMenu is loaded, 
         *        it immediately unload. But wierd thing is that even its unloaded, but still shown 
         *        on the scene in the game and SceneManager is telling me it does not have "PauseMenu" open...
         *
         *      - Hacky fix: Give small buffer time for getting next button input.
        **/

        // pause the scene
        if(m_playerInput.GetButtonDown("Pause") && !isLoaded("PauseMenu"))
        {
            buttonPressed = true; isPaused = true;

            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
            Time.timeScale = 0.001f; 
            
            StartCoroutine(waitForNextFrame() );
        }

        if (m_playerInput.GetButtonDown("Back") || m_playerInput.GetButtonDown("Pause"))
        {
            // if currently paused
            if (isLoaded("PauseMenu") && !buttonPressed && 
                SceneManager.sceneCount == 2)
            {
                isPaused = false;

                SceneManager.UnloadSceneAsync("PauseMenu");
                Time.timeScale = 1;
            }
        }
    }

    private IEnumerator waitForNextFrame()
    {
        // give small buffer time for getting next button
        yield return new WaitForEndOfFrame();
        buttonPressed = false;
    }

    private static bool isLoaded(string name)
    {
        // Debug.Log("count:" + SceneManager.sceneCount);

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
