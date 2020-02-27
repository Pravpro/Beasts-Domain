using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool monsterTrapped = false;
    private bool startDisappear = false;
    private float currAlpha;
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_renderer  = monster.GetComponent<Renderer>();
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

            if (Vector3.Distance(monsterPosition, spellPosition) < 2.5f)
                monsterTrapped = true;
        }

        if (!startDisappear && (monsterTrapped || monsterScript.hp == 0))
        {
            disappearEffect.transform.position = monster.transform.position;
            disappearEffect.Play();

            setTransparent();
            currAlpha = disappearEffect.main.duration * 50f;
            startDisappear = true;

        }

        // if hp == 0
        if (startDisappear)
        {            
            Color newColor = m_renderer.material.color;
            newColor.a     = currAlpha / (disappearEffect.main.duration * 50f);

            m_renderer.material.color = Color.Lerp(m_renderer.material.color, newColor, Time.deltaTime);
            currAlpha--;
        }

        // alpha is 0
        if (m_renderer.material.color.a <= 0.0f)
        {
            monster.GetComponent<Renderer>().enabled = false;

            // wait for few minutes

        
            // show the UIs for the game win

            // Do something...

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
