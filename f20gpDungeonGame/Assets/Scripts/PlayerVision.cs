using UnityEngine;
using System.Collections.Generic;

public class PlayerVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewAngle = 60f;
    public float viewDistance = 10f;
    public LayerMask enemyLayer;
    public float eyeHeight = 0.6f;

    private List<WeepingAngel> currentlySeen = new List<WeepingAngel>();

    void Update()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        List<WeepingAngel> seenThisFrame = new List<WeepingAngel>();

        Collider[] hits = Physics.OverlapSphere(origin, viewDistance, enemyLayer);

        foreach (Collider col in hits)
        {
            // this prevents the enemy from thinking its not seen when it gets too close
            Vector3 targetPoint = col.ClosestPoint(origin);
            Vector3 dirToTarget = (targetPoint - origin).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle <= viewAngle / 2f)
            {
                WeepingAngel angel = col.GetComponent<WeepingAngel>();
                if (angel != null)
                {
                    seenThisFrame.Add(angel);

                    if (!currentlySeen.Contains(angel))
                    {
                        Debug.Log($"[Vision] Now seeing: {angel.name}");
                        angel.Freeze();
                    }
                }
            }
        }

        foreach (WeepingAngel angel in currentlySeen)
        {
            if (!seenThisFrame.Contains(angel))
            {
                Debug.Log($"[Vision] No longer seeing: {angel.name}");
                angel.Unfreeze();
            }
        }

        currentlySeen = seenThisFrame;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, viewDistance);

        Gizmos.color = Color.yellow;
        Vector3 leftLimit = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawRay(origin, leftLimit * viewDistance);
        Gizmos.DrawRay(origin, rightLimit * viewDistance);
    }
}
