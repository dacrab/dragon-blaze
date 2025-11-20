using UnityEngine;
using Core.Constants;
using Gameplay.Characters.Player;
using Environment.Traps.Stats;

namespace Environment.Traps
{
    public class EnemySideways : TrapBase
    {
        [Header("Configuration")]
        [SerializeField] private TrapStatsSO stats;

        private float movementDistance;
        private float speed;
        
        private bool movingLeft;
        private float leftEdge;
        private float rightEdge;

        private void Awake()
        {
            InitializeStats();
            CalculateEdges();
        }

        private void InitializeStats()
        {
            if (stats != null)
            {
                movementDistance = stats.movementDistance;
                speed = stats.speed;
                damage = stats.damage;
            }
            else
            {
                // Fallback
                movementDistance = 3f;
                speed = 2f;
                damage = 1f;
            }
        }

        private void Update()
        {
            MoveEnemy();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                PlayerController playerController = collision.GetComponent<PlayerController>();
                if (playerController != null && !playerController.IsInvisible())
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
}
