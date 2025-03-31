using UnityEngine;
using System.Collections.Generic;

public class NodePlacer : MonoBehaviour
{
    public Transform centralObject; // The central object around which nodes are placed
    public GameObject nodePrefab; // Prefab for the nodes to be placed
    public float ringRadius = 3f; // Radius of the ring around the central object
    public int desiredNodeCount = 6; // Number of nodes to place
    public float minSpacingBetweenNodes = 1f; // Minimum spacing between nodes
    public LayerMask obstacleLayerMask; // LayerMask defining which layers are considered obstacles

    private List<Transform> nodeTransforms = new List<Transform>();

    [Header("Adaptive Radius")]
    public float baseRingRadius = 5f;
    public float radiusIncrement = 0.2f;
    public float maxRingRadius = 8f;
    public float radiusLerpSpeed = 1f;

    private int successfulPlacementFrames = 0;
    public int framesRequiredToShrink = 10;
    



    void Start()
    {
        PlaceNodes();
    }

    private Vector3 lastCenterPos;
    private float lastRadius;
    private int lastNodeCount;

void Update()
{
    PlacementResult result = PlaceNodes();

    // Only allow success count to increase if placement changed
    if (result.success)
    {
        if (result.changed)
            successfulPlacementFrames++;

        if (successfulPlacementFrames >= framesRequiredToShrink)
        {
            ringRadius = Mathf.MoveTowards(ringRadius, baseRingRadius, radiusLerpSpeed * Time.deltaTime);
        }
    }
    else
    {
        successfulPlacementFrames = 0;
        ringRadius = Mathf.Min(ringRadius + radiusIncrement, maxRingRadius);
    }

    lastCenterPos = centralObject.position;
    lastRadius = ringRadius;
    lastNodeCount = desiredNodeCount;
}


struct PlacementResult
{
    public bool success;
    public bool changed;
}



  PlacementResult PlaceNodes()
{
    List<Vector3> newPositions = new List<Vector3>();
    float baseAngleStep = 360f / desiredNodeCount;
    int nodesPlaced = 0;

    for (int i = 0; i < desiredNodeCount; i++)
    {
        float baseAngle = i * baseAngleStep;
        bool placed = false;

        for (int offset = 0; offset <= 180; offset += 5)
        {
            foreach (int sign in new int[] { 1, -1 })
            {
                float angle = baseAngle + sign * offset;
                float angleRad = angle * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
                Vector3 pos = centralObject.position + dir * ringRadius;

                if (IsPositionValid(pos, newPositions))
                {
                    newPositions.Add(pos);
                    nodesPlaced++;
                    placed = true;
                    break;
                }
            }
            if (placed) break;
        }
    }

    if (nodesPlaced < desiredNodeCount)
        return new PlacementResult { success = false, changed = false };

    bool changed = PositionsChanged(newPositions);
    if (changed)
    {
        ClearNodes();
        foreach (var pos in newPositions)
        {
            GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity);
            nodeTransforms.Add(node.transform);
        }
    }

    return new PlacementResult { success = true, changed = changed };
}



    bool PositionsChanged(List<Vector3> newPositions)
    {
        if (newPositions.Count != nodeTransforms.Count)
            return true;

        for (int i = 0; i < newPositions.Count; i++)
        {
            if (Vector3.Distance(newPositions[i], nodeTransforms[i].position) > 0.01f)
                return true;
        }

        return false;
}



    bool IsPositionValid(Vector3 position, List<Vector3> pendingNodes)
    {
        if (Physics.Raycast(centralObject.position, (position - centralObject.position).normalized, out RaycastHit hit, ringRadius, obstacleLayerMask))
            return false;

        foreach (Vector3 pos in pendingNodes)
        {
            if (Vector3.Distance(position, pos) < minSpacingBetweenNodes)
                return false;
        }

        return true;
    }


    void ClearNodes()
    {
        foreach (Transform nodeTransform in nodeTransforms)
        {
            Destroy(nodeTransform.gameObject);
        }
        nodeTransforms.Clear();
    }
}
