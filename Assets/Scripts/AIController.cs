using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AIController : MonoBehaviour
{
    // public variables
    public GameObject player;
    // public float viewAngle = 45.0f;
    // public float viewRadius = 10.0f;
    public float turningSpeed = 20.0f;
    public float movingSpeed  = 1.0f;
    public int hp = 2;
    public bool playerInSight = false;
    // private variables
    private bool m_playerTargeted = false;
    private Vector3 m_movement;
    private Vector3 m_targetedDir;
    // Start is called before the first frame update
    private Rigidbody rb;
    private Quaternion m_Rotation = Quaternion.identity;
    private Vector3 playerPos, playerDir;

    private PlayerController playerScript;

    void Start()
    {
        m_targetedDir = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (hp <= 0 || playerScript.hp <= 0) 
        {
            return;
        }
        // 1. rotation
        Quaternion qRotate;
        // turn the boss to the rock hit direction
        if (Vector3.Angle(m_targetedDir, transform.forward) > 2.0f)
        {
            qRotate = Quaternion.LookRotation(m_targetedDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
        }
        // if the monster already rotated to the direction of rock hit, 
        // then keep the target the same as current forward direction to prevent further turning
        else
        {
            m_targetedDir = transform.forward;
        }

        // 2. move to the target position only if the player is in the viewArea.
        //    boss ignores rocks when locked on to player
        
        // Debug.Log(playerInSight);
        if (playerInSight)
        {
            playerPos = player.transform.position;
            // get the rotation and translate vector to player
            Vector3 playerPosCopy = new Vector3(playerPos.x, 0, playerPos.z);
            Vector3 location = transform.position;
            playerDir = playerPosCopy - location;
            playerDir.y = 0;
            playerDir.Normalize();
            //Debug.Log("Im here");
            m_targetedDir = transform.forward;
            qRotate = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
            //transform.position += movingSpeed * transform.forward;
            transform.position = Vector3.MoveTowards(transform.position, playerPosCopy, Time.deltaTime * movingSpeed);
        }
        //Debug.Log(angle);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Throwable")
        {
            Vector3 targetedPos = other.gameObject.transform.position;
            m_targetedDir = targetedPos - transform.position;
            // ignore the (up) y direction
            m_targetedDir.y = 0;

            Debug.Log("Monster: the object " + other.name + " is in the target." + other.name + " position: " + targetedPos);
        }
        // if (other.tag == "Player")
        // {
        //     playerScript.hp -= 1;
        //     if (playerScript.hp <= 0)
        //     {
        //         Debug.Log("Player Dies");
        //     }
        // }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            if (playerScript.hp > 0)
            {
                playerScript.hp -= 1;
                Debug.Log("Player lose health to " + playerScript.hp);
            }
            if (playerScript.hp <= 0)
            {
                Debug.Log("Player Dies");
            }
        }
    }
}
