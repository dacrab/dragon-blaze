using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    private int storedValue;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private ParticleSystem pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CollectCoin();
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            storedValue = value;
        }
    }

    private void CollectCoin()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is null");
            return;
        }

        Debug.Log("Collecting coin");

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
            Destroy(effect.gameObject, effect.main.duration);
        }

        // Add the value of the coin to the player's score
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.AddScore(value);
        }

        // Destroy the coin
        Destroy(gameObject);
    }

    public void ResetValue()
    {
        value = storedValue;
    }
}
