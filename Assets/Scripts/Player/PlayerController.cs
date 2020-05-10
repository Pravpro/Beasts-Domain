using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Cinemachine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    // used for restarting the game by skipping the tutorial section
    public static Vector3 respawnPlayerPosition = new Vector3(-73.9f, -0.17f, 6.44f); // TODO
    [HideInInspector] public static bool tutorialFinished = false;

    // Create class for movement speeds
    [System.Serializable]
    public class PlayerSpeeds
    {
        public float walkSpeed, runSpeed, jumpSpeed, turnSpeed, crouchSpeed, mossSpeed;
    }

    [HideInInspector] public float stamina;
    public PlayerSpeeds speeds;
    public float hp, maxStamina;
    public Animator m_Animator;
    public CinemachineStateDrivenCamera SDCam;
    public AudioManagerMain audioManager;
    public GameObject disappearEffectPrefab;

    // player id for reference Rewired input
    // we only have one player id will always = 0
    private float speed, startColliderHeight;
    private int m_playerID = 0;
    private Player m_playerInput;
    private int count = 1;
    private Rigidbody rb;
    Vector3 m_Movement, moveVector, desiredForward, prevCamForward;
    Quaternion m_Rotation, lastRotation = Quaternion.identity;
    private bool isMoving, pushing, grounded, walking, running, crouching, inMoss = false;
    // private GameObject pushingObject;
    private CapsuleCollider m_collider;

    private ButtonPrompt buttonPromptScript;

    private bool infStamina, recoverStamia = false;

    // for spell
    public CinemachineFreeLook thirdPersonCam;
    public GameObject spellPrefab;
    private GameObject AimArea;
    private ParticleSystem spellEffect;

    public int spellWaitTime;
    
    [HideInInspector] public Coroutine WaitNextSpellCoroutine; // needed for UI 
    private bool spellActivated = false;
    private bool inSafeZone = false;
    private bool isInArena  = false;

    private bool inSpellTutorial = false;
    private GameObject promptTriggers;

    private void Awake()
    {
        // becauze my menu input event is written to messed up, i need to add this line here...
        Time.timeScale = 1; 

        // when restart, player position should be in the arena, instead of tutorial section
        if (tutorialFinished)
            this.transform.position = respawnPlayerPosition;

        var newPrefab = Instantiate(spellPrefab); newPrefab.name = "SpellEffect";
        spellEffect = newPrefab.GetComponent<ParticleSystem>();
        AimArea     = newPrefab.transform.GetChild(0).gameObject;
        // set to false at beginning
        AimArea.SetActive(false);
        audioManager.Localize(spellEffect.gameObject, audioManager.spell);     

        promptTriggers = GameObject.Find("promptTriggers");
        m_collider = GetComponent<CapsuleCollider>();
        buttonPromptScript = this.GetComponent<ButtonPrompt>();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    
    private void Start()
    {  
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
        maxStamina = stamina;
        startColliderHeight = m_collider.height;

        // reset the input to use rewired
        CinemachineCore.GetInputAxis = ReInput.players.GetPlayer(m_playerID).GetAxis;
    }

    private void FixedUpdate()
    {
        // Set Animator bools
        m_Animator.SetBool("IsMoving", spellActivated ? false : isMoving);
        m_Animator.SetBool("IsRunning", recoverStamia || pushing ? false : running); // not allow running if recovering stamina
        m_Animator.SetBool("IsCrouching", crouching);
        m_Animator.SetBool("IsPushing", pushing);
        m_Animator.SetBool("IsGrounded", grounded);

        m_collider.height = startColliderHeight * m_Animator.GetFloat("ColliderHeight");

        if (hp <= 0) return;

        
        ImplementMovement(); // Sets m_Movement
        ImplementRotation(); // Sets m_Rotation


        // Code for Running and walking
        if (grounded)
        {
            if (isMoving && !spellActivated)
            {
                if (crouching) speed = speeds.crouchSpeed;
                else if (inMoss) speed = speeds.mossSpeed;
                else if (pushing || recoverStamia) speed = speeds.walkSpeed;
                else if (running) { speed = speeds.runSpeed; if (!infStamina) stamina--; }
                else speed = speeds.walkSpeed;

                if (speed == speeds.walkSpeed || speed == speeds.mossSpeed) audioManager.PlayWalk();
                else if (speed == speeds.runSpeed) audioManager.PlayRun();
                moveVector = m_Movement * speed;
                rb.MovePosition(rb.position + moveVector);
            }
            else moveVector = new Vector3(0, 0, 0);
        }
        else rb.MovePosition(rb.position + moveVector * 3 / 5);

        // Rotate
        rb.MoveRotation(m_Rotation);

        if ((stamina < maxStamina) && !running) stamina++;
        else if (recoverStamia)                 stamina += 0.1f;

        if (stamina <= 0)
        {
            audioManager.Play(audioManager.stamina, new float[] {1.1f, 1.2f});
            recoverStamia = true;
        }

        // when stamina is fully recovered, player can run again (no need to buttonUp then buttonDown)
        if (stamina > maxStamina / 3) recoverStamia = false;

        // Crouch and Run Logic (Cannot do both at the same time)
        if (m_playerInput.GetButtonDown("Crouch")) crouching = !crouching;
        
        if (!crouching) running = m_playerInput.GetButton("Run");
        else running = false;

        // player spell logics
        if (tutorialFinished || inSpellTutorial)
            if (WaitNextSpellCoroutine == null) ActivateSpell();
    }


    // ------------------------ Collisions ------------------------


    // ------------------------ Triggers ------------------------
    void OnTriggerEnter(Collider col)
    {
        if (!tutorialFinished)
            enableButtonPrompt(col.gameObject);   

        if (col.tag == "Safezone")
            inSafeZone = true;
        
        if (col.name == "Arena")
        {
            tutorialFinished = true;
            isInArena = true;

            promptTriggers.SetActive(false);
        }
        
        if (col.name == "Homebase")
            infStamina = true;
       
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Moss")
        {
            inMoss = true;
            running = false;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Safezone")
            inSafeZone = false;
        if (col.tag == "Moss")
            inMoss = false;

        if (col.name == "Homebase")
            infStamina = false;
        
        if (col.name == "Arena")
            isInArena = false;
        
        if (!tutorialFinished)
            disableButtonPrompt(col.gameObject);
    }




    // =========================================== Helper Functions ===========================================


    /* Summary:
     * 1. Gets the axes for movement.
     * 2. Sets movement vector (m_Movement) in respect to the camera.
     * 3. Sets isMoving boolean if there is movement.
     */
    void ImplementMovement()
    {
        // Movement according to WASD
        float horizontal = m_playerInput.GetAxis("Horizontal");
        float vertical = m_playerInput.GetAxis("Vertical");
        m_Movement.Set(horizontal, 0f, vertical);

        // Get the movement to be respective to camera
        m_Movement = Camera.main.transform.TransformDirection(m_Movement);
        m_Movement.y = 0f;
        m_Movement.Normalize();

        // Set IsWalking bool according to movement
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        isMoving = hasHorizontalInput || hasVerticalInput;
    }

    /* Summary:
     * 1. When Aiming:
     *      - If camera forward changes, rotate (m_Rotation) with it
     * 2. Otherwise:
     *      - Rotate (m_Rotation) according to movement
     */
    void ImplementRotation()
    {
        // Conditions for Rotating
        if (SDCam.isActiveAndEnabled && SDCam.LiveChild != null && SDCam.LiveChild.Name == "CM_AimCam")
        {

            Vector3 camforward = Camera.main.transform.forward;
            camforward.y = 0f;

            // Check if aim camera has rotated/moved
            if (prevCamForward != camforward)
            {
                desiredForward = Vector3.RotateTowards(transform.forward, camforward, speeds.turnSpeed * Time.deltaTime, 0f);
                m_Rotation = Quaternion.LookRotation(desiredForward);
                lastRotation = m_Rotation;
                prevCamForward = camforward;
            }
            else m_Rotation = lastRotation;

        }
        else
        {
            // Prevent Player from turning due to collisions
            if (m_Movement.magnitude != 0 && grounded)
            {
                desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, speeds.turnSpeed * Time.deltaTime, 0f);
                m_Rotation = Quaternion.LookRotation(desiredForward);
                lastRotation = m_Rotation;
            }
            else m_Rotation = lastRotation;
        }
    }

    void ActivateSpell()
    {
        // some distance front of player
        Vector3 spellAreaPosition = this.transform.position + this.transform.forward * 10f - Vector3.down;
        
        if (m_playerInput.GetButtonDown("Spell"))
        {
            spellEffect.transform.position = spellAreaPosition;
            AimArea.SetActive(true);
            spellActivated = true;

            thirdPersonCam.m_RecenterToTargetHeading.m_enabled = true;
        }

        // TODO: should probably only allow spell after monster get some damage
        if (m_playerInput.GetButton("Spell"))
        {
            if (! AimArea.activeSelf) AimArea.SetActive(true);
            if (!tutorialFinished)
            {
                spellAreaPosition.y = 0.7f;
                spellEffect.transform.position = spellAreaPosition;
            }
            else
            {
                spellAreaPosition.y = spellEffect.transform.position.y; // update the y axis

                // ray cast to the ground
                RaycastHit hit;
                if (Physics.Raycast(spellAreaPosition + Vector3.up * 10f, Vector3.down, out hit, 100f /*max distance */, -1, QueryTriggerInteraction.Ignore) )
                {
                    Debug.Log("hitted");
                    if (hit.transform.tag == "Ground")
                        spellEffect.transform.position = hit.point + Vector3.up * 1.0f; /* a little above ground */
                }
                else
                    spellEffect.transform.position = spellAreaPosition;
            }
            
        }

        if (m_playerInput.GetButtonUp("Spell") ||
            AimArea.activeSelf && !m_playerInput.GetButton("Spell"))
        {
            // deactivate aiming for spell area
            AimArea.SetActive(false);
            audioManager.Play(audioManager.spell, 0.5f);

            // TODO: FIX THIS!!
            var newPrefab   = Instantiate(disappearEffectPrefab); newPrefab.name = "disappearEffect";

            AIController monsterScript;
            if (!tutorialFinished)
                monsterScript = GameObject.Find("MonsterSmol").GetComponentInChildren<AIController>();
            else
                monsterScript = GameObject.Find("Monster").GetComponentInChildren<AIController>();

            // activate the spell when monster is not charging
            if (!monsterScript.IsCharging())
            {
                var spellAreaEmission = spellEffect.emission;
                spellAreaEmission.enabled = true;
                spellEffect.Play();

                //audio test
                audioManager.Play(audioManager.spell);

                // wait time before next available spell
                WaitNextSpellCoroutine = StartCoroutine(WaitNextSpell() );

            }
        
            spellActivated = false;
            thirdPersonCam.m_RecenterToTargetHeading.m_enabled = false;
        }
    }

    IEnumerator WaitNextSpell()
    {
        yield return new WaitForSeconds(spellWaitTime);
        WaitNextSpellCoroutine = null;
    }

    // ------------------ tutorial -------------------------
    void enableButtonPrompt(GameObject trigger)
    {
        if (trigger.name == "jumpPrompt")
            buttonPromptScript.enableActionPrompt("Jump");
        else if (trigger.name == "spellPrompt")
        {
            inSpellTutorial = true;
            if (!buttonPromptScript.isActivated("Slingshot"))
            {
                buttonPromptScript.enableActionPrompt("Spell");
            }  
        }
        else if (trigger.name == "slingshotPrompt")
            buttonPromptScript.enableActionPrompt("Slingshot");
        else if (trigger.name == "sprintPrompt")
            buttonPromptScript.enableActionPrompt("Sprint");

    }

    void disableButtonPrompt(GameObject trigger)
    {
        if (trigger.name == "jumpPrompt")
        {
            buttonPromptScript.disableActionPrompt("Jump");
        }
        else if (trigger.name  == "spellPrompt")
        {
            buttonPromptScript.disableActionPrompt("Spell"); 
        }
        else if (trigger.name == "slingshotPrompt")
        {
            buttonPromptScript.disableActionPrompt("Slingshot");
        }   
        else if (trigger.name == "sprintPrompt")
        {
            buttonPromptScript.disableActionPrompt("Sprint");
            trigger.SetActive(false);
        }
            


    }


    // ----------------- public functions ------------------
    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsInSafeZone()
    {
        return inSafeZone;
    }
    
    public bool IsRunning()
    {
        return running;
    }

    public void startPushing()
    {
        pushing = true;
    }

    public void stopPushing()
    {
        pushing = false;
    }

    public bool isJumping()
    {
        return !grounded;
    }

    public bool finishedTutorial()
    {
        return tutorialFinished;
    }
    public bool IsInArena()
    {
        return isInArena;
    }
    public bool GetGrounded()
    {
        return this.grounded;
    }
    public void SetGrounded(bool grounded)
    {
        this.grounded = grounded;
    }
    public IEnumerator waitNextDamage(float waitTime)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        var endTime = Time.time +  waitTime;

        while(Time.time < endTime)
        {
            foreach (Renderer rend in renderers)
                rend.enabled = false;

            yield return new WaitForSeconds(0.2f);

            foreach (Renderer rend in renderers)
                rend.enabled = true;

            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }
}
