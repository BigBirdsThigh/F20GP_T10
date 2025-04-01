using UnityEngine;
using System.Collections.Generic;

public class EnemyRoom : MonoBehaviour
{
    [Header("Settings")]
    public float doorSearchRadius = 33f;

    private List<EnemyFSM> trackedEnemies = new List<EnemyFSM>();
    private List<WeepingAngel> weepingAngels = new List<WeepingAngel>();
    private List<GameObject> doorsInRoom = new List<GameObject>();

    private bool enemiesCleared = false;

    private void Start()
    {
        doorsInRoom = FindNearbyDoors();
        Debug.Log($"[EnemyRoom] Ready. Waiting for enemies...");
    }

    private void Update()
    {
        if (enemiesCleared) return;

        // Clean up dead enemies
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
                door.SendMessage("OpenDoor", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private List<GameObject> FindNearbyDoors()
    {
        List<GameObject> doors = new List<GameObject>();
        Vector3 center = transform.position;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (Vector3.Distance(center, go.transform.position) < doorSearchRadius)
            {
                doors.Add(go);
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
        Gizmos.DrawWireSphere(transform.position, doorSearchRadius);
    }
}
