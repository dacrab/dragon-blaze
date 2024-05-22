using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue; // Amount of health the collectible provides
    [SerializeField] private AudioClip pickupSound; // Sound played when the collectible is picked up
    [SerializeField] private GameObject pickupParticles; // Reference to the particle system GameObject

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only).
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") // If the collider has the "Player" tag
        {
            // Play pickup sound
            if (SoundManager.instance != null && pickupSound != null)
            {
                SoundManager.instance.PlaySound(pickupSound);
            }

            // Add health to the player
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.AddHealth(healthValue);
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

            // Deactivate the collectible object
            gameObject.SetActive(false);
        }
    }
}
