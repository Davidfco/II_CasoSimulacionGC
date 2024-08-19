using UnityEngine;

public class HealthBarAttachmentEnemies : MonoBehaviour
{
    [SerializeField] private GameObject healthBarEnemiesPrefab; // Assign the health bar prefab in the inspector
    private GameObject healthBarEnemiesInstance;

    void Start()
    {
        if (healthBarEnemiesPrefab != null)
        {
            // Instantiate the health bar and set it as a child of the player
            healthBarEnemiesInstance = Instantiate(healthBarEnemiesPrefab, transform.position, Quaternion.identity);
            healthBarEnemiesInstance.transform.SetParent(transform);

            // Set the health bar's position to be above the player
            RectTransform healthBarRectTransform = healthBarEnemiesInstance.GetComponent<RectTransform>();
            healthBarRectTransform.anchoredPosition = new Vector2(0, 2F); // Adjust this offset as needed
        }
    }

    void Update()
    {
        if (healthBarEnemiesInstance != null)
        {
            // Optionally update the position if you want to ensure it follows the player
            RectTransform healthBarRectTransform = healthBarEnemiesInstance.GetComponent<RectTransform>();
            healthBarRectTransform.anchoredPosition = new Vector2(0, 2F); // Adjust this offset as needed
        }
    }
}