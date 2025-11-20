using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float jumpPower = 15f;
        [SerializeField] private float wallSlideSpeed = 0.3f;
        [SerializeField] private float wallJumpForce = 15f;
        [SerializeField] private float wallJumpTime = 0.2f;
        
        [Header("Dash Settings")]
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.2f;
        
        [Header("Environment Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float extraHeight = 0.1f;

        private Rigidbody2D body;
        private BoxCollider2D boxCollider;
        private float horizontalInput;
        private bool isFacingRight = true;
        private bool isDashing;
        private bool isWallSliding;
        private float wallJumpCounter;

        // Properties for other components
        public bool IsGrounded { get; private set; }
        public bool IsWallSliding => isWallSliding;
        public bool IsMoving => Mathf.Abs(body.velocity.x) > 0.1f;
        public Vector2 Velocity => body.velocity;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            CheckGrounded();
            CheckWallSlide();
            
            if (wallJumpCounter > 0)
            {
                wallJumpCounter -= Time.deltaTime;
            }
        }

        public void SetInput(float input)
        {
            horizontalInput = input;
        }

        public void Move()
        {
            if (wallJumpCounter > 0) return;

            // Standard Movement
            if (!isDashing)
            {
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            }
            
            // Flip logic
            if (horizontalInput > 0.01f && !isFacingRight)
                Flip();
            else if (horizontalInput < -0.01f && isFacingRight)
                Flip();
        }

        public void Jump(bool isCoyoteAllowed, int jumpCount)
        {
            if (IsGrounded || isCoyoteAllowed)
            {
                ApplyJumpForce(Vector2.up * jumpPower);
            }
            else if (jumpCount > 0)
            {
                ApplyJumpForce(Vector2.up * jumpPower);
            }
            else if (isWallSliding)
            {
                PerformWallJump();
            }
        }

        public void CancelJump()
        {
            if (body.velocity.y > 0)
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
            }
        }

        public void Dash()
        {
            if (!isDashing && Mathf.Abs(horizontalInput) > 0.01f)
            {
                StartCoroutine(DashCoroutine());
            }
        }

        private System.Collections.IEnumerator DashCoroutine()
        {
            isDashing = true;
            float originalSpeed = speed;
            speed = dashSpeed;
            
            // Preserve vertical velocity or zero it? Original code didn't specify but usually dash overrides gravity
            float originalGravity = body.gravityScale;
            body.gravityScale = 0f;
            body.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);

            yield return new WaitForSeconds(dashDuration);

            body.gravityScale = originalGravity;
            speed = originalSpeed;
            isDashing = false;
        }

        private void PerformWallJump()
        {
            isWallSliding = false;
            wallJumpCounter = wallJumpTime;
            
            // Calculate direction opposite to wall
            // Assumes Player scale determines direction
            float direction = -transform.localScale.x;
            Vector2 jumpDir = new Vector2(direction, 1f).normalized;
            
            // Apply separate forces
            Vector2 force = new Vector2(jumpDir.x * wallJumpForce, jumpDir.y * jumpPower);
            
            body.velocity = force; // Reset velocity for consistent jump
            
            // Force flip since we are jumping away
            Flip();
        }

        private void ApplyJumpForce(Vector2 force)
        {
            body.velocity = new Vector2(body.velocity.x, 0); // Reset Y velocity for consistent jump height
            body.velocity += force; // Adding force (impulse-like via velocity change)
        }

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private void CheckGrounded()
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(
                boxCollider.bounds.center, 
                boxCollider.bounds.size, 
                0f, 
                Vector2.down, 
                extraHeight, 
                groundLayer
            );
            IsGrounded = raycastHit.collider != null;
        }

        private void CheckWallSlide()
        {
            // Simple wall check based on original code
            float direction = transform.localScale.x;
            Vector2 start = (Vector2)transform.position + new Vector2(direction * 0.4f, 0.2f);
            Vector2 end = start + Vector2.down * 0.8f;
            RaycastHit2D wallHit = Physics2D.Linecast(start, end, groundLayer);

            bool isTouchingWall = wallHit.collider != null;
            isWallSliding = isTouchingWall && !IsGrounded && Mathf.Abs(horizontalInput) > 0.1f;

            if (isWallSliding)
            {
                body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlideSpeed, float.MaxValue));
            }
        }

        public void SetSpeed(float newSpeed) => speed = newSpeed;
        public void SetJumpPower(float newPower) => jumpPower = newPower;
        
        public float GetSpeed() => speed;
        public float GetJumpPower() => jumpPower;
    }
}
