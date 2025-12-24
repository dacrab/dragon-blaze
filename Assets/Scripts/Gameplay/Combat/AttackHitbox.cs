using UnityEngine;
using System.Collections.Generic;
using Core.Constants;
using Core.Combat;
using Core.Interfaces;
using Core.Utilities;

namespace Gameplay.Combat
{
    /// <summary>
    /// Reusable attack hitbox component. Enable/disable via Animation Events.
    /// Attach to a child GameObject with a trigger collider.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AttackHitbox : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        [SerializeField] private bool canHitMultipleTargets = true;
        [SerializeField] private float knockbackForce = 5f;

        [Header("Target Settings")]
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private string[] targetTags = { "Enemy" };

        [Header("Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private AudioClip hitSound;

        private Collider2D hitboxCollider;
        private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
        private Transform owner;

        private void Awake()
        {
            hitboxCollider = GetComponent<Collider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;
            
            owner = transform.root;
        }

        /// <summary>
        /// Call from Animation Event to enable the hitbox.
        /// </summary>
        public void EnableHitbox()
        {
            hitTargets.Clear();
            hitboxCollider.enabled = true;
        }

        /// <summary>
        /// Call from Animation Event to disable the hitbox.
        /// </summary>
        public void DisableHitbox()
        {
            hitboxCollider.enabled = false;
        }

        /// <summary>
        /// Sets the damage for this attack (useful for combo systems).
        /// </summary>
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hitboxCollider.enabled) return;
            if (!canHitMultipleTargets && hitTargets.Count > 0) return;
            if (hitTargets.Contains(other)) return;
            if (!IsValidTarget(other)) return;

            hitTargets.Add(other);
            ApplyDamage(other);
            ApplyKnockback(other);
            SpawnHitEffect(other);
            PlayHitSound();
        }

        private bool IsValidTarget(Collider2D other)
        {
            // Check layer
            if (targetLayers != 0 && ((1 << other.gameObject.layer) & targetLayers) == 0)
                return false;

            // Check tags
            if (targetTags != null && targetTags.Length > 0)
            {
                bool hasValidTag = false;
                foreach (var tag in targetTags)
                {
                    if (other.CompareTag(tag))
                    {
                        hasValidTag = true;
                        break;
                    }
                }
                if (!hasValidTag) return false;
            }

            return true;
        }

        private void ApplyDamage(Collider2D target)
        {
            var damageInfo = new DamageInfo(
                damage,
                damageType,
                owner?.gameObject,
                target.ClosestPoint(transform.position),
                (target.transform.position - transform.position).normalized
            );

            // Try IDamageable first
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damageInfo);
            }
            // Fallback to Health component
            else if (target.TryGetHealth(out var health))
            {
                health.TakeDamage(damageInfo);
            }
        }

        private void ApplyKnockback(Collider2D target)
        {
            if (knockbackForce <= 0) return;

            if (target.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }
        }

        private void SpawnHitEffect(Collider2D target)
        {
            if (hitEffectPrefab != null)
            {
                Vector3 hitPoint = target.ClosestPoint(transform.position);
                // Use object pooling if available, otherwise instantiate
                var effect = Core.Optimization.ObjectPoolManager.Instance?.Get(
                    hitEffectPrefab.name, 
                    hitPoint, 
                    Quaternion.identity
                );
                
                if (effect == null)
                {
                    // Fallback to instantiate if pooling not available
                    Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
                }
            }
        }

        private void PlayHitSound()
        {
            if (hitSound != null)
            {
                Core.Managers.SoundManager.Instance?.PlaySound(hitSound);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var col = GetComponent<Collider2D>();
            if (col == null) return;

            Gizmos.color = col.enabled ? Color.red : Color.yellow;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
