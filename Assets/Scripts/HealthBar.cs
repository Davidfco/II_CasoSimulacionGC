using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFillImage; // Image component for the health bar fill

    private void Start()
    {
        if (healthFillImage == null)
        {
            Debug.LogError("HealthFillImage is not assigned!");
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthFillImage != null)
        {
            float fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0f, 1f);
            healthFillImage.fillAmount = fillAmount;
            Debug.Log($"HealthBar Updated: Fill Amount = {fillAmount}"); // Debug line to check update
        }
        else
        {
            Debug.LogWarning("HealthFillImage is not assigned!");
        }
    }
}