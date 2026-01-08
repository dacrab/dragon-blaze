using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.State;

namespace Gameplay.Characters.NPCs
{

public class NPC : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] float interactDistance = 5f;
    [SerializeField] SpriteRenderer interactSprite;
    
    [Header("Input")]
    [SerializeField] InputReader inputReader;
    
    [Header("Target (auto-finds if empty)")]
    [SerializeField] Transform playerTransform;

    void Start()
    {
        if (playerTransform == null)
        {
            var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (go != null) playerTransform = go.transform;
        }
    }

    void OnEnable() { if (inputReader != null) inputReader.InteractEvent += OnInteractInput; }
    void OnDisable() { if (inputReader != null) inputReader.InteractEvent -= OnInteractInput; }
    
    void Update()
    {
        if (!GameStateManager.IsCurrentlyPlaying || interactSprite == null) return;
        bool shouldShow = IsWithinRange();
        if (interactSprite.gameObject.activeSelf != shouldShow)
            interactSprite.gameObject.SetActive(shouldShow);
    }

    void OnInteractInput() { if (IsWithinRange()) Interact(); }
    protected virtual void Interact() { }
    bool IsWithinRange() => playerTransform != null && Vector2.Distance(playerTransform.position, transform.position) < interactDistance;
}
}