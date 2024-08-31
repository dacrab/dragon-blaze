using System.Collections;
using UnityEngine;

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

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializePlatform();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }
    #endregion

    #region Private Methods
    private void InitializePlatform()
    {
        initialPosition = transform.position;
        rb.bodyType = RigidbodyType2D.Static;
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(destroyDelay);
        gameObject.SetActive(false);
    }
    #endregion

    #region Public Methods
    public void ResetPlatform()
    {
        gameObject.SetActive(true);
        transform.position = initialPosition;
        rb.bodyType = RigidbodyType2D.Static;
    }
    #endregion
}
