using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float multiplier = 2f; // Set your desired speed boost multiplier
    [SerializeField] private float duration = 5f; // Set your desired speed boost duration
    [SerializeField] private Image powerUpImage; // Reference to the UI Image that will show on the canvas
    private Coroutine speedBoostCoroutine; // Reference to the coroutine for the speed boost timer

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PlayerMovement component from the collider's GameObject
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

        // Check if the PlayerMovement component is found
        if (playerMovement != null)
        {
            // Apply speed boost to the player with the specified multiplier and duration
            playerMovement.ApplySpeedBoost(multiplier, duration);

            // Start or restart the speed boost timer coroutine
            if (speedBoostCoroutine != null)
                StopCoroutine(speedBoostCoroutine);
            speedBoostCoroutine = StartCoroutine(SpeedBoostTimer());

            // Disable the Collider
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // Coroutine to handle the speed boost duration
    private IEnumerator SpeedBoostTimer()
    {
        float timer = duration;
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
}
