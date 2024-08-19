using UnityEngine;

public class MedKitPickup : MonoBehaviour
{
    [SerializeField] private float healAmount; // Amount of health this med kit restores

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController2D playerController = other.GetComponent<CharacterController2D>();
            if (playerController != null)
            {
                playerController.Heal(healAmount); // Heal the player
                Destroy(gameObject); // Destroy the med kit after being collected
            }
        }
    }
}