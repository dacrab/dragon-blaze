using UnityEngine;
using Core.Constants;
using Core.Utilities;

namespace Environment.Traps
{
    public class EnemySideways : TrapBase
    {
        [SerializeField] private float movementDistance;
        [SerializeField] private float speed;

        private bool movingLeft;
        private float leftEdge;
        private float rightEdge;

        private void Awake()
        {
            leftEdge = transform.position.x - movementDistance;
            rightEdge = transform.position.x + movementDistance;
        }

        private void Update()
        {
            float direction = movingLeft ? -1f : 1f;
            float newX = transform.position.x + direction * speed * Time.deltaTime;
            
            if (movingLeft && newX <= leftEdge) { newX = leftEdge; movingLeft = false; }
            else if (!movingLeft && newX >= rightEdge) { newX = rightEdge; movingLeft = true; }
            
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player) 
                && collision.TryGetPlayerController(out var pc) && !pc.IsInvisible())
                base.OnTriggerEnter2D(collision);
        }
    }
}
