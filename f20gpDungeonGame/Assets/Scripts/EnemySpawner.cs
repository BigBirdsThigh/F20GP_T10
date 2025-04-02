using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} has no enemy prefab assigned!");
            return;
        }

        GameObject enemyGO = Instantiate(enemyPrefab, transform.position, transform.rotation);

        // Set the RoomTrigger as parent for easier tracking
        Transform roomTrigger = transform.parent;
        if (roomTrigger != null)
        {
            enemyGO.transform.SetParent(roomTrigger); // make spawned enemy a child of RoomTrigger

            RoomTrigger trigger = roomTrigger.GetComponent<RoomTrigger>();
            if (trigger != null)
            {
                EnemyFSM fsm = enemyGO.GetComponent<EnemyFSM>();
                if (fsm != null)
                    trigger.RegisterEnemy(fsm);

                WeepingAngel angel = enemyGO.GetComponent<WeepingAngel>();
                if (angel != null)
                    trigger.RegisterAngel(angel);
            }
        }
    }
}
