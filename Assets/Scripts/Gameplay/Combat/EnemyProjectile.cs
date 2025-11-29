using UnityEngine;
using Core.Constants;
using Core.Interfaces;
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

            var controller = PlayerReference.Controller;
            if (controller != null && !controller.IsInvisible())
            {
                var damageable = collision.GetComponent<IDamageable>();
                if (damageable != null && !damageable.IsDead)
                {
                    damageable.TakeDamage(damage);
                }
                base.OnTriggerEnter2D(collision);
            }
        }

        public void ActivateProjectile() => SetDirection(1);
    }
}
