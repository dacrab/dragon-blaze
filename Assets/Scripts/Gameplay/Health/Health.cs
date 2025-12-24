using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.Constants;
using Core.Combat;
using Core.Interfaces;
using Core.Events;
using Core.Utilities;
using Gameplay.Characters.Player;
using Core.Managers;
using Environment.Platforms;

namespace Gameplay.Health
{
    public class Health : MonoBehaviour, IDamageable
    {
        #region Serialized Fields
        [Header("Health")]
        [SerializeField] private float startingHealth = 100f;

        [Header("Invulnerability Frames")]
        [SerializeField] private float iFramesDuration = CombatConstants.DefaultIFrameDuration;
        [SerializeField] private int numberOfFlashes = CombatConstants.DefaultFlashCount;

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

        #region IDamageable Implementation
        public float CurrentHealth => currentHealth;
        public float MaxHealth => startingHealth;
        public bool IsAlive => !dead;
        #endregion

        #region Private Fields
        private float currentHealth;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private Animator anim;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private SpriteRenderer spriteRend;
        [AutoWire(AutoWireAttribute.WireType.Self, required: false)]
        [SerializeField] private PlayerController playerController;
        private bool dead;
        private bool invulnerable;
        private bool isPlayer;
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            currentHealth = startingHealth;
            isPlayer = gameObject.CompareTag(GameConstants.Tags.Player);

            if (isPlayer)
            {
                if (playerController == null)
                {
                    Debug.LogError("PlayerController component not found on Player!");
                }
                EventBus.RaiseHealthChanged(currentHealth, startingHealth);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes damage using the new DamageInfo system.
        /// </summary>
        public float TakeDamage(DamageInfo damageInfo)
        {
            if (!damageInfo.IgnoresIFrames && invulnerable) return 0f;
            if (isPlayer && playerController?.IsInvisible() == true) return 0f;

            float actualDamage = damageInfo.FinalDamage;
            currentHealth = Mathf.Clamp(currentHealth - actualDamage, 0, startingHealth);
            
            if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, startingHealth);
            
            // Raise combat event with damage type
            EventBus.RaiseDamageDealt(actualDamage, damageInfo.DamageType);
            
            if (currentHealth > 0) HandleDamage();
            else if (!dead) Die();

            return actualDamage;
        }

        /// <summary>
        /// Simple damage method for backwards compatibility.
        /// </summary>
        public void TakeDamage(float damage)
        {
            TakeDamage(DamageInfo.Physical(damage));
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
            
            // Use LayerConstants instead of hardcoded layer indices
            int playerLayer = LayerConstants.Player;
            int enemyLayer = LayerConstants.Enemy;
            
            if (playerLayer >= 0 && enemyLayer >= 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
            }

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

            if (playerLayer >= 0 && enemyLayer >= 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            }
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
