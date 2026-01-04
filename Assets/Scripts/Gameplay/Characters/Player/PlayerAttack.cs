using UnityEngine;
using Core.Managers;
using Core.State;
using Core.Input;
using Gameplay.Combat;

namespace Gameplay.Characters.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] private float attackCooldown = 0.5f;
        
        [Header("Projectile")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] fireballs;
        
        [Header("Audio")]
        [SerializeField] private AudioClip fireballSound;
        
        [Header("Input")]
        [SerializeField] private InputReader inputReader;

        private Animator anim;
        private PlayerController playerController;
        private float cooldownTimer;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            playerController = GetComponentInParent<PlayerController>();
        }

        private void OnEnable()
        {
            if (inputReader != null) inputReader.AttackEvent += OnAttack;
        }

        private void OnDisable()
        {
            if (inputReader != null) inputReader.AttackEvent -= OnAttack;
        }

        private void Update() => cooldownTimer += Time.deltaTime;

        private void OnAttack()
        {
            if (cooldownTimer < attackCooldown) return;
            if (playerController != null && !playerController.CanAttack()) return;
            if (!GameStateManager.Instance.IsPlaying) return;

            SoundManager.Instance?.PlaySound(fireballSound);
            anim?.SetTrigger("attack");
            cooldownTimer = 0f;
            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            if (firePoint == null || fireballs == null) return;
            float direction = Mathf.Sign(transform.localScale.x);
            
            var fireball = System.Array.Find(fireballs, f => !f.activeInHierarchy);
            if (fireball != null)
            {
                fireball.transform.position = firePoint.position;
                fireball.GetComponent<ProjectileBase>()?.SetDirection(direction);
            }
        }
    }
}
