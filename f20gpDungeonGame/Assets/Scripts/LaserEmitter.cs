using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    public float laserMaxDistance = 100f;
    private LineRenderer lineRenderer;
    public Material LaserMat;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = LaserMat;
        float baseWidth = 0.5f;
        float flicker = Mathf.Sin(Time.time * 30f) * 0.01f;
        lineRenderer.startWidth = baseWidth + flicker;
        lineRenderer.endWidth = baseWidth + flicker;

    }

    void Update()
    {
        FireLaser();
    }

    void FireLaser()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit hit;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, origin);

        if (Physics.Raycast(origin, direction, out hit, laserMaxDistance))
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.TryGetComponent(out LaserReflector reflector))
            {
                reflector.ReceiveLaser(hit.point, Vector3.Reflect(direction, hit.normal));
            }

            if (hit.collider.TryGetComponent(out LaserTarget target))
            {
                target.HitByLaser();
            }
        }
        else
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, origin + direction * laserMaxDistance);
        }
    }
}
