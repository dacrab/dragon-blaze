using System.Collections;
using UnityEngine;
using Core.Constants;

namespace Environment.Platforms
{
    public class FallingPlatform : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private float fallDelay = 1f;
        [SerializeField] private float destroyDelay = 2f;
        [SerializeField] private Rigidbody2D rb;
        #endregion

        #region Private Fields
        private Vector3 initialPosition;
        #endregion

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

        #region Public Methods
        public void ResetPlatform()
        {
            gameObject.SetActive(true);
            transform.position = initialPosition;
            rb.bodyType = RigidbodyType2D.Static;
            rb.linearVelocity = Vector2.zero; // Ensure it stops moving
        }
        #endregion
    }
}
