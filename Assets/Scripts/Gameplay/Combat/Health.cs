using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Managers;
using Core.Pooling;
using Core.Services;

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
        int opponentLayerMask;
        Collider2D[] ownColliders;
        ContactFilter2D contactFilter;
        readonly List<Collider2D> ignoredColliders = new();
        readonly Collider2D[] overlapBuffer = new Collider2D[8];
        CancellationTokenSource iframesCts;

        void Awake()
        {
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
            currentHealth = maxHealth;
            isPlayer = CompareTag(GameConstants.Tags.Player);
            opponentLayerMask = 1 << LayerMask.NameToLayer(isPlayer ? GameConstants.Layers.Enemy : GameConstants.Layers.Player);
            contactFilter.SetLayerMask(opponentLayerMask);
            contactFilter.useLayerMask = true;
            ownColliders = GetComponents<Collider2D>();
            NotifyHealthChanged();
        }

        void OnEnable()
        {
            // Re-enabled dead entities (e.g. room re-entry) come back at full strength.
            if (!isPlayer && (dead || currentHealth <= 0)) ResetState(true);
            if (isPlayer) EventBus.Subscribe<PlayerRespawnEvent>(OnRespawn);
        }

        void OnDisable()
        {
            if (isPlayer) EventBus.Unsubscribe<PlayerRespawnEvent>(OnRespawn);
            iframesCts?.Cancel();
        }

        void OnRespawn(PlayerRespawnEvent _) => ResetState(true);

        /// <summary>Restores full health and clears death/i-frame state.</summary>
        void ResetState(bool clearIFrames)
        {
            currentHealth = maxHealth;
            dead = false;
            invulnerable = false;
            if (clearIFrames) iframesCts?.Cancel();
            sprite.color = normalColor;
            SetComponentsEnabled(true);
            NotifyHealthChanged();
        }

        public void TakeDamage(float damage)
        {
            if (invulnerable || dead) return;
            currentHealth = Mathf.Max(0, currentHealth - damage);
            NotifyHealthChanged();
            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            anim.SetTrigger(GameConstants.Anim.Hurt);
            EventBus.Raise(new DamagedEvent(currentHealth, maxHealth));
            iframesCts?.Cancel();
            iframesCts = new CancellationTokenSource();
            _ = IFramesAsync(iframesCts.Token);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(hurtSound);
            VfxPool.Spawn(hitParticles, transform.position, Quaternion.identity);
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
            ServiceLocator.Get<IAudioManager>()?.PlaySound(deathSound);
            VfxPool.Spawn(deathParticles, transform.position, Quaternion.identity);
            EventBus.Raise(new DiedEvent(gameObject));
            if (isPlayer) EventBus.Raise(new PlayerDiedEvent());
        }

        async Awaitable IFramesAsync(CancellationToken ct)
        {
            invulnerable = true;
            IgnoreOverlappingColliders(true);

            try
            {
                float interval = iFramesDuration / (flashCount * 2);
                for (int i = 0; i < flashCount; i++)
                {
                    sprite.color = hurtColor;
                    await Awaitable.WaitForSecondsAsync(interval, ct);
                    sprite.color = normalColor;
                    await Awaitable.WaitForSecondsAsync(interval, ct);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            IgnoreOverlappingColliders(false);
            invulnerable = false;
        }

        void OnTriggerEnter2D(Collider2D other) => EnterCollision(other);
        void OnCollisionEnter2D(Collision2D collision) => EnterCollision(collision.collider);

        void EnterCollision(Collider2D other)
        {
            if (!invulnerable || other == null) return;
            if (other.transform.IsChildOf(transform)) return;
            if ((opponentLayerMask & (1 << other.gameObject.layer)) == 0) return;
            AddIgnoredCollider(other);
        }

        void IgnoreOverlappingColliders(bool ignore)
        {
            if (ignore)
            {
                foreach (var col in ownColliders)
                {
                    if (col == null) continue;
                    int count = Physics2D.OverlapCollider(col, contactFilter, overlapBuffer);
                    for (int i = 0; i < count; i++)
                        AddIgnoredCollider(overlapBuffer[i]);
                }
            }
            else
            {
                foreach (var other in ignoredColliders)
                    foreach (var col in ownColliders)
                        if (col != null && other != null) Physics2D.IgnoreCollision(col, other, false);
                ignoredColliders.Clear();
            }
        }

        void AddIgnoredCollider(Collider2D other)
        {
            if (ignoredColliders.Contains(other)) return;
            ignoredColliders.Add(other);
            foreach (var col in ownColliders)
                if (col != null) Physics2D.IgnoreCollision(col, other, true);
        }

        void OnDestroy()
        {
            iframesCts?.Cancel();
            IgnoreOverlappingColliders(false);
        }

        void NotifyHealthChanged() { if (isPlayer) EventBus.Raise(new HealthChangedEvent(currentHealth, maxHealth)); }
        void SetComponentsEnabled(bool enabled) { if (disableOnDeath == null) return; foreach (var c in disableOnDeath) if (c != null) c.enabled = enabled; }
    }
}
