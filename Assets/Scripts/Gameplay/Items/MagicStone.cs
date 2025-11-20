using UnityEngine;
using Core.Managers;
using UI.Managers;
using UI.Menus;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Constants;

namespace Gameplay.Items
{
    public class MagicStone : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private UIManager uiManager;
        [SerializeField] private SpriteRenderer indicatorSprite;
        [SerializeField] private GameObject interactParticleSystemPrefab;
        #endregion

        #region Private Fields
        private bool playerInTrigger = false;
        private Vector3 playerPosition;
        private GameObject activeParticleSystemInstance = null;
        #endregion

        #region Unity Lifecycle Methods
        private void Start()
        {
            InitializeIndicatorSprite();
        }

        private void Update()
        {
            CheckForPlayerInteraction();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandlePlayerEnter(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            HandlePlayerExit(other);
        }
        #endregion

        #region Initialization
        private void InitializeIndicatorSprite()
        {
            if (indicatorSprite != null)
                indicatorSprite.enabled = false;
        }
        #endregion

        #region Player Interaction
        private void HandlePlayerEnter(Collider2D other)
        {
            if (other.gameObject.CompareTag(GameConstants.Tags.Player))
            {
                playerInTrigger = true;
                playerPosition = other.gameObject.transform.position;
                if (indicatorSprite != null)
                    indicatorSprite.enabled = true;
            }
        }

        private void HandlePlayerExit(Collider2D other)
        {
            if (other.gameObject.CompareTag(GameConstants.Tags.Player))
            {
                playerInTrigger = false;
                if (indicatorSprite != null)
                    indicatorSprite.enabled = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void CheckForPlayerInteraction()
        {
            if (playerInTrigger && Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(PlayParticlesThenLoadLevel(playerPosition));
            }
        }
        #endregion

        #region Particle System
        private void PlayInteractParticleSystem(Vector3 position)
        {
            if (interactParticleSystemPrefab != null)
            {
                if (activeParticleSystemInstance == null || !activeParticleSystemInstance.activeInHierarchy)
                {
                    if (activeParticleSystemInstance != null)
                        Destroy(activeParticleSystemInstance);

                    activeParticleSystemInstance = Instantiate(interactParticleSystemPrefab, position + new Vector3(0, 0, -1), Quaternion.identity);
                }
            }
            else
            {
                Debug.LogError("Interact Particle System Prefab is not assigned.");
            }
        }
        #endregion

        #region Level Loading
        private IEnumerator PlayParticlesThenLoadLevel(Vector3 position)
        {
            PlayInteractParticleSystem(position);
            yield return new WaitForSeconds(interactParticleSystemPrefab.GetComponent<ParticleSystem>().main.duration);

            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                yield return StartCoroutine(LoadSceneAndWait(SceneManager.GetActiveScene().buildIndex));
                LoadingManager.LoadSpecificLevel(0);
            }
            else
            {
                SaveGame();
                LoadingManager.LoadNextLevel();
            }
        }

        private IEnumerator LoadSceneAndWait(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
            yield return new WaitForSeconds(10);
        }
        #endregion

        #region Game State Management
        private void SaveGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveGame();
            }
        }
        #endregion
    }
}
