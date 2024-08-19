using UnityEngine;
using UnityEngine.UI;

public class MunitionBar : MonoBehaviour
{
    [SerializeField] private Image MunitionBarFill; // Reference to the HealthBarFill image
    [SerializeField] private float maxAmmo; // Maximum health value
    private float currentAmmo;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateMunition(currentAmmo, maxAmmo);
    }


    public void UpdateMunition(float currentMunition, float maxMunition)
    {
        MunitionBarFill.fillAmount = currentAmmo / maxAmmo;
    }

    public float CurrentAmmo
    {
        get { return currentAmmo; }
    }

    public float MaxAmmo
    {
        get { return maxAmmo; }
    }

    
}