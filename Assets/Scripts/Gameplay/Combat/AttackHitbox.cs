using System.Collections.Generic;
using UnityEngine;
using Core.Constants;
using Core.Interfaces;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class AttackHitbox : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] float damage = 10f;
        [SerializeField] float knockbackForce = 5f;
        
        [Header("Targeting")]
        [SerializeField] string[] targetTags = new[] { GameConstants.Tags.Enemy };
        
        [Header("Effects")]
        [SerializeField] GameObject hitEffectPrefab;
        [SerializeField] AudioClip hitSound;

        Collider2D hitbox;
        HashSet<string> targetTagSet;
        bool hasHit;

        void Awake()
        {
            hitbox = GetComponent<Collider2D>();
            hitbox.isTrigger = true;
            hitbox.enabled = false;
            targetTagSet = new HashSet<string>(targetTags);
        }

        public void EnableHitbox() { hasHit = false; hitbox.enabled = true; }
        public void DisableHitbox() => hitbox.enabled = false;
        public void SetDamage(float d) => damage = d;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit || !targetTagSet.Contains(other.tag)) return;
            hasHit = true;

            other.TryGetComponent<IDamageable>(out var target)?.TakeDamage(damage);
            ApplyKnockback(other);
            SpawnHitEffect(other);
            Core.Managers.SoundManager.Instance?.PlaySound(hitSound);
        }

        void ApplyKnockback(Collider2D target)
        {
            if (knockbackForce > 0 && target.TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce((target.transform.position - transform.position).normalized * knockbackForce, ForceMode2D.Impulse);
        }

        void SpawnHitEffect(Collider2D target) => 
            Instantiate(hitEffectPrefab, target.ClosestPoint(transform.position), Quaternion.identity);
    }
}