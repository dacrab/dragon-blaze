using UnityEngine;
using Core.Constants;
using Core.Interfaces;

namespace Gameplay.Combat
{

public sealed class EnemyProjectile : ProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player))
        {
            base.OnTriggerEnter2D(collision);
            return;
        }

        if (collision.TryGetComponent<IInvisible>(out var invisible) && invisible.IsInvisible) return;
        if (collision.TryGetComponent<IDamageable>(out var target)) target.TakeDamage(damage);
        
        base.OnTriggerEnter2D(collision);
    }

    public void ActivateProjectile() => SetDirection(transform.lossyScale.x > 0 ? 1 : -1);
}
}