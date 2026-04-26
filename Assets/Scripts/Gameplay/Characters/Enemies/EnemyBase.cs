using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Enemies
{

[RequireComponent(typeof(Animator))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float speed = 3f;
    
    [Header("Effects")]
    [SerializeField] protected GameObject deathParticles, hitParticles;
    
    [Header("Death")]
    [SerializeField] protected float deathDelay = 2f;

    protected Animator anim;
    protected Collider2D col;
    protected float currentHealth;
    protected bool isDead;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (hitParticles != null) Instantiate(hitParticles, transform.position, Quaternion.identity);
        anim.SetTrigger(GameConstants.Animation.Hurt);
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        if (col != null) col.enabled = false;
        if (deathParticles != null) Instantiate(deathParticles, transform.position, Quaternion.identity);
        anim.SetTrigger(GameConstants.Animation.Die);
        Destroy(gameObject, deathDelay);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) return;
        if (collision.GetComponent<Player.Player>() is { IsInvisible: true }) return;
        collision.GetComponent<Health.Health>()?.TakeDamage(damage);
    }
}
}