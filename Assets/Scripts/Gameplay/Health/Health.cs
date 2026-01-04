using UnityEngine;
using System.Collections;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Gameplay.Health
{
    public class Health : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float startingHealth = 100f;

        [Header("Invulnerability")]
        [SerializeField] private float iFramesDuration = 1f;
        [SerializeField] private int numberOfFlashes = 5;

        [Header("Components")]
        [SerializeField] private Behaviour[] components;

        [Header("Audio")]
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip hurtSound;

        [Header("Particles")]
        [SerializeField] private GameObject hitParticlesPrefab;
        [SerializeField] private GameObject deathParticlesPrefab;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => startingHealth;
        public bool IsAlive => !dead;

        private float currentHealth;
        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool dead;
        private bool invulnerable;
        private bool isPlayer;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
            currentHealth = startingHealth;
            isPlayer = gameObject.CompareTag(GameConstants.Tags.Player);
            if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, startingHealth);
        }

        public void TakeDamage(float damage)
        {
            if (invulnerable || dead) return;

            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
            if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, startingHealth);

            if (currentHealth > 0) HandleDamage();
            else Die();
        }

        public void AddHealth(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth);
            if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, startingHealth);
        }

        public void Respawn()
        {
            AddHealth(startingHealth);
            anim?.ResetTrigger(GameConstants.Animation.Die);
            anim?.Play("Idle");
            StartCoroutine(Invulnerability());
            dead = false;
            foreach (var c in components) if (c != null) c.enabled = true;
            var col = GetComponent<BoxCollider2D>();
            if (col != null) col.enabled = true;
            if (isPlayer) EventBus.RaisePlayerRespawn();
        }

        private void HandleDamage()
        {
            anim?.SetTrigger(GameConstants.Animation.Hurt);
            StartCoroutine(Invulnerability());
            SoundManager.Instance?.PlaySound(hurtSound);
            if (hitParticlesPrefab != null) Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        }

        private void Die()
        {
            foreach (var c in components) if (c != null) c.enabled = false;
            anim?.SetBool(GameConstants.Animation.Grounded, true);
            anim?.SetTrigger(GameConstants.Animation.Die);
            dead = true;
            SoundManager.Instance?.PlaySound(deathSound);
            if (deathParticlesPrefab != null) Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            if (isPlayer) EventBus.RaisePlayerDied();
        }

        private IEnumerator Invulnerability()
        {
            invulnerable = true;
            int playerLayer = LayerMask.NameToLayer(GameConstants.Layers.Player);
            int enemyLayer = LayerMask.NameToLayer(GameConstants.Layers.Enemy);
            if (playerLayer >= 0 && enemyLayer >= 0)
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

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
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            invulnerable = false;
        }
    }
}
