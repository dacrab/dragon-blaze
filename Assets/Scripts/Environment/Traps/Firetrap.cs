using UnityEngine;
using Core.Managers;
using System.Collections;
using Core.Constants;
using Core.Utilities;

namespace Environment.Traps
{
    public class Firetrap : TrapBase
    {
        [Header("Firetrap Timers")]
        [SerializeField] private float activationDelay;
        [SerializeField] private float activeTime;
        [SerializeField] private AudioClip firetrapSound;

        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool triggered;
        private bool active;
        private Gameplay.Health.Health playerHealth;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (playerHealth != null && active) playerHealth.TakeDamage(damage);
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (!collision.TryGetPlayerController(out var pc) || pc.IsInvisible()) return;
            playerHealth = collision.GetHealth();
            if (!triggered) StartCoroutine(ActivateFiretrap());
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) playerHealth = null;
        }

        private IEnumerator ActivateFiretrap()
        {
            triggered = true;
            spriteRend.color = Color.red;
            yield return new WaitForSeconds(activationDelay);
            SoundManager.Instance?.PlaySound(firetrapSound);
            spriteRend.color = Color.white;
            active = true;
            anim.SetBool("activated", true);
            yield return new WaitForSeconds(activeTime);
            active = false;
            triggered = false;
            anim.SetBool("activated", false);
        }
    }
}
