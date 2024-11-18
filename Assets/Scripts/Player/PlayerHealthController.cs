using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Health")] 
    [SerializeField] private int maxHealth;
    public int MaxHealth => maxHealth;
    [SerializeField] private int currentHealth;
    public int CurrentHealth => currentHealth;

    [Header("Armor")]
    [SerializeField] private int maxArmor;
    public int MaxArmor => maxArmor;
    [SerializeField] private int currentArmor;
    public int CurrentArmor => currentArmor;

    private int damageBlockers = 0;

    public bool CanTakeDamage => damageBlockers <= 0;

    private bool isDead = false;
    public bool IsDead => isDead;

    [Header("Camera Shake")] 
    [SerializeField] private float intensity = 5f;
    [SerializeField] private float time = 0.25f;

    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnArmorChanged;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Heal(10);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddArmor(10);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (CanTakeDamage)
        {
            if (currentArmor > 0)
            {
                // Armor absorbs the damage first
                int damageToArmor = Mathf.Min(damageAmount, currentArmor);
                currentArmor -= damageToArmor;
                damageAmount -= damageToArmor;
                OnArmorChanged?.Invoke(currentArmor, maxArmor);
            }

            if (damageAmount > 0)
            {
                // Remaining damage is applied to health
                currentHealth -= damageAmount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);

                if (currentHealth <= 0 && !isDead)
                {
                    isDead = true;
                    // Handle player death logic here
                    Debug.Log("Player has died.");
                }
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} is shielded and cannot take damage.");
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddArmor(int armorAmount)
    {
        currentArmor += armorAmount;
        currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
        OnArmorChanged?.Invoke(currentArmor, maxArmor);
    }

    public void AddDamageBlocker()
    {
        damageBlockers++;
        Debug.Log("Damage blocker added. Total blockers: " + damageBlockers);
    }

    public void RemoveDamageBlocker()
    {
        damageBlockers = Mathf.Max(0, damageBlockers - 1);
        Debug.Log("Damage blocker removed. Total blockers: " + damageBlockers);
    }
}
