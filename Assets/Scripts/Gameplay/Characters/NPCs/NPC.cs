using UnityEngine;
using Core.Constants;
using Gameplay.Interaction;
using UnityEngine.InputSystem;

namespace Gameplay.Characters.NPCs
{
    public abstract class NPC : MonoBehaviour, IInteractable
    {
        #region Serialized Fields
        [SerializeField] private const float INTERACT_DISTANCE = 5f;
        [SerializeField] private SpriteRenderer _interactSprite;
        #endregion

        #region Private Fields
        private Transform _playerTransform;
        #endregion

        #region Unity Lifecycle Methods
        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null)
                _playerTransform = player.transform;
        }
        
        private void Update()
        {
            HandleInteraction();
            UpdateInteractSprite();
        }
        #endregion

        #region Public Methods
        public abstract void Interact();
        #endregion

        #region Private Methods
        private void HandleInteraction()
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && IsWithinInteractDistance())
            {
                Interact();
            }
        }

        private void UpdateInteractSprite()
        {
            if (_interactSprite == null) return;
            
            bool shouldBeActive = IsWithinInteractDistance();
            if (_interactSprite.gameObject.activeSelf != shouldBeActive)
            {
                _interactSprite.gameObject.SetActive(shouldBeActive);
            }
        }

        private bool IsWithinInteractDistance()
        {
            if (_playerTransform == null) return false;
            return Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE;
        }
        #endregion
    }
}
