using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

// Idea from https://www.youtube.com/watch?v=7KiK0Aqtmzc

[RequireComponent(typeof(Rigidbody))]
public class BetterJump : MonoBehaviour
{
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private int m_playerID = 0;
    private Player m_playerInput;
    Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_playerInput = ReInput.players.GetPlayer(m_playerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.y < 0)
            ModifyVelocity(fallMultiplier);
        else if (rb.velocity.y > 0 && !m_playerInput.GetButton("Jump"))
            ModifyVelocity(lowJumpMultiplier);
    }

    private void ModifyVelocity(float multiplier)
    {
        rb.velocity += Vector3.up * Physics.gravity.y * (multiplier - 1) * Time.deltaTime;
    }
}
