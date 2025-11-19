using UnityEngine;
using Core.Constants;

public class ArrowTrap : TrapBase
{
    #region Serialized Fields
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;

    [Header("SFX")]
    [SerializeField] private AudioClip arrowSound;
    [SerializeField] private float soundRange = 10f;
    #endregion

    #region Private Fields
    private float cooldownTimer;
    #endregion

    #region Unity Lifecycle Methods
    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= attackCooldown && PlayerIsVisible())
        {
            Attack();
        }
    }
    
    // Override OnTriggerEnter2D if ArrowTrap shouldn't damage on touch?
    // ArrowTrap fires projectiles, so the trap itself might not do touch damage unless defined.
    // Original code: No OnTriggerEnter2D.
    // TrapBase has OnTriggerEnter2D logic to damage on touch.
    // We should disable touch damage for ArrowTrap by doing nothing in OnTriggerEnter2D 
    // UNLESS the trap is supposed to hurt when touched (unlikely for an arrow dispenser usually).
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Do nothing. Arrow trap deals damage via arrows.
    }

    #endregion

    #region Private Methods
    private void Attack()
    {
        cooldownTimer = 0;

        // Use FindGameObjectsWithTag instead of specific lookup if possible, but AudioSource.PlayClipAtPoint is simple
        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag(GameConstants.Tags.Player).transform.position) <= soundRange)
        {
            SoundManager.instance.PlaySound(arrowSound);
        }

        int arrowIndex = FindArrow();
        arrows[arrowIndex].transform.position = firePoint.position;
        arrows[arrowIndex].GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private int FindArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private bool PlayerIsVisible()
    {
        GameObject player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
        if (player == null) return false;
        
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        return playerMovement != null && playerMovement.IsVisible();
    }
    #endregion
}