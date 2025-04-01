using UnityEngine;
using System.Collections;

public class EnemyFSM : MonoBehaviour, IKillable
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
    private Animator anim;

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
    private WeaponHitbox swordHitbox;
    private float baseSpeed;


    private void Start()
    {
        baseSpeed = moveSpeed;
        anim = GetComponentInChildren<Animator>();
        swordHitbox = GetComponentInChildren<WeaponHitbox>();
        if (swordHitbox != null)
            swordHitbox.DisableHitbox(); // make sure it's off by default

        manager = FindObjectOfType<EnemyGroupManager>();
        if (manager != null)
            manager.RegisterEnemy(this);

        Player_Movement playerScript = FindObjectOfType<Player_Movement>();
        if (playerScript != null)
            targetPlayer = playerScript.transform;

        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            Renderer meshRenderer = GetComponentInChildren<Renderer>();
            if (meshRenderer != null)
            {
                Bounds bounds = meshRenderer.bounds;

                Vector3 size = col.size;
                size.y = bounds.size.y; // Set only height
                col.size = size;

                Vector3 center = col.center;
                center.y = bounds.center.y - transform.position.y;
                col.center = center;
            }
        }

    }


    private void Update()
    {
        RotateTowardsTarget();

        if (Input.GetKeyDown(KeyCode.T))
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
            anim.SetInteger("AttackIndex", 1);
            anim.SetTrigger("AttackTrigger");
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                MoveToTarget();
                // RotateTowardsTarget();
                HandleIdleAttackQueueing();
                break;

            case EnemyState.Attacking:
                // RotateTowardsTarget();
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
        targetPosition.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > stoppingThreshold)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            Vector3 avoidanceVector = GetBoidAvoidance();
            moveDirection = Vector3.Lerp(moveDirection, moveDirection + avoidanceVector, 0.8f);
            moveDirection.Normalize();

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if (anim != null)
            {
                anim.SetBool("isWalking", true); // Set isWalking true when moving
            }
        }
        else
        {
            if (anim != null)
            {
                anim.SetBool("isWalking", false); // Set false when stopped
            }
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


    private bool hasAttacked = false; // to not repeat attacks

    private void HandleAttackState()
    {
        moveSpeed = 10.6f;
        if (targetPlayer == null)
        {
            FinishAttack();
            return;
        }
        
        float dist = Vector3.Distance(transform.position, targetPlayer.position);
        if (dist > attackRange)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (dist <= attackRange)
        {
            if (!hasAttacked && anim != null)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", true); // Stay in combat state

                int attackIndex = Random.Range(0, 2);
                anim.SetInteger("AttackIndex", attackIndex);
                anim.SetTrigger("AttackTrigger");

                hasAttacked = true;
                moveSpeed = 0f;
                StartCoroutine(AttackAndReturnToIdle(attackIndex));
            }

            return;
        }

        // Still approaching player
        transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);

        if (anim != null)
            anim.SetBool("isWalking", true);

        approachTimer += Time.deltaTime;
        if (approachTimer >= maxApproachTime)
        {
            Debug.Log("Enemy failed to reach player in time.");
            FinishAttack();
        }
    }
    public void Die()
    {
        Debug.Log("EnemyFSM: I have died.");

        if (manager != null)
            manager.DeregisterEnemy(this);

        RoomTrigger room = GetComponentInParent<RoomTrigger>();
        if (room != null)
            room.activeEnemies.Remove(this); // let it clean up in Update

        Destroy(gameObject);
    }


    private IEnumerator AttackAndReturnToIdle(int index)
    {
        float fps = 60f;
        if(index == 0){
            
            yield return new WaitForSeconds(10f / fps);

            if (swordHitbox != null)
                swordHitbox.EnableHitbox();

            
            yield return new WaitForSeconds((36f - 10f) / fps);

            if (swordHitbox != null)
                swordHitbox.DisableHitbox();

            
            yield return new WaitForSeconds((60f - 36f) / fps);
        }else{
            yield return new WaitForSeconds(7f / fps);

            if (swordHitbox != null)
                swordHitbox.EnableHitbox();

            
            yield return new WaitForSeconds((31f - 7f) / fps); // wait from frame 7 to 23

            if (swordHitbox != null)
                swordHitbox.DisableHitbox();

            
            yield return new WaitForSeconds((69f - 31f) / fps); // wait out the rest of the anim
        }
        

        FinishAttack();
    }




    private void FinishAttack()
    {
        currentState = EnemyState.Idle;
        isAttacking = false;
        hasAttacked = false;
        idleTimer = 0f;
        hasQueuedForAttack = false;

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false); // Exit combat state
        }

        moveSpeed = baseSpeed;

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

        if (anim != null)
        {
            anim.SetBool("isWalking", true); // Start walking to player
            anim.SetBool("isAttacking", true); // Combat stance
        }

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