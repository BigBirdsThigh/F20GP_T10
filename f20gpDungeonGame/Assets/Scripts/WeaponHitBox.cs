using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 1f;

    private bool canHit = false;
    private bool hasHit = false;
    private Collider hitboxCollider;
    public LayerMask ignoreLayer;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider != null)
            hitboxCollider.enabled = false; // Start disabled
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered with: {other.name}");

        // Ignore if other object is in the ignored layer(s)
        if (((1 << other.gameObject.layer) & ignoreLayer.value) != 0)
        {
            Debug.Log($"{other.name} is on ignored layer. Ignored.");
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
