using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Input;
using Core.Managers;
using Gameplay.Combat;

namespace Gameplay.Items.Collectibles
{
    public enum CollectibleType { Coin, Health, MagicStone }

    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField] CollectibleType type;
        [SerializeField] int coinValue = 1;
        [SerializeField] float healthValue = 25f;
        [SerializeField] SpriteRenderer indicator;
        [SerializeField] AudioClip pickupSound;
        [SerializeField] ParticleSystem pickupEffect;
        [SerializeField] InputReader inputReader;

        bool playerInTrigger;

        void Start() { if (type == CollectibleType.MagicStone) SetIndicator(false); }

        void OnEnable()
        {
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
            if (!other.CompareTag(GameConstants.Tags.Player)) return;

            switch (type)
            {
                case CollectibleType.Coin:
                    Collect();
                    GameManager.Instance?.AddCoins(coinValue);
                    gameObject.SetActive(false);
                    break;
                case CollectibleType.Health:
                    other.GetComponent<Health>()?.Heal(healthValue);
                    Collect();
                    gameObject.SetActive(false);
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
            if (!playerInTrigger) return;
            GameManager.Instance?.SaveGame();
            EventBus.RaiseRequestNextLevel();
            EventBus.RaiseLevelCompleted();
        }

        void Collect()
        {
            GameManager.Instance?.PlaySound(pickupSound);
            if (pickupEffect != null)
            {
                var effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        void SetIndicator(bool enabled) { if (indicator != null) indicator.enabled = enabled; }
    }
}
