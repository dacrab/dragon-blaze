using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour , IInteractable
{
        [SerializeField] private SpriteRenderer _interactSprite;
        private Transform _playerTransform;
        private const float INTERACT_DISTANCE = 5f;

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    
        void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && isWithinInteractDistance() )
        {
            //Interact
            Interact();
        }
        if (_interactSprite.gameObject.activeSelf && !isWithinInteractDistance())
        {
            //Turn off the sprite
            _interactSprite.gameObject.SetActive(false);
        }
        else if (!_interactSprite.gameObject.activeSelf && isWithinInteractDistance())
        {
            //turn on the sprite
            _interactSprite.gameObject.SetActive(true);
        }
    }

    public abstract void Interact();

    private bool isWithinInteractDistance()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACT_DISTANCE)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
}
