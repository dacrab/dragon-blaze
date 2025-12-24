using UnityEngine;
using Core.Combat;
using Core.Interfaces;
using Core.Managers;
using Core.Constants;
using Core.Utilities;

namespace Environment.Traps
{
    public class Spikehead : TrapBase
    {
        [Header("SpikeHead Attributes")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float checkDelay = 0.5f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private AudioClip impactSound;

        private Vector3[] directions = new Vector3[4];
        private Vector3 destination;
        private float checkTimer;
        private bool attacking;

        private void Awake()
        {
            damageType = DamageType.Physical;
        }

        private void OnEnable() => Stop();

        private void Update()
        {
            if (!GameStateHelpers.IsPlaying) return;
            
            if (attacking) transform.Translate(destination * Time.deltaTime * speed);
            else
            {
                checkTimer += Time.deltaTime;
                if (checkTimer > checkDelay) CheckForPlayer();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            SoundManager.Instance?.PlaySound(impactSound);
            if (collision.CompareTag(GameConstants.Tags.Player) 
                && collision.TryGetPlayerController(out var pc) && !pc.IsInvisible())
                base.OnTriggerEnter2D(collision);
            Stop();
        }

        private void CheckForPlayer()
        {
            CalculateDirections();
            for (int i = 0; i < directions.Length; i++)
            {
                Debug.DrawRay(transform.position, directions[i], Color.red);
                var hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);
                if (hit.collider != null && !attacking && hit.collider.TryGetPlayerController(out var pc) && !pc.IsInvisible())
                {
                    attacking = true;
                    destination = directions[i];
                    checkTimer = 0;
                }
            }
        }

        private void CalculateDirections()
        {
            directions[0] = transform.right * range;
            directions[1] = -transform.right * range;
            directions[2] = transform.up * range;
            directions[3] = -transform.up * range;
        }

        private void Stop()
        {
            destination = transform.position;
            attacking = false;
        }
    }
}
