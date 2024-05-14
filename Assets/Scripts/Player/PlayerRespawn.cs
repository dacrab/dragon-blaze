using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpoint; // Sound played when reaching a checkpoint
    private Transform currentCheckpoint; // Reference to the current checkpoint
    private Health playerHealth; // Reference to the player's health component
    private UIManager uiManager; // Reference to the UIManager

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        playerHealth = GetComponent<Health>(); // Get the Health component attached to the player
        uiManager = FindObjectOfType<UIManager>(); // Find the UIManager in the scene
    }

    // Method to handle respawning the player
    public void RespawnCheck()
    {
        if (currentCheckpoint == null) // If no checkpoint is set
        {
            uiManager.GameOver(); // Display game over UI
            return;
        }

        playerHealth.Respawn(); // Restore player health and reset animation
        transform.position = currentCheckpoint.position; // Move player to checkpoint location
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only).
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Checkpoint") // If collided object is tagged as a checkpoint
        {
            currentCheckpoint = collision.transform; // Set currentCheckpoint to the collided object's transform
            SoundManager.instance.PlaySound(checkpoint); // Play checkpoint sound
            collision.GetComponent<Collider2D>().enabled = false; // Disable the collider of the checkpoint to prevent multiple triggers
            collision.GetComponent<Animator>().SetTrigger("activate"); // Trigger checkpoint activation animation
        }
    }
}
