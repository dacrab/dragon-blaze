using UnityEngine;
using Core.Managers;
using Core.Input;
using UI.Menus;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Constants;

namespace Gameplay.Items
{
    public class MagicStone : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private SpriteRenderer indicatorSprite;
        [SerializeField] private GameObject interactParticleSystemPrefab;

        private bool playerInTrigger;
        private Vector3 playerPosition;
        private GameObject activeParticleSystemInstance;

        private void Awake()
        {
            if (indicatorSprite == null)
                indicatorSprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.InteractEvent += OnInteract;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.InteractEvent -= OnInteract;
        }

        private void Start()
        {
            if (indicatorSprite != null)
                indicatorSprite.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(GameConstants.Tags.Player))
            {
                playerInTrigger = true;
                playerPosition = other.gameObject.transform.position;
                if (indicatorSprite != null)
                    indicatorSprite.enabled = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
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

        private void OnInteract()
        {
            if (playerInTrigger)
            {
                StartCoroutine(PlayParticlesThenLoadLevel(playerPosition));
            }
        }

        private void PlayInteractParticleSystem(Vector3 position)
        {
            if (interactParticleSystemPrefab == null) return;

            if (activeParticleSystemInstance != null)
                Destroy(activeParticleSystemInstance);

            activeParticleSystemInstance = Instantiate(interactParticleSystemPrefab, position + Vector3.back, Quaternion.identity);
        }

        private IEnumerator PlayParticlesThenLoadLevel(Vector3 position)
        {
            PlayInteractParticleSystem(position);
            
            var ps = interactParticleSystemPrefab.GetComponent<ParticleSystem>();
            if (ps != null)
                yield return new WaitForSeconds(ps.main.duration);

            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                yield return new WaitForSeconds(2f);
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
