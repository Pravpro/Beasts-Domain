using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

// Idea from https://www.youtube.com/watch?v=7KiK0Aqtmzc and https://www.youtube.com/watch?v=CLxXkSIaOAc&t

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CapsuleCollider))]
public class Jump : MonoBehaviour
{
    public float fallMultiplier = 2f;
    //public float lowJumpMultiplier = 2f;
    public float groundedSkin = 0.05f;
    public LayerMask mask;

    private int m_playerID = 0;
    private Player m_playerInput;

    // Components
    private PlayerController playerScript;
    private AudioManagerMain audioManager;
    private Rigidbody rb;
    private CapsuleCollider m_collider;

    private Vector3 jump = Vector3.up;
    private bool jumpRequest;

    // Start is called before the first frame update
    private void Awake()
    {
        // Find Objects
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
        audioManager = (AudioManagerMain) FindObjectOfType(typeof(AudioManagerMain));

        // Get Components
        rb = GetComponent<Rigidbody>();
        playerScript = GetComponent<PlayerController>();
        m_collider = GetComponent<CapsuleCollider>();

    }

    private void Update()
    {
        if (m_playerInput.GetButtonDown("Jump") && playerScript.GetGrounded())
            jumpRequest = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (jumpRequest)
        {
            playerScript.m_Animator.SetTrigger("IsJumping");
            rb.velocity += jump * playerScript.speeds.jumpSpeed;
            playerScript.SetGrounded(false);
            jumpRequest = false;

            audioManager.walking.Stop();
            audioManager.Play(audioManager.jumping, new float[] { 1f, 1.1f });
        }
        //else
        //{
        //    // Weird code but, idea is raycast starts below player
        //    float scale = transform.localScale.y;
        //    Vector3 rayStart = transform.position + m_collider.center * scale + new Vector3(0,0.02f,0) + Vector3.down * m_collider.height * 0.5f * scale;
        //    Debug.DrawRay(rayStart, Vector3.down*groundedSkin, Color.cyan);
        //    RaycastHit hit;
        //    bool hitSomething = Physics.Raycast(rayStart, Vector3.down, out hit, groundedSkin, mask, QueryTriggerInteraction.Ignore);
        //    Debug.Log(hitSomething);
        //    if (hitSomething) Debug.Log(hit.collider.name);
        //    playerScript.SetGrounded(hitSomething);

        //}

        if (rb.velocity.y < 0)
            ModifyVelocity(fallMultiplier);
        //else if (rb.velocity.y > 0 && !m_playerInput.GetButton("Jump"))
        //    ModifyVelocity(lowJumpMultiplier);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Ground" || col.collider.tag == "Movable" || col.collider.tag == "Monster")
        {
            playerScript.SetGrounded(true);
            Debug.Log("landed");
            //if (rb.velocity.y > 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (!audioManager.landing.isPlaying) audioManager.Play(audioManager.landing, new float[] { 0.7f, 1.3f });
        }
    }

    private void ModifyVelocity(float multiplier)
    {
        rb.velocity += Vector3.up * Physics.gravity.y * (multiplier - 1) * Time.deltaTime;
    }
}
