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
    public CinemachineBrain CB;
    public Camera aimCam;
    public Image crosshair;
    public float throwDelay;

    public AudioManagerMain audioManager;

    
    Animator playerAnimator;
    Vector3 targetVector;
    float nextThrowTime = 0f;

    // Rewired Input for throwing slingshot
    private int m_playerID = 0;
    private Player m_playerInput;

    private void Start()
    {
        crosshair.enabled = false;
        PlayerController playerScript = player.GetComponent<PlayerController>();
        playerAnimator = playerScript.m_Animator;
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
    }

    private void Update()
    {

        // Activate slingshot
        if (playerAnimator.GetBool("IsGrounded"))
        {
            if (m_playerInput.GetButtonDown("Aim"))
            {
                playerAnimator.SetBool("IsAiming", true);
            }

            if (m_playerInput.GetButtonUp("Aim"))
            {
                playerAnimator.SetBool("IsAiming", false);
                crosshair.enabled = false;
            }
        }

        if (CB.ActiveVirtualCamera.LiveChildOrSelf.Name == "CM_AimCam")
        {
            // Raycast from aimcam
            RaycastHit hit;
            Debug.DrawRay(aimCam.transform.position, aimCam.transform.forward * 80, Color.red);
            Debug.Log(Physics.Raycast(aimCam.transform.position, aimCam.transform.forward));
            if (Physics.Raycast(aimCam.transform.position, aimCam.transform.forward, out hit))
            {
                targetVector = hit.point - transform.position;
                Debug.DrawLine(transform.position, hit.point, Color.green);
                targetVector.Normalize();
            }
            else targetVector = aimCam.transform.forward;
            crosshair.enabled = true;

            // Release Slinghot
            if (nextThrowTime <= Time.time)
            {
                if (m_playerInput.GetButtonDown("Throw"))
                {
                    // Create the throwable object
                    GameObject m_rock = Instantiate(throwObject,
                                                    transform.position,
                                                    player.transform.rotation) as GameObject;
                    Rigidbody m_rb = m_rock.GetComponent<Rigidbody>();
                    ThrowableController m_rockScript = m_rock.GetComponent<ThrowableController>();
                    m_rockScript.slingshotScript = this;
                    m_rockScript.audioManager = audioManager;

                    audioManager.Play(audioManager.slingshotRelease);

                    // Add the launch arc forces to the throwable
                    m_rb.AddForce(targetVector * throwVelocity, ForceMode.VelocityChange);

                    //playerAnimator.SetBool("IsAiming", false);
                    nextThrowTime = Time.time + throwDelay;
                }
            }
        }
        else 
            crosshair.enabled = false;

        

        
    }

    public void playProjectileCollisionSound()
    {
        audioManager.Play(audioManager.rock, new float[] {0.7f, 1.2f});
    }

}
