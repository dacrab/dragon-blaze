using UnityEngine;
using Core.Constants;
using Core.Systems;
using Player; // Namespace for PlayerController

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;

    private Animator anim;
    private PlayerController playerController; // Use new Controller
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
        playerController = GetComponent<PlayerController>();
    }

    private void UpdateCooldownTimer()
    {
        cooldownTimer += Time.deltaTime;
    }

    private void CheckForAttack()
    {
        // Use legacy Input directly or better: Move Attack input to InputReader?
        // Ideally, Attack should be an event in InputReader. 
        // But for now, keeping legacy Input.GetMouseButton(0) is acceptable if we didn't add Attack to InputReader.
        // I didn't add Attack to InputReader.
        // Let's stick to legacy input here but check Time.timeScale
        
        if (CanAttack())
        {
            Attack();
        }
    }

    private bool CanAttack()
    {
        // PlayerController.CanAttack() checks grounded & not moving
        return Input.GetMouseButton(0) 
               && cooldownTimer > attackCooldown 
               && playerController != null && playerController.CanAttack() 
               && Time.timeScale > 0;
    }

    private void Attack()
    {
        if (!ValidateAttackComponents()) return;

        PerformAttack();
    }

    private bool ValidateAttackComponents()
    {
        // SoundManager check redundant if we trust it exists, but okay
        return fireballs != null && fireballs.Length > 0 && firePoint != null;
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
        // Use Object Pool or Array
        GameObject fireball = GetFireball();
        if (fireball != null)
        {
            fireball.transform.position = firePoint.position;
            // Projectile inherits ProjectileBase. 
            // SetDirection handles activation.
            fireball.GetComponent<ProjectileBase>().SetDirection(Mathf.Sign(transform.localScale.x));
        }
    }

    private GameObject GetFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return fireballs[i];
        }
        return null;
    }
}

