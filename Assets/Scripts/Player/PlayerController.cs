using UnityEngine;
using Core.Input;
using Core.Events;

namespace Player
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
        private PlayerPowerups powerups; // Added reference

        private float coyoteCounter;
        private int jumpCounter;
        private bool isInteracting;

        private void Awake()
        {
            locomotion = GetComponent<PlayerLocomotion>();
            visuals = GetComponent<PlayerVisuals>();
            playerAudio = GetComponent<PlayerAudio>();
            powerups = GetComponent<PlayerPowerups>(); // Get reference
            
            if (locomotion == null) Debug.LogError("PlayerLocomotion missing!");
            if (visuals == null) Debug.LogError("PlayerVisuals missing!");
        }

        // ... existing code ...

        public bool IsInvisible()
        {
            return powerups != null && powerups.IsInvisible;
        }

        public bool CanAttack()
        {
            return !locomotion.IsMoving && locomotion.IsGrounded;
        }
        
        public void SetInvisibility(bool invisible)
        {
             // For compatibility if needed, though usually Powerups handle this.
             // We'll assume manual set is rare or handle via Powerups
             // Actually PlayerMovement had SetInvisibility publicly.
             if (powerups) 
             { 
                 // Logic mismatch: Powerups.ApplyInvisibility is a coroutine. 
                 // PlayerVisuals.SetInvisibility is direct.
                 visuals.SetInvisibility(invisible);
             }
        }


        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.MoveEvent += OnMove;
                inputReader.JumpEvent += OnJump;
                inputReader.JumpCanceledEvent += OnJumpCanceled;
                inputReader.DashEvent += OnDash;
            }
            else
            {
                Debug.LogWarning("InputReader is not assigned in PlayerController");
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.MoveEvent -= OnMove;
                inputReader.JumpEvent -= OnJump;
                inputReader.JumpCanceledEvent -= OnJumpCanceled;
                inputReader.DashEvent -= OnDash;
            }
        }

        private void Update()
        {
            if (isInteracting) return;

            // Update Timers
            if (locomotion.IsGrounded)
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
            
            // Move is handled via Event updating a variable in Locomotion, 
            // but Locomotion.Move() needs to be called in FixedUpdate or Update?
            // Locomotion.Move() sets velocity directly, so Update is fine for non-physics or FixedUpdate for physics.
            // In PlayerLocomotion I used velocity setting. Let's call Move() here.
            locomotion.Move();
        }

        private void OnMove(float xInput)
        {
            if (isInteracting)
            {
                locomotion.SetInput(0);
                return;
            }
            locomotion.SetInput(xInput);
        }

        private void OnJump()
        {
            if (isInteracting) return;

            bool isCoyoteAllowed = coyoteCounter > 0;
            
            // Check capabilities before executing
            if (locomotion.IsGrounded || isCoyoteAllowed || jumpCounter > 0 || locomotion.IsWallSliding)
            {
                locomotion.Jump(isCoyoteAllowed, jumpCounter);
                visuals.PlayJumpEffect();
                playerAudio?.PlayJumpSound();

                // Logic to decrement counters
                if (!locomotion.IsGrounded && !isCoyoteAllowed && !locomotion.IsWallSliding)
                {
                    jumpCounter--;
                }
                else if (locomotion.IsWallSliding)
                {
                    // Wall jump logic handled in Locomotion mostly, but we might want to reset counters
                    jumpCounter = extraJumps; 
                }
                
                // Reset coyote if used
                if (isCoyoteAllowed) coyoteCounter = 0;
            }
        }

        private void OnJumpCanceled()
        {
            locomotion.CancelJump();
        }

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
            if (interacting)
            {
                locomotion.SetInput(0);
                locomotion.Move(); // Apply stop
            }
        }
    }
}
