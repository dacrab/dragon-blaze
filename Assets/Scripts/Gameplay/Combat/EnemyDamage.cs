using UnityEngine;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;

            var controller = PlayerReference.Controller;
            if (controller == null || controller.IsInvisible()) return;

            var damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsDead)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
