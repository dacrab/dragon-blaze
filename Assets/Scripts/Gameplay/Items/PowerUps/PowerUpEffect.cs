using UnityEngine;
using Core.Constants;

namespace Gameplay.Items.PowerUps
{
    public enum PowerUpType { Speed, Jump, Invisibility, Damage }

    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public sealed class PowerUpEffect : MonoBehaviour
    {
        [SerializeField] PowerUpType type;
        [SerializeField] float duration = 5f;
        [SerializeField] float multiplier = 2f;
        [SerializeField] Sprite icon;

        SpriteRenderer sprite;
        Collider2D col;
        float originalValue;

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
        }

        async void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GameConstants.Tags.Player)) return;
            var player = other.GetComponent<Characters.Player.Player>();
            if (player == null) return;

            switch (type)
            {
                case PowerUpType.Speed:
                    player.ModifySpeed(multiplier);
                    break;
                case PowerUpType.Jump:
                    player.ModifyJump(multiplier);
                    break;
                case PowerUpType.Invisibility:
                    player.SetInvisibility(true);
                    break;
                case PowerUpType.Damage:
                    originalValue = player.Damage;
                    player.SetDamage(originalValue * multiplier);
                    break;
            }

            await PowerUpRoutine(player);
        }

        async Awaitable PowerUpRoutine(Characters.Player.Player player)
        {
            col.enabled = false;
            sprite.enabled = false;
            await Awaitable.WaitForSecondsAsync(duration);

            switch (type)
            {
                case PowerUpType.Speed:
                    player.ModifySpeed(1f / multiplier);
                    break;
                case PowerUpType.Jump:
                    player.ModifyJump(1f / multiplier);
                    break;
                case PowerUpType.Invisibility:
                    player.SetInvisibility(false);
                    break;
                case PowerUpType.Damage:
                    player.SetDamage(originalValue);
                    break;
            }

            Destroy(gameObject);
        }
    }
}
