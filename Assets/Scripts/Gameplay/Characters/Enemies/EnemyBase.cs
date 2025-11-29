using UnityEngine;
using Core.Constants;
using Core.Interfaces;
using Gameplay.Characters.Player;
using Gameplay.Characters.Stats;

namespace Gameplay.Characters.Enemies
{
	public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats Configuration")]
        [SerializeField] protected CharacterStatsSO stats;

        [Header("Debug / Overrides (Optional)")]
        [SerializeField] protected float currentHealth;
        
        protected Animator anim;
        protected Rigidbody2D rb;
        protected Collider2D col;
        protected bool isDead;

        public bool IsDead => isDead;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            InitializeStats();
        }

        protected virtual void InitializeStats()
        {
            if (stats != null)
            {
                currentHealth = stats.maxHealth;
            }
            else
            {
                Debug.LogWarning($"Stats SO missing on {gameObject.name}. Using defaults.");
                currentHealth = 100f;
            }
        }

        public virtual void TakeDamage(float damageAmount)
        {
            if (isDead) return;

            currentHealth -= damageAmount;
            
            if (stats != null && stats.hitParticles != null)
                Instantiate(stats.hitParticles, transform.position, Quaternion.identity);
                
            if (anim != null)
                anim.SetTrigger("hurt");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            isDead = true;
            if (col != null) col.enabled = false;
            if (rb != null) rb.simulated = false; 
            
            if (stats != null && stats.deathParticles != null)
                Instantiate(stats.deathParticles, transform.position, Quaternion.identity);

            if (anim != null)
                anim.SetTrigger(GameConstants.Animation.Die);
                
            Destroy(gameObject, 2f); 
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null && !player.IsInvisible())
                {
                     Gameplay.Health.Health playerHealth = collision.GetComponent<Gameplay.Health.Health>();
                     if (playerHealth != null)
                     {
                         float dmg = stats != null ? stats.damage : 10f;
                         playerHealth.TakeDamage(dmg);
                     }
                }
            }
        }
    }
}
