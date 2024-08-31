using UnityEngine;
using UnityEngine.InputSystem;

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
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
        if (Keyboard.current.eKey.wasPressedThisFrame && IsWithinInteractDistance())
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
        return Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE;
    }
    #endregion
}
