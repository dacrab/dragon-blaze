using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] public int value = 1; // The value of the coin
    private int storedValue; // The value stored when reaching a checkpoint
    [SerializeField] public AudioClip pickupSound; // The sound to play when the coin is picked up

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Play the pickup sound
            SoundManager.instance.PlaySound(pickupSound);

            // Add the value of the coin to the player's score
            other.gameObject.GetComponent<PlayerMovement>().AddScore(value);

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
