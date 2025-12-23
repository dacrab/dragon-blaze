using UnityEngine;
using Core.Constants;
using Core.Utilities;
using Gameplay.Health;
using Gameplay.Characters.Enemies;

namespace Gameplay.Combat
{
    public class Projectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            if (!collision.CompareTag(GameConstants.Tags.Enemy)) return;
            
            if (collision.TryGetHealth(out var health))
                health.TakeDamage(damage);
            else if (collision.TryGetComponent<EnemyBase>(out var enemy))
                enemy.TakeDamage(damage);
        }
    }
}
