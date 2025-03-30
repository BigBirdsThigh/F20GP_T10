using UnityEngine;
using System.Collections.Generic;

public class EnemyGroupManager : MonoBehaviour
{
    public Transform player;
    public GameObject positionPlaceholderPrefab;
    public LayerMask obstacleLayerMask;

    [Header("Ring Settings")]
    public float minSpacingBetweenNodes = 1f;
    // public float minDistanceFromObstacle = 0.5f;

    [Header("Adaptive Radius")]
    public float baseRingRadius = 5f;
    public float radiusIncrement = 0.2f;
    public float maxRingRadius = 8f;
    public float radiusLerpSpeed = 1f;
    public int framesRequiredToShrink = 10;

    private float ringRadius;
    private int successfulPlacementFrames = 0;

    private Dictionary<EnemyFSM, Transform> enemyToNodeMapping = new Dictionary<EnemyFSM, Transform>();
    private List<Transform> ringNodes = new List<Transform>();
    private List<EnemyFSM> enemies = new List<EnemyFSM>();

    [Header("Attack Logic")]
    // public float attackPollRate = 1f;
    public float attackRange = 1.5f;
    [Tooltip("Minimum and maximum random delay before next enemy is allowed to attack.")]
    public float minAttackDelay = 0.5f;
    public float maxAttackDelay = 2.0f;

    private float nextAttackAllowedTime = 0f;

    private Queue<EnemyFSM> attackQueue = new Queue<EnemyFSM>();
    private EnemyFSM currentAttacker;
    // private float lastAttackPollTime;


    void Start()
    {
        ringRadius = baseRingRadius;
        ClearRing();
    }

    void Update()
    {
        PlacementResult result = PlaceNodes();

        if (result.success)
        {
            if (result.changed)
                successfulPlacementFrames++;

            if (successfulPlacementFrames >= framesRequiredToShrink)
                ringRadius = Mathf.MoveTowards(ringRadius, baseRingRadius, radiusLerpSpeed * Time.deltaTime);
        }
        else
        {
            successfulPlacementFrames = 0;
            ringRadius = Mathf.Min(ringRadius + radiusIncrement, maxRingRadius);
        }
        HandleAttackQueue();

    }

    public void RegisterEnemy(EnemyFSM enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void DeregisterEnemy(EnemyFSM enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            if (enemyToNodeMapping.TryGetValue(enemy, out Transform node))
            {
                ringNodes.Remove(node);
                if (node != null)
                    Destroy(node.gameObject);
            }

            enemyToNodeMapping.Remove(enemy);
        }
    }

    private PlacementResult PlaceNodes()
    {
        ClearRing();

        int count = enemies.Count;
        if (count == 0) return new PlacementResult { success = false, changed = false };

        List<Vector3> newPositions = new List<Vector3>();
        float baseAngleStep = 360f / count;
        int nodesPlaced = 0;

        for (int i = 0; i < count; i++)
        {
            float baseAngle = i * baseAngleStep;
            bool placed = false;

            for (int offset = 0; offset <= 180; offset += 5)
            {
                foreach (int sign in new int[] { 1, -1 })
                {
                    float angle = baseAngle + sign * offset;
                    float rad = angle * Mathf.Deg2Rad;
                    Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
                    Vector3 pos = player.position + dir * ringRadius;

                    if (IsPositionValid(pos, newPositions))
                    {
                        newPositions.Add(pos);
                        GameObject marker = Instantiate(positionPlaceholderPrefab, pos, Quaternion.identity);
                        ringNodes.Add(marker.transform);
                        placed = true;
                        nodesPlaced++;
                        break;
                    }
                }
                if (placed) break;
            }
        }

        if (nodesPlaced < count)
            return new PlacementResult { success = false, changed = false };

        AssignTargetsToEnemies();
        return new PlacementResult { success = true, changed = true };
    }

    private bool IsPositionValid(Vector3 position, List<Vector3> pendingNodes)
    {
        // Direction and distance to node
        Vector3 direction = (position - player.position).normalized;
        float distance = Vector3.Distance(player.position, position);

        // Check if the path from the player to the node is blocked
        if (Physics.Raycast(player.position, direction, out RaycastHit hit, distance, obstacleLayerMask))
            return false;

        // Enforce spacing between other pending nodes
        foreach (Vector3 other in pendingNodes)
        {
            if (Vector3.Distance(position, other) < minSpacingBetweenNodes)
                return false;
        }

        return true;
    }



    private void AssignTargetsToEnemies()
    {
        List<Transform> availableNodes = new List<Transform>(ringNodes);

        foreach (EnemyFSM enemy in enemies)
        {
            if (!enemyToNodeMapping.ContainsKey(enemy) || enemyToNodeMapping[enemy] == null)
            {
                if (availableNodes.Count > 0)
                {
                    Transform node = availableNodes[0];
                    enemy.SetTargetObject(node);
                    enemyToNodeMapping[enemy] = node;
                    availableNodes.RemoveAt(0);
                }
            }
        }
    }

    private void HandleAttackQueue()
    {
        if (Time.time < nextAttackAllowedTime)
            return;

        if (currentAttacker != null && currentAttacker.GetState() == EnemyFSM.EnemyState.Attacking)
            return;

        if (attackQueue.Count > 0)
        {
            currentAttacker = attackQueue.Dequeue();
            currentAttacker.EnterAttackState(player, attackRange, this);
        }
    }





    public void NotifyAttackFinished(EnemyFSM enemy)
    {
        if (enemy == currentAttacker)
        {
            currentAttacker = null;

            // Apply randomized delay BEFORE next attacker can be chosen
            float delay = Random.Range(minAttackDelay, maxAttackDelay);
            nextAttackAllowedTime = Time.time + delay;
        }
    }



    private void ClearRing()
    {
        foreach (Transform node in ringNodes)
        {
            if (node != null)
                Destroy(node.gameObject);
        }

        ringNodes.Clear();
        enemyToNodeMapping.Clear();
    }

    public void EnqueueForAttack(EnemyFSM enemy)
    {
        if (!attackQueue.Contains(enemy))
        {
            List<EnemyFSM> temp = new List<EnemyFSM>(attackQueue);

            int insertIndex = temp.Count == 0
                ? 0
                : Random.Range(1, temp.Count + 1); // Safe range

            temp.Insert(insertIndex, enemy);
            attackQueue = new Queue<EnemyFSM>(temp);
        }
    }



    private struct PlacementResult
    {
        public bool success;
        public bool changed;
    }
}
