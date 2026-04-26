using UnityEngine;
using UI.Menus;
using Core.Constants;
using Core.Input;
using Core.Managers;

namespace Gameplay.Items
{
    public sealed class MagicStone : MonoBehaviour
    {
        [SerializeField] SpriteRenderer indicator;
        [SerializeField] InputReader inputReader;

        bool playerInTrigger;

        void Start() => SetIndicator(false);
        void OnEnable() => inputReader.InteractEvent += OnInteract;
        void OnDisable() => inputReader.InteractEvent -= OnInteract;

        void OnInteract()
        {
            if (!playerInTrigger) return;
            GameManager.Instance?.SaveGame();
            LoadingManager.LoadNextLevel();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = true;
            SetIndicator(true);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = false;
            SetIndicator(false);
        }

        void SetIndicator(bool enabled)
        {
            if (indicator != null) indicator.enabled = enabled;
        }
    }
}
