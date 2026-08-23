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
            DetachAll(default);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player) || !HasUniformScale()) return;
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

        bool HasUniformScale() =>
            Mathf.Approximately(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
    }
}
