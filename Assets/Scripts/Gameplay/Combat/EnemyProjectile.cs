using UnityEngine;
using Core.Constants;

namespace Gameplay.Combat
{
    public class EnemyProjectile : ProjectileBase
    {
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player))
            {
                base.OnTriggerEnter2D(collision);
                return;
            }

            var player = collision.GetComponent<Gameplay.Characters.Player.PlayerController>();
            if (player != null && player.IsInvisible()) return;
            
            var health = collision.GetComponent<Gameplay.Health.Health>();
            health?.TakeDamage(damage);
            base.OnTriggerEnter2D(collision);
        }

        public void ActivateProjectile() => SetDirection(1);
    }
}
