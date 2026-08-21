using UnityEngine;

namespace Gameplay.Characters.Enemies
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "DragonBlaze/Enemies/Enemy Config")]
    public sealed class EnemyConfigSO : ScriptableObject
    {
        [Header("Combat")]
        public float damage = 10f;
        public float speed = 3f;
        public float attackCooldown = 1f;
        public float attackRange = 1.5f;
        public float chaseSpeed = 3f;
        public float detectionRange = 10f;

        [Header("Visuals")]
        public RuntimeAnimatorController animatorController;

        [Header("Audio")]
        public AudioClip attackSound;
    }
}
