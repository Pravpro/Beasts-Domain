using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpellEffect : MonoBehaviour
{
    public GameObject disappearEffectPrefab;

    private Renderer m_renderer;
    private Color m_color;

    // script for monster
    private AIController monsterScript; 
    private ParticleSystem spellEffect;
    private ParticleSystem  disappearEffect;

    private bool startDisappear = false;
    private float currAlpha;

    AudioManagerMain audioManager;


    // Start is called before the first frame update
    void Start()
    {
        // the renderer for monster is in the child object
        m_renderer    = this.GetComponentInChildren<SkinnedMeshRenderer>();
        monsterScript = this.GetComponent<AIController>();
        audioManager = (AudioManagerMain)FindObjectOfType(typeof(AudioManagerMain));

        spellEffect = GameObject.Find("SpellEffect").GetComponent<ParticleSystem>();

        // monster disappear effect
        var newPrefab = Instantiate(disappearEffectPrefab); newPrefab.name = "disappearEffect";
        disappearEffect = newPrefab.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        // check if the monster is trapped
        if (spellEffect.isPlaying)
        {
            Vector3 monsterPosition = this.transform.position;
            Vector3 spellPosition   = spellEffect.transform.position;

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
            disappearEffect.transform.position = spellEffect.transform.position;
            disappearEffect.Play();

            setTransparent();

            currAlpha = disappearEffect.main.duration * 25f;
            startDisappear = true;

            // music can be added heare
            audioManager.Play(audioManager.defeat);



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
            m_renderer.enabled = false;
            
            if (this.transform.parent.gameObject.name != "MonsterSmol" && !SceneManager.GetSceneByName("WinScreen").isLoaded)
            {
                SceneManager.LoadScene("Scenes/WinScreen", LoadSceneMode.Additive);
                Rigidbody playerRB = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();

                playerRB.constraints = RigidbodyConstraints.FreezeAll;
                CameraSelector camSelector = FindObjectOfType<CameraSelector>();
                camSelector.SetCamActive(0);

                audioManager.Play(audioManager.win);
                audioManager.SetVolume(AudioManagerMain.SnapshotState.Win);
            }
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
