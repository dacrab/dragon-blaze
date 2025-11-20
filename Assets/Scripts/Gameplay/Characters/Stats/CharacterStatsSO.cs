using UnityEngine;

namespace Gameplay.Characters.Stats
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "DragonBlaze/Stats/Character Stats")]
    public class CharacterStatsSO : ScriptableObject
    {
        [Header("Base Attributes")]
        public float maxHealth = 100f;
        public float damage = 10f;
        public float speed = 3f;

        [Header("Combat")]
        public float attackCooldown = 1f;
        public float attackRange = 1f;
        
        [Header("Effects")]
        public GameObject hitParticles;
        public GameObject deathParticles;
    }
}
