using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called once per frame after everything has ben processed in Update
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
