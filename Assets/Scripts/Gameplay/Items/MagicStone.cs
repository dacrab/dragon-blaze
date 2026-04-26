using UnityEngine;
using UI.Menus;
using Core.Constants;
using Core.Input;
using Core.Managers;

namespace Gameplay.Items
{
    public class MagicStone : MonoBehaviour
    {
        [SerializeField] SpriteRenderer indicator;
        [SerializeField] InputReader inputReader;

        bool playerInTrigger;

        void Start() { if (indicator != null) indicator.enabled = false; }
        void OnEnable() { if (inputReader != null) inputReader.InteractEvent += OnInteract; }
        void OnDisable() { if (inputReader != null) inputReader.InteractEvent -= OnInteract; }

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
            if (indicator != null) indicator.enabled = true;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = false;
            if (indicator != null) indicator.enabled = false;
        }
    }
}
