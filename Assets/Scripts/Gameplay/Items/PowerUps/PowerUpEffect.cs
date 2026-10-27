using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Services;

namespace Gameplay.Items.PowerUps
{
    /// <summary>Pickup that applies a PowerUpSO to the player and announces it to the HUD.</summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class PowerUpEffect : MonoBehaviour
    {
        [SerializeField] PowerUpSO powerUp;
        [SerializeField] float duration = 5f;
        [SerializeField] Sprite icon;

        bool collected;

        void OnTriggerEnter2D(Collider2D other)
        {
            // Two colliders on the player can enter on the same frame; apply once.
            if (collected || !other.CompareTag(GameConstants.Tags.Player)) return;
            if (powerUp == null || !other.TryGetComponent<IPlayer>(out var player)) return;

            collected = true;
            powerUp.Apply(player, duration);
            EventBus.Raise(new PowerUpActivatedEvent(powerUp.name, icon, duration));
            Destroy(gameObject);
        }
    }
}
