using UnityEngine;
using System.Collections.Generic;

public class RoomTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    private List<GameObject> doorsInRoom = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        // Trigger each EnemySpawner (each child is one spawn point)
        EnemySpawner[] spawners = GetComponentsInChildren<EnemySpawner>();
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.SpawnEnemy();
        }

        // Find and close doors nearby using parent transform
        Transform roomTransform = transform.parent;
        doorsInRoom = FindNearbyDoors(roomTransform);

        foreach (GameObject door in doorsInRoom)
        {
            door.SendMessage("CloseDoor", SendMessageOptions.DontRequireReceiver);
        }

        // Disable trigger to prevent retriggering
        GetComponent<Collider>().enabled = false;
    }

    private List<GameObject> FindNearbyDoors(Transform roomTransform)
    {
        List<GameObject> doors = new List<GameObject>();
        int reqDistance = 33;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Door"))
        {
            float dist = Vector3.Distance(roomTransform.position, go.transform.position);
            if (dist < reqDistance)
            {
                doors.Add(go);
            }
        }

        return doors;
    }
}
