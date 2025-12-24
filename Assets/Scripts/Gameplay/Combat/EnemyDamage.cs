using UnityEngine;
using Core.Combat;
using Core.Interfaces;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Combat
{
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] protected float damage = CombatConstants.DefaultDamage;
        [SerializeField] protected DamageType damageType = DamageType.Physical;

        protected virtual DamageInfo CreateDamageInfo()
        {
            return new DamageInfo(damage, damageType, gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (!collision.TryGetPlayerController(out var controller) || controller.IsInvisible()) return;
            
            if (collision.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(CreateDamageInfo());
            else if (collision.TryGetHealth(out var health))
                health.TakeDamage(damage);
        }
    }
}
