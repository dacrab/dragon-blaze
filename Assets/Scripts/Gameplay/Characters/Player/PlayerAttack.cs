using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Utilities;
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

        private void OnEnable() { if (inputReader != null) inputReader.AttackEvent += OnAttack; }
        private void OnDisable() { if (inputReader != null) inputReader.AttackEvent -= OnAttack; }
        private void Update() => cooldownTimer += Time.deltaTime;

        private void InitializeComponents()
        {
            anim = GetComponent<Animator>();
            playerController = this.GetPlayerController();
        }

        private void OnAttack()
        {
            if (CanAttack())
            {
                PerformAttack();
            }
        }

        private bool CanAttack() => cooldownTimer > attackCooldown 
            && playerController != null && playerController.CanAttack()
            && (Core.State.GameStateManager.Instance?.IsPlaying ?? Time.timeScale > 0);

        private void PerformAttack()
        {
            if (fireballs == null || fireballs.Length == 0 || firePoint == null) return;

            SoundManager.Instance?.PlaySound(fireballSound);
            anim.SetTrigger("attack");
            cooldownTimer = 0;

            var fireball = System.Array.Find(fireballs, f => !f.activeInHierarchy);
            if (fireball != null)
            {
                fireball.transform.position = firePoint.position;
                fireball.GetComponent<ProjectileBase>()?.SetDirection(Mathf.Sign(transform.localScale.x));
            }
        }
    }
}
