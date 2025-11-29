using UnityEngine;
using Core.Input;
using Core.Utilities;
using Gameplay.Interaction;

namespace Gameplay.Characters.NPCs
{
    public abstract class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private SpriteRenderer interactSprite;

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.InteractEvent += OnInteract;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.InteractEvent -= OnInteract;
        }
        
        private void Update()
        {
            UpdateInteractSprite();
        }

        public abstract void Interact();

        private void OnInteract()
        {
            if (IsWithinInteractDistance())
            {
                Interact();
            }
        }

        private void UpdateInteractSprite()
        {
            if (interactSprite == null) return;
            
            bool shouldBeActive = IsWithinInteractDistance();
            if (interactSprite.gameObject.activeSelf != shouldBeActive)
            {
                interactSprite.gameObject.SetActive(shouldBeActive);
            }
        }

        private bool IsWithinInteractDistance()
        {
            if (!PlayerReference.IsValid) return false;
            return Vector2.Distance(PlayerReference.Transform.position, transform.position) < interactDistance;
        }
    }
}
