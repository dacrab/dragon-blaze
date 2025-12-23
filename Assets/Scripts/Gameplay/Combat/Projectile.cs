using UnityEngine;
using Core.Constants;
using Gameplay.Health;
using Gameplay.Characters.Enemies;

namespace Gameplay.Combat
{
    public class Projectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            if (collision.CompareTag(GameConstants.Tags.Enemy))
            {
                 Health.Health enemyHealth = collision.GetComponent<Health.Health>();
                 if (enemyHealth) 
                    enemyHealth.TakeDamage(damage);
                 else
                 {
                     EnemyBase enemy = collision.GetComponent<EnemyBase>();
                     if (enemy) enemy.TakeDamage(damage);
                 }
            }
        }
    }
}
