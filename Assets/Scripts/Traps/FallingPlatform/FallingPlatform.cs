using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField]private float fallDelay = 1f; // Delay before the platform falls
    [SerializeField]private float destroyDelay = 2f; // Delay before the platform is destroyed after falling
    [SerializeField] private Rigidbody2D rb; // Reference to the Rigidbody2D component

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
        Destroy(gameObject, destroyDelay); // Destroy the platform after the specified destroy delay
    }
}
