using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRock : MonoBehaviour
{
    public GameObject throwObject;
    
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
        }
    }
}
