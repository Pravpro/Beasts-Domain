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
    }


    /** General Note here:
     *      - movable will only need one collider now! (no need for trigger collider)
     *      - push on command changes the constraints for the rigidbody. 
     *      - When entering the collider, we enable all the constraints for rotation and position 
     *        except for positionY (needed for reacting with height).
     *      - When exiting the coolider, we disable all the constarints, allow boulder to have natural physics.
     *     
     *      - If things is not working:
     *          1. check the collider
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

#if DEBUG_LOG
                Debug.Log("(MovableController): trigger stay start pushing");
#endif
                if (isPushing)  
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

                // always look at the movable objects when pushing
                Vector3 targetPos = this.transform.position;
                targetPos.y = player.transform.position.y;

                // currently the center point for boulder is not centered... ughhhh...
                // TODO: remove this once the center position is fixed.
                targetPos.x = targetPos.x - this.transform.localScale.x / 2;
                targetPos.z = targetPos.z + this.transform.localScale.z / 2;

                // restrict the movement to one axis
                Vector3 playerForward = col.gameObject.transform.forward;
                if (Mathf.Abs(playerForward.x) > Mathf.Abs(playerForward.z)) playerForward.z = 0;
                else                                                         playerForward.x = 0;

                Vector3 newPos = targetPos + playerForward * 2.0f;

                newPos.y = this.transform.position.y;

                this.transform.position = Vector3.Slerp(this.transform.position, newPos, Time.deltaTime * 1.0f);
                
                player.transform.LookAt(targetPos);
            }

            if (m_playerInput.GetButtonUp("Push"))
            {
                isPushing = false;
                audioManager.boulder.Stop();
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
}
