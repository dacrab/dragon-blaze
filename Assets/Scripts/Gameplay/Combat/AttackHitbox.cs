using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat;

[RequireComponent(typeof(Collider2D))]
public sealed class AttackHitbox : MonoBehaviour
{
    [SerializeField] float damage = 10f, knockbackForce = 5f;
    [SerializeField] string[] targetTags = [GameConstants.Tags.Enemy];
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
        if (hasHit || !IsValidTarget(other)) return;
        hasHit = true;

        other.GetComponent<Health.Health>()?.TakeDamage(damage);

        if (knockbackForce > 0 && other.TryGetComponent<Rigidbody2D>(out var rb))
            rb.AddForce((other.transform.position - transform.position).normalized * knockbackForce, ForceMode2D.Impulse);

        if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, other.ClosestPoint(transform.position), Quaternion.identity);
        Core.Managers.SoundManager.Instance?.PlaySound(hitSound);
    }

    bool IsValidTarget(Collider2D other)
    {
        foreach (var tag in targetTags)
            if (other.CompareTag(tag)) return true;
        return false;
    }
}
