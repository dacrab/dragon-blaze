using UnityEngine;
using Core.Constants;
using Core.Managers;
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
        [SerializeField] LayerMask playerLayer;
        [SerializeField] Vector3[] checkDirections = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };
        [SerializeField] AudioClip impactSound;

        Vector3 moveDir;
        float checkTimer;
        bool attacking;

        void Update()
        {
            if (attacking)
            {
                transform.Translate(moveDir * speed * Time.deltaTime);
                return;
            }
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkDelay) CheckForPlayer();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            ServiceLocator.Get<IAudioManager>()?.PlaySound(impactSound);
            if (collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePlayer(damage);
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
