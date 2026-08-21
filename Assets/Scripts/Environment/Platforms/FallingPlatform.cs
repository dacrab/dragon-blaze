using UnityEngine;
using Core.Constants;

namespace Environment.Platforms
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FallingPlatform : MonoBehaviour
    {
        [SerializeField] float fallDelay = 1f, destroyDelay = 2f;
        [SerializeField] Rigidbody2D rb;

        float timer;
        bool falling;

        void Start()
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        void Update()
        {
            if (!falling) return;
            timer += Time.deltaTime;
            if (timer >= fallDelay + destroyDelay)
            {
                gameObject.SetActive(false);
                return;
            }
            if (timer >= fallDelay && rb.bodyType == RigidbodyType2D.Static)
                rb.bodyType = RigidbodyType2D.Dynamic;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameConstants.Tags.Player))
                falling = true;
        }
    }
}
