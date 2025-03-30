using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Idle, Attacking }
    public EnemyState currentState = EnemyState.Idle;

    public Transform targetObject; // The object the enemy moves to
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 5f;
    private float stoppingThreshold = 0.1f; 
    private bool hasQueuedForAttack = false;
    private float idleTimer = 0f;
    private float timeBeforeQueue = 1.5f; 


    public int numRays = 7; // Number of rays to cast in FOV
    public float rayDistance = 2.5f; // How far the rays shoot
    public float avoidanceInfluence = 0.5f; // How much avoidance affects movement
    public LayerMask avoidanceLayer; 
    private Transform targetPlayer;
    private float attackRange;
    private float approachTimer;
    private float maxApproachTime = 5f;
    private EnemyGroupManager manager;
    private bool isAttacking;

    private void Start()
    {
        manager = FindObjectOfType<EnemyGroupManager>();
        if (manager != null)
            manager.RegisterEnemy(this);

        Player_Movement playerScript = FindObjectOfType<Player_Movement>();
        if (playerScript != null)
            targetPlayer = playerScript.transform;
    }


    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                MoveToTarget();
                RotateTowardsTarget();
                HandleIdleAttackQueueing();
                break;

            case EnemyState.Attacking:
                RotateTowardsTarget();
                HandleAttackState();
                break;
        }
    }

    private void HandleIdleAttackQueueing()
    {
        if (hasQueuedForAttack || manager == null) return;

        idleTimer += Time.deltaTime;

        if (idleTimer >= timeBeforeQueue)
        {
            manager.EnqueueForAttack(this);
            hasQueuedForAttack = true;
        }
    }



    private void MoveToTarget()
    {
        if (targetObject == null) return;

        Vector3 targetPosition = targetObject.position;
        targetPosition.y = transform.position.y; // Keep Y constant to avoid bouncing

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stoppingThreshold)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

        
            Vector3 avoidanceVector = GetBoidAvoidance();
            moveDirection = Vector3.Lerp(moveDirection, moveDirection + avoidanceVector, 0.8f); // Blend movement & avoidance
            moveDirection.Normalize(); // Ensure it's a unit vector

            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }


    private void RotateTowardsTarget()
    {
        if (targetPlayer == null) return;

        Vector3 direction = targetPlayer.position - transform.position;
        direction.y = 0; // Only rotate on XZ
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    private void HandleAttackState()
    {
        if (targetPlayer == null)
        {
            FinishAttack();
            return;
        }

        float dist = Vector3.Distance(transform.position, targetPlayer.position);

        if (dist <= attackRange)
        {
            Debug.Log("Enemy reached player!");
            FinishAttack();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
        approachTimer += Time.deltaTime;

        if (approachTimer >= maxApproachTime)
        {
            Debug.Log("Enemy failed to reach player in time.");
            FinishAttack();
        }
    }

    private void FinishAttack()
    {
        currentState = EnemyState.Idle;
        isAttacking = false;
        idleTimer = 0f;
        hasQueuedForAttack = false;

        if (manager != null)
            manager.NotifyAttackFinished(this);
    }

    private Vector3 GetBoidAvoidance()
{
    if (Time.frameCount % 3 != 0) return Vector3.zero; // Skip frames for performance

    int rayCount = numRays; 
    float maxAngle = 360; 
    float stepAngle = 360f / rayCount;
    float detectionRange = rayDistance;

    Vector3 avoidanceDirection = Vector3.zero;
    Vector3 surfaceNormalDirection = Vector3.zero; // Used if all rays hit
    int validHits = 0;
    int totalHits = 0;
    Vector3 bestClearPath = transform.forward;
    float bestClearPathScore = -Mathf.Infinity;

    for (int i = 0; i < rayCount; i++)
    {
        float angle = -maxAngle / 2 + (stepAngle * i);
        Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, rayDirection, out hit, detectionRange, avoidanceLayer))
        {
            totalHits++;
            Vector3 awayFromObstacle = (transform.position - hit.point).normalized; // Move away from hit point
            avoidanceDirection += awayFromObstacle;
            validHits++;

            // Capture surface normal (use the first valid hit as reference)
            surfaceNormalDirection += hit.normal;

            Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red, 0.1f);
        }
        else
        {
            float pathScore = Vector3.Dot(rayDirection, transform.forward);
            if (pathScore > bestClearPathScore)
            {
                bestClearPathScore = pathScore;
                bestClearPath = rayDirection;
            }

            Debug.DrawRay(transform.position, rayDirection * detectionRange, Color.green, 0.1f);
        }
    }
    

    // **If only some rays hit, blend normal avoidance with best open path**
    if (validHits > 0)
    {
        avoidanceDirection /= validHits; // Average direction
        return (avoidanceDirection.normalized * 2.5f) + bestClearPath;
    }

    return bestClearPath; // Default to moving forward if no obstacles
}


    public void EnterAttackState(Transform player, float range, EnemyGroupManager mgr)
    {
        targetPlayer = player;
        attackRange = range;
        approachTimer = 0f;
        manager = mgr;
        isAttacking = true;
        currentState = EnemyState.Attacking;
        idleTimer = 0f;
        hasQueuedForAttack = false;

        Debug.Log("Enemy entering attack state!");
    }

    public EnemyState GetState()
    {
        return currentState;
    }



    public void SetTargetObject(Transform newTarget)
    {
        targetObject = newTarget;
    }
}