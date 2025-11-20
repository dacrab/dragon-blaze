using UnityEngine;
using Gameplay.Characters.Player;
using Gameplay.Health;

namespace Gameplay.Combat
{
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] protected float damage;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController == null || playerController.IsInvisible()) return;

            Health.Health playerHealth = collision.GetComponent<Health.Health>();
            if (playerHealth == null) return;

            playerHealth.TakeDamage(damage);
        }
    }
}
