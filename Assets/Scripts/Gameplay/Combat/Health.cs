using System.Threading;
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
        int playerLayerIndex, enemyLayerIndex;
        CancellationTokenSource iFramesCts;

        void Awake()
        {
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
            isPlayer = CompareTag(GameConstants.Tags.Player);
            playerLayerIndex = LayerMask.NameToLayer(GameConstants.Layers.Player);
            enemyLayerIndex = LayerMask.NameToLayer(GameConstants.Layers.Enemy);
            NotifyHealthChanged();
        }

        void OnEnable()
        {
            if (!isPlayer && (dead || currentHealth <= 0))
            {
                currentHealth = maxHealth;
                dead = false;
                invulnerable = false;
                sprite.color = normalColor;
                SetComponentsEnabled(true);
                NotifyHealthChanged();
            }
            if (isPlayer) EventBus.OnPlayerRespawn += ResetForRespawn;
        }

        void OnDisable()
        {
            if (isPlayer) EventBus.OnPlayerRespawn -= ResetForRespawn;
            if (iFramesCts != null)
            {
                iFramesCts.Cancel();
                iFramesCts.Dispose();
                iFramesCts = null;
            }
        }

        void OnDestroy()
        {
            if (isPlayer) EventBus.OnPlayerRespawn -= ResetForRespawn;
        }

        void ResetForRespawn()
        {
            currentHealth = maxHealth;
            dead = false;
            invulnerable = false;
            if (iFramesCts != null)
            {
                iFramesCts.Cancel();
                iFramesCts.Dispose();
                iFramesCts = null;
            }
            sprite.color = normalColor;
            SetComponentsEnabled(true);
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
            iFramesCts?.Cancel();
            iFramesCts?.Dispose();
            iFramesCts = new CancellationTokenSource();
            var token = iFramesCts.Token;

            invulnerable = true;
            int playerLayer = playerLayerIndex;
            int enemyLayer = enemyLayerIndex;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

            float interval = iFramesDuration / (flashCount * 2);
            try
            {
                for (int i = 0; i < flashCount; i++)
                {
                    if (token.IsCancellationRequested) break;
                    sprite.color = hurtColor;
                    await Awaitable.WaitForSecondsAsync(interval);
                    if (token.IsCancellationRequested) break;
                    sprite.color = normalColor;
                    await Awaitable.WaitForSecondsAsync(interval);
                }
            }
            finally
            {
                sprite.color = normalColor;
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
                invulnerable = false;
                iFramesCts?.Dispose();
                iFramesCts = null;
            }
        }

        void NotifyHealthChanged() { if (isPlayer) EventBus.RaiseHealthChanged(currentHealth, maxHealth); }
        void SetComponentsEnabled(bool enabled) { foreach (var c in disableOnDeath) if (c != null) c.enabled = enabled; }
    }
}
