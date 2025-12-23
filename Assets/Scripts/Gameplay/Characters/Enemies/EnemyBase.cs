using UnityEngine;
using Core.Constants;
using Gameplay.Characters.Player;

namespace Gameplay.Characters.Enemies
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] protected float health = 100f;
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float speed = 3f;
        
        [Header("Effects")]
        [SerializeField] protected GameObject deathParticles;
        [SerializeField] protected GameObject hitParticles;

        protected Animator anim;
        protected Rigidbody2D rb;
        protected Collider2D col;
        protected bool isDead;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }

        public virtual void TakeDamage(float damageAmount)
        {
            if (isDead) return;

            health -= damageAmount;
            
            if (hitParticles != null)
                Instantiate(hitParticles, transform.position, Quaternion.identity);
                
            if (anim != null)
                anim.SetTrigger("hurt");

            if (health <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            isDead = true;
            if (col != null) col.enabled = false;
            if (rb != null) rb.simulated = false; 
            
            if (deathParticles != null)
                Instantiate(deathParticles, transform.position, Quaternion.identity);

            if (anim != null)
                anim.SetTrigger(GameConstants.Animation.Die);
                
            Destroy(gameObject, 2f); 
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                // Use !IsInvisible() logic or simple null check if no visibility logic exists yet on Controller
                // Assuming I will rely on Health to handle invulnerability/invisibility checks mostly,
                // OR check here. PlayerController has IsInvisible().
                if (player != null && !player.IsInvisible())
                {
                     Gameplay.Health.Health playerHealth = collision.GetComponent<Gameplay.Health.Health>();
                     if (playerHealth != null)
                     {
                         playerHealth.TakeDamage(damage);
                     }
                }
            }
        }
    }
}
