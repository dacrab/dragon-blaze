using System;
using System.Threading;
using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Managers;
using Core.Pooling;
using Core.Services;

namespace Gameplay.Characters.Player
{
    public sealed partial class Player
    {
        void OnDeath(PlayerDiedEvent _)
        {
            SpawnVfx(deathVfx);
            anim.SetTrigger(GameConstants.Anim.Die);
        }

        void OnRespawn(PlayerRespawnEvent _)
        {
            // Always respawn: with no checkpoint (or one already consumed) fall back to the spawn point,
            // otherwise the player is left dead while the UI has already resumed play.
            transform.position = checkpoint ?? spawnPosition;
            // A checkpoint is consumed on use, so the player can arm another one afterwards.
            checkpoint = null;
            stats.Clear();
            CancelDash();
            CancelInvisibility();
            jumpCount = config.extraJumps;
            coyoteTimer = config.coyoteTime;
            wallJumpTimer = 0;
            wallSliding = false;
            interacting = false;
            attackTimer = 0;
            anim.SetTrigger(GameConstants.Anim.Respawn);
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag(GameConstants.Tags.Checkpoint)) return;
            // Store the point, not the collider: the collider is disabled below and must never be re-enabled.
            checkpoint = col.transform.position;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(checkpointSound);
            col.enabled = false;
            col.GetComponent<Animator>()?.SetTrigger(GameConstants.Anim.Activate);
        }

        public void SetInvisibility(bool invisible)
        {
            IsInvisible = invisible;
            sprite.color = invisible ? config.invisibleColor : Color.white;
        }

        public void SetInvisibilityFor(float duration)
        {
            ReplaceCts(ref invisibilityCts);
            _ = InvisibilityTimeoutAsync(duration, invisibilityCts!.Token);
        }

        /// <summary>Cancels any pending invisibility timer and restores visibility.</summary>
        void CancelInvisibility()
        {
            ReplaceCts(ref invisibilityCts, replace: false);
            if (IsInvisible) SetInvisibility(false);
        }

        /// <summary>Cancels and disposes the previous source so timers don't leak across resets.</summary>
        static void ReplaceCts(ref CancellationTokenSource? cts, bool replace = true)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
            if (replace) cts = new CancellationTokenSource();
        }

        public bool HasCheckpoint() => checkpoint != null;

        async Awaitable InvisibilityTimeoutAsync(float duration, CancellationToken ct)
        {
            try
            {
                SetInvisibility(true);
                await Awaitable.WaitForSecondsAsync(duration, ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            SetInvisibility(false);
        }

        public void AddModifier(PlayerStat stat, float factor, float duration) => stats.Add(stat, factor, duration);

        void SpawnVfx(GameObject prefab) => VfxPool.Spawn(prefab, transform.position, Quaternion.identity);
    }
}
