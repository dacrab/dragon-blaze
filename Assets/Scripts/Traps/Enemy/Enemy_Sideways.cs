using UnityEngine;
using Core.Constants;

public class Enemy_Sideways : TrapBase
{
    [SerializeField] private float movementDistance;
    [SerializeField] private float speed;
    // Damage inherited from TrapBase

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

    // OnTriggerEnter2D inherited from TrapBase handles damage
    
    // But Enemy_Sideways logic had a check for IsVisible which TrapBase might not have.
    // TrapBase just calls DealDamage.
    // Let's override OnTriggerEnter2D to keep the IsVisible check if we want to be safe
    // Or better, update DealDamage to check.
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null && playerMovement.IsVisible())
            {
                base.OnTriggerEnter2D(collision);
            }
        }
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
            if (transform.position.x > leftEdge)
            {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                movingLeft = false;
            }
        }
        else
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
    }
}

