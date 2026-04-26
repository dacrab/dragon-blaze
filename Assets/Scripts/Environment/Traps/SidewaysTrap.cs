using UnityEngine;
using Core.State;

namespace Environment.Traps
{
    public sealed class SidewaysTrap : TrapBase
    {
        [SerializeField] float movementDistance = 3f, speed = 2f;

        float startX;

        void Awake() => startX = transform.position.x;

        void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            float offset = Mathf.PingPong(Time.time * speed, movementDistance * 2) - movementDistance;
            transform.position = new(startX + offset, transform.position.y, transform.position.z);
        }
    }
}
}