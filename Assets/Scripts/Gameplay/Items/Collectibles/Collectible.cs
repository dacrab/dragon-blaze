using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Input;
using Core.Managers;
using Core.Pooling;
using Core.Services;
using Core.State;
using Gameplay.Combat;
using Gameplay.Items.PowerUps;

namespace Gameplay.Items.Collectibles
{
    public enum CollectibleType { Coin, Health, PowerUp, MagicStone }

    /// <summary>Single pickup component: coins, health, timed power-ups, and the level-exit magic stone.</summary>
    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField] CollectibleType type;
        [SerializeField] int coinValue = 1;
        [SerializeField] float healthValue = 25f;

        [Header("Power Up (PowerUp type only)")]
        [SerializeField] PowerUpSO powerUp;
        [SerializeField] float powerUpDuration = 5f;
        [SerializeField] Sprite powerUpIcon;

        [Header("Presentation")]
        [SerializeField] SpriteRenderer indicator;
        [SerializeField] AudioClip pickupSound;
        [SerializeField] ParticleSystem pickupEffect;

        InputReader? inputReader;
        bool playerInTrigger, collected;

        void Start() { if (type == CollectibleType.MagicStone) SetIndicator(false); }

        void OnEnable()
        {
            inputReader = InputReader.Instance;
            if (type == CollectibleType.MagicStone && inputReader != null)
                inputReader.InteractEvent += OnInteract;
        }

        void OnDisable()
        {
            if (type == CollectibleType.MagicStone && inputReader != null)
                inputReader.InteractEvent -= OnInteract;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || !other.CompareTag(GameConstants.Tags.Player)) return;
            switch (type)
            {
                case CollectibleType.Coin:
                    ServiceLocator.Get<IGameManager>()?.AddCoins(coinValue);
                    Despawn();
                    break;
                case CollectibleType.Health:
                    // Leave the pickup in place when it would heal nothing.
                    var health = other.GetComponent<Health>();
                    if (health == null || health.CurrentHealth >= health.MaxHealth) break;
                    health.Heal(healthValue);
                    Despawn();
                    break;
                case CollectibleType.PowerUp:
                    // Misconfigured pickups must not silently consume themselves.
                    if (powerUp == null
                        || !other.TryGetComponent<IPlayer>(out var player)) break;
                    collected = true;
                    powerUp.Apply(player, powerUpDuration);
                    EventBus.Raise(new PowerUpActivatedEvent(powerUp.name, powerUpIcon, powerUpDuration));
                    Despawn();
                    break;
                case CollectibleType.MagicStone:
                    playerInTrigger = true;
                    SetIndicator(true);
                    break;
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (type == CollectibleType.MagicStone && other.CompareTag(GameConstants.Tags.Player))
            {
                playerInTrigger = false;
                SetIndicator(false);
            }
        }

        void OnInteract()
        {
            if (!playerInTrigger || collected || !GameStateManager.IsCurrentlyPlaying) return;
            collected = true;
            playerInTrigger = false;
            SetIndicator(false);
            EventBus.Raise(new LevelCompletedEvent());
        }

        void Despawn()
        {
            ServiceLocator.Get<IAudioManager>()?.PlaySound(pickupSound);
            VfxPool.Spawn(pickupEffect?.gameObject, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }

        void SetIndicator(bool enabled) { if (indicator != null) indicator.enabled = enabled; }
    }
}
