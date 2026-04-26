using UnityEngine;
using Core.Constants;
using Core.Managers;
using Gameplay.Combat;

namespace Gameplay.Items.Collectibles
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class HealthCollectible : MonoBehaviour
    {
        [SerializeField] float healthValue = 25f;
        [SerializeField] AudioClip pickupSound;
        [SerializeField] ParticleSystem pickupEffect;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            collision.GetComponent<Health>()?.Heal(healthValue);
            Collect();
        }

        void Collect()
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
}