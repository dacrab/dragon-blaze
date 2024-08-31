using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        UpdateCooldownTimer();
        CheckForAttack();
    }

    private void InitializeComponents()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void UpdateCooldownTimer()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void CheckForAttack()
    {
        if (CanAttack())
        {
            Attack();
        }
    }

    private bool CanAttack()
    {
        return Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack() && Time.timeScale > 0;
    }

    private void Attack()
    {
        if (!ValidateAttackComponents())
        {
            return;
        }

        PerformAttack();
    }

    private bool ValidateAttackComponents()
    {
        if (SoundManager.instance == null)
        {
            Debug.LogError("SoundManager instance is not initialized.");
            return false;
        }
        if (fireballs == null || fireballs.Length == 0)
        {
            Debug.LogError("Fireballs array is not initialized or empty.");
            return false;
        }
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned.");
            return false;
        }
        return true;
    }

    private void PerformAttack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        LaunchFireball();
    }

    private void LaunchFireball()
    {
        int fireballIndex = FindFireball();
        if (fireballIndex != -1 && fireballs[fireballIndex] != null)
        {
            GameObject fireball = fireballs[fireballIndex];
            fireball.transform.position = firePoint.position;
            fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        }
        else
        {
            Debug.LogError("Invalid fireball index or fireball is null.");
        }
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return -1;
    }
}
