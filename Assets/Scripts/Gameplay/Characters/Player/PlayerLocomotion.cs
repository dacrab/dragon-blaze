using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerLocomotion : MonoBehaviour
    {
        [SerializeField] private PlayerConfigSO config;

        private Rigidbody2D body;
        private BoxCollider2D boxCollider;
        private float horizontalInput;
        private bool isFacingRight = true;
        private bool isDashing;
        private bool isWallSliding;
        private float wallJumpCounter;

        // Runtime modifiable values (for power-ups)
        private float speedMultiplier = 1f;
        private float jumpMultiplier = 1f;

        public bool IsGrounded { get; private set; }
        public bool IsWallSliding => isWallSliding;
        public bool IsMoving => Mathf.Abs(body.linearVelocity.x) > 0.1f;
        public int ExtraJumps => config?.extraJumps ?? 2;
        public float CoyoteTime => config?.coyoteTime ?? 0.2f;

        private float Speed => (config?.speed ?? 10f) * speedMultiplier;
        private float JumpPower => (config?.jumpPower ?? 15f) * jumpMultiplier;
        private float WallSlideSpeed => config?.wallSlideSpeed ?? 0.3f;
        private float WallJumpForce => config?.wallJumpForce ?? 15f;
        private float WallJumpTime => config?.wallJumpTime ?? 0.2f;
        private float DashSpeed => config?.dashSpeed ?? 20f;
        private float DashDuration => config?.dashDuration ?? 0.2f;
        private LayerMask GroundLayer => config?.groundLayer ?? default;
        private float ExtraHeight => config?.groundCheckExtraHeight ?? 0.1f;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            CheckGrounded();
            CheckWallSlide();
            if (wallJumpCounter > 0) wallJumpCounter -= Time.deltaTime;
        }

        public void SetInput(float input) => horizontalInput = input;

        public void Move()
        {
            if (wallJumpCounter > 0) return;
            if (!isDashing) body.linearVelocity = new Vector2(horizontalInput * Speed, body.linearVelocity.y);
            if (horizontalInput > 0.01f && !isFacingRight) Flip();
            else if (horizontalInput < -0.01f && isFacingRight) Flip();
        }

        public void Jump(bool isCoyoteAllowed, int jumpCount)
        {
            if (IsGrounded || isCoyoteAllowed || jumpCount > 0)
                body.linearVelocity = new Vector2(body.linearVelocity.x, JumpPower);
            else if (isWallSliding)
                PerformWallJump();
        }

        public void CancelJump()
        {
            if (body.linearVelocity.y > 0)
                body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.5f);
        }

        public void Dash()
        {
            if (!isDashing && Mathf.Abs(horizontalInput) > 0.01f)
                StartCoroutine(DashCoroutine());
        }

        private System.Collections.IEnumerator DashCoroutine()
        {
            isDashing = true;
            float originalGravity = body.gravityScale;
            body.gravityScale = 0f;
            body.linearVelocity = new Vector2(transform.localScale.x * DashSpeed, 0f);
            yield return new WaitForSeconds(DashDuration);
            body.gravityScale = originalGravity;
            isDashing = false;
        }

        private void PerformWallJump()
        {
            isWallSliding = false;
            wallJumpCounter = WallJumpTime;
            float dir = -transform.localScale.x;
            Vector2 jumpDir = new Vector2(dir, 1f).normalized;
            body.linearVelocity = new Vector2(jumpDir.x * WallJumpForce, jumpDir.y * JumpPower);
            Flip();
        }

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private void CheckGrounded()
        {
            RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, ExtraHeight, GroundLayer);
            IsGrounded = hit.collider != null;
        }

        private void CheckWallSlide()
        {
            float direction = transform.localScale.x;
            Vector2 start = (Vector2)transform.position + new Vector2(direction * 0.4f, 0.2f);
            RaycastHit2D wallHit = Physics2D.Linecast(start, start + Vector2.down * 0.8f, GroundLayer);
            isWallSliding = wallHit.collider != null && !IsGrounded && Mathf.Abs(horizontalInput) > 0.1f;
            if (isWallSliding)
                body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Clamp(body.linearVelocity.y, -WallSlideSpeed, float.MaxValue));
        }

        public void SetSpeed(float newSpeed) => speedMultiplier = newSpeed / (config?.speed ?? 10f);
        public void SetJumpPower(float newPower) => jumpMultiplier = newPower / (config?.jumpPower ?? 15f);
        public float GetSpeed() => Speed;
        public float GetJumpPower() => JumpPower;
    }
}
