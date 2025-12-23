using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.Utilities;
using Gameplay.Combat;

namespace Environment.Traps
{
    public class ArrowTrap : TrapBase
    {
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] arrows;
        [SerializeField] private AudioClip arrowSound;
        [SerializeField] private float soundRange = 10f;

        private float cooldownTimer;

        private void Update()
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown && PlayerIsVisible()) Attack();
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision) { }

        private void Attack()
        {
            cooldownTimer = 0;
            if (Core.Utilities.PlayerReference.IsValid 
                && Vector3.Distance(transform.position, Core.Utilities.PlayerReference.Transform.position) <= soundRange)
                SoundManager.Instance?.PlaySound(arrowSound);

            int arrowIndex = System.Array.FindIndex(arrows, a => !a.activeInHierarchy);
            if (arrowIndex < 0) arrowIndex = 0;
            arrows[arrowIndex].transform.position = firePoint.position;
            arrows[arrowIndex].GetComponent<EnemyProjectile>().ActivateProjectile();
        }

        private bool PlayerIsVisible() => Core.Utilities.PlayerReference.IsValid 
            && Core.Utilities.PlayerReference.Controller != null 
            && !Core.Utilities.PlayerReference.Controller.IsInvisible();
    }
}
