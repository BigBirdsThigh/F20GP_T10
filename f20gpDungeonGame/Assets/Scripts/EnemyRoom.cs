using UnityEngine;
using System.Collections.Generic;

public class EnemyRoom : MonoBehaviour
{
    [Header("Settings")]
    public float doorSearchRadius = 33f;    
    public Transform doorSearchOrigin; 

    private List<EnemyFSM> trackedEnemies = new List<EnemyFSM>();
    private List<WeepingAngel> weepingAngels = new List<WeepingAngel>();
    [Header("Debug View")]
    public LinkedList<GameObject> doorsInRoom = new LinkedList<GameObject>();

    private bool enemiesCleared = false;

    private void Start()
    {
        doorsInRoom = FindNearbyDoors();
        Debug.Log($"[EnemyRoom] Ready. Waiting for enemies...");
    }

    private void Update()
    {
        if (enemiesCleared) return;

        trackedEnemies.RemoveAll(e => e == null);

        if (trackedEnemies.Count == 0)
        {
            enemiesCleared = true;
            Debug.Log("[EnemyRoom] All enemies cleared!");

            foreach (WeepingAngel angel in weepingAngels)
            {
                if (angel != null)
                    Destroy(angel.gameObject);
            }

            foreach (GameObject door in doorsInRoom)
            {
                if (door != null)
                    door.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private LinkedList<GameObject> FindNearbyDoors()
    {
        LinkedList<GameObject> doors = new LinkedList<GameObject>();
        Vector3 origin = doorSearchOrigin != null ? doorSearchOrigin.position : transform.position;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (Vector3.Distance(origin, go.transform.position) < doorSearchRadius)
            {
                doors.AddLast(go);
            }
        }

        return doors;
    }

    public void RegisterEnemy(EnemyFSM enemy)
    {
        if (!trackedEnemies.Contains(enemy))
        {
            trackedEnemies.Add(enemy);
        }
    }

    public void RegisterAngel(WeepingAngel angel)
    {
        if (!weepingAngels.Contains(angel))
        {
            weepingAngels.Add(angel);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = doorSearchOrigin != null ? doorSearchOrigin.position : transform.position;
        Gizmos.DrawWireSphere(origin, doorSearchRadius);
    }
}
