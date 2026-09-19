using System.Collections.Generic;
using UnityEngine;
using Core.Constants;
using Core.Events;

namespace Environment.Platforms
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class StickyPlatform : MonoBehaviour
    {
        readonly HashSet<Transform> riders = new();

        void OnEnable() => EventBus.Subscribe<PlayerRespawnEvent>(DetachAll);

        void OnDisable()
        {
            EventBus.Unsubscribe<PlayerRespawnEvent>(DetachAll);
            // Riders must not remain children of a platform that is being disabled or destroyed.
            DetachAll(default);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player) || !HasUniformScale()) return;
            // Parent the rider to the platform so it inherits the platform's motion.
            if (riders.Add(collision.transform)) collision.transform.SetParent(transform);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) Detach(collision.transform);
        }

        void Detach(Transform rider)
        {
            if (rider != null && riders.Remove(rider)) rider.SetParent(null);
        }

        void DetachAll(PlayerRespawnEvent _)
        {
            foreach (var rider in riders)
                if (rider != null) rider.SetParent(null);
            riders.Clear();
        }

        // A rider parented to a non-uniformly scaled platform would inherit a distorted scale.
        bool HasUniformScale() =>
            Mathf.Approximately(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
    }
}
