using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : MonoBehaviour
{
    public GameObject throwObject;
    public GameObject player;
    public float force = 500.0f;
    public float up_velocity = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) )
        {
            Debug.Log("throwing the rock");
            GameObject m_rock = Instantiate(throwObject, 
                                            transform.position, 
                                            transform.rotation) as GameObject;
            Rigidbody m_rid = m_rock.GetComponent<Rigidbody>();
            m_rid.AddForce((player.transform.forward + Vector3.up * up_velocity) * force);
        }
    }
}
