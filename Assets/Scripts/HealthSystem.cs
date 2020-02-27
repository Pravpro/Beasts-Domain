using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

/**
 * UI health and stamina system for player
 * attached to the player!
**/
public class HealthSystem : MonoBehaviour
{
    // rewired input
    private Player m_playerInput;
    public Slider healthUI;
    public Slider staminaUI;
    public AIController monsterScript;

    public Image blackScreen;

    private float fadeOutTime = 100.0f;

    private PlayerController playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript  = this.GetComponent<PlayerController>();
        m_playerInput = ReInput.players.GetPlayer(0 /* player id */);
        
        healthUI.maxValue  = (float) playerScript.hp;
        staminaUI.maxValue = (float) playerScript.stamina;

        // initialized to the max
        healthUI.value  = healthUI.maxValue;
        staminaUI.value = staminaUI.maxValue;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (healthUI.value == 0)
        {
            var color = blackScreen.color;
            // alpha for color fade out
            color.a += 1.0f / fadeOutTime;
            blackScreen.color = Color.Lerp(blackScreen.color, color, 1.0f);

            // fade out
            if (blackScreen.color.a >= 1.0f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // "TitleScreen"
                monsterScript.scenLoadTime = Time.time;
            }

            return;       
        }

        healthUI.value = playerScript.hp;      
        staminaUI.value = playerScript.stamina;

        // logic for pause menu
        // TODO: move this somewhere else
        if (m_playerInput.GetButtonDown("Pause") && !isLoaded("PauseMenu") )
        {
            Time.timeScale = 0.0f;
            // SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
            
        }

        if (m_playerInput.GetButtonDown("Pause") && isLoaded("PauseMenu"))
        {
            // SceneManager.UnloadSceneAsync("PauseMenu");
            // Time.timeScale = 1.0f;
        }
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
