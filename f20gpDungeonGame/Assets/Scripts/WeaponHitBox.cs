using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1;

    private bool canHit = false;
    private bool hasHit = false;
    private Collider hitboxCollider;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider != null)
            hitboxCollider.enabled = false; // Start disabled
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered with: {other.name}");

        // Ignore anything on the "Enemy" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log($"{other.name} is on Enemy layer. Ignored.");
            return;
        }

        if (!canHit || hasHit) return;

        Health targetHealth = other.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            hasHit = true;

            Debug.Log($"{gameObject.name} hit {other.name} for {damage} damage.");
        }
        else
        {
            Debug.Log($"{other.name} has no Health component.");
        }
    }



    public void EnableHitbox()
    {
        canHit = true;
        hasHit = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = true;

        Debug.Log($"{gameObject.name} hitbox enabled.");
    }

    public void DisableHitbox()
    {
        canHit = false;

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;

        Debug.Log($"{gameObject.name} hitbox disabled.");
    }
}
