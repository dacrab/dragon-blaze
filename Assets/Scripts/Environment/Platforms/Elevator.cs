using UnityEngine;
using Core.Physics;

namespace Environment.Platforms
{
    /// <summary>Moves a platform up and down between the two heights in <see cref="lengthRange"/>.</summary>
    public sealed class Elevator : MonoBehaviour
    {
        [SerializeField] Vector2 lengthRange = new(2f, 5f);
        [SerializeField] float waitTime = 1f;
        [SerializeField] float moveSpeed = 3f;
        [SerializeField] State startState = State.Up;
        [SerializeField] Rigidbody2D? platform;
        [SerializeField] SpriteRenderer? chainL, chainR;

        float length, waitTimer;
        State state;

        void Start()
        {
            if (platform == null) return;
            state = startState;
            Length = state == State.Up ? lengthRange.y : lengthRange.x;
        }

        void FixedUpdate()
        {
            if (platform == null) return;

            if (waitTimer > 0f)
            {
                waitTimer -= Time.fixedDeltaTime;
                return;
            }

            float target = state == State.Up ? lengthRange.y : lengthRange.x;
            Length = Mathf.MoveTowards(length, target, moveSpeed * Time.fixedDeltaTime);

            if (!Mathf.Approximately(length, target)) return;
            state = state == State.Up ? State.Down : State.Up;
            waitTimer = waitTime;
        }

        /// <summary>Distance the platform hangs below its rest position.</summary>
        float Length
        {
            get => length;
            set
            {
                length = Mathf.Max(0f, value);
                // Drive the kinematic body through the physics step instead of writing the transform under it.
                // Length is expressed in the platform's parent space, so convert to the world position MoveTo wants.
                var parent = platform!.transform.parent;
                var local = new Vector3(0f, -length, 0f);
                var world = parent != null ? parent.TransformPoint(local) : local;
                KinematicBody.MoveTo(platform, platform.transform, world);

                // Chains are tiled sprites; their height tracks the hanging length.
                if (chainL != null) chainL.size = new Vector2(0.09375f, length - 0.25f);
                if (chainR != null) chainR.size = new Vector2(0.09375f, length - 0.25f);
            }
        }

        public enum State
        {
            Up,
            Down
        }
    }
}
