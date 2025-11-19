using UnityEngine;
using Core.Constants;

public class Spikehead : TrapBase
{
    #region Serialized Fields
    [Header("SpikeHead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip impactSound;
    #endregion

    #region Private Fields
    private Vector3[] directions = new Vector3[4];
    private Vector3 destination;
    private float checkTimer;
    private bool attacking;
    #endregion

    #region Unity Lifecycle Methods
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
        // Spikehead plays sound on ANY impact (including walls?)
        // Original code called SoundManager then base.OnTriggerEnter2D
        
        SoundManager.instance.PlaySound(impactSound);
        
        // base.OnTriggerEnter2D(collision) in TrapBase checks for Player tag and deals damage
        // Original EnemyDamage did the same but with explicit IsVisible check.
        // We should check here before calling base if we want to maintain IsVisible check strictly
        // Or assume TrapBase handles damage and we just add sound.
        
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            // Check visibility
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null && player.IsVisible())
            {
                base.OnTriggerEnter2D(collision);
            }
        }
        
        Stop(); // Stop on impact with anything
    }
    #endregion

    #region Private Methods
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
        PlayerMovement playerMovement = hit.collider.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.IsVisible())
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

