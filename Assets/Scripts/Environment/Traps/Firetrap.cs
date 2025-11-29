using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Environment.Traps
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class Firetrap : TrapBase
    {
        [Header("Firetrap Timers")]
        [SerializeField] private float activationDelay = 0.5f;
        [SerializeField] private float activeTime = 2f;

        [Header("SFX")]
        [SerializeField] private AudioClip firetrapSound;

        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool triggered;
        private bool active;
        private IDamageable playerDamageable;
        private CancellationTokenSource cts;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
        }

        private void OnDestroy() => cts?.Cancel();

        private void Update()
        {
            if (playerDamageable != null && active && !playerDamageable.IsDead)
            {
                playerDamageable.TakeDamage(damage * Time.deltaTime);
            }
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;

            var controller = PlayerReference.Controller;
            if (controller != null && !controller.IsInvisible())
            {
                playerDamageable = collision.GetComponent<IDamageable>();
                if (!triggered)
                {
                    cts?.Cancel();
                    cts = new CancellationTokenSource();
                    ActivateFiretrapAsync(cts.Token).Forget();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
                playerDamageable = null;
        }

        private async UniTaskVoid ActivateFiretrapAsync(CancellationToken token)
        {
            triggered = true;
            spriteRend.color = Color.red;
            
            await UniTask.Delay((int)(activationDelay * 1000), cancellationToken: token);
            
            SoundManager.Instance?.PlaySound(firetrapSound);
            spriteRend.color = Color.white;
            active = true;
            anim.SetBool("activated", true);
            
            await UniTask.Delay((int)(activeTime * 1000), cancellationToken: token);
            
            active = false;
            triggered = false;
            anim.SetBool("activated", false);
        }
    }
}
