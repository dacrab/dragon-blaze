using UnityEngine;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Combat
{
    public class EnemyProjectile : ProjectileBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player))
            {
                base.OnTriggerEnter2D(collision);
                return;
            }

            if (collision.TryGetPlayerController(out var player) && !player.IsInvisible() 
                && collision.TryGetHealth(out var health))
            {
                health.TakeDamage(damage);
                base.OnTriggerEnter2D(collision);
            }
        }

        public void ActivateProjectile()
        {
            SetDirection(1); 
        }
    }
}
