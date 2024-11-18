using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI Components - Health")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("UI Components - Armor")]
    public Slider armorSlider;
    public TextMeshProUGUI armorText;

    [Header("Health System")]
    [SerializeField] private PlayerHealthController playerHealth;
    

    void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealthController reference is missing!");
            return;
        }

        
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
        healthText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";

        
        armorSlider.maxValue = playerHealth.MaxArmor;
        armorSlider.value = playerHealth.CurrentArmor;
        armorText.text = $"{playerHealth.CurrentArmor}/{playerHealth.MaxArmor}";

        
        playerHealth.OnHealthChanged += UpdateHealthBar;
        playerHealth.OnArmorChanged += UpdateArmorBar;
    }

    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    void UpdateArmorBar(int currentArmor, int maxArmor)
    {
        armorText.text = $"{currentArmor}/{maxArmor}";
        armorSlider.maxValue = maxArmor;
        armorSlider.value = currentArmor;
    }

    void OnDestroy()
    {
        
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
            playerHealth.OnArmorChanged -= UpdateArmorBar;
        }
    }
}
