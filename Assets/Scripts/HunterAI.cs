using UnityEngine;
using UnityEngine.AI;

public class HunterAI : MonoBehaviour
{
    [Header("Detection")]
    public float chaseRange = 30f;
    public float stopDistance = 1.5f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    public float patrolSpeed = 3.5f;
    public float patrolStopDistance = 0.3f;

    [Header("Chase")]
    public float chaseSpeed = 5f;

    [Header("Return To Spawn")]
    public Transform hunterSpawnPoint;
    public float idleTimeBeforeReturn = 10f;
    public float spawnArrivalDistance = 1f;

    private Transform player;
    private NavMeshAgent agent;

    private int currentPoint = 0;
    private float waitTimer = 0f;
    private float idleTimer = 0f;
    private float patrolTimer = 0f;

    private Vector3 lastPosition;
    private bool returningToSpawn = false;

    private enum State
    {
        Patrol,
        Chase
    }

    private State currentState = State.Patrol;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;

        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }
    }
    void Start()
    {
        SnapToNavMesh();
        MoveToNextPoint();
    }

    void SnapToNavMesh()
    {
        if (agent == null) return;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 100f, NavMesh.AllAreas))
            agent.Warp(hit.position);
    }

    void Update()
    {
        if (agent == null || !agent.enabled)
            return;

        if (!agent.isOnNavMesh)
        {
            SnapToNavMesh();
            return;
        }

        // กลับจุดเกิดก่อนทำอย่างอื่น
        if (returningToSpawn)
        {
            ReturnToSpawn();
            return;
        }

        FindPlayer();
    
        bool playerInRange = player != null &&
            Vector3.Distance(transform.position, player.position)
            <= chaseRange &&
            CanReachPlayer();

        currentState = playerInRange ? State.Chase : State.Patrol;

        if (currentState == State.Chase)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        CheckIfIdle();
    }

    void FindPlayer()
    {
        if (player != null)
            return;

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;
    }

    private float reachTimer = 0f;
    private bool playerReachable = true;

    bool CanReachPlayer()
    {
        reachTimer -= Time.deltaTime;
        if (reachTimer > 0f) return playerReachable;
        reachTimer = 0.5f;

        playerReachable = false;
        if (NavMesh.SamplePosition(player.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
                playerReachable = true;
        }
        return playerReachable;
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;

        Vector3 target = player.position;

        // ถ้าผู้เล่นยืนนอก NavMesh นิดหน่อย ให้เล็งจุดสีฟ้าที่ใกล้ที่สุดแทน
        if (NavMesh.SamplePosition(player.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            target = hit.position;

        agent.SetDestination(target);
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStopDistance;

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        currentPoint = Mathf.Clamp(
            currentPoint, 0, patrolPoints.Length - 1
        );

        if (patrolPoints[currentPoint] == null)
        {
            MoveToNextPoint();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            patrolPoints[currentPoint].position
        );

        if (distance <= patrolStopDistance + 0.2f)
        {
            agent.isStopped = true;
            waitTimer += Time.deltaTime;

            if (waitTimer >= patrolWaitTime)
            {
                waitTimer = 0f;
                MoveToNextPoint();
            }
        }
        else
        {
            agent.isStopped = false;
            
            // --- ส่วนที่แก้ไขให้จุดเป้าหมายดึงเข้าหาพื้นที่เดินได้อัตโนมัติ ---
            Vector3 target = patrolPoints[currentPoint].position;
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                target = hit.position;
            }
            agent.SetDestination(target);
            // ---------------------------------------------------

            patrolTimer += Time.deltaTime;

            //// ยอมให้เดินไปถึงจุดที่ใกล้ที่สุดได้ (PathPartial) โดยไม่ข้ามจุดทันที
            bool unreachable = !agent.pathPending &&
                agent.pathStatus == NavMeshPathStatus.PathInvalid;

            if (unreachable || patrolTimer > 25f)
            {
                MoveToNextPoint();
            }
        }
    }

    void MoveToNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        // สุ่มจุดใหม่ (ไม่ซ้ำจุดเดิม)
        int next = Random.Range(0, patrolPoints.Length);

        if (patrolPoints.Length > 1)
        {
            int safety = 0;
            while (next == currentPoint && safety < 20)
            {
                next = Random.Range(0, patrolPoints.Length);
                safety++;
            }
        }

        currentPoint = next;
        waitTimer = 0f;
        patrolTimer = 0f;

        if (patrolPoints[currentPoint] != null)
        {
            agent.isStopped = false;
            
            // --- ส่วนที่แก้ไขให้จุดเป้าหมายดึงเข้าหาพื้นที่เดินได้อัตโนมัติ ---
            Vector3 target = patrolPoints[currentPoint].position;
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                target = hit.position;
            }
            agent.SetDestination(target);
            // ---------------------------------------------------
        }
    }

    void CheckIfIdle()
    {
        if (hunterSpawnPoint == null || returningToSpawn)
            return;

        // ไม่นับเวลาเมื่อผีหยุดตามปกติหรือไม่มีเส้นทาง
        if (agent.isStopped ||
            !agent.hasPath ||
            agent.pathPending ||
            agent.remainingDistance <=
                agent.stoppingDistance + 0.2f)
        {
            idleTimer = 0f;
            lastPosition = transform.position;
            return;
        }

        float movedDistance = Vector3.Distance(
            transform.position,
            lastPosition
        );

        if (movedDistance < 0.05f)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0f;
        }

        lastPosition = transform.position;

        if (idleTimer >= idleTimeBeforeReturn)
        {
            returningToSpawn = true;
            idleTimer = 0f;
            player = null;

            agent.isStopped = false;
            agent.ResetPath();

            bool pathSet = agent.SetDestination(
                hunterSpawnPoint.position
            );

            if (!pathSet)
            {
                returningToSpawn = false;
                Debug.LogWarning(
                    "Hunter: ไม่สามารถหาเส้นทางกลับจุดเกิดได้"
                );
            }
        }
    }

    void ReturnToSpawn()
    {
        if (hunterSpawnPoint == null)
        {
            returningToSpawn = false;
            return;
        }

        agent.speed = patrolSpeed;
        agent.stoppingDistance = spawnArrivalDistance;

        if (!agent.hasPath && !agent.pathPending)
        {
            if (!agent.SetDestination(hunterSpawnPoint.position))
            {
                returningToSpawn = false;
                return;
            }
        }

        agent.isStopped = false;

        if (!agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= spawnArrivalDistance)
        {
            returningToSpawn = false;
            idleTimer = 0f;
            waitTimer = 0f;
            lastPosition = transform.position;

            agent.ResetPath();
        }
    }
}