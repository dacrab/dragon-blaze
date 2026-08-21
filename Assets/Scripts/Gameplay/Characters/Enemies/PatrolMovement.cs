using UnityEngine;
using Core.Constants;

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
        Vector3 initScale;
        bool movingLeft;
        float idleTimer;

        void Awake()
        {
            anim = enemy.GetComponent<Animator>();
            initScale = enemy.localScale;
        }

        void OnDisable() => anim?.SetBool(GameConstants.Anim.Moving, false);

        void Update()
        {
            float dir = movingLeft ? -1f : 1f;
            Transform target = movingLeft ? leftEdge : rightEdge;
            bool reached = Mathf.Sign(target.position.x - enemy.position.x) != dir;

            if (reached)
            {
                anim?.SetBool(GameConstants.Anim.Moving, false);
                if ((idleTimer += Time.deltaTime) >= idleDuration)
                {
                    movingLeft = !movingLeft;
                    idleTimer = 0;
                }
            }
            else
            {
                anim?.SetBool(GameConstants.Anim.Moving, true);
                enemy.localScale = new(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
                enemy.position += Vector3.right * (dir * speed * Time.deltaTime);
            }
        }
    }
}
