using UnityEngine;
using Core.Combat;
using Core.Interfaces;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Combat
{
    public class Projectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (!collision.CompareTag(GameConstants.Tags.Enemy)) return;
            
            var damageInfo = DamageInfo.Physical(damage, gameObject);
            
            if (collision.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damageInfo);
            else if (collision.TryGetHealth(out var health))
                health.TakeDamage(damage);
        }
    }
}
