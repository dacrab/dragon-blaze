using UnityEngine;
using Core.Constants;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Environment.Platforms
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FallingPlatform : MonoBehaviour
    {
        [SerializeField] private float fallDelay = 1f;
        [SerializeField] private float destroyDelay = 2f;

        private Rigidbody2D rb;
        private Vector3 initialPosition;
        private CancellationTokenSource cts;
        private bool isFalling;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            initialPosition = transform.position;
            rb.bodyType = RigidbodyType2D.Static;
        }

        private void OnDestroy() => cts?.Cancel();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isFalling) return;
            
            if (collision.gameObject.CompareTag(GameConstants.Tags.Player))
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();
                FallAsync(cts.Token).Forget();
            }
        }

        private async UniTaskVoid FallAsync(CancellationToken token)
        {
            isFalling = true;
            
            await UniTask.Delay((int)(fallDelay * 1000), cancellationToken: token);
            rb.bodyType = RigidbodyType2D.Dynamic;
            
            await UniTask.Delay((int)(destroyDelay * 1000), cancellationToken: token);
            gameObject.SetActive(false);
            
            isFalling = false;
        }

        public void ResetPlatform()
        {
            cts?.Cancel();
            isFalling = false;
            gameObject.SetActive(true);
            transform.position = initialPosition;
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
