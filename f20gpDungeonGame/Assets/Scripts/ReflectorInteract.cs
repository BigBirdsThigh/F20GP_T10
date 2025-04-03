using UnityEngine;

public class ReflectorInteract : MonoBehaviour
{
    public float rotateSpeed = 30f;         // Degrees per second
    public float interactRange = 2f;        // How close player needs to be
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactRange && Input.GetMouseButton(1)) // Right click held
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
