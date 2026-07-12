using UnityEngine;
using Core.Constants;

namespace Environment.Platforms
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FallingPlatform : MonoBehaviour
    {
        [SerializeField] float fallDelay = 1f, destroyDelay = 2f;
        [SerializeField] Rigidbody2D rb;

        Vector3 initialPosition;
        float timer;
        bool falling;

        void Start()
        {
            initialPosition = transform.position;
            rb.bodyType = RigidbodyType2D.Static;
        }

        void Update()
        {
            if (!falling) return;
            timer += Time.deltaTime;
            if (timer >= fallDelay && rb.bodyType == RigidbodyType2D.Static)
                rb.bodyType = RigidbodyType2D.Dynamic;
            else if (timer >= fallDelay + destroyDelay)
                gameObject.SetActive(false);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameConstants.Tags.Player) && !falling)
                falling = true;
        }

    }
}
