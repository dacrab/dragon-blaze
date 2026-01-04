using UnityEngine;
using Core.Constants;

namespace Environment.Traps
{
    public abstract class TrapBase : MonoBehaviour
    {
        [SerializeField] protected float damage = 10f;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.PlayerController>();
            if (player != null && player.IsInvisible()) return;
            
            var health = collision.GetComponent<Gameplay.Health.Health>();
            health?.TakeDamage(damage);
        }
    }
}
