using UnityEngine;
using Core.Constants;
using Core.Input;
using Core.State;

namespace Gameplay.Characters.NPCs;

public class NPC : MonoBehaviour
{
    [SerializeField] float interactDistance = 5f;
    [SerializeField] SpriteRenderer interactSprite;
    [SerializeField] InputReader inputReader;

    Transform playerTransform;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
        if (player != null) playerTransform = player.transform;
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
