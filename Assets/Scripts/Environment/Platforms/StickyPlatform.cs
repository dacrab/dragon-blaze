using UnityEngine;
using Core.Constants;

namespace Environment.Platforms
{
    [RequireComponent(typeof(Collider2D))]
    public class StickyPlatform : MonoBehaviour
    {
        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

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
}
