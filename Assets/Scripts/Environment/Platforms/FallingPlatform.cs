using System.Collections;
using UnityEngine;
using Core.Constants;

namespace Environment.Platforms
{

public sealed class FallingPlatform : MonoBehaviour
{
    [SerializeField] float fallDelay = 1f, destroyDelay = 2f;
    [SerializeField] Rigidbody2D rb;
    
    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        rb.bodyType = RigidbodyType2D.Static;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.Tags.Player)) StartCoroutine(Fall());
    }

    IEnumerator Fall()
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