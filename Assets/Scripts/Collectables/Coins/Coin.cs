using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1; // The value of the coin
    private int storedValue; // The value stored when reaching a checkpoint
    [SerializeField] private AudioClip pickupSound; // The sound to play when the coin is picked up
    [SerializeField] private ParticleSystem pickupEffect; // The particle effect to play when the coin is picked up

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Play the pickup sound
            if (SoundManager.instance != null && pickupSound != null)
            {
                SoundManager.instance.PlaySound(pickupSound);
            }

            // Play the pickup particle effect
            if (pickupEffect != null)
            {
                ParticleSystem effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                effect.Play();
            }

            // Add the value of the coin to the player's score
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.AddScore(value);
            }

            // Destroy the coin
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            // Store the current value of the coin when reaching a checkpoint
            storedValue = value;
        }
    }

    public void ResetValue()
    {
        // Reset the value of the coin to the stored value
        value = storedValue;
    }
}
