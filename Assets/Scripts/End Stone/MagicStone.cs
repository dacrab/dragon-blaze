using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement; // Make sure to include this for scene management

public class MagicStone : MonoBehaviour
{
    public UIManager uiManager; // Assign your UIManager in the Inspector
    public SpriteRenderer indicatorSprite; // Assign your SpriteRenderer in the Inspector
    public GameObject interactParticleSystemPrefab; // Assign your particle system prefab in the Inspector
    private bool playerInTrigger = false;
    private Vector3 playerPosition; // Store the player's position when they enter the trigger area
    private GameObject activeParticleSystemInstance = null;

    void Start()
    {
        if (indicatorSprite != null)
            indicatorSprite.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = true;
            playerPosition = other.gameObject.transform.position;
            if (indicatorSprite != null)
                indicatorSprite.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (indicatorSprite != null)
                indicatorSprite.enabled = false;
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
        // Play the particle system at player position
        PlayInteractParticleSystem(position);

        // Wait for the particle system to finish
        yield return new WaitForSeconds(interactParticleSystemPrefab.GetComponent<ParticleSystem>().main.duration);

        // Show loading screen (assuming UIManager handles this)
        uiManager.ShowLoadingScreen(true);

        // Save the game
        SaveGame();

        // Load the next level asynchronously
        yield return StartCoroutine(LoadNextLevelAsync());

        // Hide loading screen after the level is loaded
        uiManager.ShowLoadingScreen(false);
    }

    private void SaveGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame(); // Call SaveGame on the GameManager
        }
    }

    IEnumerator LoadNextLevelAsync()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneIndex + 1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void PlayInteractParticleSystem(Vector3 position)
    {
        if (interactParticleSystemPrefab != null)
        {
            if (activeParticleSystemInstance == null || !activeParticleSystemInstance.activeInHierarchy)
            {
                if (activeParticleSystemInstance != null)
                    Destroy(activeParticleSystemInstance);

                activeParticleSystemInstance = Instantiate(interactParticleSystemPrefab, position, Quaternion.identity);
                ParticleSystem ps = activeParticleSystemInstance.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                }

                // Optionally, automatically destroy the particle system after it has finished
                Destroy(activeParticleSystemInstance, ps.main.duration);
            }
        }
    }
}
