using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.State;
using UI.Dialogue;

namespace Gameplay.Characters.NPCs
{
    public sealed class TalkableNPC : MonoBehaviour
    {
        [SerializeField] float interactDistance = 5f;
        [SerializeField] SpriteRenderer interactSprite;
        [SerializeField] InputReader inputReader;
        [SerializeField] Transform playerTransform;
        [SerializeField] DialogueData dialogueText;
        [SerializeField] AudioClip dialogueSound;

        void Start() { if (playerTransform == null) playerTransform = GameConstants.FindPlayer(); }
        void OnEnable() => inputReader.InteractEvent += OnInteract;
        void OnDisable() => inputReader.InteractEvent -= OnInteract;

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
                DialogueController.Instance?.DisplayNextParagraph(dialogueText, dialogueSound ?? dialogueText.dialogueSound);
        }

        bool IsWithinRange() =>
            playerTransform != null && Vector2.Distance(playerTransform.position, transform.position) < interactDistance;
    }
}
