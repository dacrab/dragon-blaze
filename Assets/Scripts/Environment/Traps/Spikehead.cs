using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Physics;
using Core.Services;
using Gameplay.Characters.Player;
using Gameplay.Combat;

namespace Environment.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Spikehead : MonoBehaviour
    {
        [SerializeField] float damage = 10f;
        [SerializeField] float speed = 5f;
        [SerializeField] float range = 5f;
        [SerializeField] float checkDelay = 0.5f;
        [SerializeField] float maxAttackDistance = 12f;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] Vector3[] checkDirections = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };
        [SerializeField] AudioClip impactSound;

        Vector3 moveDir;
        float checkTimer, attackDistance;
        bool attacking;
        Rigidbody2D body;

        void Awake() => body = KinematicBody.Prepare(this);

        void FixedUpdate()
        {
            if (attacking)
            {
                float step = speed * Time.fixedDeltaTime;
                KinematicBody.MoveTo(body, transform, transform.position + moveDir * step);
                attackDistance += step;
                if (attackDistance >= maxAttackDistance) StopAttack();
                return;
            }
            checkTimer += Time.fixedDeltaTime;
            if (checkTimer >= checkDelay) CheckForPlayer();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            ServiceLocator.Get<IAudioManager>()?.PlaySound(impactSound);
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(damage);
            StopAttack();
        }

        void StopAttack()
        {
            attacking = false;
            moveDir = Vector3.zero;
        }

        void CheckForPlayer()
        {
            checkTimer = 0;
            foreach (var dir in checkDirections)
            {
                var worldDir = transform.TransformDirection(dir);
                var hit = Physics2D.Raycast(transform.position, worldDir, range, playerLayer);
                if (hit.collider == null) continue;
                if (hit.collider.TryGetComponent<Player>(out var player) && !player.IsInvisible)
                {
                    attacking = true;
                    moveDir = worldDir;
                    attackDistance = 0f;
                    return;
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            foreach (var dir in checkDirections)
                Gizmos.DrawRay(transform.position, transform.TransformDirection(dir) * range);
        }
    }
}
