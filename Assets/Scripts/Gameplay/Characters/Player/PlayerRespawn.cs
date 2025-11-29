using UnityEngine;
using Core.Managers;
using Core.Constants;
using UI.Managers;

namespace Gameplay.Characters.Player
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] private AudioClip checkpointSound;

        private Transform currentCheckpoint;
        private Gameplay.Health.Health playerHealth;
        private UIManager uiManager;

        private void Awake()
        {
            playerHealth = GetComponent<Gameplay.Health.Health>();
            uiManager = FindFirstObjectByType<UIManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Checkpoint))
            {
                currentCheckpoint = collision.transform;
                SoundManager.Instance?.PlaySound(checkpointSound);
                collision.enabled = false;
                collision.GetComponent<Animator>()?.SetTrigger("activate");
            }
        }

        public void RespawnCheck()
        {
            if (currentCheckpoint == null)
            {
                uiManager?.GameOver();
                return;
            }

            playerHealth?.Respawn();
            transform.position = currentCheckpoint.position;
        }

        public Transform GetCurrentCheckpoint() => currentCheckpoint;
    }
}
