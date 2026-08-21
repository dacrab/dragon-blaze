using UnityEngine;
using Core.Constants;
using Core.Events;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public sealed class PowerUpEffect : MonoBehaviour
    {
        [SerializeField] PowerUpSO powerUp;
        [SerializeField] float duration = 5f;
        [SerializeField] Sprite icon;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (powerUp == null) return;
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            if (!other.TryGetComponent<Player>(out var player)) return;

            powerUp.Apply(player, duration);
            EventBus.Raise(new PowerUpActivatedEvent(powerUp.name, icon, duration));
            Destroy(gameObject);
        }
    }
}
