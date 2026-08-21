using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.State;
using Gameplay.Dialogue;
using Core.Services;

namespace Gameplay.Characters.NPCs
{
    public sealed class TalkableNPC : MonoBehaviour
    {
        [SerializeField] float interactDistance = 5f;
        [SerializeField] SpriteRenderer interactSprite;
        [SerializeField] Transform playerTransform;
        [SerializeField] DialogueData dialogueText;
        [SerializeField] AudioClip dialogueSound;

        InputReader inputReader;

        void Start() { if (playerTransform == null) playerTransform = GameConstants.FindPlayer(); }
        void OnEnable()
        {
            inputReader = InputReader.Instance;
            if (inputReader != null) inputReader.InteractEvent += OnInteract;
        }
        void OnDisable() { if (inputReader != null) inputReader.InteractEvent -= OnInteract; }

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            bool show = IsWithinRange();
            if (interactSprite.gameObject.activeSelf != show)
                interactSprite.gameObject.SetActive(show);
        }

        void OnInteract()
        {
            if (IsWithinRange())
                ServiceLocator.Get<IDialogueController>()?.DisplayNextParagraph(dialogueText, dialogueSound ?? dialogueText.dialogueSound);
        }

        bool IsWithinRange() =>
            playerTransform != null && (playerTransform.position - transform.position).sqrMagnitude < interactDistance * interactDistance;
    }
}
