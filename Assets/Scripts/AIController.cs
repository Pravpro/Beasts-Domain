using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using UnityEngine.AI;

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
    public AudioClip[] bossStrings;
    public AudioSource damage;
    public AudioSource choir;
    public AudioSource roar1;
    public AudioSource fight1;
    public AudioSource fightStrings;
    public AudioSource strings;
    public AudioMixerSnapshot IntroFight;
    public AudioMixerSnapshot Boss;

    // public variables
    public GameObject player;
    public float turningSpeed = 60.0f;
    public float movingSpeed  = 1.0f;
    public int maxHp = 2;
    public int hp = 2;
    public float maxChargeDistance = 40.0f;

    // give some buffer time for player to get next damange
    private Coroutine damageCoroutine = null;
    private bool gotHit = false;

    public bool playerInSight
    {
        get {return m_playerInSight;}
        set {
            if (m_playerInSight == value) return;
            else m_playerInSight = value;
        }
    }

    public float scenLoadTime = 0f;

    public GameObject targetObj;
    // private variables
    private bool m_playerInSight = false;
    private bool m_roarPlayed = false;

    private Rigidbody rb;
    private PlayerController playerScript;
    private MonsterHearing hearingScript;
    private FieldOfView fovScript;
    private UnityEngine.AI.NavMeshAgent agent;

    private Vector3Wrapper target = null;
    private Vector3Wrapper lastSense = null;

    private Vector3Wrapper interruptPos = null;
    private Vector3 m_targetedDir;
    private Vector3 playerPos, playerDir;
    private bool playerReachable = false;

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

    //private LineRenderer lineRenderer;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // Debug.Log("Monster size:" + GetComponent<MeshRenderer>().bounds.size);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateUpAxis = false;
        m_targetedDir = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        fovScript = GameObject.Find("FieldOfView").GetComponent<FieldOfView>();
        hearingScript = gameObject.GetComponent<MonsterHearing>();

        //lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.positionCount = 2;

        IntroFight.TransitionTo(0.0f);
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
            //lineRenderer.SetPositions(corners);
            //lineRenderer.enabled = true;
        }
        //else
            //lineRenderer.enabled = false;
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
            agent.isStopped = true;
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
    }

    void Navigate()
    {
        // for debug purpose
        if (targetObj)
        {
            //agent.SetDestination(targetObj.transform.position);
            SetDestination(targetObj.transform.position);
            return;
        }

        // if (gotHit)
        //     return;
        playerReachable = PlayerReachable();
        bool sensedPlayer = fovScript.PlayerInSight() && playerReachable; // !playerScript.IsInSafeZone();
        
        if (sensedPlayer)
        {
            if(!m_roarPlayed)
            {
                //BeastRoar1 fight starts
                choir.Stop();
                strings.Stop();
                roar1.Play();
                Boss.TransitionTo(4f);
                int randomClip = Random.Range(0, bossStrings.Length);
                fightStrings.clip = bossStrings[randomClip];
                fightStrings.PlayDelayed(2.5f);
                fight1.PlayDelayed(2.5f);
                m_roarPlayed = true;
            }
            interruptPos = new Vector3Wrapper(player.transform.position);
        }
        // Debug.Log(state);
        switch(state)
        {
            case State.Idle:
                if (sensedPlayer) { ReactToPlayer(); return; }

                // TODO: rest for a few secs?

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
                if (ReachedWanderDest())
                {
                    if (interruptPos != null)
                    {
                        SetDestination(interruptPos.vector);
                        //agent.SetDestination(interruptPos.vector);
                        state = State.Interrupted;
                    }
                    else
                    {
                        state = State.RandomSearch;
                    }
                    UnsetChargeSpeed();
                    // if (lastSense != null)
                    // {
                    //     target = lastSense;
                    //     lastSense = null;
                    // }
                    // else
                    // {
                    //     state = State.RandomSearch;
                    //     RestartAgent();
                    //     target = null;
                    // }
                }
                // else
                //     Charge();

                break;
            case State.Interrupted:
                if (sensedPlayer) { ReactToPlayer(); return; }

                if (interruptPos == null) { state = State.RandomSearch; return; }

                Vector3 tDir = interruptPos.vector - transform.position;
                tDir.y = 0;
                if ( (Vector3.Angle(tDir, transform.forward) < 15.0f && tDir.magnitude < 15f)
                    || ReachedWanderDest()) // tDir.magnitude < 3f)
                {
                    interruptPos = null;
                    state = State.RandomSearch;
                }
                
                // turn the boss to the rock hit direction
                // Quaternion qRotate;
                // Vector3 tDir = interruptPos.vector - transform.position;
                // tDir.y = 0;
                // if (Vector3.Angle(tDir, transform.forward) > 15.0f)
                // {
                //     qRotate = Quaternion.LookRotation(tDir);
                //     transform.rotation = Quaternion.Slerp(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
                // }
                // else
                // {
                //     RestartAgent();
                //     state = State.RandomSearch;
                // }

                break;
            default:
                break;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Monster: the object " + col.collider.name + " is in the target." + col.collider.tag);
        if (col.collider.tag == "Throwable")
        {
            Vector3 targetedPos = col.collider.gameObject.transform.position;
            m_targetedDir = targetedPos - transform.position;
            // ignore the (up) y direction
            m_targetedDir.y = 0;
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
        // react to hitting geysers etc
        else if (col.collider.tag == "Trigger" || col.collider.tag == "Movable")
        {
            // state = State.Interrupted;
            // Vector3 awayFromHit = transform.position - col.transform.position;
            // awayFromHit.y = 0;
            // awayFromHit = awayFromHit.normalized;
            // hardcoded for now
            // interruptPos = new Vector3Wrapper(transform.position + awayFromHit * 5f);
            // SetDestination(interruptPos.vector);
            // gotHit = true;
            // Debug.Log("damage subrout");
            // MonsterDamaged();
        }

        // not allow monster to push boulder
        if (col.gameObject.name.Contains("Boulder") ||
            col.gameObject.name.Contains("boulder") )
                col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    bool PlayerReachable()
    {
        NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        NavMeshHit hit;
        bool hasPath = agent.CalculatePath(player.transform.position, path);
        bool close = NavMesh.SamplePosition(player.transform.position, out hit, 2f, 1 << NavMesh.GetNavMeshLayerFromName("Walkable"));
        // Debug.Log(hasPath + "" + close);
        return hasPath || close;
    }

    IEnumerator playerInvincible()
    {
        yield return StartCoroutine(playerScript.waitNextDamage(3)  );
        playerScript.stamina = playerScript.maxStamina;
        damageCoroutine = null;
    }

    IEnumerator MonsterDamaged()
    {
        yield return new WaitForSeconds(3);

        gotHit = false;
    }

    void MoveToRandomPos()
    {
        agent.ResetPath();
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        Vector3 finalPosition = SampleRandomPos();
        SetDestination(finalPosition);
        //agent.SetDestination(finalPosition);
    }

    void MoveToPos(Vector3 pos)
    {
        Vector3 position = SamplePos(pos);
        SetDestination(pos);
    }

    void SetDestination(Vector3 pos)
    {
        // UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        Vector3 finalPos = SamplePos(pos);
        finalPos.y = transform.position.y;
        agent.SetDestination(finalPos);
        // agent.CalculatePath(pos, path);
        // agent.SetPath(path);
    }

    Vector3 SampleRandomPos()
    {
        float walkRadius = 80f;//Random.Range(1f, 80f);
        float x = Random.Range(-85f, 15f);
        float z = Random.Range(-5f, 90f);
        // Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        // Vector3 center = new Vector3(-20f, transform.position.y, 20f);
        // randomDirection += center;
        Vector3 randomDirection = new Vector3(x, transform.position.y, z);
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1 << NavMesh.GetNavMeshLayerFromName("Walkable"));
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }

    Vector3 SamplePos(Vector3 dest, float radius=80f)
    {
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(dest, out hit, radius, 1 << NavMesh.GetNavMeshLayerFromName("Walkable"));
        return hit.position;
    }

    void ReactToPlayer()
    {
        if (IsPhaseOne())
            phase = Phase.Two;
        
        Vector3 playerPos = player.transform.position;
        Vector3 toPlayer = playerPos - transform.position;
        toPlayer.y = 0;
        toPlayer = toPlayer.normalized;

        Vector3 targetPos;
        RaycastHit hitInfo;
        NavMeshHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

        // check if player can be directly charged at
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y = 0.5f;
        bool blocked = Physics.Raycast(raycastOrigin, toPlayer, out hitInfo, maxChargeDistance) && hitInfo.collider.tag != "Player";
        bool noStraightPath = NavMesh.Raycast(transform.position, playerPos, out hit, NavMesh.AllAreas);
        if (hit.distance < 2f)
            noStraightPath = false;

        if (!blocked && !noStraightPath)
        {
            if (Physics.Raycast(raycastOrigin, toPlayer, out hitInfo, Mathf.Infinity, layerMask) )
                targetPos = transform.position + Mathf.Min(maxChargeDistance, (hitInfo.point - raycastOrigin).magnitude ) * toPlayer;
            else
                targetPos = transform.position + maxChargeDistance * toPlayer;
            state = State.Charge;
            SetChargeSpeed();
        }
        // cannot charge at player directly
        else
        {
            targetPos = player.transform.position;
            UnsetChargeSpeed();
            // TODO: if cannot directly charge at player, what should the state be?
            state = State.Interrupted;
        }

        target = new Vector3Wrapper(targetPos);
        SetDestination(target.vector);
        //agent.SetDestination(target.vector);
    }

    void Charge()
    {
        // Vector3 tPos = target.vector;
        // Vector3 location = transform.position;
        // Vector3 tDir = tPos - location;
        // tDir.y = transform.forward.y;
        // tDir.Normalize();
        // Quaternion qRotate = Quaternion.LookRotation(tDir, transform.up);
        // transform.position = Vector3.MoveTowards(transform.position, tPos, Time.deltaTime * movingSpeed);
        // transform.rotation = Quaternion.Slerp(transform.rotation, qRotate, Time.deltaTime * turningSpeed);
    }

    public void Interrupt(Vector3 pos, bool fromPlayer = false)
    {
        if (state == State.Charge)
        {
            if (fromPlayer && playerReachable)
                interruptPos = new Vector3Wrapper(pos);
        }
        else
        {
            if (fromPlayer && !playerReachable)
                return;
            interruptPos = new Vector3Wrapper(pos);
            // if (state != State.Interrupted)
            //     StopAgent();
            SetDestination(interruptPos.vector);
            state = State.Interrupted;
        }
    }

    bool SensedPlayer()
    {
        return fovScript.PlayerInSight();
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

    // TODO: not hard code the speed
    void SetChargeSpeed()
    {
        agent.speed = 30f;
    }

    void UnsetChargeSpeed()
    {
        agent.speed = 5f;
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
             agent.remainingDistance <= agent.stoppingDistance + 1
             || !agent.hasPath;
    }
}
