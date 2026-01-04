using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class AttackHitbox : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private bool canHitMultipleTargets = true;
        [SerializeField] private float knockbackForce = 5f;

        [Header("Target")]
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private string[] targetTags = { "Enemy" };

        [Header("Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private AudioClip hitSound;

        private Collider2D hitboxCollider;
        private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

        private void Awake()
        {
            hitboxCollider = GetComponent<Collider2D>();
            hitboxCollider.isTrigger = true;
            hitboxCollider.enabled = false;
        }

        public void EnableHitbox()
        {
            hitTargets.Clear();
            hitboxCollider.enabled = true;
        }

        public void DisableHitbox() => hitboxCollider.enabled = false;
        public void SetDamage(float newDamage) => damage = newDamage;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hitboxCollider.enabled) return;
            if (!canHitMultipleTargets && hitTargets.Count > 0) return;
            if (hitTargets.Contains(other)) return;
            if (!IsValidTarget(other)) return;

            hitTargets.Add(other);
            
            var health = other.GetComponent<Gameplay.Health.Health>();
            health?.TakeDamage(damage);
            
            if (knockbackForce > 0 && other.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
            
            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, other.ClosestPoint(transform.position), Quaternion.identity);
            Core.Managers.SoundManager.Instance?.PlaySound(hitSound);
        }

        private bool IsValidTarget(Collider2D other)
        {
            if (targetLayers != 0 && ((1 << other.gameObject.layer) & targetLayers) == 0)
                return false;
            if (targetTags == null || targetTags.Length == 0) return true;
            foreach (var tag in targetTags)
                if (other.CompareTag(tag)) return true;
            return false;
        }
    }
}
