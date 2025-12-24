using UnityEngine;
using Core.Managers;
using Core.Constants;

namespace Gameplay.Items
{
    public class Coin : Collectable
    {
        [SerializeField] private int value = 1;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private ParticleSystem pickupEffect;

        private int storedValue;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GameConstants.Tags.Player)) Collect();
            else if (other.CompareTag(GameConstants.Tags.Checkpoint)) storedValue = value;
        }

        public override void Collect()
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
