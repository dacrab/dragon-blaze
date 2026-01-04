using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat
{
    public class Projectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (!collision.CompareTag(GameConstants.Tags.Enemy)) return;
            
            var health = collision.GetComponent<Gameplay.Health.Health>();
            health?.TakeDamage(damage);
        }
    }
}
