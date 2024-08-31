using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public Healthbar healthBar;
    #endregion

    #region Private Fields
    private Animator anim;
    private SpriteRenderer spriteRend;
    private PlayerMovement playerMovement;
    private bool dead;
    private bool invulnerable;
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

        if (gameObject.CompareTag("Player"))
        {
            playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement component not found on Player!");
            }
        }

        if (anim == null) Debug.LogError("Animator component not found!");
        if (spriteRend == null) Debug.LogError("SpriteRenderer component not found!");

        if (healthBar == null)
        {
            healthBar = FindObjectOfType<Healthbar>();
            if (healthBar == null)
            {
                Debug.LogError("Healthbar component is not found in the scene.");
            }
        }
    }
    #endregion

    #region Public Methods
    public void TakeDamage(float _damage)
    {
        if (invulnerable || (playerMovement != null && playerMovement.IsInvisible())) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        UpdateHealthBar();

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
    }
    #endregion

    #region Private Methods
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthUI(currentHealth, startingHealth);
        }
        else
        {
            Debug.LogError("Healthbar reference not set in Health script.");
        }
    }

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
    }

    private void DisableComponents()
    {
        foreach (Behaviour component in components)
        {
            if (component != null)
            {
                component.enabled = false;
            }
            else
            {
                Debug.LogError("Component in components array is null!");
            }
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void TriggerDeathAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("grounded", true);
            anim.SetTrigger("die");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(clip);
        }
        else
        {
            Debug.LogError("SoundManager instance not found!");
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
            else
            {
                Debug.LogError("SpriteRenderer component not found!");
            }
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void ResetAnimations()
    {
        if (anim != null)
        {
            anim.ResetTrigger("die");
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
            else
            {
                Debug.LogError("Component in components array is null!");
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
        else
        {
            Debug.LogError("BoxCollider2D component not found!");
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
            else
            {
                Debug.LogError("FallingPlatform in fallingPlatforms list is null!");
            }
        }
    }
    #endregion
}
