using UnityEngine;

public class HigherJump : MonoBehaviour
{
    [SerializeField] private float jumpMultiplier = 1.5f; // Multiplier for the higher jump

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PlayerMovement component from the collider's GameObject
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        // Check if the PlayerMovement component is found
        if (playerMovement != null)
        {
            // Apply the higher jump multiplier to the player's jump
            playerMovement.ApplyHigherJump(jumpMultiplier);
            
            // Destroy this GameObject (the higher jump power-up)
            Destroy(gameObject);
        }
    }
}
