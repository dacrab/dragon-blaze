using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat;

public sealed class EnemyProjectile : ProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player))
        {
            base.OnTriggerEnter2D(collision);
            return;
        }

        if (collision.GetComponent<Characters.Player.Player>() is { IsInvisible: true }) return;
        
        collision.GetComponent<Health.Health>()?.TakeDamage(damage);
        base.OnTriggerEnter2D(collision);
    }

    public void ActivateProjectile() => SetDirection(transform.lossyScale.x > 0 ? 1 : -1);
}
