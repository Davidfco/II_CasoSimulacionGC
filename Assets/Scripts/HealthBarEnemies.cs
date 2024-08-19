using UnityEngine;
using UnityEngine.UI;

public class HealthBarEnemies : MonoBehaviour
{
    [SerializeField] private Image healthBarFill; // Reference to the HealthBarFill image
    [SerializeField] private float maxHealth; // Maximum health value
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
    }

    public float HealthPercentage
    {
        get { return currentHealth / maxHealth; }
    }
}