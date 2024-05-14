using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HigherJump : MonoBehaviour
{
    [SerializeField] private float jumpMultiplier = 1.5f; // Multiplier for the higher jump
    [SerializeField] private float powerUpDuration = 5f; // Duration of the power-up in seconds
    [SerializeField] private Image powerUpImage; // Reference to the UI Image that will flicker on the canvas
    private bool isActive = true; // Flag to track if the power-up is currently active
    private Coroutine powerUpTimerCoroutine; // Reference to the coroutine for the power-up timer

 private void OnTriggerEnter2D(Collider2D collision)
{
    // Get the PlayerMovement component from the collider's GameObject
    PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

    // Check if the PlayerMovement component is found and the power-up is active
    if (playerMovement != null && isActive)
    {
        // Apply the higher jump multiplier to the player's jump
        ApplyHigherJump(playerMovement);

        // Start or restart the power-up timer coroutine
        if (powerUpTimerCoroutine != null)
            StopCoroutine(powerUpTimerCoroutine);
        powerUpTimerCoroutine = StartCoroutine(PowerUpTimer());

        // Disable the Collider
        GetComponent<Collider2D>().enabled = false;
    }
}

// Coroutine to handle the power-up duration and flickering effect
private IEnumerator PowerUpTimer()
{
    float timer = powerUpDuration;
    float waitTime = 0.2f; // Initial wait time
    float decreaseAmount = 0.01f; // Amount to decrease the wait time in each iteration

    while (timer > 0f)
    {
        // Flash the power-up image by toggling its visibility
        powerUpImage.enabled = !powerUpImage.enabled;

        // Wait for the current duration
        yield return new WaitForSeconds(waitTime);

        // Decrease the timer
        timer -= waitTime;

        // Decrease the wait time for the next iteration
        waitTime = Mathf.Max(0.05f, waitTime - decreaseAmount); // Ensure wait time doesn't go below 0.05
    }

    // Ensure the power-up image is disabled when the power-up is over
    powerUpImage.enabled = false;

    // Re-enable the collider
    GetComponent<Collider2D>().enabled = true;
}

    // Method to apply the higher jump multiplier to the player's jump
    private void ApplyHigherJump(PlayerMovement playerMovement)
    {
        playerMovement.ApplyHigherJump(jumpMultiplier);
    }
}
