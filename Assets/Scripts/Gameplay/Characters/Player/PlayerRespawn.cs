using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Core.Utilities;
using UI.Managers;

namespace Gameplay.Characters.Player
{
    public class PlayerRespawn : MonoBehaviour
    {
        [SerializeField] private AudioClip checkpoint;
        
        private Transform currentCheckpoint;
        private Gameplay.Health.Health playerHealth;
        [AutoWire(AutoWireAttribute.WireType.Service, required: false)]
        [SerializeField] private UIManager uiManager;

        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
            playerHealth = this.GetHealth();
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
