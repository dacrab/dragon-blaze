using UnityEngine;
using Core.Constants;
using Core.State;

namespace Environment.Traps
{
    public class EnemySideways : TrapBase
    {
        [SerializeField] private float movementDistance = 3f;
        [SerializeField] private float speed = 2f;

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
            if (!GameStateManager.Instance.IsPlaying) return;
            
            float direction = movingLeft ? -1f : 1f;
            float newX = transform.position.x + direction * speed * Time.deltaTime;
            
            if (movingLeft && newX <= leftEdge) { newX = leftEdge; movingLeft = false; }
            else if (!movingLeft && newX >= rightEdge) { newX = rightEdge; movingLeft = true; }
            
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.PlayerController>();
            if (player != null && player.IsInvisible()) return;
            base.OnTriggerEnter2D(collision);
        }
    }
}
