using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cinemachine;
using Rewired;

public class SlingshotController : MonoBehaviour
{
    public GameObject throwObject, player;
    public int throwVelocity;
    public LaunchArcMesh launchArc;
    public CinemachineBrain CB;
    public Camera aimCam;
    public Image crosshair;

    //Audio Design 
    public AudioClip Hold;
    public AudioClip Release;
    public AudioSource Slingshot;
    public AudioMixerGroup output;

    //GameObject arc = null;
    Animator playerAnimator;
    Vector3 targetVector;

    // Rewired Input for throwing slingshot
    private int m_playerID = 0;
    private Player m_playerInput;

    private void Start()
    {
        crosshair.enabled = false;
        playerAnimator = player.GetComponent<Animator>();
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
    }

    private void Update()
    { 
        // Activate slingshot
        if (m_playerInput.GetButtonDown("Throw"))
        {
            playerAnimator.SetBool("IsAiming", true);
            //arc = Instantiate(launchArc.gameObject,
            //                  transform.position,
            //                  player.transform.rotation) as GameObject;
            Slingshot.clip = Hold;
            Slingshot.PlayOneShot(Hold, 1f);
        }

        if (CB.ActiveVirtualCamera.LiveChildOrSelf.Name == "CM_AimCam")
        {
            // Raycast from aimcam
            RaycastHit hit;
            Debug.DrawRay(aimCam.transform.position, aimCam.transform.forward * 80, Color.red);
            Debug.Log(Physics.Raycast(aimCam.transform.position, aimCam.transform.forward));
            if (Physics.Raycast(aimCam.transform.position, aimCam.transform.forward, out hit))
            {
                //Debug.Log(hit.transform.name);
                targetVector = hit.point - transform.position;
                Debug.DrawLine(transform.position, hit.point, Color.green);
                targetVector.Normalize();
            }
            crosshair.enabled = true;

            // Release Slinghot
            if (m_playerInput.GetButtonUp("Throw"))
            {
                // Create the throwable object
                GameObject m_rock = Instantiate(throwObject,
                                                transform.position,
                                                player.transform.rotation) as GameObject;
                Rigidbody m_rb = m_rock.GetComponent<Rigidbody>();
                Slingshot.clip = Release;
                Slingshot.PlayOneShot(Release, 1f);
                Slingshot.outputAudioMixerGroup = output;

                // Add the launch arc forces to the throwable
                //LaunchArcMesh arcScript = arc.GetComponent<LaunchArcMesh>();
                //m_rb.AddForce(Quaternion.AngleAxis(90-arcScript.angle, arc.transform.right) * arc.transform.up * arcScript.velocity, ForceMode.Impulse);
                m_rb.AddForce(targetVector * throwVelocity, ForceMode.VelocityChange);

                playerAnimator.SetBool("IsAiming", false);

                //Destroy(arc);
            }
        }
        else crosshair.enabled = false;

        if (m_playerInput.GetButtonUp("Throw"))
        {
            playerAnimator.SetBool("IsAiming", false);
        }

        // Make arc follow slingshot
        //if (arc != null)
        //{


        //    //player.transform.forward = Camera.current.transform.forward;
        //    arc.transform.position = transform.position;
        //    arc.transform.rotation = player.transform.rotation;
        //}

        
    }

}
