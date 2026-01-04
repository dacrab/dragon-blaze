using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;

namespace Environment.Traps
{
    public class Spikehead : TrapBase
    {
        [Header("SpikeHead")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float range = 5f;
        [SerializeField] private float checkDelay = 0.5f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private AudioClip impactSound;

        private readonly Vector3[] directions = new Vector3[4];
        private Vector3 destination;
        private float checkTimer;
        private bool attacking;

        private void OnEnable() => Stop();

        private void Update()
        {
            if (!GameStateManager.IsCurrentlyPlaying) return;
            
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
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                var player = collision.GetComponent<Gameplay.Characters.Player.PlayerController>();
                if (player == null || !player.IsInvisible())
                    base.OnTriggerEnter2D(collision);
            }
            Stop();
        }

        private void CheckForPlayer()
        {
            directions[0] = transform.right;
            directions[1] = -transform.right;
            directions[2] = transform.up;
            directions[3] = -transform.up;
            
            for (int i = 0; i < directions.Length; i++)
            {
                var hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);
                if (hit.collider != null && !attacking)
                {
                    var player = hit.collider.GetComponent<Gameplay.Characters.Player.PlayerController>();
                    if (player == null || !player.IsInvisible())
                    {
                        attacking = true;
                        destination = directions[i];
                        checkTimer = 0;
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
