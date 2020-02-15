using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Create variable for movement speed
    public float walkSpeed, runSpeed, jumpSpeed, turnSpeed, pushForce;
    public int hp;

    private int count = 1;
    // private List<Collider> colliders = new List<Collider>();
    private Rigidbody rb;
    private bool grounded = true;
    Vector3 m_Movement, jump;
    Quaternion m_Rotation, lastRotation = Quaternion.identity;
    Animator m_Animator;
    private bool pushing = false;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        jump = new Vector3(0, 1.0f, 0);
        hp = 2;
    }

    private void FixedUpdate()
    {
        if (hp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // "TitleScreen"
            return;
        }

        // Movement according to WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        // Get the movement to be respective to camera
        m_Movement = Camera.main.transform.TransformDirection(m_Movement);
        m_Movement.y = 0.0f;

        // Set IsWalking bool according to movement
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isMoving = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isMoving);

        // Rotate the player according to desired rotation
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        
        // Prevent Player from turning due to collsions
        if (m_Movement.magnitude == 0)
        {
            m_Rotation = lastRotation;
        }
        else
        {
            m_Rotation = Quaternion.LookRotation(desiredForward);
            lastRotation = m_Rotation;
        }

        // Code for Running and walking
        if (isMoving)
        {
            if (Input.GetButton("Run"))
            {
                m_Animator.SetBool("IsRunning", true);
                rb.MovePosition(rb.position + m_Movement * runSpeed);
            }
            // Else walk
            else
            {
                m_Animator.SetBool("IsRunning", false);
                rb.MovePosition(rb.position + m_Movement * walkSpeed);
            }
        }
        rb.MoveRotation(m_Rotation);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.velocity += jump * jumpSpeed;
            grounded = false;
        }

        
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Ground")
        {
            grounded = true;
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
            if (Input.GetButton("Push"))
            {
                rbMovable.isKinematic = false;
                rbMovable.AddForce(transform.forward * pushForce);
            }
            if (Input.GetButtonUp("Push"))
            {
                rbMovable.isKinematic = true;
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
        }
        
    }
    
}
