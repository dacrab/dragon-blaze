using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.Utilities;
using Environment.Traps.Stats;

namespace Environment.Traps
{
    public class Spikehead : TrapBase
    {
        [Header("Configuration")]
        [SerializeField] private TrapStatsSO stats;

        [Header("SpikeHead Specifics")]
        [SerializeField] private float checkDelay;
        [SerializeField] private LayerMask playerLayer;

        [Header("SFX")]
        [SerializeField] private AudioClip impactSound;

        private float speed;
        private float range;
        private Vector3[] directions = new Vector3[4];
        private Vector3 destination;
        private float checkTimer;
        private bool attacking;

        private void Awake()
        {
            if (stats != null)
            {
                speed = stats.speed;
                range = stats.attackRange;
                damage = stats.damage;
            }
            else
            {
                speed = 10f;
                range = 10f;
                damage = 1f;
            }
        }

        private void OnEnable() => Stop();

        private void Update()
        {
            if (attacking)
            {
                transform.Translate(destination * Time.deltaTime * speed);
            }
            else
            {
                checkTimer += Time.deltaTime;
                if (checkTimer > checkDelay)
                    CheckForPlayer();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            SoundManager.Instance?.PlaySound(impactSound);
            
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                var controller = PlayerReference.Controller;
                if (controller != null && !controller.IsInvisible())
                {
                    base.OnTriggerEnter2D(collision);
                }
            }
            
            Stop();
        }

        private void CheckForPlayer()
        {
            directions[0] = transform.right * range;
            directions[1] = -transform.right * range;
            directions[2] = transform.up * range;
            directions[3] = -transform.up * range;

            for (int i = 0; i < directions.Length; i++)
            {
                Debug.DrawRay(transform.position, directions[i], Color.red);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

                if (hit.collider != null && !attacking)
                {
                    var controller = hit.collider.GetComponent<Gameplay.Characters.Player.PlayerController>();
                    if (controller != null && !controller.IsInvisible())
                    {
                        attacking = true;
                        destination = directions[i];
                        checkTimer = 0;
                        break;
                    }
                }
            }
        }

        private void Stop()
        {
            destination = transform.position;
            attacking = false;
        }
    }
}
