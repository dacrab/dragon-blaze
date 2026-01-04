using UnityEngine;
using Core.Constants;
using Core.Managers;
using UI.Managers;

namespace Gameplay.Characters.Player
{
    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] private AudioClip checkpoint;
        
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
            if (!collision.CompareTag(GameConstants.Tags.Checkpoint)) return;
            
            currentCheckpoint = collision.transform;
            SoundManager.Instance?.PlaySound(checkpoint);
            collision.enabled = false;
            collision.GetComponent<Animator>()?.SetTrigger("activate");
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
