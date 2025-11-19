using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.Constants;
using Core.Events;

public class Health : MonoBehaviour
{
    #region Serialized Fields
    [Header("Health")]
    [SerializeField] private float startingHealth = 100f;

    [Header("Invulnerability Frames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    [Header("Particle Systems")]
    [SerializeField] private GameObject hitParticleSystemPrefab;
    [SerializeField] private GameObject deathParticleSystemPrefab;

    [Header("Respawn")]
    [SerializeField] private List<FallingPlatform> fallingPlatforms;
    #endregion

    #region Public Properties
    public float currentHealth { get; private set; }
    // Removed Healthbar direct reference
    #endregion

    #region Private Fields
    private Animator anim;
    private SpriteRenderer spriteRend;
    private PlayerMovement playerMovement;
    private bool dead;
    private bool invulnerable;
    private bool isPlayer;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeComponents();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        
        isPlayer = gameObject.CompareTag(GameConstants.Tags.Player);

        if (isPlayer)
        {
            playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement component not found on Player!");
            }
            // Raise initial health event for UI
            EventBus.RaiseHealthChanged(currentHealth, startingHealth);
        }

        if (anim == null) Debug.LogError("Animator component not found!");
        if (spriteRend == null) Debug.LogError("SpriteRenderer component not found!");
    }
    #endregion

    #region Public Methods
    public void TakeDamage(float _damage)
    {
        if (invulnerable || (isPlayer && playerMovement != null && playerMovement.IsInvisible())) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (isPlayer)
        {
            EventBus.RaiseHealthChanged(currentHealth, startingHealth);
        }

        if (currentHealth > 0)
        {
            HandleDamage();
        }
        else
        {
            if (!dead)
            {
                Die();
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
        if (isPlayer)
        {
            EventBus.RaiseHealthChanged(currentHealth, startingHealth);
        }
    }

    public void Respawn()
    {
        AddHealth(startingHealth);
        ResetAnimations();
        StartCoroutine(Invulnerability());
        dead = false;

        EnableComponents();
        EnableCollider();
        ResetFallingPlatforms();
        
        if (isPlayer)
        {
            EventBus.RaisePlayerRespawn();
        }
    }
    #endregion

    #region Private Methods

    private void HandleDamage()
    {
        if (anim != null)
        {
            anim.SetTrigger("hurt");
        }
        StartCoroutine(Invulnerability());
        PlaySound(hurtSound);
        SpawnParticles(hitParticleSystemPrefab);
    }

    private void Die()
    {
        DisableComponents();
        TriggerDeathAnimation();
        dead = true;
        PlaySound(deathSound);
        SpawnParticles(deathParticleSystemPrefab);
        
        if (isPlayer)
        {
            // PlayerMovement.Die() calls EventBus.RaisePlayerDied();
            // But duplicate logic alert: Health calls Die logic AND PlayerMovement calls Die Logic?
            // PlayerMovement.Die() handles UI and particles. Health.Die handles particles and components.
            // Let's unify. If PlayerMovement exists, let it handle "Game Over" state via its Die method.
            // Health handles pure health logic.
            
            if (playerMovement != null)
            {
                playerMovement.Die();
            }
        }
    }

    private void DisableComponents()
    {
        foreach (Behaviour component in components)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
        // PlayerMovement disabled separately via its own Die logic or here
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void TriggerDeathAnimation()
    {
        if (anim != null)
        {
            anim.SetBool(GameConstants.Animation.Grounded, true);
            anim.SetTrigger(GameConstants.Animation.Die);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(clip);
        }
    }

    private void SpawnParticles(GameObject particleSystemPrefab)
    {
        if (particleSystemPrefab != null)
        {
            Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        // Physics2D.IgnoreLayerCollision(10, 11, true); 
        // Need layer IDs or use Layer names. 
        // Assuming Player (Layer 10?) and Enemy (Layer 11?)
        // Let's use LayerMask.NameToLayer to be safe if we know names.
        // Player = Layer 10, Enemy = Layer 11?
        // Better to use constants or just keep as is if we are sure about IDs. 
        // I'll assume previous dev knew IDs.
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            if (spriteRend != null)
            {
                spriteRend.color = new Color(1, 0, 0, 0.5f);
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
                spriteRend.color = Color.white;
                yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            }
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    private void ResetAnimations()
    {
        if (anim != null)
        {
            anim.ResetTrigger(GameConstants.Animation.Die);
            anim.Play("Idle");
        }
    }

    private void EnableComponents()
    {
        foreach (Behaviour component in components)
        {
            if (component != null)
            {
                component.enabled = true;
            }
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void EnableCollider()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }

    private void ResetFallingPlatforms()
    {
        foreach (var platform in fallingPlatforms)
        {
            if (platform != null)
            {
                platform.ResetPlatform();
            }
        }
    }
    #endregion
}

