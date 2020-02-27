using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

class Vector3Wrapper
{
    public Vector3 vector;

    public Vector3Wrapper(Vector3 vec)
    {
        vector = vec;
    }
}

public class AIController : MonoBehaviour
{
    //Audio design
    public AudioClip MoodBoard;
    public AudioClip Death;
    public AudioClip Hurt;
    public AudioClip Violin;
    public AudioSource damage;
    public AudioSource choir;
    public AudioSource roar1;
    public AudioSource fight1;
    public AudioSource strings;
    public AudioMixerSnapshot IntroMoodboard;
    public AudioMixerSnapshot IntroStrings;
    public AudioMixerSnapshot BossFight1;

    // public variables
    public GameObject player;
    public float turningSpeed = 60.0f;
    public float movingSpeed  = 1.0f;
    public int maxHp = 2;
    public int hp = 2;

    // give some buffer time for player to get next damange
    private Coroutine damageCoroutine = null;

    public bool playerInSight
    {
        get {return m_playerInSight;}
        set {
            if (m_playerInSight == value) return;
            else m_playerInSight = value;
        }
    }

    public float farHearRange = 20f;
    public float nearHearRange = 10f;
    public float scenLoadTime = 0f;

    public GameObject targetObj;
    // private variables
    private bool m_playerInSight = false;
    private bool m_roarPlayed = false;

    private Rigidbody rb;
    private PlayerController playerScript;
    private UnityEngine.AI.NavMeshAgent agent;

    private Vector3Wrapper target = null;
    private Vector3Wrapper lastSense = null;

    private Vector3 interruptPos;
    private Vector3 m_targetedDir;
    private Vector3 playerPos, playerDir;

    private enum State
    {
        Idle,
        RandomSearch,
        Charge,
        Defeated,
        Interrupted
    }
    private enum Phase
    {
        One,
        Two,
        Three
    }
    private State state = State.Idle;
    private Phase phase = Phase.One;

