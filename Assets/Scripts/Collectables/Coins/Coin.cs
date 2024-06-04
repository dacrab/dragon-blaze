using UnityEngine;

public class Coin : Collectable
{
    [SerializeField] private int value = 1;
    private int storedValue;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private ParticleSystem pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            storedValue = value;
        }
    }

    public override void Collect()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is null");
            return;
        }
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

        // Add the value of the coin to the GameManager's total coins
        GameManager.instance.AddCoins(value);

        // Destroy the coin
        Destroy(gameObject);
    }

    public void ResetValue()
    {
        value = storedValue;
    }
}
