using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Create variable for movement speed
    public float walkSpeed, runSpeed, jumpSpeed, turnSpeed, pushForce;
    public Vector3 jump;
    public int hp;

    private int count = 1;
    // private List<Collider> colliders = new List<Collider>();
    private Rigidbody rb;
    private bool grounded = true;
    Vector3 m_Movement;
    Quaternion m_Rotation, lastRotation = Quaternion.identity;

    private bool pushing = false;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        jump = new Vector3(0, 1.0f, 0);
        hp = 2;
    }

    private void FixedUpdate()
    {
        if (hp <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // "TitleScreen"
            //Debug.Log("Player hp below 0");
            return;
        }
        // Movement according to WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        // get the movement to be respective to camera
        m_Movement = Camera.main.transform.TransformDirection(m_Movement);
        m_Movement.y = 0.0f;



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
        
        // Code for Running
        if (Input.GetButton("Run") )
        {
            rb.MovePosition(rb.position + m_Movement * runSpeed);
        }
        // Else walk
        else
        {
            rb.MovePosition(rb.position + m_Movement * walkSpeed);
        }
        rb.MoveRotation(m_Rotation);

        if (Input.GetButton("Jump") && grounded)
        {
            Debug.Log("jump");
            //rb.AddForce(0, 100, 0);
            rb.velocity += jump * jumpSpeed;
            grounded = false;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     if (count > 0) 
        //     {
        //         Instantiate(collectible, new Vector3(transform.position.x, 0.5f, transform.position.z), transform.rotation);
        //         count--;
        //     }
        // }

        if (Input.GetButtonDown("Push"))
            pushing = true;
        if (Input.GetButtonUp("Push"))
            pushing = false;

    }

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Collectible" && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Stay collectible and E");
            count++;
            Destroy (col.gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Ground")
        {
            Debug.Log("grounded");
            grounded = true;
        }

        // push on command
        if (pushing && (col.collider.tag == "Movable") )
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        if (!pushing && (col.collider.tag == "Movable") )
            col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    void OnCollisionStay(Collision col)
    {
        if (col.collider.tag == "Movable")
        {
            if (Input.GetButtonDown("Push"))
            {
                pushing = true;
                col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (Input.GetButtonUp("Push"))
            {
                pushing = false;
                col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            if (pushing)
                col.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * pushForce);
        }   
    }

    void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "Movable")
        {
            // stop any residual force that might cause object move
            col.gameObject.GetComponent<Rigidbody>().Sleep();

            // allow monster to move around objects
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        
    }
    
}
