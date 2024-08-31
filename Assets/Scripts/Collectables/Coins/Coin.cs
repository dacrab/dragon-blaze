using UnityEngine;

public class Coin : Collectable
{
    [SerializeField] private int value = 1;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private ParticleSystem pickupEffect;

    private int storedValue;

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
            return;
        }

        PlayPickupSound();
        PlayPickupEffect();
        AddCoinsToGameManager();
        DestroyCoin();
    }

    private void PlayPickupSound()
    {
        if (SoundManager.instance != null && pickupSound != null)
        {
            SoundManager.instance.PlaySound(pickupSound);
        }
    }

    private void PlayPickupEffect()
    {
        if (pickupEffect != null)
        {
            ParticleSystem effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    private void AddCoinsToGameManager()
    {
        GameManager.instance.AddCoins(value);
    }

    private void DestroyCoin()
    {
        Destroy(gameObject);
    }

    public void ResetValue()
    {
        value = storedValue;
    }
}
