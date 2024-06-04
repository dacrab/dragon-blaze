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

        // Check if it's the final level
        if (SceneManager.GetActiveScene().buildIndex == 5) // Replace FINAL_LEVEL_INDEX with the index of your final level
        {
            // Load the credits scene by index
            yield return StartCoroutine(LoadSceneAndWait(5)); // Replace 5 with your credits scene index

            // After credits, load the main menu
            SceneManager.LoadScene(0); // Assuming 0 is your main menu index
        }
        else
        {
            // Continue with the existing logic for non-final levels
            uiManager.ShowLoadingScreen(true);
            SaveGame();
            yield return StartCoroutine(LoadNextLevelAsync());
            uiManager.ShowLoadingScreen(false);
        }
    }

    IEnumerator LoadSceneAndWait(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSeconds(10); // Wait for 10 seconds or the duration of your credits
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
            }
        }
        else
        {
            Debug.LogError("Interact Particle System Prefab is not assigned.");
        }
    }
}
