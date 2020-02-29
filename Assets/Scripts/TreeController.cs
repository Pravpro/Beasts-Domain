using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject log;
    public TreeAudio treeAudioScript;
    private Mesh mesh;
    private Bounds bounds;

    //Audio
    public AudioSource Tree;
    public AudioClip Fall;
    void Start()
    {
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {
            treeAudioScript.TreeFall();
            Instantiate(log, new Vector3(transform.position.x, bounds.max.y, transform.position.z), transform.rotation);
            Destroy(gameObject);
        }
    }
}
