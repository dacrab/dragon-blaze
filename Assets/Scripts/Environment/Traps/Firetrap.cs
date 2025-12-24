using UnityEngine;
using Core.Combat;
using Core.Interfaces;
using Core.Managers;
using System.Collections;
using Core.Constants;
using Core.Utilities;

namespace Environment.Traps
{
    public class Firetrap : TrapBase
    {
        private const string AnimActivated = "activated";
        
        [Header("Firetrap Timers")]
        [SerializeField] private float activationDelay = 0.5f;
        [SerializeField] private float activeTime = 2f;
        [SerializeField] private AudioClip firetrapSound;

        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool triggered;
        private bool active;
        private IDamageable currentTarget;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
            damageType = DamageType.Fire;
        }

        protected override DamageInfo CreateDamageInfo()
        {
            // Fire traps deal continuous hazard damage that ignores i-frames
            return DamageInfo.Hazard(damage * Time.deltaTime, DamageType.Fire);
        }

        private void Update()
        {
            if (currentTarget != null && active && currentTarget.IsAlive)
                currentTarget.TakeDamage(CreateDamageInfo());
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (!collision.TryGetPlayerController(out var pc) || pc.IsInvisible()) return;
            
            collision.TryGetComponent(out currentTarget);
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
            anim.SetBool(AnimActivated, true);
            yield return new WaitForSeconds(activeTime);
            active = false;
            triggered = false;
            anim.SetBool(AnimActivated, false);
        }
    }
}
