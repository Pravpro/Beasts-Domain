// macro for logging debug message to console
// #define DEBUG_LOG

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

    public GameObject buttonPrompt;

    public AudioManagerMain audioManager;
    public PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(0 /*m_playerID */);
        m_rbMovable   = this.GetComponent<Rigidbody>();
        curPos = lastPos = new Vector2(transform.position.x, transform.position.z);

        m_origRBConstarints = m_rbMovable.constraints;

        // center of mass matters?
        // var centerofMass = m_rbMovable.centerOfMass;
        // centerofMass.x -= this.transform.localScale.x / 2;
        // centerofMass.z += this.transform.localScale.z / 2;
        // this.GetComponent<Rigidbody>().centerOfMass = centerofMass;

        buttonPrompt.SetActive(false);
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
        
        if (col.collider.tag == "Monster" || col.collider.tag == "Player")
            m_rbMovable.constraints = m_origRBConstarints;


        if (col.collider.tag == "Monster")
        {
            // not allow monster to push boulder
            if (this.name.Contains("Boulder") ||
                this.name.Contains("boulder") )
            {
                m_monsterCollided = true;   
            }

#if DEBUG_LOG
            Debug.Log("(MovableController): trigger entered");
#endif

            m_rbMovable.Sleep();     

        }
  
    }

    void OnCollisionStay(Collision col)
    {
        
        // maybe check if the monster is static? if so allow pushing
        if (m_monsterCollided)
            return;

        if (col.collider.tag == "Player")
        {
#if DEBUG_LOG
            Debug.Log("(MovableController): trigger staying");
#endif
            // avoid pushing without button pressed
            // m_rbMovable.isKinematic = true;

            GameObject player = col.collider.gameObject;

            if (m_playerInput.GetButton("Push"))
            {
                isPushing = true;
                m_rbMovable.constraints = m_origRBConstarints;
        
#if DEBUG_LOG
                Debug.Log("(MovableController): trigger stay start pushing");
#endif
                if (!isPushing)  
                {
                    m_rbMovable.constraints &= ~RigidbodyConstraints.FreezeRotationX;
                    m_rbMovable.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                }
                else
                    m_rbMovable.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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
                audioManager.boulder.Stop();

                m_rbMovable.constraints = RigidbodyConstraints.None;
            }
        }
    }

    // void OnTriggerExit(Collider col)
    void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "Monster") m_monsterCollided = false;

        //audioManager.boulder.Stop();

        isPushing = false;
        m_rbMovable.constraints = RigidbodyConstraints.None;

#if DEBUG_LOG
        Debug.Log("(MovableController): trigger exit");
#endif
    }

    // following is for button prompt
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
            buttonPrompt.SetActive(true);
    
    }

    void OnTriggerStay(Collider col)
    {
        // when pushing no need for
        if (isPushing)
            buttonPrompt.SetActive(false);
        // else if (col.tag == "Player")
            // StartCoroutine(disableButtonPrompt(3) );

    }

    void OnTriggerExit(Collider col)
    {
        // remove the button prompt even if player not pushed
        buttonPrompt.SetActive(false);
    }

    IEnumerator disableButtonPrompt(float time)
    {
        yield return new WaitForSeconds(time);
        buttonPrompt.SetActive(false);
    }
}
