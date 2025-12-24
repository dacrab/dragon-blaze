using UnityEngine;
using Core.Combat;
using Core.Interfaces;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Combat
{
    public class EnemyProjectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player))
            {
                base.OnTriggerEnter2D(collision);
                return;
            }

            if (!collision.TryGetPlayerController(out var player) || player.IsInvisible()) return;
            
            var damageInfo = DamageInfo.Physical(damage, gameObject);
            
            if (collision.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damageInfo);
            else if (collision.TryGetHealth(out var health))
                health.TakeDamage(damage);
                
            base.OnTriggerEnter2D(collision);
        }

        public void ActivateProjectile()
        {
            SetDirection(1); 
        }
    }
}
