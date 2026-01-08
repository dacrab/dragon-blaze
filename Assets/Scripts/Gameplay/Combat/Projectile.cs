using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat;

public sealed class Projectile : ProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag(GameConstants.Tags.Enemy))
            collision.GetComponent<Health.Health>()?.TakeDamage(damage);
    }
}
