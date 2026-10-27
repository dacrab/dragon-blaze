using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Input;

namespace Environment.Interactables
{
    /// <summary>End-of-level stone: interacting with it completes the level and loads the next one.</summary>
    public sealed class LevelExit : MonoBehaviour
    {
        [SerializeField] SpriteRenderer? indicatorSprite;
        [SerializeField] GameObject? interactParticleSystemPrefab;

        InputReader? inputReader;
        bool playerInTrigger, used;

        // Awake, not Start: trigger callbacks can run before Start, and Start would then hide the indicator
        // while the player is already standing on the exit.
        void Awake() => SetIndicator(playerInTrigger);

        void OnEnable()
        {
            inputReader = InputReader.Instance;
            if (inputReader != null) inputReader.InteractEvent += OnInteract;
        }

        void OnDisable()
        {
            if (inputReader != null) inputReader.InteractEvent -= OnInteract;
            inputReader = null;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = true;
            SetIndicator(true);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = false;
            SetIndicator(false);
        }

        void OnInteract()
        {
            if (!playerInTrigger || used) return;
            used = true;
            playerInTrigger = false;
            SetIndicator(false);

            PlayParticles();
            // LoadingManager and GameManager both listen: this saves progress and loads the next level.
            EventBus.Raise(new LevelCompletedEvent());
        }

        void PlayParticles()
        {
            if (interactParticleSystemPrefab == null) return;
            var instance = Instantiate(interactParticleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(instance, 2f);
        }

        void SetIndicator(bool visible)
        {
            if (indicatorSprite != null) indicatorSprite.enabled = visible;
        }
    }
}
