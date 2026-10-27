using System;
using System.Threading;
using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Managers;
using Core.Services;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Player
{
    public sealed partial class Player
    {
        void OnMove(float x) => horizontalInput = interacting ? 0 : x;
        void OnDialogueChanged(DialogueStateChangedEvent e) => interacting = e.Open;

        void OnJump()
        {
            if (interacting) return;
            if (wallSliding) { WallJump(); return; }
            bool canCoyote = coyoteTimer > 0;
            if (!IsGrounded && !canCoyote && jumpCount <= 0) return;
            rb.linearVelocity = new(rb.linearVelocity.x, JumpPower);
            SpawnVfx(jumpVfx);
            if (!IsGrounded && !canCoyote) jumpCount--;
            if (canCoyote) coyoteTimer = 0;
        }

        void OnJumpCanceled()
        {
            if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new(rb.linearVelocity.x, rb.linearVelocity.y * config.jumpCancelMultiplier);
        }

        void OnDash()
        {
            if (interacting || dashing || Mathf.Abs(horizontalInput) < config.movementThreshold) return;
            dashing = true;
            dashRestoreGravity = rb.gravityScale;
            rb.gravityScale = 0;
            rb.linearVelocity = new(transform.localScale.x * config.dashSpeed, 0);
            SpawnVfx(dashVfx);
            ReplaceCts(ref dashCts);
            _ = EndDashAsync(dashCts!.Token);
        }

        /// <summary>Cancels any active dash and restores gravity. Safe to call from any teardown path,
        /// so a pause or respawn mid-dash can't leave the player frozen at zero gravity.</summary>
        void CancelDash()
        {
            ReplaceCts(ref dashCts, replace: false);
            StopDash();
        }

        void StopDash()
        {
            if (!dashing) return;
            dashing = false;
            rb.gravityScale = dashRestoreGravity;
        }

        async Awaitable EndDashAsync(CancellationToken ct)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(config.dashDuration, ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            StopDash();
        }

        void OnAttack()
        {
            if (attackTimer < config.attackCooldown) return;
            if (Mathf.Abs(rb.linearVelocity.x) > config.velocityThreshold || !IsGrounded) return;
            if (!GameStateManager.IsCurrentlyPlaying) return;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(fireballSound);
            anim.SetTrigger(GameConstants.Anim.Attack);
            attackTimer = 0;
            if (firePoint == null) return;
            ProjectileBase.Fire(fireballs, ref fireballIndex, firePoint.position)?
                .SetDirection(Mathf.Sign(transform.localScale.x));
        }

        void WallJump()
        {
            wallSliding = false;
            wallJumpTimer = config.wallJumpTime;
            jumpCount = config.extraJumps;
            float dir = -transform.localScale.x;
            rb.linearVelocity = new(dir * config.wallJumpForce, JumpPower);
            Flip();
            SpawnVfx(jumpVfx);
        }
    }
}
