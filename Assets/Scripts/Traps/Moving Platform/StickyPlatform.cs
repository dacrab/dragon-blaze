using UnityEngine;
using Core.Constants;

public class StickyPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetPlayerParent(collision, transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetPlayerParent(collision, null);
    }

    private void SetPlayerParent(Collider2D collision, Transform parent)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            collision.transform.SetParent(parent);
        }
    }
}