    private LineRenderer lineRenderer;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Debug.Log("Monster size:" + GetComponent<MeshRenderer>().bounds.size);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateUpAxis = false;
        m_targetedDir = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        IntroMoodboard.TransitionTo(0.0f);
        choir.Play();
        strings.clip = Violin;
        strings.PlayDelayed(30.0f);
        strings.loop = true;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3[] corners = {transform.position, target.vector};
            lineRenderer.SetPositions(corners);
            lineRenderer.enabled = true;
        }
        else
            lineRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        float theta = Vector3.Angle(Vector3.up, transform.up) * Mathf.Deg2Rad;
        if (theta > 0)
        {
            Vector3 side = Vector3.Cross(transform.up, Vector3.up);
            Vector3 upHillDir = Quaternion.AngleAxis(90, side) * transform.up;
            Vector3 force = rb.mass * 10f * Mathf.Sin(theta) * upHillDir.normalized;
            // Debug.Log("Up: " + transform.up + " Force: " + force);
            rb.AddForce(force);
        }

        if (hp <= 0 || playerScript.hp <= 0)
        {
            rb.isKinematic = true;
            agent.enabled = false;
            return;
        }

        // ADRIAN LOOK HERE!!!! :)
        if (Time.time - scenLoadTime == 25f)
        {
            
        }

        // 1. rotation
        Quaternion qRotate;
        // turn the boss to the rock hit direction
        if (Vector3.Angle(m_targetedDir, transform.forward) > 2.0f)
        {
            qRotate = Quaternion.LookRotation(m_targetedDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
        }
        // if the monster already rotated to the direction of rock hit,
        // then keep the target the same as current forward direction to prevent further turning
        else
        {
            m_targetedDir = transform.forward;
        }

        // 2. move to the target position only if the player is in the viewArea.
        //    boss ignores rocks when locked on to player
        if (m_playerInSight)
        {

            // playerPos = player.transform.position;
            // // get the rotation and translate vector to player
            // Vector3 playerPosCopy = new Vector3(playerPos.x, 0, playerPos.z);
            // Vector3 location = transform.position;
            // playerDir = playerPosCopy - location;
            // playerDir.y = transform.forward.y;
            // playerDir.Normalize();
            // m_targetedDir = transform.forward;
            // qRotate = Quaternion.LookRotation(playerDir);
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
            // //transform.position += movingSpeed * transform.forward;
            // transform.position = Vector3.MoveTowards(transform.position, playerPosCopy, Time.deltaTime * movingSpeed);
        }

        Navigate();
        //Debug.Log(state);
    }

    void Navigate()
    {
        // for debug purpose
        if (targetObj)
        {
            agent.SetDestination(targetObj.transform.position);
            return;
        }

        bool sensedPlayer = SensedPlayer() && !playerScript.IsInSafeZone();
        if (sensedPlayer)
        {
            if(!m_roarPlayed)
            {
                //BeastRoar1 fight starts
                choir.Stop();
                strings.Stop();
                roar1.Play();
                BossFight1.TransitionTo(4f);
                fight1.PlayDelayed(2.5f);
                m_roarPlayed = true;
            }
            lastSense = new Vector3Wrapper(player.transform.position);
        }
        switch(state)
        {
            case State.Idle:
                if (sensedPlayer) { ReactToPlayer(); return; }

                MoveToRandomPos();
                state = State.RandomSearch;
                break;
            case State.RandomSearch:
                if (sensedPlayer) { ReactToPlayer(); return; }

                if (ReachedWanderDest()) {
                    if (IsPhaseOne())
                        state = State.Idle;
                    else {
                        MoveToRandomPos();
                    }
                }
                break;
            case State.Charge:
                if (ReachedChargeDest())
                {
                    if (lastSense != null)
                    {
                        target = lastSense;
                        lastSense = null;
                    }
                    else
                    {
                        state = State.RandomSearch;
                        RestartAgent();
                        target = null;
                    }
                }
                else
                    Charge();

                break;
            case State.Interrupted:
                if (sensedPlayer) { ReactToPlayer(); return; }

                // turn the boss to the rock hit direction
                Quaternion qRotate;
                Vector3 tDir = interruptPos - transform.position;
                tDir.y = 0;
                if (Vector3.Angle(tDir, transform.forward) > 15.0f)
                {
                    qRotate = Quaternion.LookRotation(tDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
                }
                else
                {
                    RestartAgent();
                    state = State.RandomSearch;
                }
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Throwable")
        {
            Vector3 targetedPos = col.collider.gameObject.transform.position;
            m_targetedDir = targetedPos - transform.position;
            // ignore the (up) y direction
            m_targetedDir.y = 0;

            Debug.Log("Monster: the object " + col.collider.name + " is in the target." + col.collider.name + " position: " + targetedPos);
        }
        else if (col.collider.tag == "Player")
        {
            if (playerScript.hp > 0 && damageCoroutine == null)
            {
                playerScript.hp -= 1;
                Debug.Log("Player lose health to " + playerScript.hp);

                // avoid getting both hurt sound and dead sound when hp = 0
                // check the updated hp
                if (playerScript.hp > 0)
                {   
                    damage.clip = Hurt;
                    damage.PlayOneShot(Hurt, 0.2f);
                    damage.pitch = Random.Range(0.9f, 1.1f);

                    // give some buffer
                    damageCoroutine = StartCoroutine(playerInvincible() );
                }
            }

            if (playerScript.hp <= 0)
            {
                Debug.Log("Player Dies");
                damage.clip = Death;
                damage.PlayOneShot(Death, 0.5f);
            }
        }

        // not allow monster to push boulder
        if (col.gameObject.name.Contains("Boulder") ||
            col.gameObject.name.Contains("boulder") )
                col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    IEnumerator playerInvincible()
    {
        yield return StartCoroutine(playerScript.waitNextDamage(3)  );
        playerScript.stamina = playerScript.maxStamina;
        damageCoroutine = null;
    }

    void MoveToRandomPos()
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        Vector3 finalPosition = SampleRandomPos();
        while (!agent.CalculatePath(finalPosition, path))
            finalPosition = SampleRandomPos();
        
        agent.SetPath(path);
        //agent.SetDestination(finalPosition);
    }

    Vector3 SampleRandomPos()
    {
        float walkRadius = 80f;//Random.Range(1f, 80f);
        float x = Random.Range(-80f, 15f);
        float z = Random.Range(5f, 90f);
        // Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        // Vector3 center = new Vector3(-20f, transform.position.y, 20f);
        // randomDirection += center;
        Vector3 randomDirection = new Vector3(x, transform.position.y, z);
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }

    void ReactToPlayer()
    {
        target = new Vector3Wrapper(player.transform.position);
        state = State.Charge;
        if (IsPhaseOne())
            phase = Phase.Two;
        StopAgent();
    }

    void Charge()
    {
        Vector3 tPos = target.vector;
        // get the rotation and translate vector to player
        Vector3 location = transform.position;
        Vector3 tDir = tPos - location;
        tDir.y = transform.forward.y;
        tDir.Normalize();
        Quaternion qRotate = Quaternion.LookRotation(tDir, transform.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
        transform.position = Vector3.MoveTowards(transform.position, tPos, Time.deltaTime * movingSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
    }

    public void MakeSound(Vector3 pos)
    {
        float d = DistanceTo(transform.position);
        if (d < farHearRange)
            Interrupt(pos);
    }

    void Interrupt(Vector3 pos)
    {
        interruptPos = pos;
        if (state != State.Charge)
        {
            if (state != State.Interrupted)
                StopAgent();
            state = State.Interrupted;
        }
    }

    bool SensedPlayer()
    {
        return m_playerInSight || HeardPlayer();
    }

    bool HeardPlayer()
    {
        float d = DistanceTo(player.transform.position);
        if (d < nearHearRange)
            return true;
        if (d < farHearRange && playerScript.IsMoving())
            return true;
        return false;
    }

    void RestartAgent()
    {
        if (agent.enabled)
            return;
        agent.enabled = true;
        agent.isStopped = false;
    }

    void StopAgent()
    {
        if (!agent.enabled)
            return;
        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;
    }

    float DistanceTo(Vector3 v, bool ignoreY = false)
    {
        Vector3 diff = transform.position - v;
        if (ignoreY)
            diff.y = 0;
        return diff.magnitude;
    }

    bool IsPhaseOne()
    {
        return phase == Phase.One;
    }

    bool ReachedChargeDest()
    {
        return DistanceTo(target.vector, true) < 3f;
    }

    bool ReachedWanderDest()
    {
        return //!agent.pathPending
             agent.remainingDistance <= agent.stoppingDistance
             || !agent.hasPath;
    }
}
