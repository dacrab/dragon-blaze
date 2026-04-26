using UnityEngine;
using Core.Constants;
using Core.Interfaces;

namespace Gameplay.Combat
{

public sealed class Projectile : ProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag(GameConstants.Tags.Enemy))
            if (collision.TryGetComponent<IDamageable>(out var target)) target.TakeDamage(damage);
    }
}
}