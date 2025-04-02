using UnityEngine;
using System.Collections.Generic;

public class RoomTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    [Header("Debug View")]
    public LinkedList<GameObject> doorsInRoom = new LinkedList<GameObject>();

    [Header("Enemy Tracking")]
    public List<EnemyFSM> activeEnemies = new List<EnemyFSM>();
    public List<WeepingAngel> activeAngels = new List<WeepingAngel>();

    private bool cleared = false;

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

    void Update()
    {
        if (cleared) return;

        activeEnemies.RemoveAll(e => e == null); // auto clean up destroyed enemies

        if (activeEnemies.Count == 0 && activeAngels.Count > 0)
        {
            cleared = true;
            Debug.Log("[RoomTrigger] All enemies cleared. Opening doors and cleaning up angels.");

            foreach (WeepingAngel angel in activeAngels)
            {
                if (angel != null)
                    Destroy(angel.gameObject);
            }

            foreach (GameObject door in doorsInRoom)
            {
                door.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void RegisterEnemy(EnemyFSM enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void RegisterAngel(WeepingAngel angel)
    {
        if (!activeAngels.Contains(angel))
            activeAngels.Add(angel);
    }

    private LinkedList<GameObject> find_doors(){
        LinkedList<GameObject> doors = new LinkedList<GameObject>();
        int req_distance = 33;
        
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Door")) {
            if (Vector3.Distance(this.gameObject.transform.position, go.transform.position) < req_distance)
            {
                    doors.AddLast(go);
            }
        }

        return doors;
    }
}
