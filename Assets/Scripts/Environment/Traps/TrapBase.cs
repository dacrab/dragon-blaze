using UnityEngine;
using Core.Constants;
using Core.Interfaces;
using Gameplay.Characters.Player;

namespace Environment.Traps
{
    public abstract class TrapBase : MonoBehaviour
    {
        [SerializeField] protected float damage;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;

            var pc = collision.GetComponent<PlayerController>();
            if (pc != null && pc.IsInvisible()) return;

            DealDamage(collision.gameObject);
        }

        protected virtual void DealDamage(GameObject target)
        {
            var damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
        }
    }
}
