using UnityEngine;
using Core.Managers;
using Core.Constants;
using Gameplay.Characters.Player;
using Environment.Traps.Stats;

namespace Environment.Traps
{
    public class Spikehead : TrapBase
    {
        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private TrapStatsSO stats;

        [Header("SpikeHead Specifics")]
        [SerializeField] private float checkDelay;
        [SerializeField] private LayerMask playerLayer;

        [Header("SFX")]
        [SerializeField] private AudioClip impactSound;
        #endregion

        #region Private Fields
        private float speed;
        private float range;
        
        private Vector3[] directions = new Vector3[4];
        private Vector3 destination;
        private float checkTimer;
        private bool attacking;
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            InitializeStats();
        }

        private void OnEnable()
        {
            Stop();
        }

        private void Update()
        {
            if (attacking)
            {
                MoveSpikehead();
            }
            else
            {
                UpdateCheckTimer();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            SoundManager.instance.PlaySound(impactSound);
            
            if (collision.CompareTag(GameConstants.Tags.Player))
            {
                // Check visibility
                PlayerController playerController = collision.GetComponent<PlayerController>();
                if (playerController != null && !playerController.IsInvisible())
                {
                    base.OnTriggerEnter2D(collision);
                }
            }
            
            Stop(); // Stop on impact with anything
        }
        #endregion

        #region Private Methods
        private void InitializeStats()
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

        private void MoveSpikehead()
        {
            transform.Translate(destination * Time.deltaTime * speed);
        }

        private void UpdateCheckTimer()
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }

        private void CheckForPlayer()
        {
            CalculateDirections();

            for (int i = 0; i < directions.Length; i++)
            {
                Debug.DrawRay(transform.position, directions[i], Color.red);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

                if (hit.collider != null && !attacking)
                {
                    TryAttackPlayer(hit, i);
                }
            }
        }

        private void TryAttackPlayer(RaycastHit2D hit, int directionIndex)
        {
            PlayerController playerController = hit.collider.GetComponent<PlayerController>();
            if (playerController != null && !playerController.IsInvisible())
            {
                attacking = true;
                destination = directions[directionIndex];
                checkTimer = 0;
            }
        }

        private void CalculateDirections()
        {
            directions[0] = transform.right * range;   // Right direction
            directions[1] = -transform.right * range;  // Left direction
            directions[2] = transform.up * range;      // Up direction
            directions[3] = -transform.up * range;     // Down direction
        }

        private void Stop()
        {
            destination = transform.position;
            attacking = false;
        }
        #endregion
    }
}
