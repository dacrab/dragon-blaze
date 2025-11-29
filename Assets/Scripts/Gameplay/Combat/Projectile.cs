using UnityEngine;
using Core.Constants;
using Core.Interfaces;

namespace Gameplay.Combat
{
    public class Projectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            if (collision.CompareTag(GameConstants.Tags.Enemy))
            {
                var damageable = collision.GetComponent<IDamageable>();
                if (damageable != null && !damageable.IsDead)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}
