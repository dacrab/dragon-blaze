using UnityEngine;
using Core.Constants;
using Core.Physics;
using Gameplay.Combat;

namespace Environment.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class SidewaysTrap : MonoBehaviour
    {
        [SerializeField] float damage = 10f;
        [SerializeField] float movementDistance = 3f, speed = 2f;

        float startX;
        Rigidbody2D body;

        void Awake()
        {
            startX = transform.position.x;
            body = KinematicBody.Prepare(this);
        }

        void FixedUpdate()
        {
            float offset = Mathf.PingPong(Time.time * speed, movementDistance * 2) - movementDistance;
            KinematicBody.MoveTo(body, transform, new(startX + offset, transform.position.y, transform.position.z));
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(damage);
        }
    }
}
