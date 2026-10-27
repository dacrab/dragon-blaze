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
        readonly List<Collider2D> overlapResults = new();
        CancellationTokenSource? iframesCts;

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
            // Death is sticky: only a respawn revives, so re-entering a room can't resurrect cleared enemies.
            if (isPlayer) EventBus.Subscribe<PlayerRespawnEvent>(OnRespawn);
        }

        void OnDisable()
        {
            if (isPlayer) EventBus.Unsubscribe<PlayerRespawnEvent>(OnRespawn);
            CancelIFrames();
        }

        void CancelIFrames()
        {
            iframesCts?.Cancel();
            iframesCts?.Dispose();
            iframesCts = null;
        }

        void OnRespawn(PlayerRespawnEvent _) => ResetState(true);

        /// <summary>Restores full health and clears death/i-frame state.</summary>
        void ResetState(bool clearIFrames)
        {
            currentHealth = maxHealth;
            dead = false;
            invulnerable = false;
            IgnoreOverlappingColliders(false);
            if (clearIFrames) CancelIFrames();
            sprite.color = normalColor;
            SetComponentsEnabled(true);
            foreach (var col in ownColliders)
                if (col != null) col.enabled = true;
            NotifyHealthChanged();
        }

        /// <summary>Debug/cheat hook: makes this health immune to damage until switched back off.</summary>
        public void SetInvulnerable(bool value) => invulnerable = value;

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
            CancelIFrames();
            iframesCts = new CancellationTokenSource();
            _ = IFramesAsync(iframesCts.Token);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(hurtSound);
            VfxPool.Spawn(hitParticles, transform.position, Quaternion.identity);
        }

        /// <summary>Applies continuous damage without arming i-frames (DoT surfaces).</summary>
        public void TakeDamagePerSecond(float amount)
        {
            if (invulnerable || dead || amount <= 0f) return;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            NotifyHealthChanged();
            if (currentHealth <= 0) Die();
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + value);
            NotifyHealthChanged();
        }

        void Die()
        {
            if (dead) return;
            dead = true;
            SetComponentsEnabled(false);
            // A corpse must not keep blocking or damaging anything; a respawn re-enables these.
            foreach (var col in ownColliders)
                if (col != null) col.enabled = false;
            anim.SetBool(GameConstants.Anim.Grounded, true);
            anim.SetTrigger(GameConstants.Anim.Die);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(deathSound);
            VfxPool.Spawn(deathParticles, transform.position, Quaternion.identity);
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
                IgnoreOverlappingColliders(false);
                invulnerable = false;
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
                    int count = Physics2D.OverlapCollider(col, contactFilter, overlapResults);
                    for (int i = 0; i < count; i++)
                        AddIgnoredCollider(overlapResults[i]);
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
            CancelIFrames();
            IgnoreOverlappingColliders(false);
        }

        void NotifyHealthChanged() { if (isPlayer) EventBus.Raise(new HealthChangedEvent(currentHealth, maxHealth)); }
        void SetComponentsEnabled(bool enabled) { if (disableOnDeath == null) return; foreach (var c in disableOnDeath) if (c != null) c.enabled = enabled; }
    }
}
