using UnityEngine;

namespace Gameplay.Characters.Enemies
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "DragonBlaze/Enemies/Enemy Config")]
    public class EnemyConfigSO : ScriptableObject
    {
        [Header("Stats")]
        public float damage = 10f;
        public float speed = 3f;
        public float maxHealth = 50f;

        [Header("Combat")]
        public float attackCooldown = 1f;
        public float attackRange = 1.5f;
        public float chaseSpeed = 3f;
        public float detectionRange = 10f;

        [Header("Patrol")]
        public float patrolSpeed = 2f;
        public float idleDuration = 1f;

        [Header("Ranged (optional)")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;

        [Header("Visuals")]
        public RuntimeAnimatorController animatorController;

        [Header("Audio")]
        public AudioClip attackSound;
        public AudioClip deathSound;
        public AudioClip hurtSound;

        [Header("Drops")]
        public GameObject[] possibleDrops;
        [Range(0, 1)] public float dropChance = 0.3f;
    }
}
