using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f; // Delay before the platform falls
    [SerializeField] private float destroyDelay = 2f; // Delay before the platform is disabled after falling
    [SerializeField] private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector3 initialPosition; // To store the initial position of the platform

    private void Start()
    {
        initialPosition = transform.position; // Store the initial position
        rb.bodyType = RigidbodyType2D.Static; // Ensure the Rigidbody is static initially
    }

    // Called when this collider/rigidbody has begun touching another rigidbody/collider.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Check if the collision is with the player
        {
            StartCoroutine(Fall()); // Start the coroutine to make the platform fall
        }
    }

    // Coroutine to make the platform fall after a delay
    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay); // Wait for the specified fall delay
        rb.bodyType = RigidbodyType2D.Dynamic; // Change the Rigidbody2D body type to Dynamic to allow falling
        yield return new WaitForSeconds(destroyDelay); // Wait for the specified delay before disabling
        gameObject.SetActive(false); // Disable the platform instead of destroying it
    }

    // Method to reset the platform to its initial state
    public void ResetPlatform()
    {
        gameObject.SetActive(true); // Reactivate the platform
        transform.position = initialPosition; // Reset position to the initial position
        rb.bodyType = RigidbodyType2D.Static; // Change the Rigidbody back to Static
    }
}

