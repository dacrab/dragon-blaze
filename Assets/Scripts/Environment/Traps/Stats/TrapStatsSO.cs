using UnityEngine;

namespace Environment.Traps.Stats
{
    [CreateAssetMenu(fileName = "TrapStats", menuName = "DragonBlaze/Stats/Trap Stats")]
    public class TrapStatsSO : ScriptableObject
    {
        [Header("General")]
        public float damage = 1f;
        public float speed = 2f;
        
        [Header("Movement/Range")]
        public float movementDistance = 3f;
        public float attackRange = 10f;
        public float attackCooldown = 1f;
    }
}
