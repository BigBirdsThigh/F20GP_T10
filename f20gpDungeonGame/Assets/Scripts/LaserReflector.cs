using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserReflector : MonoBehaviour
{
    public float reflectDistance = 100f;
    private LineRenderer lineRenderer;
    public Material LaserMat;

    private bool isBeingHit = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = LaserMat;
        lineRenderer.positionCount = 0; // Start disabled
    }

    private void Update()
    {
        if (!isBeingHit)
        {
            // Laser not hitting this reflector anymore, clear the line
            lineRenderer.positionCount = 0;
        }

        // Reset hit state for next frame
        isBeingHit = false;
    }

    public void ReceiveLaser(Vector3 origin, Vector3 direction)
    {
        isBeingHit = true;

        RaycastHit hit;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);

        if (Physics.Raycast(origin, direction, out hit, reflectDistance))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.TryGetComponent(out LaserReflector nextReflector))
            {
                nextReflector.ReceiveLaser(hit.point, Vector3.Reflect(direction, hit.normal));
            }

            if (hit.collider.TryGetComponent(out LaserTarget target))
            {
                target.HitByLaser();
            }
        }
        else
        {
            lineRenderer.SetPosition(1, origin + direction * reflectDistance);
        }
    }
}
