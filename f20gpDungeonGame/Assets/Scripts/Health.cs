using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    [Header("State Flags")]
    public bool isBlocking = false;

    private IKillable deathHandler;

    [Header("Damage Feedback")]
    public float flashDuration = 0.2f;
    public Color flashColor = Color.red;
    private Material matInstance;
    private Color originalColor;
    private Coroutine flashRoutine;

    [Header("Debug")]
    public bool destroyOnDeath = true;

    private void Awake()
    {
        currentHealth = maxHealth;
        deathHandler = GetComponent<IKillable>();

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            matInstance = rend.material; // creates an instance, safe to modify
            originalColor = matInstance.color;
        }
    }

    public void TakeDamage(int amount)
    {
        // Block check
        if (isBlocking)
        {
            Debug.Log($"{gameObject.name} blocked the attack!");
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}");

        if (matInstance != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(FlashMaterial());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private IEnumerator FlashMaterial()
    {
        matInstance.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        matInstance.color = originalColor;
    }


    public float getHealth(){
        return currentHealth;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        bool usedExternalKill = false;

        // Check for IKillable on this object
        if (deathHandler != null)
        {
            deathHandler.Die();
            usedExternalKill = true;
        }
        // Else look for IKillable on parent
        else if (transform.parent != null)
        {
            IKillable parentKillable = transform.parent.GetComponent<IKillable>();
            if (parentKillable != null)
            {
                parentKillable.Die();
                usedExternalKill = true;
            }
            else
            {
                // As a fallback, call parent's Health.Die if it exists
                Health parentHealth = transform.parent.GetComponent<Health>();
                if (parentHealth != null && parentHealth != this) // avoid recursion
                {
                    parentHealth.TakeDamage(currentHealth); // Force death
                    usedExternalKill = true;
                }
            }
        }
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name}'s health reset to {maxHealth}.");
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
