using UnityEngine;
using Core.Constants;

namespace Core.Data.Stats
{
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "DragonBlaze/Stats/Character Stats")]
    public class CharacterStatsSO : ScriptableObject
    {
        [Header("Base Attributes")]
        public float maxHealth = 100f;
        public float damage = CombatConstants.DefaultDamage;
        public float speed = 3f;

        [Header("Combat")]
        public float attackCooldown = CombatConstants.DefaultAttackCooldown;
        public float attackRange = 1f;
        
        [Header("Effects")]
        public GameObject hitParticles;
        public GameObject deathParticles;
    }
}
