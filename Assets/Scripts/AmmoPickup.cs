using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount; // Amount of ammo this pickup provides
    [SerializeField] private float lifetime; // Time before the pickup disappears

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the pickup after its lifetime
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player picked up ammo");

            CharacterController2D playerController = other.GetComponent<CharacterController2D>();
            if (playerController != null)
            {
              
                Destroy(gameObject); // Destroy the ammo pickup after being collected
            }
        }
    }


}