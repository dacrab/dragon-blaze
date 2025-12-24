using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.Utilities;
using Gameplay.Interaction;

namespace Gameplay.Characters.NPCs
{
    public abstract class NPC : MonoBehaviour, IInteractable
    {
        #region Serialized Fields
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private SpriteRenderer _interactSprite;
        [SerializeField] private InputReader inputReader;
        #endregion

        #region Private Fields
        private Transform _playerTransform;
        #endregion

        private void Start()
        {
            _playerTransform = PlayerReference.Transform;
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.InteractEvent += OnInteractInput;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.InteractEvent -= OnInteractInput;
        }
        
        private void Update()
        {
            if (!GameStateHelpers.IsPlaying) return;
            UpdateInteractSprite();
        }

        private void OnInteractInput()
        {
            if (IsWithinInteractDistance())
                Interact();
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
            && Vector2.Distance(_playerTransform.position, transform.position) < interactDistance;
    }
}
