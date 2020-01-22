using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // public variables
    public GameObject player;
    public float turningSpeed = 20.0f;
    public float movingSpeed  = 0.1f;
    // private variables
    private bool m_playerTargeted = false;
    private Vector3 m_movement;
    private Vector3 m_targetedPos;
    // Start is called before the first frame update
    void Start()
    {
        m_targetedPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = m_targetedPos - transform.position;
        
        // ignore the (up) y direction
        direction.y = 0.0f;
        m_targetedPos.y = 0.0f;

        // 1. rotation
        if (Vector3.Angle(direction, transform.forward) > 2.0f)
        {            
            Quaternion qRotate = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
        
        }

        // 2. move to the target position only if the player is in the viewArea.
        //    rock throwed to the boss will only affect the rotation.
        if (m_playerTargeted && 
            Vector3.Distance(transform.position, m_targetedPos) > 1.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_targetedPos, Time.deltaTime * movingSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        m_playerTargeted = (other.name == "Player");

        if (other.name == "Player" || other.name == "flyingRock")
        {
            m_targetedPos = other.gameObject.transform.position;

            Debug.Log("Monster: the object " + other.name + " is in the target." + other.name + " position: " + m_targetedPos);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            Debug.Log("Monster: the object " + other.name + " is no longer in the target.");
            m_playerTargeted = false;
        }
    }
}
