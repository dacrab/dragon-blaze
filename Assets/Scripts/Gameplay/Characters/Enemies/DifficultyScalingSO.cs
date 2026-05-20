using UnityEngine;

namespace Gameplay.Characters.Enemies
{
    [System.Serializable]
    public struct LevelDifficulty
    {
        [Range(0.5f, 5f)] public float damageMultiplier;
        [Range(0.5f, 5f)] public float healthMultiplier;
        [Range(0.5f, 3f)] public float speedMultiplier;
    }

    public struct ScaledEnemyStats
    {
        public float damage, maxHealth, speed, chaseSpeed, attackCooldown;
    }

    /// <summary>
    /// Scales enemy stats per level. Create one asset, assign level multipliers.
    /// Use: var stats = difficultyScaling.GetScaledStats(enemyConfig, currentLevel);
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultyScaling", menuName = "DragonBlaze/Config/Difficulty Scaling")]
    public class DifficultyScalingSO : ScriptableObject
    {
        [Header("Per-Level Multipliers (index = level)")]
        [SerializeField] LevelDifficulty[] levels =
        {
            new() { damageMultiplier = 1f, healthMultiplier = 1f, speedMultiplier = 1f },
            new() { damageMultiplier = 1.2f, healthMultiplier = 1.3f, speedMultiplier = 1.1f },
            new() { damageMultiplier = 1.5f, healthMultiplier = 1.6f, speedMultiplier = 1.2f },
            new() { damageMultiplier = 2f, healthMultiplier = 2f, speedMultiplier = 1.3f },
        };

        [Header("Beyond Defined Levels")]
        [SerializeField] float scalingPerLevel = 0.2f;
        [SerializeField] float maxSpeedMultiplier = 3f;

        public ScaledEnemyStats GetScaledStats(EnemyConfigSO config, int levelIndex)
        {
            var m = GetMultipliers(levelIndex);
            return new ScaledEnemyStats
            {
                damage = config.damage * m.damageMultiplier,
                maxHealth = config.maxHealth * m.healthMultiplier,
                speed = config.speed * m.speedMultiplier,
                chaseSpeed = config.chaseSpeed * m.speedMultiplier,
                attackCooldown = config.attackCooldown / m.speedMultiplier
            };
        }

        LevelDifficulty GetMultipliers(int levelIndex)
        {
            if (levels != null && levelIndex >= 0 && levelIndex < levels.Length)
                return levels[levelIndex];

            float scale = 1f + (levelIndex * scalingPerLevel);
            return new LevelDifficulty
            {
                damageMultiplier = scale,
                healthMultiplier = scale,
                speedMultiplier = Mathf.Min(scale, maxSpeedMultiplier)
            };
        }
    }
}
