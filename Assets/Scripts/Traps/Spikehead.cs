using UnityEngine;

public class Spikehead : EnemyDamage
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

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.instance.PlaySound(impactSound);
        base.OnTriggerEnter2D(collision);
        Stop();
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
