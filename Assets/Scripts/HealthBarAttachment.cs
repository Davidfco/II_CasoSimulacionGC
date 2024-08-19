using UnityEngine;

public class HealthBarAttachment : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab; // Assign the health bar prefab in the inspector
    private GameObject healthBarInstance;

    void Start()
    {
        if (healthBarPrefab != null)
        {
            // Instantiate the health bar and set it as a child of the player
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBarInstance.transform.SetParent(transform);

            // Set the health bar's position to be above the player
            RectTransform healthBarRectTransform = healthBarInstance.GetComponent<RectTransform>();
            healthBarRectTransform.anchoredPosition = new Vector2(0, 2F); // Adjust this offset as needed
        }
    }

    void Update()
    {
        if (healthBarInstance != null)
        {
            // Optionally update the position if you want to ensure it follows the player
            RectTransform healthBarRectTransform = healthBarInstance.GetComponent<RectTransform>();
            healthBarRectTransform.anchoredPosition = new Vector2(0, 2F); // Adjust this offset as needed
        }
    }
}