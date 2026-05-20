using UnityEngine;
using Core.State;
using Core.Constants;

namespace Environment.Traps
{
    public sealed class SidewaysTrap : MonoBehaviour
    {
        [SerializeField] float damage = 10f;
        [SerializeField] float movementDistance = 3f, speed = 2f;

        float startX;

        void Awake() => startX = transform.position.x;

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            float offset = Mathf.PingPong(Time.time * speed, movementDistance * 2) - movementDistance;
            transform.position = new(startX + offset, transform.position.y, transform.position.z);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.Player>();
            if (player is { IsInvisible: true }) return;
            collision.GetComponent<Gameplay.Combat.Health>()?.TakeDamage(damage);
        }
    }
}
