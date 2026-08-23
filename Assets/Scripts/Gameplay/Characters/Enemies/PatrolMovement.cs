using UnityEngine;
using Core.Constants;
using Core.Physics;

namespace Gameplay.Characters.Enemies
{
    [RequireComponent(typeof(Animator))]
    public class PatrolMovement : MonoBehaviour
    {
        [SerializeField] Transform leftEdge, rightEdge, enemy;
        [SerializeField] float speed = 2f, idleDuration = 1f;

        public Transform LeftEdge => leftEdge;
        public Transform RightEdge => rightEdge;

        Animator anim;
        Rigidbody2D enemyBody;
        Vector3 initScale;
        bool movingLeft;
        float idleTimer;

        void Awake()
        {
            anim = enemy.GetComponent<Animator>();
            initScale = enemy.localScale;
            enemyBody = KinematicBody.Prepare(enemy);
        }

        void OnDisable() => anim?.SetBool(GameConstants.Anim.Moving, false);

        void FixedUpdate()
        {
            float dir = movingLeft ? -1f : 1f;
            Transform target = movingLeft ? leftEdge : rightEdge;
            bool reached = Mathf.Sign(target.position.x - enemy.position.x) != dir;

            if (reached)
            {
                anim?.SetBool(GameConstants.Anim.Moving, false);
                if ((idleTimer += Time.fixedDeltaTime) >= idleDuration)
                {
                    movingLeft = !movingLeft;
                    idleTimer = 0;
                }
            }
            else
            {
                anim?.SetBool(GameConstants.Anim.Moving, true);
                enemy.localScale = new(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
                var next = enemy.position + Vector3.right * (dir * speed * Time.fixedDeltaTime);
                KinematicBody.MoveTo(enemyBody, enemy, next);
            }
        }
    }
}
