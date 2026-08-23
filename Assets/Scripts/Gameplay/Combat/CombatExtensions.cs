using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Combat
{
    /// <summary>Shared player-contact rules for traps, enemies, and projectiles.</summary>
    public static class CombatExtensions
    {
        public static bool IsInvisiblePlayer(this Collider2D collider) =>
            collider.TryGetComponent<Player>(out var player) && player.IsInvisible;

        /// <summary>Applies damage unless the target is an invisible player.</summary>
        public static void DamagePlayer(this Collider2D collider, float amount, bool respectInvisibility = true)
        {
            if (respectInvisibility && collider.IsInvisiblePlayer()) return;
            if (collider.TryGetComponent<Health>(out var health))
                health.TakeDamage(amount);
        }

        /// <summary>Applies continuous per-second damage without arming i-frames.</summary>
        public static void DamagePerSecond(this Collider2D collider, float amount)
        {
            if (!collider.IsInvisiblePlayer() && collider.TryGetComponent<Health>(out var health))
                health.TakeDamagePerSecond(amount);
        }
    }
}
