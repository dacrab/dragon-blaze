using UnityEngine;
using Core.Constants;
using Core.Events;

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
        string typeName;

        void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            typeName = type.ToString();
        }

		void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag(GameConstants.Tags.Player)) return;
			if (!other.TryGetComponent<Characters.Player.Player>(out var player)) return;

			ApplyEffect(player);
			col.enabled = false;
			sprite.enabled = false;
			_ = RevertAfterDelayAsync(player);
		}

		async Awaitable RevertAfterDelayAsync(Characters.Player.Player player)
		{
			await Awaitable.WaitForSecondsAsync(duration);
			RevertEffect(player);
			Destroy(gameObject);
		}

        void ApplyEffect(Characters.Player.Player player)
        {
            switch (type)
            {
                case PowerUpType.Speed: player.ModifySpeed(multiplier); break;
                case PowerUpType.Jump: player.ModifyJump(multiplier); break;
                case PowerUpType.Invisibility: player.SetInvisibility(true); break;
                case PowerUpType.Damage:
                    originalValue = player.Damage;
                    player.SetDamage(originalValue * multiplier);
                    break;
            }

            EventBus.RaisePowerUpActivated(typeName, icon, duration);
        }

        void RevertEffect(Characters.Player.Player player)
        {
            switch (type)
            {
                case PowerUpType.Speed: player.ModifySpeed(1f / multiplier); break;
                case PowerUpType.Jump: player.ModifyJump(1f / multiplier); break;
                case PowerUpType.Invisibility: player.SetInvisibility(false); break;
                case PowerUpType.Damage: player.SetDamage(originalValue); break;
            }
        }
    }
}
