using UnityEngine;
using Core.Constants;

namespace Core.Data.Stats
{
    [CreateAssetMenu(fileName = "TrapStats", menuName = "DragonBlaze/Stats/Trap Stats")]
    public class TrapStatsSO : ScriptableObject
    {
        [Header("General")]
        public float damage = CombatConstants.DefaultDamage;
        public float speed = 2f;
        
        [Header("Movement/Range")]
        public float movementDistance = 3f;
        public float attackRange = 10f;
        public float attackCooldown = CombatConstants.DefaultAttackCooldown;
    }
}
