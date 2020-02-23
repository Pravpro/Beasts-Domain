using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

public class AIController : MonoBehaviour
{
    //Audio design
    public AudioClip Death;
    public AudioSource Respawn;

    public AudioSource choir;
    public AudioSource roar1;
    public AudioSource fight1;

    // public variables
    public GameObject player;
    public float turningSpeed = 20.0f;
    public float movingSpeed  = 1.0f;
    public int hp = 2;
    public bool playerInSight
    {
        get {return m_playerInSight;}
        set {
            if (m_playerInSight == value) return;
            else
            {
                if (!m_playerInSight)
                {
                    //BeastRoar1 fight starts
                    choir.Stop();
                    roar1.Play();
                    fight1.PlayDelayed(2.5f);
                }
                m_playerInSight = value;
            }
        }
    }

    private bool m_playerInSight = false;
    public GameObject target;
    // private variables
    private Vector3 m_targetedDir;
    private Rigidbody rb;
    private Vector3 playerPos, playerDir;

    private PlayerController playerScript;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Debug.Log("Monster size:" + GetComponent<MeshRenderer>().bounds.size);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateUpAxis = false;
        m_targetedDir = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        float theta = Vector3.Angle(Vector3.up, transform.up) * Mathf.Deg2Rad;
        if (theta > 0)
        {
            Vector3 side = Vector3.Cross(transform.up, Vector3.up);
            Vector3 upHillDir = Quaternion.AngleAxis(90, side) * transform.up;
            Vector3 force = rb.mass * 10f * Mathf.Sin(theta) * upHillDir.normalized;
            // Debug.Log("Up: " + transform.up + " Force: " + force);
            rb.AddForce(force);
        }

        if (hp <= 0 || playerScript.hp <= 0)
        {
            rb.isKinematic = false;
            return;
        }
        if (target)
            agent.SetDestination(target.transform.position);
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
        if (m_playerInSight)
        {
            playerPos = player.transform.position;
            // get the rotation and translate vector to player
            Vector3 playerPosCopy = new Vector3(playerPos.x, 0, playerPos.z);
            Vector3 location = transform.position;
            playerDir = playerPosCopy - location;
            playerDir.y = transform.forward.y;
            playerDir.Normalize();
            m_targetedDir = transform.forward;
            qRotate = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
            //transform.position += movingSpeed * transform.forward;
            transform.position = Vector3.MoveTowards(transform.position, playerPosCopy, Time.deltaTime * movingSpeed);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Throwable")
        {
            Vector3 targetedPos = col.collider.gameObject.transform.position;
            m_targetedDir = targetedPos - transform.position;
            // ignore the (up) y direction
            m_targetedDir.y = 0;

            Debug.Log("Monster: the object " + col.collider.name + " is in the target." + col.collider.name + " position: " + targetedPos);
        }
        else if (col.collider.tag == "Player")
        {
            if (playerScript.hp > 0)
            {
                playerScript.hp -= 1;
                Debug.Log("Player lose health to " + playerScript.hp);
            }
            if (playerScript.hp <= 0)
            {
                Debug.Log("Player Dies");
                Respawn.clip = Death;
                Respawn.PlayOneShot(Death, 0.5f);
            }
        }
    }
}
