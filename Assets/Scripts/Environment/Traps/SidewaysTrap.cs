using UnityEngine;
using Core.Constants;
using Gameplay.Combat;

namespace Environment.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class SidewaysTrap : MonoBehaviour
    {
        [SerializeField] float damage = 10f;
        [SerializeField] float movementDistance = 3f, speed = 2f;

        float startX;

        void Awake() => startX = transform.position.x;

        void Update()
        {
            float offset = Mathf.PingPong(Time.time * speed, movementDistance * 2) - movementDistance;
            transform.position = new(startX + offset, transform.position.y, transform.position.z);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(damage);
        }
    }
}
