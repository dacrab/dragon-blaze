using UnityEngine;
using UI.Menus;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Input;
using Core.Managers;

namespace Gameplay.Items
{
    public class MagicStone : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer indicatorSprite;
        [SerializeField] private GameObject interactParticleSystemPrefab;
        [SerializeField] private InputReader inputReader;

        private bool playerInTrigger;
        private Vector3 playerPosition;
        private GameObject activeParticleInstance;

        private void Start()
        {
            if (indicatorSprite != null) indicatorSprite.enabled = false;
        }

        private void OnEnable()
        {
            if (inputReader != null) inputReader.InteractEvent += OnInteract;
        }

        private void OnDisable()
        {
            if (inputReader != null) inputReader.InteractEvent -= OnInteract;
        }

        private void OnInteract()
        {
            if (playerInTrigger) StartCoroutine(PlayParticlesThenLoadLevel(playerPosition));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = true;
            playerPosition = other.transform.position;
            if (indicatorSprite != null) indicatorSprite.enabled = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            playerInTrigger = false;
            if (indicatorSprite != null) indicatorSprite.enabled = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void PlayInteractParticleSystem(Vector3 position)
        {
            if (interactParticleSystemPrefab == null) return;
            if (activeParticleInstance == null || !activeParticleInstance.activeInHierarchy)
            {
                if (activeParticleInstance != null) Destroy(activeParticleInstance);
                activeParticleInstance = Instantiate(interactParticleSystemPrefab, position + new Vector3(0, 0, -1), Quaternion.identity);
            }
        }

        private IEnumerator PlayParticlesThenLoadLevel(Vector3 position)
        {
            PlayInteractParticleSystem(position);
            yield return new WaitForSeconds(interactParticleSystemPrefab.GetComponent<ParticleSystem>().main.duration);

            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                yield return new WaitForSeconds(10);
                LoadingManager.LoadSpecificLevel(0);
            }
            else
            {
                GameManager.Instance?.SaveGame();
                LoadingManager.LoadNextLevel();
            }
        }
    }
}
