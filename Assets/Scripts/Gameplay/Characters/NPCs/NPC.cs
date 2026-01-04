using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.State;

namespace Gameplay.Characters.NPCs
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 5f;
        [SerializeField] private SpriteRenderer interactSprite;
        [SerializeField] private InputReader inputReader;

        private Transform playerTransform;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (player != null) playerTransform = player.transform;
        }

        private void OnEnable()
        {
            if (inputReader != null) inputReader.InteractEvent += OnInteractInput;
        }

        private void OnDisable()
        {
            if (inputReader != null) inputReader.InteractEvent -= OnInteractInput;
        }
        
        private void Update()
        {
            if (!GameStateManager.Instance.IsPlaying) return;
            if (interactSprite == null) return;
            bool shouldShow = IsWithinInteractDistance();
            if (interactSprite.gameObject.activeSelf != shouldShow)
                interactSprite.gameObject.SetActive(shouldShow);
        }

        private void OnInteractInput()
        {
            if (IsWithinInteractDistance()) Interact();
        }

        protected virtual void Interact() { }

        private bool IsWithinInteractDistance() => playerTransform != null 
            && Vector2.Distance(playerTransform.position, transform.position) < interactDistance;
    }
}
