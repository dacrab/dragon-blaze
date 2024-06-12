using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public GameObject magicStone; // The magic stone collider
    public SpriteRenderer indicator; // The sprite renderer
    public ParticleSystem myParticleSystem; // The particle system
    [SerializeField] private float minLoadingTime = 2f; // Minimum time to display the loading screen
    [SerializeField] private float loadingImageDelay = 1f; // Delay before showing the loading image
    public UIManager uiManager; // Reference to the UIManager

    private bool isNearMagicStone = false;

    void Update()
    {
        if (isNearMagicStone && Input.GetKeyDown(KeyCode.E))
        {
            indicator.enabled = false; // Disable the sprite renderer
            myParticleSystem.Play(); // Play the particle system
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(loadingImageDelay); // Add delay before showing the loading screen

        uiManager.ShowLoadingScreen(true); // Show the loading screen
        Debug.Log("Loading screen shown");

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        operation.allowSceneActivation = false; // Prevent the scene from activating immediately
        float startTime = Time.time;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            uiManager.UpdateLoadingImage(progress); // Update the loading image
            Debug.Log("Loading progress: " + progress);

            if (operation.progress >= 0.9f && Time.time - startTime >= minLoadingTime)
            {
                operation.allowSceneActivation = true; // Allow the scene to activate
            }

            yield return null;
        }

        uiManager.ShowLoadingScreen(false); // Hide the loading screen
        Debug.Log("Loading screen hidden");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isNearMagicStone = true;
            indicator.enabled = true; // Enable the sprite renderer
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isNearMagicStone = false;
            indicator.enabled = false; // Disable the sprite renderer
        }
    }
}
