using UnityEngine;

public class Invisibility : MonoBehaviour
{
    [SerializeField] private float duration = 10f; // Duration of invisibility

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PlayerMovement component from the collider's GameObject
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        // Check if the PlayerMovement component is found
        if (playerMovement != null)
        {
            // Apply invisibility to the player for the specified duration
            playerMovement.ApplyInvisibility(duration);
            
            // Destroy this GameObject (the invisibility power-up)
            Destroy(gameObject);
        }
    }
}
