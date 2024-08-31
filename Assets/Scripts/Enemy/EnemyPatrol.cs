using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    #region Serialized Fields
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float idleDuration;

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;
    #endregion

    #region Properties
    public Transform LeftEdge => leftEdge;
    public Transform RightEdge => rightEdge;
    #endregion

    #region Private Fields
    private Vector3 initScale;
    private bool movingLeft;
    private float idleTimer;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        initScale = enemy.localScale;
    }

    private void OnDisable()
    {
        anim.SetBool("moving", false);
    }

    private void Update()
    {
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }
    #endregion

    #region Movement Methods
    private void DirectionChange()
    {
        anim.SetBool("moving", false);
        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0;
        }
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        anim.SetBool("moving", true);

        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);

        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }
    #endregion
}