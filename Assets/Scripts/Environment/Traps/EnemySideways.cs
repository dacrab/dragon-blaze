using UnityEngine;
using Core.Constants;
using Core.Utilities;
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
            if (stats != null)
            {
                movementDistance = stats.movementDistance;
                speed = stats.speed;
                damage = stats.damage;
            }
            else
            {
                movementDistance = 3f;
                speed = 2f;
                damage = 1f;
            }
            
            leftEdge = transform.position.x - movementDistance;
            rightEdge = transform.position.x + movementDistance;
        }

        private void Update()
        {
            float currentX = transform.position.x;
            float direction = movingLeft ? -1f : 1f;
            
            transform.position += Vector3.right * (direction * speed * Time.deltaTime);
            
            if (movingLeft && currentX <= leftEdge) movingLeft = false;
            else if (!movingLeft && currentX >= rightEdge) movingLeft = true;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            
            var controller = PlayerReference.Controller;
            if (controller != null && !controller.IsInvisible())
            {
                base.OnTriggerEnter2D(collision);
            }
        }
    }
}
