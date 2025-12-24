using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Utilities;
using Core.Input;
using Gameplay.Combat;

namespace Gameplay.Characters.Player
{
    /// <summary>
    /// Handles player attack input and projectile spawning.
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackCooldownDuration = 0.5f;
        [SerializeField] private float damage = 10f;
        
        [Header("Projectile")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] fireballs;
        [SerializeField] private bool usePooling = false;
        [SerializeField] private string projectilePoolTag = "PlayerFireball";
        
        [Header("Audio")]
        [SerializeField] private AudioClip fireballSound;
        
        [Header("Input")]
        [SerializeField] private InputReader inputReader;

        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private Animator anim;
        [AutoWire(AutoWireAttribute.WireType.Parent)]
        [SerializeField] private PlayerController playerController;
        private CooldownTimer attackCooldown;

        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
            attackCooldown = new CooldownTimer(attackCooldownDuration);
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.AttackEvent += OnAttack;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.AttackEvent -= OnAttack;
        }

        private void Update()
        {
            attackCooldown.Update();
        }

        private void OnAttack()
        {
            if (CanAttack())
            {
                PerformAttack();
            }
        }

        private bool CanAttack()
        {
            // Check cooldown
            if (!attackCooldown.IsReady) return false;
            
            // Check player state
            if (playerController != null && !playerController.CanAttack()) return false;
            
            // Check game state
            if (!GameStateHelpers.CanProcessInput) return false;
            
            return true;
        }

        private void PerformAttack()
        {
            if (firePoint == null) return;

            SoundManager.Instance?.PlaySound(fireballSound);
            anim?.SetTrigger("attack");
            attackCooldown.Reset();

            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            float direction = Mathf.Sign(transform.localScale.x);
            
            if (usePooling)
            {
                // Use object pool
                var projectile = Core.Optimization.ObjectPoolManager.Instance?.Get(
                    projectilePoolTag, 
                    firePoint.position, 
                    Quaternion.identity
                );
                
                if (projectile != null)
                {
                    var projectileBase = projectile.GetComponent<ProjectileBase>();
                    projectileBase?.SetDirection(direction);
                }
            }
            else
            {
                // Use pre-instantiated array (original behavior)
                if (fireballs == null || fireballs.Length == 0) return;
                
                var fireball = System.Array.Find(fireballs, f => !f.activeInHierarchy);
                if (fireball != null)
                {
                    fireball.transform.position = firePoint.position;
                    var projectileBase = fireball.GetComponent<ProjectileBase>();
                    projectileBase?.SetDirection(direction);
                }
            }
        }

        /// <summary>
        /// Sets the attack damage (for power-ups).
        /// </summary>
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }

        /// <summary>
        /// Gets the current attack damage.
        /// </summary>
        public float GetDamage() => damage;
    }
}
