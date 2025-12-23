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

        private void Start() => _playerTransform = Core.Utilities.PlayerReference.Transform;
        
        private void Update()
        {
            if (Keyboard.current?.eKey.wasPressedThisFrame == true && IsWithinInteractDistance())
                Interact();
            UpdateInteractSprite();
        }

        public abstract void Interact();

        private void UpdateInteractSprite()
        {
            if (_interactSprite == null) return;
            bool shouldBeActive = IsWithinInteractDistance();
            if (_interactSprite.gameObject.activeSelf != shouldBeActive)
                _interactSprite.gameObject.SetActive(shouldBeActive);
        }

        private bool IsWithinInteractDistance() => _playerTransform != null 
            && Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE;
    }
}
