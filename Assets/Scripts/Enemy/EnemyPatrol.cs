using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge; // Left edge of the patrol area
    [SerializeField] private Transform rightEdge; // Right edge of the patrol area

    public Transform LeftEdge => leftEdge;
    public Transform RightEdge => rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy; // Reference to the enemy's transform

    [Header("Movement parameters")]
    [SerializeField] private float speed; // Speed at which the enemy moves
    private Vector3 initScale; // Initial scale of the enemy
    private bool movingLeft; // Flag indicating whether the enemy is currently moving left

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration; // Duration of idle time after reaching an edge
    private float idleTimer; // Timer for tracking idle time

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim; // Reference to the enemy's animator component


    private void Awake()
    {
        initScale = enemy.localScale;
    }

    private void OnDisable()
    {
        // Set the "moving" parameter in the animator to false when the enemy is disabled
        anim.SetBool("moving", false);
    }

    private void Update()
    {
        // Check if the enemy should move left or right based on its current direction
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1); // Move left
            else
                DirectionChange(); // Change direction if the left edge is reached
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1); // Move right
            else
                DirectionChange(); // Change direction if the right edge is reached
        }
    }

    // Method to handle changing direction and idle behavior
    private void DirectionChange()
    {
        anim.SetBool("moving", false); // Stop moving animation
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0; // Reset the idle timer only when direction changes
        }
    }

    // Method to move the enemy in a specific direction
    private void MoveInDirection(int _direction)
    {
        idleTimer = 0; // Reset the idle timer
        anim.SetBool("moving", true); // Set the "moving" parameter to true in the animator

        // Make the enemy face the specified direction
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);

        // Move the enemy in the specified direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }
}