using UnityEngine;

namespace Gameplay.Characters.Enemies
{
    public class EnemyPatrol : MonoBehaviour
    {
        private const string AnimMoving = "moving";
        
        #region Serialized Fields
        [Header("Patrol Points")]
        [SerializeField] private Transform leftEdge;
        [SerializeField] private Transform rightEdge;

        [Header("Enemy")]
        [SerializeField] private Transform enemy;

        [Header("Movement Parameters")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float idleDuration = 1f;

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
            if (anim != null) anim.SetBool(AnimMoving, false);
        }

        private void Update()
        {
            float direction = movingLeft ? -1f : 1f;
            float targetX = movingLeft ? leftEdge.position.x : rightEdge.position.x;
            
            if ((movingLeft && enemy.position.x >= targetX) || (!movingLeft && enemy.position.x <= targetX))
            {
                idleTimer = 0;
                anim.SetBool(AnimMoving, true);
                enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
                enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed, enemy.position.y, enemy.position.z);
            }
            else
            {
                anim.SetBool(AnimMoving, false);
                idleTimer += Time.deltaTime;
                if (idleTimer > idleDuration) { movingLeft = !movingLeft; idleTimer = 0; }
            }
        }
        #endregion
    }
}
