using UnityEngine;
using Core.Constants;

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
        _playerTransform = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player).transform;
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
        // Use legacy input directly for simplicity unless we have reference to InputReader
        if (Input.GetKeyDown(KeyCode.E) && IsWithinInteractDistance())
        {
            Interact();
        }
    }

    private void UpdateInteractSprite()
    {
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

