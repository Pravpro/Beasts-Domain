using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Create variable for movement speed
    public float speed;
    public GameObject collectible;
    private int count = 1;
    // private List<Collider> colliders = new List<Collider>();

    private void start()
    {
        
    }

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
    
}
