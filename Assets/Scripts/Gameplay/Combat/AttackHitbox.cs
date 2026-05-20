using UnityEngine;
using Core.Constants;
using Core.Managers;

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
        public void SetDamage(float d) => damage = d;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit || System.Array.IndexOf(targetTags, other.tag) < 0) return;
            hasHit = true;
            if (other.TryGetComponent<Health>(out var target)) target.TakeDamage(damage);
            ApplyKnockback(other);
            if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, other.ClosestPoint(transform.position), Quaternion.identity);
            GameManager.Instance?.PlaySound(hitSound);
        }

        void ApplyKnockback(Collider2D target)
        {
            if (knockbackForce > 0 && target.TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce((target.transform.position - transform.position).normalized * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
