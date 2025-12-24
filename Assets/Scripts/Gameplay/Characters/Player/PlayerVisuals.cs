using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Utilities;

namespace Gameplay.Characters.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerVisuals : MonoBehaviour
    {
        [Header("Particles")]
        [SerializeField] private ParticleSystem wallSlideParticles;
        [SerializeField] private GameObject jumpParticlesPrefab;
        [SerializeField] private GameObject dashParticlesPrefab;
        [SerializeField] private GameObject deathParticlesPrefab;
        
        [Header("Renderer")]
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color invisibleColor = new Color(1f, 1f, 1f, 0.5f);

        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private Animator anim;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private PlayerLocomotion locomotion;

        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
        }

        private void OnEnable()
        {
            EventBus.OnPlayerDied += PlayDeathEffect;
            EventBus.OnPlayerRespawn += PlayRespawnEffect;
        }

        private void OnDisable()
        {
            EventBus.OnPlayerDied -= PlayDeathEffect;
            EventBus.OnPlayerRespawn -= PlayRespawnEffect;
        }

        private void Update()
        {
            UpdateAnimations();
            UpdateParticles();
        }

        private void UpdateAnimations()
        {
            anim.SetBool(GameConstants.Animation.Grounded, locomotion.IsGrounded);
            anim.SetBool(GameConstants.Animation.Run, locomotion.IsMoving);
        }

        private void UpdateParticles()
        {
            if (wallSlideParticles == null) return;
            if (locomotion.IsWallSliding && !wallSlideParticles.isPlaying) wallSlideParticles.Play();
            else if (!locomotion.IsWallSliding && wallSlideParticles.isPlaying) wallSlideParticles.Stop();
        }

        public void PlayJumpEffect()
        {
            if (jumpParticlesPrefab != null) 
                Instantiate(jumpParticlesPrefab, transform.position, Quaternion.identity);
        }
        
        public void PlayDashEffect()
        {
            if (dashParticlesPrefab != null)
                Instantiate(dashParticlesPrefab, transform.position, Quaternion.identity);
        }
        
        public void PlayDeathEffect()
        {
            if (deathParticlesPrefab != null) 
                Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            anim.SetTrigger(GameConstants.Animation.Die);
        }
        public void PlayRespawnEffect() => anim.SetTrigger(GameConstants.Animation.Respawn);
        public void SetInvisibility(bool isInvisible) => spriteRenderer.color = isInvisible ? invisibleColor : Color.white;
    }
}
