using System.Collections;
using UnityEngine;
using Core.Constants;

namespace Environment.Platforms
{
    public class FallingPlatform : MonoBehaviour
    {
        [SerializeField] private float fallDelay = 1f;
        [SerializeField] private float destroyDelay = 2f;
        [SerializeField] private Rigidbody2D rb;
        
        private Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.position;
            rb.bodyType = RigidbodyType2D.Static;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameConstants.Tags.Player)) StartCoroutine(Fall());
        }

        private IEnumerator Fall()
        {
            yield return new WaitForSeconds(fallDelay);
            rb.bodyType = RigidbodyType2D.Dynamic;
            yield return new WaitForSeconds(destroyDelay);
            gameObject.SetActive(false);
        }

        public void ResetPlatform()
        {
            gameObject.SetActive(true);
            transform.position = initialPosition;
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
