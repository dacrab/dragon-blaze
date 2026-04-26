using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Enemies
{

[RequireComponent(typeof(Animator))]
public class EnemyPatrol : MonoBehaviour
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

    void OnDisable() => anim.SetBool(GameConstants.Animation.Moving, false);

    void Update()
    {
        float dir = movingLeft ? -1f : 1f;
        float targetX = movingLeft ? leftEdge.position.x : rightEdge.position.x;
        bool reachedTarget = movingLeft ? enemy.position.x <= targetX : enemy.position.x >= targetX;

        if (!reachedTarget)
        {
            anim.SetBool(GameConstants.Animation.Moving, true);
            enemy.localScale = new(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);
            enemy.position += Vector3.right * (dir * speed * Time.deltaTime);
        }
        else
        {
            anim.SetBool(GameConstants.Animation.Moving, false);
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration) { movingLeft = !movingLeft; idleTimer = 0; }
        }
    }
}
}