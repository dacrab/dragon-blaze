using UnityEngine;
using System.Collections;
using Core.Interfaces;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Gameplay.Health
{

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public sealed class Health : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    
    [Header("Invulnerability")]
    [SerializeField] float iFramesDuration = 1f;
    [SerializeField] int flashCount = 5;
    [SerializeField] Color hurtColor = new(1, 0, 0, 0.5f);
    [SerializeField] Color normalColor = Color.white;
    
    [Header("Components")]
    [SerializeField] Behaviour[] disableOnDeath;
    
    [Header("Audio")]
    [SerializeField] AudioClip deathSound, hurtSound;
    
    [Header("Effects")]
    [SerializeField] GameObject hitParticles, deathParticles;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => !dead;

    float currentHealth;
    Animator anim;
    SpriteRenderer sprite;
    bool dead, invulnerable, isPlayer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        isPlayer = CompareTag(GameConstants.Tags.Player);
        if (isPlayer) EventBus.HealthChanged(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (invulnerable || dead) return;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (isPlayer) EventBus.HealthChanged(currentHealth, maxHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger(GameConstants.Animation.Hurt);
            StartCoroutine(IFrames());
            SoundManager.Instance?.PlaySound(hurtSound);
            if (hitParticles != null) Instantiate(hitParticles, transform.position, Quaternion.identity);
        }
        else Die();
    }

    public void Heal(float value)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + value);
        if (isPlayer) EventBus.HealthChanged(currentHealth, maxHealth);
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        if (isPlayer) EventBus.HealthChanged(currentHealth, maxHealth);
        anim.ResetTrigger(GameConstants.Animation.Die);
        anim.Play(GameConstants.Animation.Idle);
        StartCoroutine(IFrames());
        dead = false;
        foreach (var c in disableOnDeath) if (c != null) c.enabled = true;
        GetComponent<Collider2D>().enabled = true;
        if (isPlayer) EventBus.PlayerRespawn();
    }

    void Die()
    {
        foreach (var c in disableOnDeath) if (c != null) c.enabled = false;
        anim.SetBool(GameConstants.Animation.Grounded, true);
        anim.SetTrigger(GameConstants.Animation.Die);
        dead = true;
        SoundManager.Instance?.PlaySound(deathSound);
        if (deathParticles != null) Instantiate(deathParticles, transform.position, Quaternion.identity);
        if (isPlayer) EventBus.PlayerDied();
    }

    IEnumerator IFrames()
    {
        invulnerable = true;
        int playerLayer = LayerMask.NameToLayer(GameConstants.Layers.Player);
        int enemyLayer = LayerMask.NameToLayer(GameConstants.Layers.Enemy);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        float interval = iFramesDuration / (flashCount * 2);
        for (int i = 0; i < flashCount; i++)
        {
            sprite.color = hurtColor;
            yield return new WaitForSeconds(interval);
            sprite.color = normalColor;
            yield return new WaitForSeconds(interval);
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        invulnerable = false;
    }
}
}