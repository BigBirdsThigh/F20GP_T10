using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no enemy prefab assigned!");
        }
    }
}
