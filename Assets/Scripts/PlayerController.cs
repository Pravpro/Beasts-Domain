using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Cinemachine;
using Rewired;

public class PlayerController : MonoBehaviour
{
#if false
    // used for restarting the game by skipping the tutorial section
    public static Vector3 respawnPlayerPosition; // TODO
    public static bool tutorialFinished = false;
#endif

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


    // player id for reference Rewired input
    // we only have one player id will always = 0
    private float speed;
    private int m_playerID = 0;
    private Player m_playerInput;
    private int count = 1;
    private Rigidbody rb;
    Vector3 m_Movement, moveVector, jump, desiredForward, prevCamForward;
    Quaternion m_Rotation, lastRotation = Quaternion.identity;
    private bool isMoving, pushing, grounded, walking, running, crouching, inMoss = false;
    private GameObject pushingObject;

    private bool recoverStamia = false;

    // for spell
    public CinemachineFreeLook thirdPersonCam;
    ParticleSystem spellArea;
    GameObject AimArea;

    public int spellWaitTime;
    
    [HideInInspector]
    public Coroutine WaitNextSpellCoroutine; // needed for UI 
    private bool spellActivated = false;
    private bool inSafeZone = false;

    private void Awake()
    {
        // becauze my menu input event is written to messed up, i need to add this line here...
        Time.timeScale = 1; 

#if false
        // when restart, player position should be in the arena, instead of tutorial section
        if (tutorialFinished)
            this.transform.position = respawnPlayerPosition;
#endif
    }
    
    private void Start()
    {  
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(m_playerID);

        maxStamina = stamina;
        
        // reset the input to use rewired
        CinemachineCore.GetInputAxis = ReInput.players.GetPlayer(m_playerID).GetAxis;
        rb = gameObject.GetComponent<Rigidbody>();
        jump = new Vector3(0, 1.0f, 0);

        spellArea = GameObject.Find("SpellArea").GetComponent<ParticleSystem>();
        AimArea = GameObject.Find("SpellArea/AimArea");
        // set to false at beginning
        AimArea.SetActive(false);
        audioManager.Localize(spellArea.gameObject, audioManager.spell);

        pushingObject = null;
        
    }

