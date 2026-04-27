using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.State;
using UI.Dialogue;

namespace Gameplay.Characters.NPCs
{

public sealed class TalkableNPC : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactDistance = 5f;
    [SerializeField] SpriteRenderer interactSprite;
    
    [Header("Input")]
    [SerializeField] InputReader inputReader;
    
    [Header("Target (auto-finds if empty)")]
    [SerializeField] Transform playerTransform;

    [Header("Dialogue")]
    [SerializeField] DialogueData dialogueText;
    [SerializeField] DialogueController dialogueController;
    [SerializeField] AudioClip dialogueSound;

    void Awake() => dialogueController ??= FindFirstObjectByType<DialogueController>();

    void Start()
    {
        if (playerTransform == null)
        {
            var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (go != null) playerTransform = go.transform;
        }
    }

    void OnEnable() => inputReader.InteractEvent += OnInteractInput;
    void OnDisable() => inputReader.InteractEvent -= OnInteractInput;
    
    void Update()
    {
        if (!GameStateManager.IsCurrentlyPlaying) return;
        bool shouldShow = IsWithinRange();
        if (interactSprite.gameObject.activeSelf != shouldShow)
            interactSprite.gameObject.SetActive(shouldShow);
    }

    void OnInteractInput() { if (IsWithinRange()) Interact(); }
    
    void Interact() => dialogueController.DisplayNextParagraph(dialogueText, dialogueSound ?? dialogueText.dialogueSound);
    
    bool IsWithinRange() => playerTransform != null && Vector2.Distance(playerTransform.position, transform.position) < interactDistance;
}
}
