using UnityEngine;
using Core.Managers;
using System.Collections;
using Core.Constants;
using Gameplay.Characters.Player;
using Gameplay.Health;

namespace Environment.Traps
{
    public class Firetrap : TrapBase
    {
        #region Serialized Fields
        // Damage inherited from TrapBase

        [Header("Firetrap Timers")]
        [SerializeField] private float activationDelay;
        [SerializeField] private float activeTime;

        [Header("SFX")]
        [SerializeField] private AudioClip firetrapSound;
        #endregion

        #region Private Fields
        private Animator anim;
        private SpriteRenderer spriteRend;
        private bool triggered; // When the trap gets triggered
        private bool active;    // When the trap is active and can hurt the player
        private Health playerHealth;  // Reference to PlayerHealth component
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            ApplyDamageIfActive();
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;

            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null && !playerController.IsInvisible())
            {
                SetPlayerHealth(collision);
                ActivateTrapIfNotTriggered();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(GameConstants.Tags.Player))
                playerHealth = null;
        }
        #endregion

        #region Private Methods
        private void InitializeComponents()
        {
            anim = GetComponent<Animator>();
            spriteRend = GetComponent<SpriteRenderer>();
        }

        private void ApplyDamageIfActive()
        {
            if (playerHealth != null && active)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        private void SetPlayerHealth(Collider2D collision)
        {
            playerHealth = collision.GetComponent<Health>();
        }

        private void ActivateTrapIfNotTriggered()
        {
            if (!triggered)
                StartCoroutine(ActivateFiretrap());
        }

        private IEnumerator ActivateFiretrap()
        {
            SetTrapTriggered();
            yield return new WaitForSeconds(activationDelay);
            ActivateTrap();
            yield return new WaitForSeconds(activeTime);
            DeactivateTrap();
        }

        private void SetTrapTriggered()
        {
            triggered = true;
            spriteRend.color = Color.red; // Visual cue
        }

        private void ActivateTrap()
        {
            SoundManager.instance.PlaySound(firetrapSound);
            spriteRend.color = Color.white;
            active = true;
            anim.SetBool("activated", true);
        }

        private void DeactivateTrap()
        {
            active = false;
            triggered = false;
            anim.SetBool("activated", false);
        }
        #endregion
    }
}
