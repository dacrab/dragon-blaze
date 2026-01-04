using UnityEngine;
using Core.Constants;
using Core.Managers;

namespace Gameplay.Items
{
    [RequireComponent(typeof(Collider2D))]
    public class HealthCollectible : Collectable
    {
        [SerializeField] private float healthValue = 25f;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private ParticleSystem pickupEffect;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var health = collision.GetComponent<Gameplay.Health.Health>();
            health?.AddHealth(healthValue);
            Collect();
        }

        public override void Collect()
        {
            SoundManager.Instance?.PlaySound(pickupSound);
            if (pickupEffect != null)
            {
                var effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
            gameObject.SetActive(false);
        }
    }
}
