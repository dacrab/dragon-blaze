using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Managers;

namespace Gameplay.Combat
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public sealed class Health : MonoBehaviour
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
            NotifyHealthChanged();
        }

        public void TakeDamage(float damage)
        {
            if (invulnerable || dead) return;
            currentHealth = Mathf.Max(0, currentHealth - damage);
            NotifyHealthChanged();

            if (currentHealth > 0)
            {
                anim.SetTrigger(GameConstants.Anim.Hurt);
                _ = IFramesAsync();
                GameManager.Instance?.PlaySound(hurtSound);
                if (hitParticles != null) Instantiate(hitParticles, transform.position, Quaternion.identity);
            }
            else Die();
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + value);
            NotifyHealthChanged();
        }

        public void Respawn()
        {
            currentHealth = maxHealth;
            NotifyHealthChanged();
            anim.ResetTrigger(GameConstants.Anim.Die);
            anim.Play(GameConstants.Anim.Idle);
            _ = IFramesAsync();
            dead = false;
            SetComponentsEnabled(true);
            GetComponent<Collider2D>().enabled = true;
            if (isPlayer) EventBus.RaisePlayerRespawn();
        }

        void Die()
        {
            SetComponentsEnabled(false);
            anim.SetBool(GameConstants.Anim.Grounded, true);
            anim.SetTrigger(GameConstants.Anim.Die);
            dead = true;
            GameManager.Instance?.PlaySound(deathSound);
            if (deathParticles != null) Instantiate(deathParticles, transform.position, Quaternion.identity);
            if (isPlayer) EventBus.RaisePlayerDied();
        }

        async Awaitable IFramesAsync()
        {
            invulnerable = true;
            int playerLayer = LayerMask.NameToLayer(GameConstants.Layers.Player);
            int enemyLayer = LayerMask.NameToLayer(GameConstants.Layers.Enemy);
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

            float interval = iFramesDuration / (flashCount * 2);
            for (int i = 0; i < flashCount; i++)
            {
                sprite.color = hurtColor;
                await Awaitable.WaitForSecondsAsync(interval);
                sprite.color = normalColor;
                await Awaitable.WaitForSecondsAsync(interval);
            }

            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            invulnerable = false;
        }

        void NotifyHealthChanged() { if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, maxHealth); }
        void SetComponentsEnabled(bool enabled) { foreach (var c in disableOnDeath) if (c != null) c.enabled = enabled; }
    }
}
