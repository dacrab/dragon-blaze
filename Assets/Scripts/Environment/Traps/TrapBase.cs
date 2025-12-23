using UnityEngine;
using Core.Constants;
using Core.Utilities;
using Gameplay.Characters.Player;
using Gameplay.Health;

namespace Environment.Traps
{
    public abstract class TrapBase : MonoBehaviour
    {
        [SerializeField] protected float damage;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (collision.TryGetPlayerController(out var pc) && pc.IsInvisible()) return;
            if (collision.TryGetHealth(out var health)) health.TakeDamage(damage);
        }
    }
}
