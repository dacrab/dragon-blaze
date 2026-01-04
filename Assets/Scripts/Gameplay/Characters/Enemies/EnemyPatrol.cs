using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Enemies
{
    public class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private Transform leftEdge;
        [SerializeField] private Transform rightEdge;
        [SerializeField] private Transform enemy;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float idleDuration = 1f;
        [SerializeField] private Animator anim;

        public Transform LeftEdge => leftEdge;
        public Transform RightEdge => rightEdge;

        private Vector3 initScale;
        private bool movingLeft;
        private float idleTimer;

        private void Awake() => initScale = enemy.localScale;
        private void OnDisable() => anim?.SetBool(GameConstants.Animation.Moving, false);

        private void Update()
        {
            float direction = movingLeft ? -1f : 1f;
            float targetX = movingLeft ? leftEdge.position.x : rightEdge.position.x;
            
            if ((movingLeft && enemy.position.x >= targetX) || (!movingLeft && enemy.position.x <= targetX))
            {
                idleTimer = 0;
                anim.SetBool(GameConstants.Animation.Moving, true);
                enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
                enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y, enemy.position.z);
            }
            else
            {
                anim.SetBool(GameConstants.Animation.Moving, false);
                idleTimer += Time.deltaTime;
                if (idleTimer > idleDuration) { movingLeft = !movingLeft; idleTimer = 0; }
            }
        }
    }
}
