using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowRotate : MonoBehaviour
{
    public GameObject target;

    // Late Update is called once per frame after all update statements
    void LateUpdate()
    {
        transform.LookAt(target.transform);
        transform.rotation = target.transform.rotation;
    }
}
