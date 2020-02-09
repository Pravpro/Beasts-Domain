using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 5.0f;
    public float smoothTuring = 20f;

    // min Rnage, max Range
    public Vector2 fixedRange_y = new Vector2(10.0f, 55.0f);

    private Vector3 offset;
    private Vector3 m_origOffset;

    private Vector3 m_rotation;

    private bool m_throwing = false;


    // Start is called before the first frame update
    void Start()
    {
        // distance from player to the camera
        offset       = transform.position - player.transform.position;
        m_origOffset = offset;
    }

    void Update()
    {
        if (Input.GetButtonDown("Throw") )
            m_throwing = true;

        if (Input.GetButtonUp("Throw") )
            m_throwing = false;
    }

    // LateUpdate is called once per frame after everything has ben processed in Update
    void LateUpdate()
    {
        UpdateCameraRotation();

        if (Input.GetButton("ResetCamera") )
        {
            ResetCameraPosition();
        }
    }

    void ResetCameraPosition()
    {     
        Vector3 projected_forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        
        float angle = Vector3.SignedAngle(projected_forward.normalized, player.transform.forward.normalized, Vector3.up);

        Quaternion rotAngle_x = Quaternion.AngleAxis(angle * Mathf.Deg2Rad, Vector3.up);

        if (Mathf.Approximately(angle, 0.0f) )
            rotAngle_x = Quaternion.identity;

        Vector3 wantedPosition = rotAngle_x * offset + player.transform.position;

        transform.position = Vector3.Lerp(transform.position, wantedPosition, smoothTuring);
        
        transform.LookAt(player.transform.position);
        offset = rotAngle_x * offset;
    }

    void UpdateCameraRotation()
    {
        float horizontal = Input.GetAxis("HorizontalTurn"); 
        float vertical   = (m_throwing) ? 0.0f : -Input.GetAxis("VerticalTurn");

        // to calculate the vertical angle between player and camera, project the camera forward to the player.forwward 
        // we currently assume player.forward = Vector3.forward
        Vector3 projected = Vector3.ProjectOnPlane(transform.forward, Vector3.up /* normal vector */);

        // calculate the angle for vertical axis
        float angle = Mathf.Atan(-transform.forward.y / projected.magnitude);

        // fix the angle for vertical axis -- upper range
        if (angle <= fixedRange_y[0] * Mathf.Deg2Rad)
            vertical = Mathf.Clamp(vertical, -0.9f, 0.0f);

        // fix the angle for vertical axis -- lower range
        if (angle >= fixedRange_y[1] * Mathf.Deg2Rad)
            vertical = Mathf.Clamp(vertical, 0.0f, 0.9f);

        horizontal *= rotationSpeed;
        vertical   *= rotationSpeed;

        Quaternion rotAngle_x = Quaternion.AngleAxis(horizontal, Vector3.up);

        // forward axis -- always facing player
        // up axis      -- currently assume Vector3.up
        // known forwad axis, up axis use right hand rule to compute right axis for vertical rotation using cross product
        Vector3         right = Vector3.Cross(transform.forward, Vector3.up);
        Quaternion rotAngle_y = Quaternion.AngleAxis(  vertical, right);

        // combine horizontal and vertical rotation
        Quaternion rotAngle = rotAngle_y * rotAngle_x;
    
        // spherical ratation for camera centered at player 
        // offset is the fixed distance from player to camera
        Vector3 m_newPos = player.transform.position + rotAngle * offset;

        if (rotAngle != Quaternion.identity) 
        {         
            // smooth interpolation between current position and new position
            transform.position = Vector3.Slerp(transform.position, m_newPos, smoothTuring);

            // always look at player
            transform.LookAt(player.transform.position);  

            offset = rotAngle * offset;
        }      
        else 
        {
            // if rotation exceeds the view range, do not rotate, only follow the player positionzw
            transform.position = player.transform.position + offset;
            transform.LookAt(player.transform.position);  
        }  
    }
}
