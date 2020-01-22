using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Create variable for movement speed
    public float walkSpeed, runSpeed, jumpSpeed, turnSpeed;
    public GameObject collectible;
    public Vector3 jump;

    private int count = 1;
    // private List<Collider> colliders = new List<Collider>();
    private Rigidbody rb;
    private bool grounded = true;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        jump = new Vector3(0, 1.0f, 0);
    }

    private void FixedUpdate()
    {
        // Movement according to WASD or arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        // Rotate the player according to desired rotation
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
        
        // Code for Running
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            rb.MovePosition(rb.position + m_Movement * runSpeed);
        }
        // Else walk
        else
        {
            rb.MovePosition(rb.position + m_Movement * walkSpeed);
        }
        rb.MoveRotation(m_Rotation);

        if (Input.GetKey(KeyCode.Space) && grounded)
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (count > 0) 
            {
                Instantiate(collectible, new Vector3(transform.position.x, 0.5f, transform.position.z), transform.rotation);
                count--;
            }
        }
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
    }
}
