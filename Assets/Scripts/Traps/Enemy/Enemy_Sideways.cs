using UnityEngine;

public class Enemy_Sideways : MonoBehaviour
{
    [SerializeField] private float movementDistance;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    private bool movingLeft;
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        CalculateEdges();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    private void CalculateEdges()
    {
        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
    }

    private void MoveEnemy()
    {
        if (movingLeft)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
    }

    private void MoveLeft()
    {
        if (transform.position.x > leftEdge)
        {
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else
        {
            movingLeft = false;
        }
    }

    private void MoveRight()
    {
        if (transform.position.x < rightEdge)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        else
        {
            movingLeft = true;
        }
    }

    private void HandleCollision(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.IsVisible())
            {
                Health playerHealth = collision.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }
}
