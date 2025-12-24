using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Extension methods for efficient tag comparisons.
    /// Uses GameConstants for type-safe tag checking.
    /// </summary>
    public static class TagHelpers
    {
        /// <summary>
        /// Checks if GameObject has the Player tag.
        /// </summary>
        public static bool IsPlayer(this GameObject go) => go.CompareTag(Core.Constants.GameConstants.Tags.Player);

        /// <summary>
        /// Checks if GameObject has the Enemy tag.
        /// </summary>
        public static bool IsEnemy(this GameObject go) => go.CompareTag(Core.Constants.GameConstants.Tags.Enemy);

        /// <summary>
        /// Checks if GameObject has the Checkpoint tag.
        /// </summary>
        public static bool IsCheckpoint(this GameObject go) => go.CompareTag(Core.Constants.GameConstants.Tags.Checkpoint);

        /// <summary>
        /// Checks if Collider2D's GameObject has the Player tag.
        /// </summary>
        public static bool IsPlayer(this Collider2D col) => col.CompareTag(Core.Constants.GameConstants.Tags.Player);

        /// <summary>
        /// Checks if Collider2D's GameObject has the Enemy tag.
        /// </summary>
        public static bool IsEnemy(this Collider2D col) => col.CompareTag(Core.Constants.GameConstants.Tags.Enemy);

        /// <summary>
        /// Checks if Collider2D's GameObject has the Checkpoint tag.
        /// </summary>
        public static bool IsCheckpoint(this Collider2D col) => col.CompareTag(Core.Constants.GameConstants.Tags.Checkpoint);
    }
}

