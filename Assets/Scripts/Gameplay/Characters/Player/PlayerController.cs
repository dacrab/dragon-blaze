using UnityEngine;
using Core.Input;
using Core.Events;

namespace Gameplay.Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputReader inputReader;
        
        [Header("Jump Settings")]
        [SerializeField] private int extraJumps = 2;
        [SerializeField] private float coyoteTime = 0.2f;

        private PlayerLocomotion locomotion;
        private PlayerVisuals visuals;
        private PlayerAudio playerAudio;
        private PlayerPowerups powerups;

        private float coyoteCounter;
        private int jumpCounter;
        private bool isInteracting;

        private void Awake()
        {
            locomotion = GetComponent<PlayerLocomotion>();
            visuals = GetComponent<PlayerVisuals>();
            playerAudio = GetComponent<PlayerAudio>();
            powerups = GetComponent<PlayerPowerups>();
            
            if (locomotion == null) Debug.LogError("PlayerLocomotion missing!");
            if (visuals == null) Debug.LogError("PlayerVisuals missing!");
        }

        private void OnEnable()
        {
            if (inputReader == null) { Debug.LogWarning("InputReader is not assigned in PlayerController"); return; }
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
            if (locomotion.IsGrounded) { coyoteCounter = coyoteTime; jumpCounter = extraJumps; }
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
            else if (locomotion.IsWallSliding) jumpCounter = extraJumps;
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
