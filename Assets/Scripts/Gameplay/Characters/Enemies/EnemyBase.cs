using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Enemies
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float speed = 3f;
        
        [Header("Effects")]
        [SerializeField] protected GameObject deathParticles;
        [SerializeField] protected GameObject hitParticles;

        protected Animator anim;
        protected Rigidbody2D rb;
        protected Collider2D col;
        protected bool isDead;
        protected float currentHealth;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float amount)
        {
            if (isDead) return;
            currentHealth -= amount;
            if (hitParticles != null) Instantiate(hitParticles, transform.position, Quaternion.identity);
            anim?.SetTrigger(GameConstants.Animation.Hurt);
            if (currentHealth <= 0) Die();
        }

        protected virtual void Die()
        {
            isDead = true;
            if (col != null) col.enabled = false;
            if (rb != null) rb.simulated = false;
            if (deathParticles != null) Instantiate(deathParticles, transform.position, Quaternion.identity);
            anim?.SetTrigger(GameConstants.Animation.Die);
            Destroy(gameObject, 2f);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.PlayerController>();
            if (player != null && player.IsInvisible()) return;
            
            var health = collision.GetComponent<Gameplay.Health.Health>();
            health?.TakeDamage(damage);
        }
    }
}
