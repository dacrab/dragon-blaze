using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] public int value = 1; // The value of the coin
    private int storedValue; // The value stored when reaching a checkpoint
    [SerializeField] public AudioClip pickupSound; // The sound to play when the coin is picked up
    public GameObject pickupParticles; // Reference to the particle system GameObject

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Play the pickup sound
            if (SoundManager.instance != null && pickupSound != null)
            {
                SoundManager.instance.PlaySound(pickupSound);
            }

            // Add the value of the coin to the player's score
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.AddScore(value);
            }

            // Enable and play the particle system
            if (pickupParticles != null)
            {
                pickupParticles.transform.position = transform.position;
                pickupParticles.SetActive(true);
                ParticleSystem particles = pickupParticles.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    particles.Play();
                }
                else
                {
                    Debug.LogWarning("The pickupParticles GameObject does not have a ParticleSystem component.");
                }
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
