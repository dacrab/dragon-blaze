using UnityEngine;
using Core.Constants;
using Core.Managers;

namespace Gameplay.Items;

[RequireComponent(typeof(Collider2D))]
public sealed class HealthCollectible : Collectable
{
    [SerializeField] float healthValue = 25f;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] ParticleSystem pickupEffect;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) return;
        collision.GetComponent<Health.Health>()?.AddHealth(healthValue);
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
