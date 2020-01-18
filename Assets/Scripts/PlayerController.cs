using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Create variable for movement speed
    public float speed;


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow)){
            transform.Translate(new Vector3(0,0,1) * speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -1) * speed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(1, 0, 0) * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-1, 0, 0) * speed);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
