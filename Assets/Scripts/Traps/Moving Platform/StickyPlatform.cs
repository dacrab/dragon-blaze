using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    private const string PlayerTag = "Player";

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
        if (collision.CompareTag(PlayerTag))
        {
            collision.transform.SetParent(parent);
        }
    }
}
