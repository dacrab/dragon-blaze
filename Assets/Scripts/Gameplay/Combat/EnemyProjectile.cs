using UnityEngine;
using Core.Constants;
using Gameplay.Characters.Player;
using Gameplay.Health;

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

            PlayerController player = collision.GetComponent<PlayerController>();
            
            if (player != null && !player.IsInvisible())
            {
                 Health.Health playerHealth = collision.GetComponent<Health.Health>();
                 if (playerHealth) playerHealth.TakeDamage(damage);
                 
                 base.OnTriggerEnter2D(collision);
            }
        }

        public void ActivateProjectile()
        {
            SetDirection(1); 
        }
    }
}
