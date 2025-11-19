using UnityEngine;
using Core.Constants;

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
            anim.SetTrigger("hurt"); // Assuming 'hurt' trigger exists, standardize if not

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        if (col != null) col.enabled = false;
        if (rb != null) rb.simulated = false; // Stop physics interactions
        
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        if (anim != null)
            anim.SetTrigger(GameConstants.Animation.Die);
            
        // Destroy usually happens after animation via event or time
        Destroy(gameObject, 2f); 
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            // Assuming Player has a Health component or we use EventBus to deal damage
            // Since Health.cs exists (saw it in file list), let's try to use it
            // But we should check for IVisible interface or PlayerController
            
            // Using the Shim for now as it's the safest bet for existing Health system
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null && player.IsVisible())
            {
                 // Need to find how damage is applied. 
                 // Previous MeleeEnemy used: collision.GetComponent<Health>().TakeDamage(damage);
                 Health playerHealth = collision.GetComponent<Health>();
                 if (playerHealth != null)
                 {
                     playerHealth.TakeDamage(damage);
                 }
            }
        }
    }
}
