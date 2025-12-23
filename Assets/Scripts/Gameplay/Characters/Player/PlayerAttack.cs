using UnityEngine;
using Core.Constants;
using Core.Managers;
using Gameplay.Combat;
using Core.Input;

namespace Gameplay.Characters.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] fireballs;
        [SerializeField] private AudioClip fireballSound;
        [SerializeField] private InputReader inputReader;

        private Animator anim;
        private PlayerController playerController; // Use new Controller
        private float cooldownTimer = Mathf.Infinity;

        private void Awake()
        {
            InitializeComponents();
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.AttackEvent += OnAttack;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.AttackEvent -= OnAttack;
            }
        }

        private void Update()
        {
            UpdateCooldownTimer();
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

        private void OnAttack()
        {
            if (CanAttack())
            {
                PerformAttack();
            }
        }

        private bool CanAttack()
        {
            return cooldownTimer > attackCooldown 
                   && playerController != null && playerController.CanAttack() 
                   && Time.timeScale > 0;
        }

        private bool ValidateAttackComponents()
        {
            return fireballs != null && fireballs.Length > 0 && firePoint != null;
        }

        private void PerformAttack()
        {
            if (!ValidateAttackComponents()) return;

            SoundManager.instance.PlaySound(fireballSound);
            anim.SetTrigger("attack");
            cooldownTimer = 0;

            LaunchFireball();
        }

        private void LaunchFireball()
        {
            GameObject fireball = GetFireball();
            if (fireball != null)
            {
                fireball.transform.position = firePoint.position;
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
}
