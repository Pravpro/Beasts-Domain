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
    public Image staminaUIFill;
    public Image spellUIFill;

    private float hpCurrValue, hpMaxValue, staminaMaxValue;

    public Image healthImage;
    public AIController monsterScript;
    public Image blackScreen;
    

    private float fadeOutTime = 100.0f;

    private bool animatingDamage = false;
    private int frameCounter = 0;
    private bool startRecargeSpell = false;
    private float rechargeStartTime;

    private Sprite[] maxHP, damagedAnimation, lessHP;

    private PlayerController playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript  = this.GetComponent<PlayerController>();
        m_playerInput = ReInput.players.GetPlayer(0 /* player id */);
        
        hpMaxValue = (float) playerScript.hp;
        staminaMaxValue = (float) playerScript.stamina;

        // initialized to the max
        hpCurrValue  = hpMaxValue;

        spellUIFill.fillAmount   = 0.5f;
        staminaUIFill.fillAmount = 0.5f;

        // load all the resources for Sprite Animation
        maxHP            = Resources.LoadAll<Sprite>("Sprites/PlayerHP/maxHP"          ).OrderBy(img => img.name).ToArray();
        damagedAnimation = Resources.LoadAll<Sprite>("Sprites/PlayerHP/lossHPAnimation").OrderBy(img => img.name).ToArray();
        lessHP           = Resources.LoadAll<Sprite>("Sprites/PlayerHP/lessHP"         ).OrderBy(img => img.name).ToArray();

        frameCounter = 0;
#if false
        Debug.Log("maxHP: " + maxHP.Length);
        Debug.Log("maxHP: " + damagedAnimation.Length);
        Debug.Log("maxHP: " + lessHP.Length);
#endif
    }

    // Update is called once per frame

    void FixedUpdate()
    {   
        if (!animatingDamage)
        {
            if (hpCurrValue == hpMaxValue) animateSprite(ref maxHP);
            else                           animateSprite(ref lessHP);
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
        if (playerScript.hp < hpCurrValue)
        {
            frameCounter = 0;
            animatingDamage = true;
            Debug.Log("start animating damage");
        }

        // player Died,  avoid monster and player die at same time 
        if (hpCurrValue == 0 && (monsterScript.hp > 0))
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

        // keep track of player hp and update stamina and spell 
        hpCurrValue              = playerScript.hp;     
        staminaUIFill.fillAmount = playerScript.stamina / staminaMaxValue * 0.5f; 

        if (playerScript.WaitNextSpellCoroutine != null) 
        {
            if (!startRecargeSpell)
            {
                startRecargeSpell = true;
                rechargeStartTime = Time.time;
            }
            else
                spellUIFill.fillAmount =  (Time.time - rechargeStartTime) / playerScript.spellWaitTime * 0.5f;            
        }
        else
            startRecargeSpell = false;
        
        // hacky way of restarting
        float buttonPressed = m_playerInput.GetButtonTimePressed("Pause");
        if (buttonPressed > 2.0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        }
    }

    void Update()
    {
        
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
