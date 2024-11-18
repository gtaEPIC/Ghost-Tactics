using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    [Header("Death Settings")]
    [SerializeField] private float deathDelay = 0.5f; 
    [SerializeField] private GameObject deathEffect; // Optional: 

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    

    private void Start()
    {
        currentHealth = maxHealth;
    }

    

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Die()
    {
        if (deathEffect != null)
        {       
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, deathDelay);
    }
}