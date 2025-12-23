using UnityEngine;
using UI.Menus;
using UI.Managers;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Utilities;

namespace Gameplay.Items
{
    public class MagicStone : MonoBehaviour
    {
        #region Serialized Fields
        [AutoWire(AutoWireAttribute.WireType.Scene)]
        [SerializeField] private UIManager uiManager;
        
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private SpriteRenderer indicatorSprite;
        
        [SerializeField] private GameObject interactParticleSystemPrefab;
        #endregion

        #region Private Fields
        private bool playerInTrigger = false;
        private Vector3 playerPosition;
        private GameObject activeParticleSystemInstance = null;
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
        }

        private void Start() { if (indicatorSprite != null) indicatorSprite.enabled = false; }

        private void Update()
        {
            if (playerInTrigger && Keyboard.current?.eKey.wasPressedThisFrame == true)
                StartCoroutine(PlayParticlesThenLoadLevel(playerPosition));
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
            if (activeParticleSystemInstance == null || !activeParticleSystemInstance.activeInHierarchy)
            {
                if (activeParticleSystemInstance != null) Destroy(activeParticleSystemInstance);
                activeParticleSystemInstance = Instantiate(interactParticleSystemPrefab, position + new Vector3(0, 0, -1), Quaternion.identity);
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
                Core.Managers.GameManager.Instance?.SaveGame();
                LoadingManager.LoadNextLevel();
            }
        }
        #endregion
    }
}
