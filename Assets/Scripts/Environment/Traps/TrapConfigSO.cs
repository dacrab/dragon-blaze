using UnityEngine;

namespace Environment.Traps
{
    [CreateAssetMenu(fileName = "TrapConfig", menuName = "DragonBlaze/Traps/Trap Config")]
    public class TrapConfigSO : ScriptableObject
    {
        [Header("Damage")]
        public float damage = 10f;
        public bool damageOverTime;
        public float damageInterval = 0.5f;

        [Header("Timing")]
        public float activationDelay = 0.5f;
        public float activeTime = 2f;
        public float cooldown = 1f;

        [Header("Movement (for moving traps)")]
        public float speed = 2f;
        public float movementDistance = 3f;

        [Header("Projectile (for shooting traps)")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;

        [Header("Visuals")]
        public Color warningColor = Color.red;
        public Color activeColor = Color.white;

        [Header("Audio")]
        public AudioClip activateSound;
        public AudioClip impactSound;
    }
}
