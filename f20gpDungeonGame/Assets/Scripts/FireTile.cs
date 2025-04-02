using UnityEngine;
using System.Collections;

public class FireTile : MonoBehaviour
{
    private FireRoom fr;
    bool canDamage = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fr = FindAnyObjectByType<FireRoom>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && fr.getFireState() && canDamage)
        {
            other.GetComponent<Health>().TakeDamage(1);
            Debug.Log("Player damaged");

            canDamage = false;
            StartCoroutine(ResetDamageCooldown());
        }
    }

    private IEnumerator ResetDamageCooldown()
    {
        yield return new WaitForSeconds(1f);
        canDamage = true;
    }
}
