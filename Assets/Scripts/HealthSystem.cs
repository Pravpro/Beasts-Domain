using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
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

    public Image healthImage;
    public AIController monsterScript;
    public Image blackScreen;
    

    private float fadeOutTime = 100.0f;

    private bool animatingDamage = false;
    private int frameCounter = 0;

    // private Object[] maxHP, damagedAnimation, lessHP;
    private Sprite[] maxHP, damagedAnimation, lessHP;

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

        // load all the resources for Sprite Animation
        maxHP            = Resources.LoadAll<Sprite>("Sprites/PlayerHP/maxHP"          ).OrderBy(img => img.name).ToArray();
        damagedAnimation = Resources.LoadAll<Sprite>("Sprites/PlayerHP/lossHPAnimation").OrderBy(img => img.name).ToArray();
        lessHP           = Resources.LoadAll<Sprite>("Sprites/PlayerHP/lessHP"         ).OrderBy(img => img.name).ToArray();

        frameCounter = 0;
        Debug.Log("maxHP: " + maxHP.Length);
        Debug.Log("maxHP: " + damagedAnimation.Length);
        Debug.Log("maxHP: " + lessHP.Length);
    }

    // Update is called once per frame

    void FixedUpdate()
    {   
        if (!animatingDamage)
        {
            if (healthUI.value == healthUI.maxValue) animateSprite(ref maxHP);
            if (healthUI.value < healthUI.maxValue) animateSprite(ref lessHP);
        }
        else
        {
            int keepTime = 3;
            animateSprite(ref damagedAnimation, keepTime /* num of frames to keep the same sprite */);

            // wait till damage animation finish
            if (frameCounter >= damagedAnimation.Length * keepTime) 
            {
                animatingDamage = false;
                Debug.Log("finish animating damage");
            }
        }
            
        
        // health decreased, start damage animation
        if (playerScript.hp < healthUI.value)
        {
            frameCounter = 0;
            animatingDamage = true;
            Debug.Log("start animating damage");
        }

        

        // player Died, avoid monster and player die at same time 
        if (healthUI.value == 0 && (monsterScript.hp > 0))
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

        // hacky way of restarting
        float buttonPressed = m_playerInput.GetButtonTimePressed("Pause");
        if (buttonPressed > 2.0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }



#if false
        // logic for pause menu
        // TODO: move this somewhere else
        if (m_playerInput.GetButtonDown("Pause") && !isLoaded("ControlsDesc") )
        {
            Time.timeScale = 0.0f;
            SceneManager.LoadScene("ControlsDesc", LoadSceneMode.Additive);
            
        }

        if (m_playerInput.GetButtonDown("Pause") && isLoaded("ControlsDesc"))
        {
            SceneManager.UnloadSceneAsync("ControlsDesc");
            Time.timeScale = 1.0f;
        }
#endif
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

    // pass in sprite object as reference
    // The damage animation is too fast, so to slow down the animation, use keepTime
    // rendering same image for set of frames.
    // default 1 means that it does not skip any frames, will be switching the iamge for each frame.
    private void animateSprite(ref Sprite[] spriteList, int keepTime = 1)
    {

        if (frameCounter >= spriteList.Length * keepTime ) frameCounter = 0;

            healthImage.sprite = spriteList[frameCounter / keepTime];
            frameCounter++;
    }

}
