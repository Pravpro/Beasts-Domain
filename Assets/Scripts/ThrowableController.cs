using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    public AudioClip[] Hit;
    public AudioSource Rock;
    
    public float destroyDelay = 5;

    Rigidbody m_rb;

    private void Start()
    {
        m_rb = gameObject.GetComponent<Rigidbody>();
        m_rb.useGravity = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name != "Player")
        {
            int randomClip = Random.Range(0, Hit.Length);
            Rock.clip = Hit[randomClip];
            Rock.PlayOneShot(Hit[randomClip], 1f);
            Rock.pitch = Random.Range(0.4f, 1.4f);

            Destroy(gameObject);
        }
    }
}
