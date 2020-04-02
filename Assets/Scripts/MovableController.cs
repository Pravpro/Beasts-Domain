// macro for logging debug message to console
#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MovableController : MonoBehaviour
{
    [HideInInspector]
    public bool isPushing, isMoving = false;
    private Player m_playerInput;
    private Rigidbody m_rbMovable;
    private Vector2 curPos, lastPos;

    private bool m_monsterCollided = false;
    private RigidbodyConstraints m_origRBConstarints;

    public AudioManagerMain audioManager;
    public PlayerController playerScript;

    private ButtonPrompt buttonPromptScript;

    private GameObject player;
    private Vector3 playerLeftAxis;


    // Start is called before the first frame update
    void Start()
    {
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(0 /*m_playerID */);
        m_rbMovable   = this.GetComponent<Rigidbody>();
        curPos = lastPos = new Vector2(transform.position.x, transform.position.z);

        m_origRBConstarints = m_rbMovable.constraints;

        player = GameObject.FindGameObjectWithTag("Player");

        buttonPromptScript = player.GetComponent<ButtonPrompt>();

        // center of mass matters?
        // var centerofMass = m_rbMovable.centerOfMass;
        // centerofMass.x -= this.transform.localScale.x / 2;
        // centerofMass.z += this.transform.localScale.z / 2;
        // this.GetComponent<Rigidbody>().centerOfMass = centerofMass;
    }

    /** General Note here:
     *      - movable will only require non-trigger collider. (Trigger collider is used for button prompt)
     *      - push on command changes the constraints for the rigidbody. 
     *      - When entering the collider, we enable all the constraints for rotation and position 
     *        except for positionY (needed for reacting with height).
     *      - When exiting the coolider, we disable all the constarints, allow boulder to have natural physics.
     *     
     *      - If things is not working:
     *          1. check the collider if exists and check the position of the collider. (might be shifted esp when player can walk in)
     *          2. check the constraints (need to check all except PositoinY)
     * 
     *       (extra note: develper who wrote this is very happy this worked! and been so dumb with using isKinematic=true/false...)
    **/

    void OnCollisionEnter(Collision col)
    {
        isPushing = false;
        if (col.collider.tag == "Monster" || col.collider.tag == "Player")
            m_rbMovable.constraints = m_origRBConstarints;


        if (col.collider.tag == "Monster")
        {
            // not allow monster to push boulder
            m_monsterCollided       = true;   
            m_rbMovable.isKinematic = true;

#if DEBUG_LOG
            Debug.Log("(MovableController): trigger entered :" + col.collider.gameObject.name);
#endif
        }  

        if (col.collider.tag == "Player")
            m_rbMovable.constraints = RigidbodyConstraints.FreezeAll;
    }

    void OnCollisionStay(Collision col)
    {
        if (playerScript.isJumping())
        {
            m_rbMovable.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }  

        if (col.collider.tag == "Player")
        {
#if DEBUG_LOG
            Debug.Log("(MovableController): trigger staying"+ col.collider.gameObject.name);
#endif
            m_rbMovable.constraints = RigidbodyConstraints.FreezeAll;

            GameObject player = col.collider.gameObject;

            if (m_playerInput.GetButton("Push"))
            {
                playerScript.startPushing();
                isPushing = true;

                m_rbMovable.constraints = m_origRBConstarints
                                        | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        
#if DEBUG_LOG
                Debug.Log("(MovableController): trigger stay start pushing");
#endif
                
                //curPos = new Vector2(transform.position.x, transform.position.z);
                //isMoving = curPos.x != lastPos.x || curPos.y != lastPos.y;
                //lastPos = curPos;
                //Debug.Log(isMoving);

                //Audio
                if (playerScript.IsMoving())
                {
                    if(!audioManager.boulder.isPlaying)
                        audioManager.Play(audioManager.boulder);
                }
                else
                    audioManager.boulder.Stop();

                
                // get contact forward for calculating the direction of boulder movement
                Vector3 playerForward = col.GetContact(0).normal;
                playerForward.y = 0;
                // move the boulder
                Vector3 newPos = this.transform.position + playerForward * 2.0f;

                newPos.y = this.transform.position.y;
                this.transform.position = Vector3.Slerp(this.transform.position, newPos, Time.deltaTime * 1.0f);
                
                // look at the boulder
                Vector3 playerLookAt = col.GetContact(0).point;
                playerLookAt.y = player.transform.position.y;
                player.transform.LookAt(playerLookAt);
            }

            if (m_playerInput.GetButtonUp("Push"))
            {
                isPushing = false;
                playerScript.stopPushing();
                audioManager.boulder.Stop();

                m_rbMovable.constraints = m_origRBConstarints;
            }
        }
    }

    // void OnTriggerExit(Collider col)
    void OnCollisionExit(Collision col)
    {
        //audioManager.boulder.Stop();
        isPushing = false;
        playerScript.stopPushing();

        if (!m_monsterCollided)
            m_rbMovable.constraints = RigidbodyConstraints.None;

#if DEBUG_LOG
        Debug.Log("(MovableController): collision exit"+ col.collider.gameObject.name);
#endif
    }


    // following is for button prompt
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Monster")
        {
            m_monsterCollided = true;  
            m_rbMovable.isKinematic = true;
            m_rbMovable.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        if (col.tag == "Player")
        {
            buttonPromptScript.enableActionPrompt("Push");

            m_rbMovable.constraints = RigidbodyConstraints.FreezeAll;
        }

        
              

    }

    void OnTriggerStay(Collider col)
    {
        if (m_monsterCollided)
            return;

        if (col.tag == "Player")
        {
#if DEBUG_LOG
            Debug.Log("(MovableController): trigger stay, push enabled: ");
#endif

            if (isPushing)
                buttonPromptScript.disableActionPrompt("Push");   
        }
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Monster") 
        {
            m_monsterCollided       = false;
            m_rbMovable.isKinematic = false;
        }
            
        if (col.tag == "Player")
        {
#if DEBUG_LOG
            Debug.Log("(MovableController): trigger exit");
#endif
            // remove the button prompt even if player not pushed
            buttonPromptScript.disableActionPrompt("Push");
        }

        m_rbMovable.constraints = RigidbodyConstraints.None;
    }
}
