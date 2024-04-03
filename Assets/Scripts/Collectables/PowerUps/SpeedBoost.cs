using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float multiplier = 2f; // Set your desired speed boost multiplier
    [SerializeField] private float duration = 5f; // Set your desired speed boost duration

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PlayerMovement component from the collider's GameObject
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        // Check if the PlayerMovement component is found
        if (playerMovement != null)
        {
            // Apply speed boost to the player with the specified multiplier and duration
            playerMovement.ApplySpeedBoost(multiplier, duration);
            
            // Destroy this GameObject (the speed boost power-up)
            Destroy(gameObject);
        }
    }
}
