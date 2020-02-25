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
    public int hp;
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

    //Audio design
    public AudioClip[] Jumps;
    public AudioClip Landing;
    public AudioSource Jumping;
    public AudioMixerGroup output;

    public AudioSource spellSound;

    

    private void Start()
    {
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
        
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
        m_Animator.SetBool("IsRunning", running);
        m_Animator.SetBool("IsCrouching", crouching); 

        if (hp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // "TitleScreen"
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
            if (running)
            {
                // hacky way of making push same speed while running and walking
                rb.MovePosition(rb.position + m_Movement * (pushing ? walkSpeed 
                                                                    : runSpeed) );
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

        if (m_playerInput.GetButtonDown("Jump") && grounded)
        {
            rb.velocity += jump * jumpSpeed;
            grounded = false;
            int randomClip = Random.Range(0, Jumps.Length);
            Jumping.clip = Jumps[randomClip];
            Jumping.outputAudioMixerGroup = output;
            Jumping.PlayOneShot(Jumps[randomClip], 0.15f);
            Jumping.pitch = Random.Range(0.9f, 1.0f);
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


        // some distance front of player
        Vector3 spellAreaPosition = this.transform.position;
        spellAreaPosition.y = 1.0f; /* hardcoded again.. a bit above ground... */

        if (m_playerInput.GetButtonDown("Spell"))
        {
            // activate aiming for spell area
            AimArea.SetActive(true);
        }

        // TODO: should probably only allow spell after monster get some damage
        if (m_playerInput.GetButton("Spell"))
        {
            // get aiming area position respective to player position
            spellArea.transform.position = spellAreaPosition + this.transform.forward * 10.0f /* TODO: hardcoded distance */; 
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
        }
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
        }
        
    }
    
}
