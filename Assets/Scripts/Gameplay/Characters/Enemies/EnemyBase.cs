using UnityEngine;
using Core.Constants;
using Core.Combat;
using Core.Interfaces;
using Core.Events;
using Core.Utilities;
using Core.Data.Stats;

namespace Gameplay.Characters.Enemies
{
    /// <summary>
    /// Base class for all enemies. Implements IDamageable for consistent damage handling.
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats Configuration")]
        [SerializeField] protected CharacterStatsSO stats;
        
        [Header("Base Stats (used if no SO assigned)")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float speed = 3f;
        
        [Header("Effects")]
        [SerializeField] protected GameObject deathParticles;
        [SerializeField] protected GameObject hitParticles;

        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected Animator anim;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected Rigidbody2D rb;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] protected Collider2D col;
        protected bool isDead;
        protected float currentHealth;

        #region IDamageable Implementation
        public float CurrentHealth => currentHealth;
        public float MaxHealth => stats != null ? stats.maxHealth : maxHealth;
        public bool IsAlive => !isDead;
        #endregion

        protected virtual void Awake()
        {
            AutoWireHelper.WireAllFields(this);
            InitializeFromStats();
        }

        protected virtual void InitializeFromStats()
        {
            if (stats != null)
            {
                currentHealth = stats.maxHealth;
                damage = stats.damage;
                speed = stats.speed;
                
                if (stats.hitParticles != null) hitParticles = stats.hitParticles;
                if (stats.deathParticles != null) deathParticles = stats.deathParticles;
            }
            else
            {
                currentHealth = maxHealth;
            }
        }

        /// <summary>
        /// Takes damage using the new DamageInfo system.
        /// </summary>
        public virtual float TakeDamage(DamageInfo damageInfo)
        {
            if (isDead) return 0f;

            float actualDamage = damageInfo.FinalDamage;
            currentHealth -= actualDamage;
            
            SpawnParticles(hitParticles);
            anim?.SetTrigger(GameConstants.Animation.Hurt);
            
            EventBus.RaiseDamageDealt(actualDamage, damageInfo.DamageType);

            if (currentHealth <= 0)
            {
                Die();
            }

            return actualDamage;
        }

        /// <summary>
        /// Simple damage method for backwards compatibility.
        /// </summary>
        public virtual void TakeDamage(float damageAmount)
        {
            TakeDamage(DamageInfo.Physical(damageAmount));
        }

        protected virtual void Die()
        {
            isDead = true;
            if (col != null) col.enabled = false;
            if (rb != null) rb.simulated = false; 
            
            SpawnParticles(deathParticles);
            anim?.SetTrigger(GameConstants.Animation.Die);
            
            EventBus.RaiseEnemyKilled();
            
            // Delay destruction to allow death animation
            Destroy(gameObject, 2f); 
        }

        protected virtual void SpawnParticles(GameObject particlePrefab)
        {
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, transform.position, Quaternion.identity);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (!collision.TryGetPlayerController(out var player) || player.IsInvisible()) return;
            
            if (collision.TryGetHealth(out var health))
            {
                health.TakeDamage(DamageInfo.Physical(damage, gameObject));
            }
        }

        /// <summary>
        /// Heals the enemy by the specified amount.
        /// </summary>
        public virtual void Heal(float amount)
        {
            if (isDead) return;
            currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        }
    }
}
