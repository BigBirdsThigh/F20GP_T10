using UnityEngine;
using System.Collections.Generic;

public class LaserRoomTrigger : MonoBehaviour
{
    private bool hasOpened = false;
    private bool hasTriggered = false;

    [Header("Debug View")]
    public LinkedList<GameObject> doorsInRoom = new LinkedList<GameObject>();

    private void Awake()
    {        
        doorsInRoom = find_doors();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;

        // Spawn enemies from each child spawner
        EnemySpawner[] spawners = GetComponentsInChildren<EnemySpawner>();
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.SpawnEnemy();
        }

        // Find doors nearby
        doorsInRoom = find_doors();

        foreach (GameObject door in doorsInRoom)
        {
            door.SendMessage("CloseDoor", SendMessageOptions.DontRequireReceiver);
        }

        // Disable trigger
        GetComponent<Collider>().enabled = false;
    }


    public void TriggerOpen()
    {
        if (hasOpened) return;
        hasOpened = true;

        Debug.Log("[RoomTrigger] Triggered by laser. Opening doors.");

        foreach (GameObject door in doorsInRoom)
        {
            if (door != null)
                door.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
        }
    }

    private LinkedList<GameObject> find_doors()
    {
        LinkedList<GameObject> doors = new LinkedList<GameObject>();
        int req_distance = 33;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (Vector3.Distance(transform.position, go.transform.position) < req_distance)
            {
                doors.AddLast(go);
            }
        }

        return doors;
    }
}
