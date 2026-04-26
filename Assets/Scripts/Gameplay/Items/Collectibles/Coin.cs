using UnityEngine;
using Core.Managers;
using Core.Constants;

namespace Gameplay.Items.Collectibles
{
    public sealed class Coin : MonoBehaviour
    {
        [SerializeField] int value = 1;
        [SerializeField] AudioClip pickupSound;
        [SerializeField] ParticleSystem pickupEffect;

        int storedValue;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GameConstants.Tags.Player)) Collect();
            else if (other.CompareTag(GameConstants.Tags.Checkpoint)) storedValue = value;
        }

        void Collect()
        {
            if (GameManager.Instance == null) return;
            SoundManager.Instance?.PlaySound(pickupSound);
            if (pickupEffect != null)
            {
                var effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
            GameManager.Instance.AddCoins(value);
            Destroy(gameObject);
        }

        public void ResetValue() => value = storedValue;
    }
}
}