using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject log;
    private Mesh mesh;
    private Bounds bounds;
    void Start()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            Instantiate(log, new Vector3(transform.position.x, bounds.max.y, transform.position.z), transform.rotation);
            Destroy(gameObject);
        }
    }
}
