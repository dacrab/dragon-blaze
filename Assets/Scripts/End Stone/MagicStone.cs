using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MagicStone : MonoBehaviour
{
    public UIManager uiManager; // Assign your UIManager in the Inspector
    public SpriteRenderer indicatorSprite; // Assign your SpriteRenderer in the Inspector
    public GameObject interactParticleSystemPrefab; // Assign your particle system prefab in the Inspector
    public string levelToLoad; // Set this to the name of the level to load in the Inspector
    private bool playerInTrigger = false;
    private Vector3 playerPosition; // Store the player's position when they enter the trigger area

    void Start()
    {
        // Ensure the SpriteRenderer is initially disabled
        if (indicatorSprite != null)
            indicatorSprite.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = true;
            playerPosition = other.gameObject.transform.position; // Store the player's position
            if (indicatorSprite != null)
                indicatorSprite.enabled = true; // Enable SpriteRenderer
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerPosition = other.gameObject.transform.position; // Continuously update player's position
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = false;

            if (indicatorSprite != null)
                indicatorSprite.enabled = false; // Disable SpriteRenderer

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (playerInTrigger && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PlayParticlesThenLoadLevel(playerPosition));
        }
    }

    IEnumerator PlayParticlesThenLoadLevel(Vector3 position)
    {
        PlayInteractParticleSystem(position); // Play the particle system at player position

        // Wait for the particle system to finish
        yield return new WaitForSeconds(interactParticleSystemPrefab.GetComponent<ParticleSystem>().main.duration);

        LoadLevel(); // Then load the level
    }

    private void LoadLevel()
    {
        if (uiManager != null)
        {
            uiManager.Play(levelToLoad); // This will handle the particle system and loading screen
        }
    }

    private void PlayInteractParticleSystem(Vector3 position)
    {
        // Instantiate the particle system at the player's position
        if (interactParticleSystemPrefab != null)
        {
            GameObject particleSystemInstance = Instantiate(interactParticleSystemPrefab, position, Quaternion.identity);
            ParticleSystem ps = particleSystemInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
    }
}
