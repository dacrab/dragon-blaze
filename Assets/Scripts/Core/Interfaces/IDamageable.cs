using Core.Combat;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for objects that can receive damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Current health value.
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// Maximum health value.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// Whether the entity is currently alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Applies damage to this entity.
        /// </summary>
        /// <param name="damageInfo">Damage information struct.</param>
        /// <returns>Actual damage dealt after modifiers/resistances.</returns>
        float TakeDamage(DamageInfo damageInfo);

        /// <summary>
        /// Simple damage method for backwards compatibility.
        /// </summary>
        /// <param name="damage">Raw damage amount.</param>
        void TakeDamage(float damage);
    }
}

