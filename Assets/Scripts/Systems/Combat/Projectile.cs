using UnityEngine;
using Core.Constants;

public class Projectile : ProjectileBase
{
    // Inherits speed, damage, lifetime
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Specific logic for Player Projectile:
        // Ignore Player? Usually handled by Physics Layers (Player Projectile Layer vs Player Layer)
        // But let's assume we hit anything that triggers.
        
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag(GameConstants.Tags.Enemy))
        {
             Health enemyHealth = collision.GetComponent<Health>();
             // Or EnemyBase?
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

