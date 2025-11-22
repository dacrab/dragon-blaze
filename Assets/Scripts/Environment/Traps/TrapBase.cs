using UnityEngine;
using Core.Constants;
using Gameplay.Characters.Player;
using Gameplay.Health;

namespace Environment.Traps
{
	public abstract class TrapBase : MonoBehaviour
    {
        [SerializeField] protected float damage;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                 PlayerController pc = collision.GetComponent<PlayerController>();
                 if (pc != null && pc.IsInvisible()) return;
                 
                 DealDamage(collision.gameObject);
            }
        }

        protected virtual void DealDamage(GameObject target)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
