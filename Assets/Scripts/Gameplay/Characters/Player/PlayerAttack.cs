using UnityEngine;
using Core.Constants;
using Core.Managers;
using Gameplay.Combat;
using Core.Input;
using Core.Optimization;

namespace Gameplay.Characters.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private string projectileTag = "Fireball"; // Replaced direct prefab ref with tag for pool
        [SerializeField] private AudioClip fireballSound;
        [SerializeField] private InputReader inputReader;

        private Animator anim;
        private PlayerController playerController;
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

        private void PerformAttack()
        {
            if (firePoint == null) return;

            SoundManager.instance.PlaySound(fireballSound);
            anim.SetTrigger("attack");
            cooldownTimer = 0;

            LaunchFireball();
        }

        private void LaunchFireball()
        {
            if (ObjectPoolManager.Instance != null)
            {
                GameObject fireball = ObjectPoolManager.Instance.SpawnFromPool(projectileTag, firePoint.position, Quaternion.identity);
                if (fireball != null)
                {
                    fireball.GetComponent<ProjectileBase>().SetDirection(Mathf.Sign(transform.localScale.x));
                }
            }
            else
            {
                Debug.LogWarning("ObjectPoolManager missing! Cannot spawn fireball.");
            }
        }
    }
}
