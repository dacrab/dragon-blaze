using UnityEngine;
using Core.Input;
using Core.Events;

namespace Gameplay.Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private PlayerLocomotion locomotion;
        [SerializeField] private PlayerVisuals visuals;
        [SerializeField] private PlayerAudio playerAudio;
        [SerializeField] private PlayerPowerups powerups;

        private float coyoteCounter;
        private int jumpCounter;
        private bool isInteracting;

        // Read from locomotion config
        private int ExtraJumps => locomotion?.ExtraJumps ?? 2;
        private float CoyoteTime => locomotion?.CoyoteTime ?? 0.2f;

        private void Awake()
        {
            if (locomotion == null) locomotion = GetComponent<PlayerLocomotion>();
            if (visuals == null) visuals = GetComponent<PlayerVisuals>();
            if (playerAudio == null) playerAudio = GetComponent<PlayerAudio>();
            if (powerups == null) powerups = GetComponent<PlayerPowerups>();
        }

        private void OnEnable()
        {
            if (inputReader == null) return;
            inputReader.MoveEvent += OnMove;
            inputReader.JumpEvent += OnJump;
            inputReader.JumpCanceledEvent += OnJumpCanceled;
            inputReader.DashEvent += OnDash;
            EventBus.OnDialogueStateChanged += SetInteracting;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;
            inputReader.MoveEvent -= OnMove;
            inputReader.JumpEvent -= OnJump;
            inputReader.JumpCanceledEvent -= OnJumpCanceled;
            inputReader.DashEvent -= OnDash;
            EventBus.OnDialogueStateChanged -= SetInteracting;
        }

        private void Update()
        {
            if (isInteracting) return;
            if (locomotion.IsGrounded) { coyoteCounter = CoyoteTime; jumpCounter = ExtraJumps; }
            else coyoteCounter -= Time.deltaTime;
            locomotion.Move();
        }

        public bool IsInvisible() => powerups?.IsInvisible == true;
        public bool CanAttack() => !locomotion.IsMoving && locomotion.IsGrounded;
        public void SetInvisibility(bool invisible) => visuals?.SetInvisibility(invisible);

        private void OnMove(float xInput)
        {
            if (isInteracting) { locomotion.SetInput(0); return; }
            locomotion.SetInput(xInput);
        }

        private void OnJump()
        {
            if (isInteracting) return;
            bool isCoyoteAllowed = coyoteCounter > 0;
            if (!(locomotion.IsGrounded || isCoyoteAllowed || jumpCounter > 0 || locomotion.IsWallSliding)) return;
            
            locomotion.Jump(isCoyoteAllowed, jumpCounter);
            visuals.PlayJumpEffect();
            playerAudio?.PlayJumpSound();

            if (!locomotion.IsGrounded && !isCoyoteAllowed && !locomotion.IsWallSliding) jumpCounter--;
            else if (locomotion.IsWallSliding) jumpCounter = ExtraJumps;
            if (isCoyoteAllowed) coyoteCounter = 0;
        }

        private void OnJumpCanceled() => locomotion.CancelJump();

        private void OnDash()
        {
            if (isInteracting) return;
            locomotion.Dash();
            visuals.PlayDashEffect();
            playerAudio?.PlayDashSound();
        }

        public void SetInteracting(bool interacting)
        {
            isInteracting = interacting;
            if (interacting) { locomotion.SetInput(0); locomotion.Move(); }
        }
    }
}