    private void FixedUpdate()
    {
        // something is wrong with this push thing... I will investigate later...
        if (pushingObject != null) pushing = pushingObject.GetComponent<MovableController>().isPushing;

        // Set Animator bools
        m_Animator.SetBool("IsMoving", spellActivated ? false : isMoving);
        m_Animator.SetBool("IsRunning", recoverStamia || pushing ? false : running); // not allow running if recovering stamina
        m_Animator.SetBool("IsCrouching", crouching);
        m_Animator.SetBool("IsPushing", pushing);
        m_Animator.SetBool("IsGrounded", grounded);

        if (hp <= 0)
        {
            return;
        }

        // Movement according to WASD
        float horizontal = m_playerInput.GetAxis("Horizontal");
        float vertical   = m_playerInput.GetAxis("Vertical");
        m_Movement.Set(horizontal, 0f, vertical);

        // Get the movement to be respective to camera
        m_Movement = Camera.main.transform.TransformDirection(m_Movement);
        m_Movement.y = 0f;
        m_Movement.Normalize();

        // Set IsWalking bool according to movement
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        isMoving = hasHorizontalInput || hasVerticalInput;

        // Conditions for Rotating
        if (SDCam.LiveChildOrSelf.Name == "CM_AimCam")
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
            if (m_Movement.magnitude != 0)
            {
                desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, speeds.turnSpeed * Time.deltaTime, 0f);
                m_Rotation = Quaternion.LookRotation(desiredForward);
                lastRotation = m_Rotation;
            }
            else m_Rotation = lastRotation;
        }

        // Code for Running and walking
        if (isMoving && !spellActivated)
        {
            if (grounded)
            {
                if (crouching) speed = speeds.crouchSpeed;
                else if (inMoss) speed = speeds.mossSpeed;
                else if (pushing || recoverStamia) speed = speeds.walkSpeed;
                else if (running) { speed = speeds.runSpeed; stamina--; }
                else speed = speeds.walkSpeed;

                if (speed == speeds.walkSpeed || speed == speeds.mossSpeed) audioManager.PlayWalk();
                else if (speed == speeds.runSpeed) audioManager.PlayRun();
                moveVector = m_Movement * speed;
                rb.MovePosition(rb.position + moveVector);
                rb.MoveRotation(m_Rotation);
            }
            else rb.MovePosition(rb.position + moveVector * 3 / 5);
        }
        else
        {
            if (grounded)
            {
                rb.MoveRotation(m_Rotation);
                moveVector = new Vector3(0, 0, 0);
            }
        }

        if ((stamina < maxStamina) && !running) stamina++;
        else if (recoverStamia)                 stamina += 0.1f;

        if (stamina <= 0)
        {
            audioManager.Play(audioManager.stamina, new float[] {1.1f, 1.2f});
            recoverStamia = true;
        }

        // when stamina is fully recovered, player can run again (no need to buttonUp then buttonDown)
        if (stamina > maxStamina / 3) recoverStamia = false;


        if (m_playerInput.GetButtonDown("Jump") && grounded)
        {
            m_Animator.SetTrigger("IsJumping");
            rb.velocity += jump * speeds.jumpSpeed;
            grounded = false;

            audioManager.walking.Stop();
            audioManager.Play(audioManager.jumping, new float[] {1f, 1.1f});
        }

        // Crouch and Run Logic (Cannot do both at the same time)
        if (m_playerInput.GetButtonDown("Crouch"))
        {
            crouching = crouching ^ true;
        }
        
        if (!crouching)
        {
            running = m_playerInput.GetButton("Run");
        }
        else running = false;

        // player spell logics
        if (WaitNextSpellCoroutine == null)
        {
            ActivateSpell();
        }
    }

    void ActivateSpell()
    {
        // some distance front of player
        Vector3 spellAreaPosition = this.transform.position + this.transform.forward * 10f;
        spellAreaPosition.y = spellArea.transform.position.y; // update the y axis
        if (m_playerInput.GetButtonDown("Spell"))
        {
            spellArea.transform.position = spellAreaPosition;
            AimArea.SetActive(true);
            spellActivated = true;
            thirdPersonCam.m_RecenterToTargetHeading.m_enabled = true;
        }

        // TODO: should probably only allow spell after monster get some damage
        if (m_playerInput.GetButton("Spell"))
        {
            spellArea.transform.position = spellAreaPosition;
            // ray cast to the ground
            RaycastHit hit;
            int mask = (1 << LayerMask.NameToLayer("Ground"));
            if (Physics.Raycast(spellAreaPosition + Vector3.up * 10f, Vector3.down, out hit, 40.0f /*max distance */, mask) )
            {
                spellArea.transform.position = hit.point + Vector3.up * 1.0f; /* a little above ground */
            }
        }

        if (m_playerInput.GetButtonUp("Spell"))
        {
            // deactivate aiming for spell area
            AimArea.SetActive(false);
            audioManager.Play(audioManager.spell, 0.5f);

            // activate the spell when monster is not charging
            if (!GameObject.FindGameObjectWithTag("Monster").GetComponent<AIController>().IsCharging())
            {
                var spellAreaEmission = spellArea.emission;
                spellAreaEmission.enabled = true;
                spellArea.Play();

                

                //audio test
                audioManager.Play(audioManager.spell);

                // wait time before next available spell
                WaitNextSpellCoroutine = StartCoroutine(WaitNextSpell() );

            }
        
            spellActivated = false;
            thirdPersonCam.m_RecenterToTargetHeading.m_enabled = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Ground")
        {
            grounded = true;
            Debug.Log("landed");
            audioManager.Play(audioManager.landing, new float[] {0.7f, 1.3f});
        }

        if (col.collider.tag == "Movable")
        {
            pushingObject = col.collider.gameObject;
        }
    }

    void OnCollisionExit(Collision col)
    {
        pushingObject = null;
    }

    IEnumerator WaitNextSpell()
    {
        yield return new WaitForSeconds(spellWaitTime);
        WaitNextSpellCoroutine = null;
    }


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

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Safezone")
            inSafeZone = true;
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
