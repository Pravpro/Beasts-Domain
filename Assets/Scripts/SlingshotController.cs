using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class SlingshotController : MonoBehaviour
{
    public GameObject throwObject, player;
    public LaunchArcMesh launchArc;
    public CinemachineBrain CB;
    public Camera aimCam;
    public Image crosshair;

    GameObject arc = null;
    Animator playerAnimator;

    private void Start()
    {
        crosshair.enabled = false;
        playerAnimator = player.GetComponent<Animator>();
    }

    private void Update()
    {
        if (CB.ActiveVirtualCamera.LiveChildOrSelf.Name == "CM_AimCam")
            crosshair.enabled = true;
        else crosshair.enabled = false;

        // Activate slingshot
        if (Input.GetButtonDown("Throw"))
        {
            playerAnimator.SetBool("IsAiming", true);
            // Create the Launch Arc
            arc = Instantiate(launchArc.gameObject,
                              transform.position,
                              player.transform.rotation) as GameObject;
            
        }
        // Make arc follow slingshot
        if (arc != null)
        {
            // Raycast from aimcam
            RaycastHit hit;
            if (Physics.Raycast(aimCam.transform.position, aimCam.transform.forward, out hit))
            {
                //Debug.Log(hit.transform.name);
            }

            //player.transform.forward = Camera.current.transform.forward;
            arc.transform.position = transform.position;
            arc.transform.rotation = player.transform.rotation;
        }

        // Release Slinghot
        if (Input.GetButtonUp("Throw"))
        {
            playerAnimator.SetBool("IsAiming", false);

            // Create the throwable object
            GameObject m_rock = Instantiate(throwObject,
                                            transform.position,
                                            player.transform.rotation) as GameObject;
            Rigidbody m_rb = m_rock.GetComponent<Rigidbody>();

            // Add the launch arc forces to the throwable
            LaunchArcMesh arcScript = arc.GetComponent<LaunchArcMesh>();
            m_rb.AddForce(Quaternion.AngleAxis(90-arcScript.angle, arc.transform.right) * arc.transform.up * arcScript.velocity, ForceMode.Impulse);

            // Destroy Arc when slingsot launched
            Destroy(arc);
        }
    }

}
