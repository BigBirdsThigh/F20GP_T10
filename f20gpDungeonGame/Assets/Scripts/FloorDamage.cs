using UnityEngine;

public class FloorDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Floor collided with: {other.name}");

        /* Ignore if other object is in the ignored layer(s)
        if (((1 << other.gameObject.layer) & ignoreLayer.value) != 0)
        {
            Debug.Log($"{other.name} is on ignored layer. Ignored.");
            return;
        }*/

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(2);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 spawn_location = GameObject.Find("Dungeon").GetComponent<DungeonGen>().spawn_location;
            //GameObject dungeon_obj = GameObject.Find("Dungeon");
            //DungeonGen script = dungeon_obj.GetComponent<>
            player.transform.position = spawn_location;

            Debug.Log($"{gameObject.name} hit {other.name} for 1 damage.");
        }
        else
        {
            Debug.Log($"{other.name} has no Health component.");
        }
    }
}
