using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Debug")]
    public bool destroyOnDeath = true;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

   
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

   
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}");
    }

   
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");


        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
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
