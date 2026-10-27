using System.Collections.Generic;
using UnityEngine;

namespace Environment.Platforms
{
    /// <summary>Platform that springs up and launches whatever is standing on it.</summary>
    public sealed class BoundingPlatform : MonoBehaviour
    {
        [SerializeField] Transform? platform;
        [SerializeField] float waitTime = 1f;
        [SerializeField] float retrieveSpeed = 1f;
        [SerializeField] float pushSpeed = 15f;

        readonly List<Rigidbody2D> riders = new();
        readonly List<Rigidbody2D> launched = new();

        float restY, currentY, waitTimer;
        bool retreating;

        void Start()
        {
            if (platform == null) return;
            restY = platform.localPosition.y;
            currentY = restY;
        }

        void FixedUpdate()
        {
            if (platform == null) return;

            waitTimer += Time.fixedDeltaTime;
            if (waitTimer < waitTime) return;
            waitTimer = 0f;

            if (!retreating)
            {
                // Launch happens at the resting height; forcing y=0 teleported the platform to its parent origin.
                currentY = restY;
                Push();
                retreating = true;
            }
            else
            {
                currentY = Mathf.MoveTowards(currentY, restY, retrieveSpeed * Time.fixedDeltaTime);
                if (Mathf.Approximately(currentY, restY)) retreating = false;
            }

            platform.localPosition = new Vector3(0f, currentY, 0f);
        }

        void Push()
        {
            launched.Clear();
            launched.AddRange(riders);
            riders.Clear();

            foreach (var body in launched)
            {
                if (body == null) continue;
                body.linearVelocity += pushSpeed * Vector2.up;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            var body = collision.attachedRigidbody;
            if (body == null || body.bodyType != RigidbodyType2D.Dynamic || riders.Contains(body)) return;
            riders.Add(body);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            var body = collision.attachedRigidbody;
            if (body != null) riders.Remove(body);
        }
    }
}
