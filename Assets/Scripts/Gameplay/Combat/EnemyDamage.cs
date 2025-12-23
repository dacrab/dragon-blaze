using UnityEngine;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Combat
{
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] protected float damage;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (!collision.TryGetPlayerController(out var controller) || controller.IsInvisible()) return;
            if (!collision.TryGetHealth(out var health)) return;
            
            health.TakeDamage(damage);
        }
    }
}
