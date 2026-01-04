using UnityEngine;
using Core.Managers;
using System.Collections;
using Core.Constants;

namespace Environment.Traps
{
    public class Firetrap : TrapBase
    {
        [Header("Firetrap")]
        [SerializeField] private float activationDelay = 0.5f;
        [SerializeField] private float activeTime = 2f;
        [SerializeField] private AudioClip firetrapSound;

        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool triggered;
        private bool active;
        private Gameplay.Health.Health currentTarget;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (currentTarget != null && active && currentTarget.IsAlive)
                currentTarget.TakeDamage(damage * Time.deltaTime);
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.PlayerController>();
            if (player != null && player.IsInvisible()) return;
            
            currentTarget = collision.GetComponent<Gameplay.Health.Health>();
            if (!triggered) StartCoroutine(ActivateFiretrap());
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player)) currentTarget = null;
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
