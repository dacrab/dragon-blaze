using System;
using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Pooling;
using Core.Services;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class AttackHitbox : MonoBehaviour
    {
        [SerializeField] float damage = 10f;
        [SerializeField] float knockbackForce = 5f;
        [SerializeField] string[] targetTags = { GameConstants.Tags.Enemy };
        [SerializeField] GameObject hitEffectPrefab;
        [SerializeField] AudioClip hitSound;

        Collider2D hitbox;
        bool hasHit;

        void Awake()
        {
            hitbox = GetComponent<Collider2D>();
            hitbox.isTrigger = true;
            hitbox.enabled = false;
        }

        public void EnableHitbox() { hasHit = false; hitbox.enabled = true; }
        public void DisableHitbox() => hitbox.enabled = false;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit || Array.IndexOf(targetTags, other.tag) < 0) return;
            hasHit = true;
            if (other.TryGetComponent<Health>(out var target)) target.TakeDamage(damage);
            ApplyKnockback(other);
            VfxPool.Spawn(hitEffectPrefab, other.ClosestPoint(transform.position), Quaternion.identity);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(hitSound);
        }

        void ApplyKnockback(Collider2D target)
        {
            if (knockbackForce > 0 && target.TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce(((Vector2)(target.transform.position - transform.position)).normalized * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
