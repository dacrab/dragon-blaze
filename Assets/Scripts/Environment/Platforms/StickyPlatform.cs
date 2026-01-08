using UnityEngine;
using Core.Constants;

namespace Environment.Platforms;

[RequireComponent(typeof(Collider2D))]
public sealed class StickyPlatform : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
            collision.transform.SetParent(transform);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
            collision.transform.SetParent(null);
    }
}
