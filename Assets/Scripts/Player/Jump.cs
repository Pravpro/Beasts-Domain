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
    private TextManager textManager;

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
        textManager = FindObjectOfType<TextManager>();

    }

    private void Update()
    {

        if (m_playerInput.GetButtonDown("Jump") && playerScript.GetGrounded())
        {
            // Must not send jump request if text is playing
            if (textManager != null) if (!textManager.TextIsPlaying()) jumpRequest = true;
            //else jumpRequest = true;
        }
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
            if (!audioManager.landing.isPlaying) audioManager.Play(audioManager.landing, new float[] { 0.7f, 1.3f });
        }
    }

    private void ModifyVelocity(float multiplier)
    {
        rb.velocity += Vector3.up * Physics.gravity.y * (multiplier - 1) * Time.deltaTime;
    }
}
