using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public GameObject magicStone; // The magic stone collider
    public SpriteRenderer indicator; // The sprite renderer
    public ParticleSystem myParticleSystem; // The particle system

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
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        while (!operation.isDone)
        {
            // Here you can add your loading screen logic
            // For example, you can update a progress bar using operation.progress

            yield return null;
        }
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
