using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int damage;
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
    #endregion

    #region Private Methods
    private void Attack()
    {
        cooldownTimer = 0;

        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= soundRange)
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
        PlayerMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        return playerMovement != null && playerMovement.IsVisible();
    }
    #endregion
}