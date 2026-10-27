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

        float startX, startTime;
        Rigidbody2D? body;

        void Awake()
        {
            startX = transform.position.x;
            body = KinematicBody.Prepare(this);
        }

        void OnEnable() => startTime = Time.fixedTime;

        void FixedUpdate()
        {
            // Phase from this trap's own elapsed time, so it always starts at its leftmost point instead of
            // snapping to wherever the global clock happened to be when the scene loaded.
            // PingPong sweeps [0, 2 * movementDistance]; the subtraction centers it to [-movementDistance, +movementDistance] around startX.
            float elapsed = Time.fixedTime - startTime;
            float offset = Mathf.PingPong(elapsed * speed, movementDistance * 2) - movementDistance;
            KinematicBody.MoveTo(body, transform, new(startX + offset, transform.position.y, transform.position.z));
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(damage);
        }
    }
}
