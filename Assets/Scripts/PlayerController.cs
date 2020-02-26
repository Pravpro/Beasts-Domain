using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Cinemachine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    // Create variable for movement speed
    public float walkSpeed, runSpeed, jumpSpeed, turnSpeed, crouchSpeed;

    public int hp, stamina;
    private int maxStamina; //maxHp;
    private bool recoverStamia = false;
    
    public CinemachineStateDrivenCamera SDCam;

    // player id for reference Rewired input
    // we only have one player id will always = 0
    private int m_playerID = 0;
    private Player m_playerInput;
    private int count = 1;
    // private List<Collider> colliders = new List<Collider>();
    private Rigidbody rb;
    Vector3 m_Movement, jump, desiredForward, prevCamForward;
    Quaternion m_Rotation, lastRotation = Quaternion.identity;
    Animator m_Animator;
    private bool isMoving, pushing, grounded, walking, running, crouching = false;
    private ICinemachineCamera thirdPersonCam;


    ParticleSystem spellArea;
    GameObject AimArea;

    public int spellWaitTime;
    private bool waitForSpell = false;

    //Audio design
    public AudioClip[] Jumps;
    public AudioClip Landing;
    public AudioSource Jumping;
    public AudioSource spellSound;
    public AudioSource Stamina;
    public AudioMixerGroup output;

    private void Start()
    {  
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(m_playerID);

        maxStamina = stamina;
        
        // reset the input to use rewired
        CinemachineCore.GetInputAxis = ReInput.players.GetPlayer(m_playerID).GetAxis;

        m_Animator = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        jump = new Vector3(0, 1.0f, 0);

        spellArea = GameObject.Find("SpellArea").GetComponent<ParticleSystem>();
        AimArea = GameObject.Find("SpellArea/AimArea");
        // set to false at beginning
        AimArea.SetActive(false);
    }

    private void FixedUpdate()
    {
        // Set Animator bools
        m_Animator.SetBool("IsMoving", isMoving);
        m_Animator.SetBool("IsRunning", recoverStamia ? false : running); // not allow running if recovering stamina
        m_Animator.SetBool("IsCrouching", crouching); 

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
                desiredForward = Vector3.RotateTowards(transform.forward, camforward, turnSpeed * Time.deltaTime, 0f);
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
                desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
                m_Rotation = Quaternion.LookRotation(desiredForward);
                lastRotation = m_Rotation;
            }
            else m_Rotation = lastRotation;
        }

        // Code for Running and walking
        if (isMoving)
        {
            if (running && !recoverStamia)
            {
                // hacky way of making push same speed while running and walking
                rb.MovePosition(rb.position + m_Movement * (pushing ? walkSpeed 
                                                                    : runSpeed) );
                stamina--;
            }
            if (crouching)
            {
                rb.MovePosition(rb.position + m_Movement * crouchSpeed);
            }
            // Else walk
            else
            {
                rb.MovePosition(rb.position + m_Movement * walkSpeed);
            }

            
        }
        rb.MoveRotation(m_Rotation);

        if (stamina < maxStamina && !running) stamina++;
        if (stamina == 0) recoverStamia = true;
        {
            Stamina.Play();
        }

        // some buffer for recovering, set to half of the max stamina
        // player is required to recover to that point for using stamina for running.
        // TODO: change to other way like time buffer (?)
        if (recoverStamia && stamina > maxStamina / 2) recoverStamia = false;

        if (m_playerInput.GetButtonDown("Jump") && grounded)
        {
            rb.velocity += jump * jumpSpeed;
            grounded = false;
            int randomClip = Random.Range(0, Jumps.Length);
            Jumping.clip = Jumps[randomClip];
            Jumping.outputAudioMixerGroup = output;
            Jumping.PlayOneShot(Jumps[randomClip], 0.15f);
            Jumping.pitch = Random.Range(1f, 1.1f);
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


        if (!waitForSpell)
        {
            // some distance front of player
            Vector3 spellAreaPosition = this.transform.position + this.transform.forward * 10f;
            if (m_playerInput.GetButtonDown("Spell"))
            {
                spellArea.transform.position = spellAreaPosition;
                AimArea.SetActive(true);
            }

            // TODO: should probably only allow spell after monster get some damage
            if (m_playerInput.GetButton("Spell"))
            {
                // ray cast to the ground
                RaycastHit hit;
                if (Physics.Raycast(spellAreaPosition + Vector3.up * 5f, Vector3.down, out hit, 80.0f /*max distance */) )
                {
                    spellArea.transform.position = spellAreaPosition;

                    if (hit.collider.tag == "Ground")
                        spellArea.transform.position = hit.point + Vector3.up * 1.0f; /* a little above ground */
                }
            }

            if (m_playerInput.GetButtonUp("Spell"))
            {
                // activate the area
                var spellAreaEmission = spellArea.emission;
                spellAreaEmission.enabled = true;
                spellArea.Play();

                //audio test
                spellSound.pitch = Random.Range(0.9f, 1.3f);
                spellSound.Play();

                // deactivate aiming for spell area
                AimArea.SetActive(false);

                // wait time before next available spell
                StartCoroutine(WaitNextSpell() );
            }

        }

    }

    IEnumerator WaitNextSpell()
    {
        waitForSpell = true;
        yield return new WaitForSeconds(spellWaitTime);
        waitForSpell = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Ground")
        {
            grounded = true;
            Jumping.clip = Landing;
            Jumping.PlayOneShot(Landing, 1.2f);
        }

        // Avoid unwaned moving of a movable object
        if (col.collider.tag == "Movable")
        {
            Rigidbody rbMovable = col.gameObject.GetComponent<Rigidbody>();
            rbMovable.isKinematic = true;
        }
    }

    void OnCollisionStay(Collision col)
    {
        // Logic for moving objects
        if (col.collider.tag == "Movable")
        {
            Rigidbody rbMovable = col.gameObject.GetComponent<Rigidbody>();
            if (m_playerInput.GetButton("Push"))
            {
                pushing = true;
                rbMovable.isKinematic = false;

                // get the contact point of player with the movable object to look at.
                Vector3 targetPos = col.GetContact(0).point; 
                // set y same height as player ** might have issues with hills?
                targetPos.y = transform.position.y;

                transform.LookAt(targetPos);
            }
            if (m_playerInput.GetButtonUp("Push"))
            {
                rbMovable.isKinematic = true;
                pushing = false;
            }
                
        }   
    }

    void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "Movable")
        {
            Rigidbody rbMovable = col.gameObject.GetComponent<Rigidbody>();
            
            // stop any residual force that might cause object move
            rbMovable.Sleep();

            // allow monster to move around objects
            rbMovable.isKinematic = false;

            // make sure pushing has been reset properly
            pushing = false;

            // if the pushing object is boulder, we set the isKinematic to true
            // not allow monster to move.
            if (col.gameObject.name.Contains("Boulder") ||
                col.gameObject.name.Contains("boulder") )
                rbMovable.isKinematic = true;
        }
        
    }

    public bool IsMoving()
    {
        return isMoving;
    }
    
}
