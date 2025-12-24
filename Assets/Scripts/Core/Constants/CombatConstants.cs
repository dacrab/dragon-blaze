namespace Core.Constants
{
    /// <summary>
    /// Combat-related constants to eliminate magic numbers.
    /// </summary>
    public static class CombatConstants
    {
        /// <summary>
        /// Buffer distance added to attack range checks for more forgiving hit detection.
        /// </summary>
        public const float AttackRangeBuffer = 1.0f;

        /// <summary>
        /// Default projectile lifetime in seconds.
        /// </summary>
        public const float DefaultProjectileLifetime = 5f;

        /// <summary>
        /// Default damage value when not specified.
        /// </summary>
        public const float DefaultDamage = 10f;

        /// <summary>
        /// Default attack cooldown in seconds.
        /// </summary>
        public const float DefaultAttackCooldown = 1f;

        /// <summary>
        /// Default invulnerability frame duration in seconds.
        /// </summary>
        public const float DefaultIFrameDuration = 1.0f;

        /// <summary>
        /// Default number of flashes during invulnerability.
        /// </summary>
        public const int DefaultFlashCount = 5;
    }
}
