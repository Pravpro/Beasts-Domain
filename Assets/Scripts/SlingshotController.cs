using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class SlingshotController : MonoBehaviour
{
    public GameObject throwObject, player;
    public int throwVelocity;
    public LaunchArcMesh launchArc;
    public CinemachineBrain CB;
    public Camera aimCam;
    public Image crosshair;

    //GameObject arc = null;
    Animator playerAnimator;
    Vector3 targetVector;

    private void Start()
    {
        crosshair.enabled = false;
        playerAnimator = player.GetComponent<Animator>();
    }

    private void Update()
    { 
        // Activate slingshot
        if (Input.GetButtonDown("Throw"))
        {
            playerAnimator.SetBool("IsAiming", true);
            //arc = Instantiate(launchArc.gameObject,
            //                  transform.position,
            //                  player.transform.rotation) as GameObject;
            
        }

        if (CB.ActiveVirtualCamera.LiveChildOrSelf.Name == "CM_AimCam")
        {
            // Raycast from aimcam
            RaycastHit hit;
            if (Physics.Raycast(aimCam.transform.position, aimCam.transform.forward, out hit))
            {
                //Debug.Log(hit.transform.name);
                targetVector = hit.point - transform.position;
                targetVector.Normalize();
            }
            crosshair.enabled = true;
        }
        else crosshair.enabled = false;

        // Make arc follow slingshot
        //if (arc != null)
        //{


        //    //player.transform.forward = Camera.current.transform.forward;
        //    arc.transform.position = transform.position;
        //    arc.transform.rotation = player.transform.rotation;
        //}

        // Release Slinghot
        if (Input.GetButtonUp("Throw"))
        {
            // Create the throwable object
            GameObject m_rock = Instantiate(throwObject,
                                            transform.position,
                                            player.transform.rotation) as GameObject;
            Rigidbody m_rb = m_rock.GetComponent<Rigidbody>();

            // Add the launch arc forces to the throwable
            //LaunchArcMesh arcScript = arc.GetComponent<LaunchArcMesh>();
            //m_rb.AddForce(Quaternion.AngleAxis(90-arcScript.angle, arc.transform.right) * arc.transform.up * arcScript.velocity, ForceMode.Impulse);
            m_rb.AddForce(targetVector * throwVelocity, ForceMode.Impulse);

            playerAnimator.SetBool("IsAiming", false);

            //Destroy(arc);
        }
    }

}
