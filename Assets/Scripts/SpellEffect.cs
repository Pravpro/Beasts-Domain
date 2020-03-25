﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpellEffect : MonoBehaviour
{

    public GameObject monster;
    public GameObject disappearEffectObject;

    private Renderer m_renderer;
    private Color m_color;

    // script for monster
    private AIController monsterScript; 
    private ParticleSystem spellEffect;
    private ParticleSystem  disappearEffect;
    
    private bool startDisappear = false;
    private float currAlpha;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // for now the renderer for monster is in the child object
        m_renderer  = monster.GetComponentInChildren<SkinnedMeshRenderer>();
        monsterScript = monster.GetComponent<AIController>();

        spellEffect = GetComponent<ParticleSystem>();

        // monster disappear effect
        disappearEffect = disappearEffectObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // check if the monster is trapped
        if (spellEffect.isPlaying)
        {
            Vector3 monsterPosition = monster.transform.position;
            Vector3 spellPosition   = this.transform.position;

            // ignore both y axis
            monsterPosition.y = 0;
            spellPosition.y   = 0;
            startDisappear = false;

            if (Vector3.Distance(monsterPosition, spellPosition) < 3.0f)
            {
                Debug.Log("monster trapped");

                // set monster hp = 0 if spell succeed
                monsterScript.hp = 0;
            }
                
        }

        if (!startDisappear && monsterScript.hp == 0)
        {
            disappearEffect.transform.position = monster.transform.position;
            disappearEffect.Play();

            setTransparent();

            currAlpha = disappearEffect.main.duration * 25f;
            startDisappear = true;

            // music can be added heare

        }

        // if hp == 0, start disappear by making monster transparent
        if (startDisappear)
        {            
            Color newColor = m_renderer.material.color;
            newColor.a     = currAlpha / (disappearEffect.main.duration * 25f);
            
            m_renderer.material.color = Color.Lerp(m_renderer.material.color, newColor, Time.deltaTime);
            currAlpha--;
        }

        // alpha is 0 restart the scene
        if (m_renderer.material.color.a <= 0.0f)
        {
            monster.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // "TitleScreen"

        }
    }

    // online source
    // set the renderer to transparent during runtime
    // our shader does not have render type as transparent currently, therefore not allowed to change alpha
    // I feel like this is a hacky way so
    // --------  REMOVE THIS AFTER WE HAVE THE PROPER MATERIAL SHADER --------
    void setTransparent()
    {
        m_renderer.material.SetOverrideTag("RenderType", "Transparent");
        m_renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m_renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m_renderer.material.SetInt("_ZWrite", 0);
        m_renderer.material.DisableKeyword("_ALPHATEST_ON");
        m_renderer.material.DisableKeyword("_ALPHABLEND_ON");
        m_renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        m_renderer.material.renderQueue = 3000;

        m_renderer.material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
