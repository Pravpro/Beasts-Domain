using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            //Instantiate(log, new Vector3(transform.position.x, 0.5f, transform.position.z), transform.rotation);
            Destroy(gameObject);
        }
    }
}
