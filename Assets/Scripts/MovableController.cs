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

    public AudioManagerMain audioManager;
    public PlayerController playerScript;

    // Start is called before the first frame update
    void Start()
    {
        // to access input using rewired
        m_playerInput = ReInput.players.GetPlayer(0 /*m_playerID */);
        m_rbMovable   = this.GetComponent<Rigidbody>();
        curPos = lastPos = new Vector2(transform.position.x, transform.position.z);
        
    }

    /** General Note here:
     *      - movable will require two colliders on it one with Trigger option enabled, one with it disabled.
     *        The collider with trigger option enabled will be mainly responsible for the logics for push. (like a wrapper) 
     *        The collider without trigger will be mainly for physics interaction.
     *
     *        Push on command requires the object to change isKinematic state. Whenever isKinematic = true, 
     *        the object will ignore the collision and skip the OnCollisionStay etc functions.
     *        Use trigger to not ignore any collision happened with isKinematic = true/false.
    **/
    void OnTriggerEnter(Collider col)
    {

        if (col.tag == "Monster")
        {
            // not allow monster to push boulder
            if (this.name.Contains("Boulder") ||
                this.name.Contains("boulder") )
            {
                m_rbMovable.isKinematic = true;

                m_monsterCollided = true;   
                
            }
        }

        m_rbMovable.Sleep();       
    }

    void OnTriggerStay(Collider col)
    {
        
        // maybe check if the monster is static? if so allow pushing
        if (m_monsterCollided)
            return;

        if (col.tag == "Player")
        {
            // avoid pushing without button pressed
            m_rbMovable.isKinematic = true;

            GameObject player = col.gameObject;

            // if object is set as isKinematics, onCollisionStay will be skipped
            // directly go to OnCollisionExit
            if (m_playerInput.GetButton("Push"))
            {
                isPushing = true;
                m_rbMovable.isKinematic = false;


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

                player.transform.LookAt(targetPos);
            }

            if (m_playerInput.GetButtonUp("Push"))
            {
                m_rbMovable.isKinematic = true;
                isPushing = false;
                audioManager.boulder.Stop();
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Monster") m_monsterCollided = false;

        //audioManager.boulder.Stop();


        if (!m_monsterCollided)
        {            
            // stop any residual force that might cause object move
            m_rbMovable.Sleep();

            // allow monster to move around objects
            m_rbMovable.isKinematic = false;
        }

        isPushing = false;
    }
}
