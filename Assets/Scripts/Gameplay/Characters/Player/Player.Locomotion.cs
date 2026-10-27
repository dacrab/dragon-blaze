using UnityEngine;
using Core.Constants;

namespace Gameplay.Characters.Player
{
    public sealed partial class Player
    {
        void Update()
        {
            attackTimer += Time.deltaTime;
            if (wallJumpTimer > 0) wallJumpTimer -= Time.deltaTime;
            stats.Tick(Time.deltaTime);
            UpdateGroundCheck();
            UpdateWallSlide();
            SetAnimBool(groundedId, IsGrounded);
            SetAnimBool(runId, Mathf.Abs(rb.linearVelocity.x) > config.velocityThreshold);
            if (IsGrounded) { coyoteTimer = config.coyoteTime; jumpCount = config.extraJumps; }
            else coyoteTimer -= Time.deltaTime;
        }

        void FixedUpdate()
        {
            if (interacting || dashing || wallJumpTimer > 0) return;
            rb.linearVelocity = new(horizontalInput * Speed, rb.linearVelocity.y);
            if ((horizontalInput > config.movementThreshold && !facingRight) ||
                (horizontalInput < -config.movementThreshold && facingRight)) Flip();
        }

        void Flip()
        {
            facingRight = !facingRight;
            var s = transform.localScale;
            transform.localScale = new(-s.x, s.y, s.z);
        }

        void UpdateGroundCheck()
        {
            var hit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down,
                config.groundCheckExtraHeight, config.groundLayer);
            IsGrounded = hit.collider != null;
        }

        void UpdateWallSlide()
        {
            bool shouldCheck = !IsGrounded && Mathf.Abs(horizontalInput) > config.velocityThreshold;
            if (shouldCheck)
            {
                float dir = transform.localScale.x;
                var start = (Vector2)transform.position + new Vector2(dir * config.wallCheckOffsetX, config.wallCheckOffsetY);
                var hit = Physics2D.Linecast(start, start + Vector2.down * config.wallCheckLength, config.groundLayer);
                wallSliding = hit.collider != null;
                if (wallSliding)
                    rb.linearVelocity = new(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -config.wallSlideSpeed));
            }
            else wallSliding = false;

            if (wallSlideParticles != null)
            {
                if (wallSliding && !wallSlideParticles.isPlaying) wallSlideParticles.Play();
                else if (!wallSliding && wallSlideParticles.isPlaying) wallSlideParticles.Stop();
            }
        }

        void SetAnimBool(int id, bool value)
        {
            if (anim.GetBool(id) != value) anim.SetBool(id, value);
        }
    }
}
