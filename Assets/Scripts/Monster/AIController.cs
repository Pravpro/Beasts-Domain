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
    public AudioManagerMain audioManager;

    // public variables
    public GameObject player;
    public Animator animator;
    public float turningSpeed = 60.0f;
    public float movingSpeed  = 1.0f;
    public int maxHp = 2;
    public int hp = 2;
    public float maxChargeDistance = 50.0f;
    public float maxAoeDistance = 20.0f;
    public float maxMeleeDistance = 7.0f;
    public float maxMeleeAngle = 30f;
    public int maxMoss = 6;
    public float mossCD = 30f;

    // give some buffer time for player to get next damange

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
    public GameObject mossObj;

    // private variables
    private bool m_playerInSight = false;
    private bool m_roarPlayed = false;
    private bool gotHit = false;
    private Coroutine damageCoroutine = null;

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
        Attack,
        Defeated,
        Interrupted
    }
    private enum Phase
    {
        One,
        Two,
        Three
    }
    private enum SubState
    {
        Charge,
        AOE,
        Horn,
        Kick
    }
    private State state = State.Idle;
    private SubState subState = SubState.Horn;
    private Phase phase = Phase.One;

    private int throwedMoss = 0;
    private float nextThrowTime = 0f;
    private float throwDelay = 0.33f;
    private float mossCDTime = 0f;

    private float stopRestTime = 0f;

    private bool horned = false;
    private float nextRoarTime = 0f;

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

        mossCDTime = Time.time + mossCD;

        //lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.positionCount = 2;

        animator = gameObject.GetComponent<Animator>();

        // mask defaults to all areas and we don't want that
        UnsetChargeAreaMask();

        //Audio
        LocalizeAudio();

        audioManager.InvokeRepeating("Hoofsteps", 0.0f, 0.6f);
    }

    // void Update()
    // {
    //     if (target != null)
    //     {
    //         Vector3[] corners = {transform.position, target.vector};
    //         //lineRenderer.SetPositions(corners);
    //         //lineRenderer.enabled = true;
    //     }
    //     //else
    //         //lineRenderer.enabled = false;
    // }

    private void Update()
    {
        UpdateAnimator();
        // float theta = Vector3.Angle(Vector3.up, transform.up) * Mathf.Deg2Rad;
        // if (theta > 0)
        // {
        //     Vector3 side = Vector3.Cross(transform.up, Vector3.up);
        //     Vector3 upHillDir = Quaternion.AngleAxis(90, side) * transform.up;
        //     Vector3 force = rb.mass * 10f * Mathf.Sin(theta) * upHillDir.normalized;
        //     // Debug.Log("Up: " + transform.up + " Force: " + force);
        //     rb.AddForce(force);
        // }

        if (hp <= 0 || playerScript.hp <= 0)
        {
            // rb.isKinematic = true;
            agent.isStopped = true;
            return;
        }

        // ADRIAN LOOK HERE!!!! :)
        if (Time.time - scenLoadTime == 25f)
        {
            
        }

        Navigate();
    }

    public bool IsCharging()
    {
        return state == State.Attack && subState == SubState.Charge;
    }

    void Navigate()
    {
        // for debug purpose
        if (targetObj)
        {
            //agent.SetDestination(targetObj.transform.position);
            state = State.Attack;
            subState = SubState.Charge;
            SetChargeAreaMask();
            SetChargeSpeed();
            SetDestination(targetObj.transform.position);
            return;
        }

        if (gotHit)
            return;
        
        playerReachable = PlayerReachable();
        bool sensedPlayer = fovScript.PlayerInSight() && playerReachable; // !playerScript.IsInSafeZone();
        
        if (sensedPlayer)
        {
            if(!m_roarPlayed)
            {
                audioManager.PlayPhaseOne();
                m_roarPlayed = true;
            }
            interruptPos = new Vector3Wrapper(player.transform.position);
        }
        // Debug.Log(state + "  " + subState);
        switch(state)
        {
            case State.Idle:
                if (sensedPlayer) { ReactToPlayer(); return; }

                if (Time.time > stopRestTime)
                {
                    MoveToRandomPos();
                    state = State.RandomSearch;
                // TODO: rest for a few secs?
                }
                break;
            case State.RandomSearch:
                if (sensedPlayer) { ReactToPlayer(); return; }
                
                if (ReachedWanderDest()) {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        state = State.Idle;
                        stopRestTime = Time.time + Random.Range(3f, 6f);
                        agent.ResetPath();
                        return;
                    }
                    // if (IsPhaseOne())
                    //     state = State.Idle;
                    // else {
                        MoveToRandomPos();
                    // }
                }
                break;
            case State.Attack:
                UpdateAttackSubStateMachine();
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

    void UpdateAnimator()
    {
        bool isIdle = (state == State.Idle);
        bool isAttack = (state == State.Attack);
        bool isCharge = (isAttack && subState == SubState.Charge);
        animator.SetBool("IsIdle", isIdle);
        animator.SetBool("IsAttacking", isAttack);
        animator.SetBool("IsCharging", isCharge);
        animator.SetBool("IsAOE", isAttack && subState == SubState.AOE);
        animator.SetBool("IsHorn", isAttack && subState == SubState.Horn);
        animator.SetBool("IsWalking", state == State.RandomSearch || state == State.Interrupted);
    }

    void UpdateAttackSubStateMachineStates()
    {
        if (!PlayerReachable()) { state = State.Interrupted; return;}
        
        float d = DistanceToPlayer();
        float angle = AngleToPlayer();

        if (d < maxMeleeDistance && angle <= 30)
        {
            subState = SubState.Horn;
        }
        else if (d < maxAoeDistance)
        {
            subState = SubState.AOE;
        }
        else if (d < maxChargeDistance)
        {
            bool canCharge = PrepareCharge();
            if (canCharge)
            {
                subState = SubState.Charge;
                SetChargeSpeed();
                SetChargeAreaMask();
                SetDestination(target.vector);
            }
        }
    }

    void UpdateAttackSubStateMachine()
    {
        // Debug.Log(state + " " + subState);
        if (subState != SubState.Charge)
            UpdateAttackSubStateMachineStates();
        
        switch (subState)
        {
            case SubState.Charge:
                if (ReachedWanderDest())
                {
                    UnsetChargeSpeed();
                    UnsetChargeAreaMask();
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
                    UpdateAttackSubStateMachineStates();
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
                else
                {
                    // SetChargeSpeed();
                    // SetChargeAreaMask();
                }
                break;
            case SubState.AOE:
                if (throwedMoss < maxMoss && Time.time > mossCDTime)
                    ThrowMoss();
                else
                {
                    SetDestination(interruptPos.vector);
                    
                    // if (interruptPos != null)
                    // {
                    //     SetDestination(interruptPos.vector);
                    //     //agent.SetDestination(interruptPos.vector);
                    //     state = State.Interrupted;
                    // }
                    // else
                    // {
                    //     state = State.RandomSearch;
                    // }
                }
                break;
            case SubState.Horn:
                if (!horned)
                    StartCoroutine( HornPlayer() );
                break;
            default:
                break;
        }
    }

    IEnumerator HornPlayer()
    {
        horned = true;
        audioManager.Play(audioManager.flare, 0.8f);
        yield return new WaitForSeconds(1);

        if (DistanceToPlayer() <= maxMeleeDistance && AngleToPlayer() <= 30f)
            DamagePlayer();
        
        horned = false;        
    }

    bool PrepareCharge()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 toPlayer = playerPos - transform.position;
        toPlayer.y = 0;
        toPlayer = toPlayer.normalized;

        Vector3 targetPos;
        RaycastHit hitInfo;
        NavMeshHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Player"));

        // check if player can be directly charged at
        Vector3 raycastOrigin = transform.position + new Vector3(0,3,0);
        raycastOrigin.y = 0.5f;
        bool blocked = Physics.Raycast(raycastOrigin, toPlayer, out hitInfo, maxChargeDistance) && hitInfo.collider.tag != "Player";
        bool noStraightPath = NavMesh.Raycast(raycastOrigin, playerPos - toPlayer * 2f, out hit, NavMesh.AllAreas);
        if (hit.distance < 2f)
            noStraightPath = false;

        if (!noStraightPath && blocked)
            Debug.Log("nononononono");

        if (!noStraightPath)
        {
            // if (Physics.Raycast(raycastOrigin, toPlayer, out hitInfo, Mathf.Infinity, layerMask) )
            //     targetPos = transform.position + Mathf.Min(maxChargeDistance, (hitInfo.point - raycastOrigin).magnitude ) * toPlayer;
            // else
            //     targetPos = transform.position + maxChargeDistance * toPlayer;
            NavMesh.Raycast(raycastOrigin, playerPos + toPlayer * 1000, out hit, NavMesh.AllAreas);
            targetPos = raycastOrigin + Mathf.Min(maxChargeDistance, Mathf.Max(hit.distance, agent.stoppingDistance + 2f) ) * toPlayer;
            //hoof drag before charge
            audioManager.Play(audioManager.hoof, 0.7f);
        }
        // cannot charge at player directly
        else
        {
            targetPos = player.transform.position;
            UnsetChargeSpeed();
            UnsetChargeAreaMask();
            // TODO: if cannot directly charge at player, what should the state be?
            // state = State.Interrupted;
            subState = SubState.AOE;
        }

        target = new Vector3Wrapper(targetPos);
        return !noStraightPath;
        //agent.SetDestination(target.vector);
    }

    void ThrowMoss()
    {
        if (Time.time <= nextThrowTime)
            return;
        Vector2 randVec2 = Random.insideUnitCircle.normalized;
        // 1f hardcoded!!! need a better way
        Vector3 dir = new Vector3(randVec2.x, 1f, randVec2.y);
        GameObject moss = Instantiate(mossObj, transform.position + 3f * dir, transform.rotation);
        moss.GetComponent<Rigidbody>().velocity += dir * 5f;
        throwedMoss += 1;
        nextThrowTime = Time.time + throwDelay;
        audioManager.Play(audioManager.flare, 0.8f);
        if (throwedMoss == maxMoss)
        {
            mossCDTime = Time.time + mossCD;
            throwedMoss = 0;
        }
    }

    void LocalizeAudio()
    {
        audioManager.roar1 = audioManager.Localize(gameObject, audioManager.roar1);
        audioManager.hurt = audioManager.Localize(gameObject, audioManager.hurt);
        audioManager.flare = audioManager.Localize(gameObject, audioManager.flare);
        audioManager.hoof = audioManager.Localize(gameObject, audioManager.hoof);
        audioManager.hoofSteps = audioManager.Localize(gameObject, audioManager.hoofSteps);
        audioManager.defeat = audioManager.Localize(gameObject, audioManager.defeat);
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
            Vector3 playerPos = player.transform.position;
            if (IsCharging() || playerPos.y > transform.position.y + 1f)
                DamagePlayer();
        }
    }

    bool PlayerReachable()
    {
        NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        NavMeshHit hit;
        bool hasPath = agent.CalculatePath(player.transform.position, path);
        int mask = ~(1 << NavMesh.GetAreaFromName("Not Walkable"));
        bool close = NavMesh.SamplePosition(player.transform.position, out hit, 2f, mask);
        // Debug.Log(hasPath + "" + close);
        return hasPath || close;
    }

    IEnumerator playerInvincible()
    {
        yield return StartCoroutine(playerScript.waitNextDamage(3) );
        playerScript.stamina = playerScript.maxStamina;
        damageCoroutine = null;
    }

    IEnumerator TimedDamageRecoil()
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
        finalPos.y = transform.position.y + 3f;
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
        Vector3 randomDirection = new Vector3(x, transform.position.y + 3f, z);
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1 << NavMesh.GetAreaFromName("Walkable"));
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }

    Vector3 SamplePos(Vector3 dest, float radius=80f)
    {
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(dest, out hit, radius, 1 << NavMesh.GetAreaFromName("Walkable"));
        return hit.position;
    }

    void ReactToPlayer()
    {
        if (IsPhaseOne())
            phase = Phase.Two;

        state = State.Attack;
        if (Time.time >= nextRoarTime)
        {
            audioManager.Play(audioManager.roar1);
            nextRoarTime = Time.time + Random.Range(3f, 10f);
        }
        UpdateAttackSubStateMachineStates();
    }

    void DamagePlayer()
    {
        if (damageCoroutine == null)
        {
            playerScript.m_Animator.SetTrigger("IsHit");
            playerScript.hp -= 1;

            if (playerScript.hp > 0)
            {
                audioManager.Play(audioManager.damage);
                damageCoroutine = StartCoroutine(playerInvincible());
            }
            else audioManager.Play(audioManager.death);
        }
    }

    public void Interrupt(Vector3 pos, bool fromPlayer = false)
    {
        if (state == State.Attack)
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

    public void TakeDamage(Vector3 hitPos)
    {

        // react to hitting geysers etc
        Vector3 awayFromHit = transform.position - hitPos;
        awayFromHit.y = 0;
        awayFromHit = awayFromHit.normalized;
        // hardcoded for now
        SetDestination(transform.position + awayFromHit * 5f);
        gotHit = true;
        state = State.Interrupted;
        UnsetChargeAreaMask();
        UnsetChargeSpeed();
        StartCoroutine(TimedDamageRecoil());
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

    float AngleToPlayer()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0;
        float angle = Vector3.Angle(transform.forward, toPlayer);
        return angle;
    }

    float DistanceToPlayer()
    {
        return DistanceTo(player.transform.position, true);
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

    void SetChargeAreaMask()
    {
        int areaMask = NavMesh.AllAreas;
        agent.areaMask = areaMask;
    }

    void UnsetChargeAreaMask()
    {
        int areaMask = ~(1 << NavMesh.GetAreaFromName("Geyser"));
        agent.areaMask = areaMask;
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
        // Debug.Log(!agent.hasPath +  "" + (agent.remainingDistance <= agent.stoppingDistance + 1));
        return //!agent.pathPending
             agent.remainingDistance <= agent.stoppingDistance + 1
             || !agent.hasPath;
    }
}
