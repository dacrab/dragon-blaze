using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.Constants;
using Core.Events;
using Core.Utilities;
using Gameplay.Characters.Player;
using Core.Managers;
using Environment.Platforms;

namespace Gameplay.Health
{
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
        #endregion

        #region Private Fields
        private Animator anim;
        private SpriteRenderer spriteRend;
        private PlayerController playerController; // Replaced PlayerMovement
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
                // Use PlayerController instead of PlayerMovement
                playerController = GetComponent<PlayerController>();
                if (playerController == null)
                {
                    Debug.LogError("PlayerController component not found on Player!");
                }
                // Raise initial health event for UI
                EventBus.RaiseHealthChanged(currentHealth, startingHealth);
            }

            if (anim == null) Debug.LogError("Animator component not found!");
            if (spriteRend == null) Debug.LogError("SpriteRenderer component not found!");
        }
        #endregion

        #region Public Methods
        public void TakeDamage(float damage)
        {
            if (invulnerable || (isPlayer && playerController?.IsInvisible() == true)) return;
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, startingHealth);
            if (currentHealth > 0) HandleDamage();
            else if (!dead) Die();
        }

        public void AddHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
            if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, startingHealth);
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
            anim?.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
            if (hurtSound != null) SoundManager.Instance?.PlaySound(hurtSound);
            if (hitParticleSystemPrefab != null) SpawnParticles(hitParticleSystemPrefab);
        }

        private void Die()
        {
            DisableComponents();
            anim?.SetBool(GameConstants.Animation.Grounded, true);
            anim?.SetTrigger(GameConstants.Animation.Die);
            dead = true;
            if (deathSound != null) SoundManager.Instance?.PlaySound(deathSound);
            if (deathParticleSystemPrefab != null) SpawnParticles(deathParticleSystemPrefab);
            if (isPlayer) EventBus.RaisePlayerDied();
        }

        private void DisableComponents()
        {
            foreach (var component in components) component?.SetEnabled(false);
            if (playerController != null) playerController.SetEnabled(false);
        }
        
        private void SpawnParticles(GameObject particlePrefab)
        {
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, transform.position, Quaternion.identity);
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
            if (playerController != null)
            {
                playerController.enabled = true;
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
}
