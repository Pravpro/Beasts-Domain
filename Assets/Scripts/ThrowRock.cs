using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : MonoBehaviour
{
    public GameObject throwObject, player, launchArc;
    public float force = 500.0f;
    public float up_velocity = 0.5f;
    GameObject arc = null;


    // Update is called once per frame
    void FixedUpdate()
    {
        // Activate slingshot
        if (Input.GetKeyDown(KeyCode.V) )
        {
            // Create the Launch Arc
            arc = Instantiate(launchArc,
                              transform.position,
                              player.transform.rotation) as GameObject;
        }
        if (arc != null)
        {
            Debug.Log("Entered");
            arc.transform.position = transform.position;
            arc.transform.rotation = player.transform.rotation;

        }
        // Release Slinghot
        if (Input.GetKeyUp(KeyCode.V))
        {
            GameObject m_rock = Instantiate(throwObject,
                                            transform.position,
                                            transform.rotation) as GameObject;
            Rigidbody m_rb = m_rock.GetComponent<Rigidbody>();
            m_rb.AddForce((player.transform.forward + Vector3.up * up_velocity) * force);
        }
    }

    private void Update()
    {
        //if (arc != null)
        //{
        //    Debug.Log("Entered");
        //    arc.transform.position = transform.position;
        //    arc.transform.rotation = player.transform.rotation;

        //}
    }
}
